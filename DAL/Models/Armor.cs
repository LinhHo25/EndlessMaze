namespace DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Armor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Armor()
        {
            PlayerSessionInventories = new HashSet<PlayerSessionInventory>();
        }

        public int ArmorID { get; set; }

        [Required]
        [StringLength(50)]
        public string ArmorName { get; set; }

        [Required]
        [StringLength(1)]
        public string ArmorRank { get; set; }

        public int DefensePoints { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerSessionInventory> PlayerSessionInventories { get; set; }
    }
}
