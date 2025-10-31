namespace DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MonsterLootTables
    {
        [Key]
        public int LootTableID { get; set; }

        public int MonsterID { get; set; }

        public int ItemType { get; set; }

        [Required]
        [StringLength(1)]
        public string ItemRank { get; set; }

        public double DropChance { get; set; }

        public virtual Monsters Monsters { get; set; }
    }
}
