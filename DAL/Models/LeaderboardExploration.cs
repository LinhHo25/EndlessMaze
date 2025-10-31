namespace DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LeaderboardExploration")]
    public partial class LeaderboardExploration
    {
        [Key]
        public int ScoreID { get; set; }

        public int UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        public int CompletionTimeSeconds { get; set; }

        public int ItemsCollected { get; set; }

        public int MapsCompleted { get; set; }

        public DateTime AchievedDate { get; set; }

        public virtual User User { get; set; }
    }
}
