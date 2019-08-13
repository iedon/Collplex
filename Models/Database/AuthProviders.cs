using System;
using System.Collections.Generic;

namespace Collplex.Models
{
    public partial class AuthProviders
    {
        public string Provider { get; set; }
        public uint Gid { get; set; }
        public string Comment { get; set; }

        public virtual Groups Group { get; set; }
    }
}
