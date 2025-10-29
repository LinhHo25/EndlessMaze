namespace DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Accessories
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Accessories()
        {
            PlayerState = new HashSet<PlayerState>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccessoryID { get; set; }

        [Required]
        [StringLength(50)]
        public string AccessoryName { get; set; }

        [Required]
        [StringLength(1)]
        public string Rarity { get; set; }

        [StringLength(100)]
        public string EffectDescription { get; set; }

        public double EffectValue { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerState> PlayerState { get; set; }
    }
}
