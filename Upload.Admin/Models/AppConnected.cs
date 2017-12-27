namespace Upload.Admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AppConnected")]
    public partial class AppConnected
    {
        [Key]
        public string AppId { get; set; }

        [StringLength(256)]
        public string AppName { get; set; }

        [StringLength(16)]
        public string Version { get; set; }

        public DateTime? Date { get; set; }

        public int? Status { get; set; }
    }
}
