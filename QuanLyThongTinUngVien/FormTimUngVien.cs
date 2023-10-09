using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyThongTinUngVien.Models;

namespace QuanLyThongTinUngVien
{
    public partial class FormTimUngVien : Form
    {
        ModelUngTuyen model = new ModelUngTuyen();
        public FormTimUngVien()
        {
            InitializeComponent();
        }

        private void FormTimUngVien_Load(object sender, EventArgs e)
        {
            List<Job> listJob = model.Job.ToList();
            cbbLocation.DataSource = listJob;
            cbbLocation.DisplayMember = "JobTitle";
            cbbLocation.ValueMember = "JobId";
            LoadTxt();

            List<Candidate> listCandidate = model.Candidate.ToList();
            LoadData(listCandidate);
        }
        private void LoadData(List<Candidate> listCandidate)
        {
            try
            {
                listCandidate = model.Candidate.ToList();
                var displayCandidates = listCandidate.Select(p => new
                {
                    p.CVNo,
                    p.FullName,
                    p.EmailAddress,
                    p.WorkExperienceYear,
                    p.ExpectedSalary
                }).ToList();
                dataGridViewConform.DataSource = displayCandidates;
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi khi load data");
            }
        }

        private void LoadTxt()
        {
            try
            {
                if (int.TryParse(cbbLocation.SelectedValue.ToString(), out int selectedJobId))
                {
                    var selectedJob = model.Job.FirstOrDefault(p => p.JobId == selectedJobId);
                    if (selectedJob != null)
                    {
                        txtRequiredExp.Text = selectedJob.RequiredExperience.ToString();
                        txtSalary.Text = selectedJob.RequiredMaxSalary.ToString();
                        txtSkill.Text = selectedJob.RequiredSkillId.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy công việc!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cbbLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                List<Candidate> listCandidate = model.Candidate.ToList();
                if (cbbLocation.SelectedValue != null)
                {
                    LoadTxt();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy giá trị!");
                }
            }
            catch (Exception)
            {

                
            } 
        }
    }
}
