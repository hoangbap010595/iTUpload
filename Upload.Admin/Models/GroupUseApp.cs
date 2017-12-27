namespace Upload.Admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GroupUseApp")]
    public partial class GroupUseApp
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string AppId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string Address { get; set; }

        [StringLength(128)]
        public string Phone { get; set; }

        [StringLength(128)]
        public string Email { get; set; }

        public string Note { get; set; }

        public int? Members { get; set; }
    }
}
