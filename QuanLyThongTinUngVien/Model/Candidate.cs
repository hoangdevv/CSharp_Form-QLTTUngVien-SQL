namespace QuanLyThongTinUngVien.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Candidate")]
    public partial class Candidate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Candidate()
        {
            Skills = new HashSet<Skill>();
        }

        [Key]
        public int CVNo { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("Họ và tên")]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Email")]
        public string EmailAddress { get; set; }

        [DisplayName("Kinh nghiệm")]
        public int? WorkExperienceYear { get; set; }

        [DisplayName("Mức lương")]
        public int? ExpectedSalary { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Skill> Skills { get; set; }
    }
}
