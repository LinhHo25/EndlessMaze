namespace DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    

    [Table("PlayerSessionInventory")]
    public partial class PlayerSessionInventory
    {
        [Key]
        public int SessionInventoryID { get; set; }

        public int CharacterID { get; set; }

        public int EquippedArmorID { get; set; }

        public int EquippedWeaponID { get; set; }

        public int HealthPotionCount { get; set; }

        
        [ForeignKey("EquippedArmorID")]
        public virtual Armors Armors { get; set; }

        [ForeignKey("EquippedWeaponID")]
        public virtual Weapons Weapons { get; set; }

        

        // public virtual Armors Armors { get; set; } 

        public virtual PlayerCharacters PlayerCharacters { get; set; }

        // public virtual Weapons Weapons { get; set; }
    }
}