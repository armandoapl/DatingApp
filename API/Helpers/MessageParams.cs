using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class MessageParams : PaginationParams
    {
        public string UserName { get; set; } // our currently login user
        public string Container { get; set; } = "Unread";

    }
}
