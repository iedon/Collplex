using System;
using System.Collections.Generic;

namespace Collplex.Models
{
    public partial class Groups
    {
        public Groups()
        {
            Accounts = new HashSet<Accounts>();
            AuthProviders = new HashSet<AuthProviders>();
        }

        public uint Gid { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }

        public virtual ICollection<Accounts> Accounts { get; set; }
        public virtual ICollection<AuthProviders> AuthProviders { get; set; }
    }
}
