namespace DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Potions
    {
        [Key]
        public int PotionID { get; set; }

        [Required]
        [StringLength(50)]
        public string PotionName { get; set; }

        public int HealAmount { get; set; }

        [StringLength(100)]
        public string Description { get; set; }
    }
}
