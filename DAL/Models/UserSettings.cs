namespace DAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserSettings
    {
        [Key]
        public int SettingID { get; set; }

        public int UserID { get; set; }

        public double MusicVolume { get; set; }

        public double SfxVolume { get; set; }

        public virtual Users Users { get; set; }
    }
}
