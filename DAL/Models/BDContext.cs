using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DAL.Models
{
    public partial class BDContext : DbContext
    {
        public BDContext()
            : base("name=BDContext")
        {
        }

        public virtual DbSet<Accessories> Accessories { get; set; }
        public virtual DbSet<Armors> Armors { get; set; }
        public virtual DbSet<Leaderboard> Leaderboard { get; set; }
        public virtual DbSet<LootTables> LootTables { get; set; }
        public virtual DbSet<Monsters> Monsters { get; set; }
        public virtual DbSet<PlayerInventory> PlayerInventory { get; set; }
        public virtual DbSet<PlayerState> PlayerState { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UserSettings> UserSettings { get; set; }
        public virtual DbSet<Weapons> Weapons { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accessories>()
                .Property(e => e.Rarity)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Accessories>()
                .HasMany(e => e.PlayerState)
                .WithOptional(e => e.Accessories)
                .HasForeignKey(e => e.EquippedAccessoryID);

            modelBuilder.Entity<Armors>()
                .Property(e => e.Rarity)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Armors>()
                .HasMany(e => e.PlayerState)
                .WithOptional(e => e.Armors)
                .HasForeignKey(e => e.EquippedArmorID);

            modelBuilder.Entity<Monsters>()
                .HasMany(e => e.LootTables)
                .WithRequired(e => e.Monsters)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Leaderboard)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.PlayerInventory)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.PlayerState)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.UserSettings)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Weapons>()
                .Property(e => e.Rarity)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Weapons>()
                .HasMany(e => e.PlayerState)
                .WithOptional(e => e.Weapons)
                .HasForeignKey(e => e.EquippedWeaponID);
        }
    }
}
