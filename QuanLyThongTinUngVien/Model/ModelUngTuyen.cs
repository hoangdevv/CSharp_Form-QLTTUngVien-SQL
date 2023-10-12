using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace QuanLyThongTinUngVien.Model
{
    public partial class ModelUngTuyen : DbContext
    {
        public ModelUngTuyen()
            : base("name=Model1")
        {
        }

        public virtual DbSet<Candidate> Candidates { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Candidate>()
                .HasMany(e => e.Skills)
                .WithMany(e => e.Candidates)
                .Map(m => m.ToTable("CandidateSkill").MapLeftKey("CVNo").MapRightKey("SkillNo"));

            modelBuilder.Entity<Skill>()
                .HasMany(e => e.Jobs)
                .WithRequired(e => e.Skill)
                .HasForeignKey(e => e.RequiredSkillId)
                .WillCascadeOnDelete(false);
        }
    }
}
