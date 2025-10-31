namespace DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ChestLootTable
    {
        [Key]
        public int LootTableID { get; set; }

        public int ItemType { get; set; }

        [StringLength(1)]
        public string ItemRank { get; set; }

        public int? PotionID { get; set; }

        public int MinQuantity { get; set; }

        public int MaxQuantity { get; set; }

        public double DropChance { get; set; }
    }
}
