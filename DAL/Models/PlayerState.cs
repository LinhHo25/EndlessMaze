namespace DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PlayerState")]
    public partial class PlayerState
    {
        public int PlayerStateID { get; set; }

        public int UserID { get; set; }

        public int? CurrentLevel { get; set; }

        public int? EquippedWeaponID { get; set; }

        public int? EquippedArmorID { get; set; }

        public int? EquippedAccessoryID { get; set; }

        public virtual Accessories Accessories { get; set; }

        public virtual Armors Armors { get; set; }

        public virtual Weapons Weapons { get; set; }

        public virtual Users Users { get; set; }
    }
}
