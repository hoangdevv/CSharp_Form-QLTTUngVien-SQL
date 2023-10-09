﻿using System;
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
    public partial class FormUngTuyen : Form
    {
        ModelUngTuyen model = new ModelUngTuyen();

        public FormUngTuyen()
        {
            InitializeComponent();
        }

        private void tìmỨngViênPhùHợpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTimUngVien formTimUngVien = new FormTimUngVien();
            formTimUngVien.ShowDialog();
        }

        private void FormUngTuyen_Load(object sender, EventArgs e)
        {
            List<Skill> listSkill = model.Skill.ToList();
            checkedLBSkill.DataSource = listSkill;
            checkedLBSkill.DisplayMember = "SkillName";
            checkedLBSkill.ValueMember = "SkillNo";

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                List<Candidate> listCandidate = model.Candidate.ToList();
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (var transaction = model.Database.BeginTransaction())
            {
                try
                {
                    string email = txtEmail.Text.Trim();

                    //kiểm tra định dạng mail
                    if(IsValidEmail(email) == false)
                    {
                        MessageBox.Show("Sai định dạng email!");
                        return;
                    }

                    Candidate emailCandidate = model.Candidate.FirstOrDefault(p => p.EmailAddress == email);
                    if (emailCandidate != null)
                    {

                        emailCandidate.FullName = txtFullName.Text;
                        emailCandidate.EmailAddress = txtEmail.Text;
                        emailCandidate.WorkExperienceYear = int.Parse(txtNumberExperience.Text);
                        emailCandidate.ExpectedSalary = int.Parse(txtSalary.Text);

                        emailCandidate.Skill.Clear(); //xoá dữ liệu cũ để thêm dữ liệu mới
                        foreach (var skill in checkedLBSkill.CheckedItems.OfType<Skill>())
                        {
                            emailCandidate.Skill.Add(skill);
                        }

                        model.SaveChanges();
                        LoadData();
                        MessageBox.Show("Cập nhật thành công!");
                        transaction.Commit();
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
                            AddCandidte.Skill.Add(item);
                        }
                        model.Candidate.Add(AddCandidte);
                        model.SaveChanges();
                        LoadData();
                        MessageBox.Show("Thêm thành công!");
                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Lỗi khi Update");
                    transaction.Rollback();
                }
            }
        }

        private bool IsValidEmail(string email)
        {
            // Đây là một biểu thức chính quy cơ bản để kiểm tra định dạng email.
            // Bạn có thể sử dụng biểu thức chính quy phức tạp hơn để kiểm tra định dạng email chính xác hơn.
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            return System.Text.RegularExpressions.Regex.IsMatch(email, pattern);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                Candidate emailCandidate = model.Candidate.FirstOrDefault(p => p.EmailAddress == email);
                if (emailCandidate != null)
                {
                    // Xóa ứng viên từ cơ sở dữ liệu
                    Candidate candidateToDelete = model.Candidate.FirstOrDefault(c => c.EmailAddress == email);
                    if (candidateToDelete != null)
                    {
                        model.Candidate.Remove(candidateToDelete);
                        model.SaveChanges();
                    }

                    // Tải lại dữ liệu vào dataGridViewMain
                    LoadData();
                    MessageBox.Show("Xoá thành công!");
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
                    Candidate selectedCandidate = model.Candidate.FirstOrDefault(p => p.CVNo == selectedCVNo);
                    if (selectedCandidate != null)
                    {
                        foreach (Skill skill in selectedCandidate.Skill)
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
    }
}
