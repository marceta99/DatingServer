﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class MessageParams : PaginationParams
    {

        public string Username { get; set; } //trenutno ulogovan user
        public string Container { get; set; } = "Unread"; 

    }
}
