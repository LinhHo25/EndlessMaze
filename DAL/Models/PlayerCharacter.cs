namespace DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PlayerCharacter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PlayerCharacter()
        {
            PlayerSessionInventories = new HashSet<PlayerSessionInventory>();
        }

        [Key]
        public int CharacterID { get; set; }

        public int UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string CharacterName { get; set; }

        public int BaseHealth { get; set; }

        public int BaseAttack { get; set; }

        public int BaseDefense { get; set; }

        public int BaseStamina { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerSessionInventory> PlayerSessionInventories { get; set; }
    }
}
