using System;
using System.Collections.Generic;

namespace Collplex.Models
{
    public partial class Accounts
    {
        public ulong Uid { get; set; }
        public uint Gid { get; set; }
        public uint? Cid { get; set; }
        public string Uuid { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public byte Gender { get; set; }
        public string Avatar { get; set; }
        public string Bio { get; set; }
        public DateTime? Birthday { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Language { get; set; }
        public string RegIp { get; set; }
        public DateTime? RegDate { get; set; }
        public string LastIp { get; set; }
        public DateTime? LastLogin { get; set; }
        public string Comment { get; set; }

        public virtual Colleges College { get; set; }
        public virtual Groups Group { get; set; }
        public virtual AccountBans AccountBan { get; set; }
    }
}
