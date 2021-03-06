﻿using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.Collections.Generic;

namespace ChatApp.Models
{
    public class User : IdentityUser
    {
        public ICollection<ChatUser> Chats { get; set; }
    }

}
