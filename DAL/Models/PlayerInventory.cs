namespace DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PlayerInventory")]
    public partial class PlayerInventory
    {
        [Key]
        public int InventoryID { get; set; }

        public int UserID { get; set; }

        public int ItemID { get; set; }

        [Required]
        [StringLength(20)]
        public string ItemType { get; set; }

        public int Quantity { get; set; }

        public virtual Users Users { get; set; }
    }
}
