using System;
using System.Collections.Generic;

namespace Collplex.Models
{
    public partial class AccountBans
    {
        public ulong Uid { get; set; }
        public ulong Operator { get; set; }
        public string OperatorIp { get; set; }
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
        public string Reason { get; set; }

        public virtual Accounts Account { get; set; }
    }
}
