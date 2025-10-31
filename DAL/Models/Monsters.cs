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
            MonsterLootTables = new HashSet<MonsterLootTables>();
        }

        [Key]
        public int MonsterID { get; set; }

        [Required]
        [StringLength(50)]
        public string MonsterName { get; set; }

        public int Health { get; set; }

        public int Attack { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MonsterLootTables> MonsterLootTables { get; set; }
    }
}
