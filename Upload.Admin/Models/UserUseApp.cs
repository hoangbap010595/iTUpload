namespace Upload.Admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserUseApp")]
    public partial class UserUseApp
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string AppID { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        public int GroupID { get; set; }
    }
}
