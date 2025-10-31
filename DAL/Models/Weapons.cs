namespace DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Weapons
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Weapons()
        {
            PlayerSessionInventory = new HashSet<PlayerSessionInventory>();
        }

        [Key]
        public int WeaponID { get; set; }

        [Required]
        [StringLength(50)]
        public string WeaponName { get; set; }

        [Required]
        [StringLength(1)]
        public string WeaponRank { get; set; }

        public int AttackBonus { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerSessionInventory> PlayerSessionInventory { get; set; }
    }
}
