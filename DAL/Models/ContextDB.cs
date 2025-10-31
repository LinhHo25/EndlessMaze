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

        public virtual DbSet<Armor> Armors { get; set; }
        public virtual DbSet<ChestLootTable> ChestLootTables { get; set; }
        public virtual DbSet<LeaderboardAdventure> LeaderboardAdventures { get; set; }
        public virtual DbSet<LeaderboardExploration> LeaderboardExplorations { get; set; }
        public virtual DbSet<MonsterLootTable> MonsterLootTables { get; set; }
        public virtual DbSet<Monster> Monsters { get; set; }
        public virtual DbSet<PlayerCharacter> PlayerCharacters { get; set; }
        public virtual DbSet<PlayerSessionInventory> PlayerSessionInventories { get; set; }
        public virtual DbSet<Potion> Potions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Weapon> Weapons { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Armor>()
                .Property(e => e.ArmorRank)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Armor>()
                .HasMany(e => e.PlayerSessionInventories)
                .WithRequired(e => e.Armor)
                .HasForeignKey(e => e.EquippedArmorID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChestLootTable>()
                .Property(e => e.ItemRank)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<MonsterLootTable>()
                .Property(e => e.ItemRank)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Monster>()
                .HasMany(e => e.MonsterLootTables)
                .WithRequired(e => e.Monster)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.LeaderboardAdventures)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.LeaderboardExplorations)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Weapon>()
                .Property(e => e.WeaponRank)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Weapon>()
                .HasMany(e => e.PlayerSessionInventories)
                .WithRequired(e => e.Weapon)
                .HasForeignKey(e => e.EquippedWeaponID)
                .WillCascadeOnDelete(false);
        }
    }
}
