namespace Upload.Admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LogConnected")]
    public partial class LogConnected
    {
        public int Id { get; set; }

        public DateTime? Date { get; set; }

        [StringLength(32)]
        public string IpAddress { get; set; }

        public string Browser { get; set; }

        public int? Type { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

        public string CusName { get; set; }

    }
}
