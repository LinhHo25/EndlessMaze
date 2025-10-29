namespace DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Monsters
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Monsters()
        {
            LootTables = new HashSet<LootTables>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MonsterID { get; set; }

        [Required]
        [StringLength(50)]
        public string MonsterName { get; set; }

        [Required]
        [StringLength(20)]
        public string MonsterType { get; set; }

        public int BaseHealth { get; set; }

        public int BaseDamage { get; set; }

        [StringLength(100)]
        public string SpriteName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LootTables> LootTables { get; set; }
    }
}
