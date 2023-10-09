using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace QuanLyThongTinUngVien.Models
{
    public partial class ModelUngTuyen : DbContext
    {
        public ModelUngTuyen()
            : base("name=ModelUngTuyen")
        {
        }

        public virtual DbSet<Candidate> Candidate { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<Skill> Skill { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Candidate>()
                .HasMany(e => e.Skill)
                .WithMany(e => e.Candidate)
                .Map(m => m.ToTable("CandidateSkill").MapLeftKey("CVNo").MapRightKey("SkillNo"));

            modelBuilder.Entity<Skill>()
                .HasMany(e => e.Job)
                .WithRequired(e => e.Skill)
                .HasForeignKey(e => e.RequiredSkillId)
                .WillCascadeOnDelete(false);
        }
    }
}
