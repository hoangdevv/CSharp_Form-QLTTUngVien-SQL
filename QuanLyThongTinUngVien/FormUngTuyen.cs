using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyThongTinUngVien.Model;

namespace QuanLyThongTinUngVien
{
    public partial class FormUngTuyen : Form
    {
        ModelUngTuyen model = new ModelUngTuyen();

        public FormUngTuyen()
        {
            InitializeComponent();
        }
        #region Method
        private void LoadData()
        {
            try
            {
                List<Candidate> listCandidate = model.Candidates.ToList();
                var displayCandidates = listCandidate.Select(p => new
                {
                    p.CVNo,
                    p.FullName,
                    p.EmailAddress,
                    p.WorkExperienceYear,
                    p.ExpectedSalary
                }).ToList();
                dataGridViewMain.DataSource = displayCandidates;
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi khi load data");
            }
        }

        private bool IsValidEmail(string email)
        {
            // dinh dang email
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            return System.Text.RegularExpressions.Regex.IsMatch(email, pattern);
        }

        void clearInput()
        {
            txtFullName.Clear();txtEmail.Clear();txtNumberExperience.Clear();txtSalary.Clear();
            for(int i = 0; i < checkedLBSkill.Items.Count; i++)
            {
                checkedLBSkill.SetItemChecked(i, false);
            }
        }
        
        bool checkInput()
        {
            if (txtFullName.Text == "" || txtNumberExperience.Text == "" || txtSalary.Text == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return false;
            }
            if(!int.TryParse(txtSalary.Text,out int resurtSalary )||!int.TryParse(txtNumberExperience.Text,out int resurtNEx))
            {
                MessageBox.Show("Nhập đúng định dạng số tiền hoặc số năm kn");
                return false;
            }
            return true;
        }
        #endregion

        #region Event
        private void FormUngTuyen_Load(object sender, EventArgs e)
        {
            List<Skill> listSkill = model.Skills.ToList();
            checkedLBSkill.DataSource = listSkill;
            checkedLBSkill.DisplayMember = "SkillName";
            checkedLBSkill.ValueMember = "SkillNo";

            LoadData();
        }

        private void tìmỨngViênPhùHợpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTimUngVien formTimUngVien = new FormTimUngVien();
            formTimUngVien.ShowDialog();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (var transaction = model.Database.BeginTransaction())
            {
                try
                {
                    string email = txtEmail.Text.Trim();
                    if (!checkInput())
                    {
                        return;
                    }
                    if(IsValidEmail(email) == false)
                    {
                        MessageBox.Show("Sai định dạng email!");
                        return;
                    }
                   
                    Candidate emailCandidate = model.Candidates.FirstOrDefault(p => p.EmailAddress == email);
                    if (emailCandidate != null)
                    {

                        emailCandidate.FullName = txtFullName.Text;
                        emailCandidate.EmailAddress = txtEmail.Text;
                        emailCandidate.WorkExperienceYear = int.Parse(txtNumberExperience.Text);
                        emailCandidate.ExpectedSalary = int.Parse(txtSalary.Text);

                        emailCandidate.Skills.Clear(); //xoá dữ liệu cũ để thêm dữ liệu mới
                        foreach (var skill in checkedLBSkill.CheckedItems.OfType<Skill>())
                        {
                            emailCandidate.Skills.Add(skill);
                        }

                        model.SaveChanges();
                        LoadData();
                        MessageBox.Show("Cập nhật thành công!");
                        transaction.Commit();
                        clearInput();
                    }
                    else
                    {
                        Candidate AddCandidte = new Candidate
                        {
                            FullName = txtFullName.Text,
                            EmailAddress = txtEmail.Text,
                            WorkExperienceYear = int.Parse(txtNumberExperience.Text),
                            ExpectedSalary = int.Parse(txtSalary.Text),
                        };
                        foreach (var item in checkedLBSkill.CheckedItems.OfType<Skill>())
                        {
                            AddCandidte.Skills.Add(item);
                        }
                        model.Candidates.Add(AddCandidte);
                        model.SaveChanges();
                        LoadData();
                        MessageBox.Show("Thêm thành công!");
                        transaction.Commit();
                        clearInput();
                    }
                   
                }
                catch (Exception)
                {
                    MessageBox.Show("Lỗi khi Update");
                    transaction.Rollback();
                }
            }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                Candidate emailCandidate = model.Candidates.FirstOrDefault(p => p.EmailAddress == email);
                if (emailCandidate != null)
                {
                    if(MessageBox.Show("Chắc chắn xóa?","Xác nhận",MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        model.Candidates.Remove(emailCandidate);
                        model.SaveChanges();
                        LoadData();
                        MessageBox.Show("Xoá thành công!");
                        clearInput();
                    }
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu để xóa!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xoá! " + ex.Message);
            }
        }

        private void dataGridViewMain_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridViewMain.Rows[e.RowIndex];

                    for (int i = 0; i < checkedLBSkill.Items.Count; i++)
                    {
                        checkedLBSkill.SetItemCheckState(i, CheckState.Unchecked);
                    }

                    txtFullName.Text = row.Cells["FullName"].Value.ToString();
                    txtEmail.Text = row.Cells["EmailAddress"].Value.ToString();
                    if (row.Cells["WorkExperienceYear"].Value != null)
                    {
                        txtNumberExperience.Text = row.Cells["WorkExperienceYear"].Value.ToString();
                    }
                    else
                    {
                        txtNumberExperience.Text = null;
                    }
                    if (row.Cells["ExpectedSalary"].Value != null)
                    {
                        txtSalary.Text = row.Cells["ExpectedSalary"].Value.ToString();
                    }
                    else
                    {
                        txtSalary.Text = null;
                    }

                    //lấy dữ liệu từ trường Skill trong Candidate
                    int selectedCVNo = int.Parse(row.Cells["CVNo"].Value.ToString());
                    Candidate selectedCandidate = model.Candidates.FirstOrDefault(p => p.CVNo == selectedCVNo);
                    if (selectedCandidate != null)
                    {
                        foreach (Skill skill in selectedCandidate.Skills)
                        {
                            int index = checkedLBSkill.Items.IndexOf(skill);
                            if (index >= 0)
                            {
                                checkedLBSkill.SetItemChecked(index, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FormUngTuyen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(MessageBox.Show("Bạn có muốn thoát","Xác nhận",MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel=false;
            }
        }
        #endregion

    }
}
