namespace DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Monster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Monster()
        {
            MonsterLootTables = new HashSet<MonsterLootTable>();
        }

        public int MonsterID { get; set; }

        [Required]
        [StringLength(50)]
        public string MonsterName { get; set; }

        public int Health { get; set; }

        public int Attack { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MonsterLootTable> MonsterLootTables { get; set; }
    }
}
