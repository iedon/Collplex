using System;
using Collplex.Models;
using Microsoft.EntityFrameworkCore;

namespace Collplex
{
    public partial class MainContext : DbContext
    {

        public MainContext(DbContextOptions<MainContext> options) : base(options) {}

        public virtual DbSet<AccountBans> AccountBans { get; set; }
        public virtual DbSet<Accounts> Accounts { get; set; }
        public virtual DbSet<AuthProviders> AuthProviders { get; set; }
        public virtual DbSet<Colleges> Colleges { get; set; }
        public virtual DbSet<Groups> Groups { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }

        /*
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder.UseMySql("");
                }
            }
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountBans>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PRIMARY");

                entity.ToTable("account_bans");

                entity.HasIndex(e => e.Operator)
                    .HasName("operator_idx");

                entity.HasIndex(e => e.OperatorIp)
                    .HasName("operator_ip_idx");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.Property(e => e.From)
                    .HasColumnName("from")
                    .HasColumnType("datetime");

                entity.Property(e => e.Operator).HasColumnName("operator");

                entity.Property(e => e.OperatorIp)
                    .IsRequired()
                    .HasColumnName("operator_ip")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Reason)
                    .HasColumnName("reason")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.To)
                    .HasColumnName("to")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.AccountBan)
                    .HasForeignKey<AccountBans>(d => d.Uid)
                    .HasConstraintName("uid_fk");
            });

            modelBuilder.Entity<Accounts>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PRIMARY");

                entity.ToTable("accounts");

                entity.HasIndex(e => e.Account)
                    .HasName("account_idx")
                    .IsUnique();

                entity.HasIndex(e => e.Cid)
                    .HasName("cid_idx");

                entity.HasIndex(e => e.Gid)
                    .HasName("gid_idx");

                entity.HasIndex(e => e.LastIp)
                    .HasName("last_ip_idx");

                entity.HasIndex(e => e.LastLogin)
                    .HasName("last_login_idx");

                entity.HasIndex(e => e.Name)
                    .HasName("name_idx");

                entity.HasIndex(e => e.RegDate)
                    .HasName("reg_date_idx");

                entity.HasIndex(e => e.RegIp)
                    .HasName("reg_ip_idx");

                entity.HasIndex(e => e.Uuid)
                    .HasName("uuid_idx")
                    .IsUnique();

                entity.HasIndex(e => new { e.Gid, e.Email })
                    .HasName("gid_email_idx")
                    .IsUnique();

                entity.HasIndex(e => new { e.Gid, e.Phone })
                    .HasName("gid_phone_idx")
                    .IsUnique();

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.Property(e => e.Account)
                    .IsRequired()
                    .HasColumnName("account")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Avatar)
                    .HasColumnName("avatar")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Bio)
                    .HasColumnName("bio")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Birthday)
                    .HasColumnName("birthday")
                    .HasColumnType("date");

                entity.Property(e => e.Cid).HasColumnName("cid");

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Gender)
                    .HasColumnName("gender")
                    .HasDefaultValue();

                entity.Property(e => e.Gid).HasColumnName("gid");

                entity.Property(e => e.Language)
                    .HasColumnName("language")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.LastIp)
                    .HasColumnName("last_ip")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.LastLogin)
                    .HasColumnName("last_login")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.RegDate)
                    .HasColumnName("reg_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.RegIp)
                    .HasColumnName("reg_ip")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Region)
                    .HasColumnName("region")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Salt)
                    .HasColumnName("salt")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Uuid)
                    .IsRequired()
                    .HasColumnName("uuid")
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.College)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.Cid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("cid_fk");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.Gid)
                    .HasConstraintName("gid_fk");
            });

            modelBuilder.Entity<AuthProviders>(entity =>
            {
                entity.HasKey(e => e.Provider)
                    .HasName("PRIMARY");

                entity.ToTable("auth_providers");

                entity.HasIndex(e => e.Gid)
                    .HasName("gid_oauth_fk");

                entity.Property(e => e.Provider)
                    .HasColumnName("provider")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Gid).HasColumnName("gid");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.AuthProviders)
                    .HasForeignKey(d => d.Gid)
                    .HasConstraintName("gid_oauth_fk");
            });

            modelBuilder.Entity<Colleges>(entity =>
            {
                entity.HasKey(e => e.Cid)
                    .HasName("PRIMARY");

                entity.ToTable("colleges");

                entity.HasIndex(e => e.ClientId)
                    .HasName("client_id_idx")
                    .IsUnique();

                entity.HasIndex(e => e.ClientSecret)
                    .HasName("client_secret_idx");

                entity.HasIndex(e => e.Name)
                    .HasName("name_idx");

                entity.Property(e => e.MaxUrls)
                    .IsRequired()
                    .HasColumnName("max_urls")
                    .HasDefaultValue();

                entity.Property(e => e.RegInterval)
                    .IsRequired()
                    .HasColumnName("reg_interval")
                    .HasDefaultValue();

                entity.Property(e => e.Timeout)
                    .IsRequired()
                    .HasColumnName("timeout")
                    .HasDefaultValue();

                entity.Property(e => e.Cid).HasColumnName("cid");

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasColumnName("client_id")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ClientSecret)
                    .IsRequired()
                    .HasColumnName("client_secret")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<Groups>(entity =>
            {
                entity.HasKey(e => e.Gid)
                    .HasName("PRIMARY");

                entity.ToTable("groups");

                entity.HasIndex(e => e.Name)
                    .HasName("name_idx");

                entity.Property(e => e.Gid).HasColumnName("gid");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<Settings>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PRIMARY");

                entity.ToTable("settings");

                entity.Property(e => e.Key)
                    .HasColumnName("key")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasColumnType("varchar(255)");
            });
        }
    }
}
