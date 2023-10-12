namespace QuanLyThongTinUngVien.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Job")]
    public partial class Job
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int JobId { get; set; }

        [Required]
        [StringLength(250)]
        public string JobTitle { get; set; }

        [Required]
        public string JobDescription { get; set; }

        public int RequiredSkillId { get; set; }

        public int? RequiredExperience { get; set; }

        public int? RequiredMaxSalary { get; set; }

        public virtual Skill Skill { get; set; }
    }
}
