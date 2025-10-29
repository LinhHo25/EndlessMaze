namespace DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Leaderboard")]
    public partial class Leaderboard
    {
        [Key]
        public int ScoreID { get; set; }

        public int UserID { get; set; }

        [Required]
        [StringLength(20)]
        public string GameMode { get; set; }

        public int Score { get; set; }

        public int CompletionTime { get; set; }

        public DateTime AchievedDate { get; set; }

        public virtual Users Users { get; set; }
    }
}
