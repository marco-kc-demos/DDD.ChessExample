using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Test
{
    public class OpenAccount
    {
        public string Owner { get; set; }

        public OpenAccount(string owner)
        {
            Owner = owner;
        }
    }
}
