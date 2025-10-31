using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DAL.Models
{
    public partial class ContextDB : DbContext
    {
        public ContextDB()
            : base("name=ContextDB")
        {
        }

        public virtual DbSet<Armors> Armors { get; set; }
        public virtual DbSet<ChestLootTables> ChestLootTables { get; set; }
        public virtual DbSet<LeaderboardAdventure> LeaderboardAdventure { get; set; }
        public virtual DbSet<LeaderboardExploration> LeaderboardExploration { get; set; }
        public virtual DbSet<MonsterLootTables> MonsterLootTables { get; set; }
        public virtual DbSet<Monsters> Monsters { get; set; }
        public virtual DbSet<PlayerCharacters> PlayerCharacters { get; set; }
        public virtual DbSet<PlayerSessionInventory> PlayerSessionInventory { get; set; }
        public virtual DbSet<Potions> Potions { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Weapons> Weapons { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Armors>()
                .Property(e => e.ArmorRank)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Armors>()
                .HasMany(e => e.PlayerSessionInventory)
                .WithRequired(e => e.Armors)
                .HasForeignKey(e => e.EquippedArmorID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChestLootTables>()
                .Property(e => e.ItemRank)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<MonsterLootTables>()
                .Property(e => e.ItemRank)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Monsters>()
                .HasMany(e => e.MonsterLootTables)
                .WithRequired(e => e.Monsters)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.LeaderboardAdventure)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.LeaderboardExploration)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Weapons>()
                .Property(e => e.WeaponRank)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Weapons>()
                .HasMany(e => e.PlayerSessionInventory)
                .WithRequired(e => e.Weapons)
                .HasForeignKey(e => e.EquippedWeaponID)
                .WillCascadeOnDelete(false);
        }
    }
}
