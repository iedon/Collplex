using System;
using System.Collections.Generic;

namespace Collplex.Models
{
    public partial class Colleges
    {
        public Colleges()
        {
            Accounts = new HashSet<Accounts>();
        }

        public uint Cid { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Name { get; set; }
        public int MaxUrls { get; set; }
        public int RegInterval { get; set; }
        public int Timeout { get; set; }
        public string Comment { get; set; }

        public virtual ICollection<Accounts> Accounts { get; set; }
    }
}
