using ChatApp.Database;
using ChatApp.Hubs;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//send receive leave join

namespace ChatApp.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ChatController: Controller
    {
        private IHubContext<ChatHub> _chat; //Hub for sync chat - signalR (ChatHub.cs)

        public ChatController(IHubContext<ChatHub> chat)
        {
            _chat = chat;
        }

        //View - Home/Chat.cshtml

        [HttpPost("[action]/{connectionId}/{roomId}")]
        public async Task<IActionResult> JoinRoom(string connectionId, string roomId)
        {
            await _chat.Groups.AddToGroupAsync(connectionId, roomId);
            return Ok();
        }

        [HttpPost("[action]/{connectionId}/{roomId}")]
        public async Task<IActionResult> LeaveRoom(string connectionId, string roomId)
        {
            await _chat.Groups.RemoveFromGroupAsync(connectionId, roomId);
            return Ok();
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(
            int roomId,
            string message, 
            
            [FromServices] AppDbContext ctx)
        {
            //Message model
            var Message = new Message
{
                ChatId = roomId,
                Text = message,
                Name = User.Identity.Name,
                Timestamp = DateTime.Now
            };

            ctx.Messages.Add(Message);
            await ctx.SaveChangesAsync();

            await _chat.Clients.Group(roomId.ToString())
                .SendAsync("RecieveMessage", new {
                    Text = Message.Text,
                    Name = Message.Name,
                    Timestamp = Message.Timestamp.ToString("dd/MM/yyyy hh:mm:ss")

                });
            return Ok();
        }
    }
}
