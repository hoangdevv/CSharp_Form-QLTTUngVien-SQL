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
using Microsoft.Office.Interop.Excel;
namespace QuanLyThongTinUngVien
{
    public partial class FormTimUngVien : Form
    {
        ModelUngTuyen model = new ModelUngTuyen();
        public FormTimUngVien()
        {
            InitializeComponent();
        }
        #region Method
        private void LoadTxt()
        {
            try
            {
                if (int.TryParse(cbbLocation.SelectedValue.ToString(), out int selectedJobId))
                {
                    if (selectedJobId == 0)
                    {
                        txtRequiredExp.Text = "";
                        txtSalary.Text = "";
                        txtSkill.Text = "";
                        dataGridViewConform.DataSource = null;
                    }
                    else
                    {
                        var selectedJob = model.Jobs.FirstOrDefault(p => p.JobId == selectedJobId);
                        if (selectedJob != null)
                        {
                            txtRequiredExp.Text = selectedJob.RequiredExperience.HasValue ? selectedJob.RequiredExperience.ToString() : "Không yêu cầu";
                            txtSalary.Text = selectedJob.RequiredMaxSalary.HasValue ? selectedJob.RequiredMaxSalary.ToString() : "Không yêu cầu";
                            txtSkill.Text = selectedJob.RequiredSkillId > 0 ? selectedJob.RequiredSkillId.ToString() : "Không yêu cầu";
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy công việc!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

       

        #endregion

        #region Event
        private void FormTimUngVien_Load(object sender, EventArgs e)
        {
            List<Job> listJob = model.Jobs.ToList();
            List<Job> listJobZero = new List<Job>();
            listJobZero.Add(new Job { JobTitle = "", JobDescription = "" });
            listJobZero.AddRange(listJob);
            cbbLocation.DataSource = listJobZero;
            cbbLocation.DisplayMember = "JobTitle";
            cbbLocation.ValueMember = "JobId";
            cbbLocation.SelectedIndex = 0;
        }

 
        private List<Candidate> GetMatchingCandidates(Job selectedJob)
        {
            var matchingCandidates = model.Candidates
                .Where(candidate =>
                     (selectedJob.RequiredExperience == null || candidate.WorkExperienceYear >= selectedJob.RequiredExperience) &&
                     (selectedJob.RequiredMaxSalary == null || candidate.ExpectedSalary <= selectedJob.RequiredMaxSalary) &&
                     candidate.Skills.Any(p => p.SkillNo == selectedJob.RequiredSkillId))
                .ToList();

            return matchingCandidates;
        }
        private void cbbLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(cbbLocation.SelectedValue.ToString(), out int selectedJobId))
                {
                    LoadTxt();
                    if (selectedJobId != 0)
                    {
                        var selectedJob = model.Jobs.FirstOrDefault(job => job.JobId == selectedJobId);

                        if (selectedJob != null)
                        {
                            var matchingCandidates = GetMatchingCandidates(selectedJob);
                            dataGridViewConform.DataSource = matchingCandidates;
                            dataGridViewConform.Columns.Remove("Skills");
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy công việc!");
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
                
            } 
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
        #endregion

        #region Excel
        void exportExcel(DataGridView d, string duongdan, string ten)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application obj = new Microsoft.Office.Interop.Excel.Application();
                obj.Workbooks.Add(Type.Missing);
                obj.Columns.ColumnWidth = 25;
                obj.StandardFontSize = 12;
                obj.StandardFont = "Arial";

                // Đặt tiêu đề lớn
                Microsoft.Office.Interop.Excel.Range titleRange = obj.get_Range("A1", "E1");
                titleRange.Merge(); // Gộp các ô lại với nhau
                titleRange.Value2 = "DANH SÁCH ỨNG VIÊN TÌM ĐƯỢC"; // Tiêu đề lớn
                titleRange.Font.Size = 16; // Đặt kích thước font cho tiêu đề lớn
                titleRange.Font.Bold = true; // Đặt độ đậy font
                                             // Căn giữa ngang và dọc
                titleRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                titleRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                for (int i = 1; i < d.Columns.Count + 1; i++)
                {
                    Microsoft.Office.Interop.Excel.Range headerRange = obj.Cells[2, i];
                    headerRange.Value2 = d.Columns[i - 1].HeaderText;
                    headerRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    headerRange.Font.Bold = true;
                    headerRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                }

                for (int i = 0; i < d.Rows.Count; i++)
                {
                    for (int j = 0; j < d.Columns.Count; j++)
                    {
                        if (d.Rows[i].Cells[j].Value != null)
                        {
                            Microsoft.Office.Interop.Excel.Range dataRange = obj.Cells[i + 3, j + 1];
                            dataRange.Value2 = d.Rows[i].Cells[j].Value.ToString();
                            dataRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                            dataRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                        }
                    }
                }

                // Kiểm tra xem tệp đã tồn tại
                string filePath = duongdan + ten + ".xlsx";
                if (System.IO.File.Exists(filePath))
                {
                    // Tệp đã tồn tại, hiển thị thông báo xác nhận
                    DialogResult dialogResult = MessageBox.Show("Tên tệp đã tồn tại ở vị trí này. Bạn có muốn thay thế?", "Xác nhận", MessageBoxButtons.YesNo);

                    if (dialogResult == DialogResult.Yes)
                    {
                        // Thực hiện lưu tệp Excel và ghi đè lên tệp cũ (không hỏi)
                        obj.ActiveWorkbook.SaveAs(filePath);
                    }
                }
                else
                {
                    // Tệp không tồn tại, thực hiện lưu mà không cần xác nhận
                    obj.ActiveWorkbook.SaveAs(filePath);
                }

                obj.ActiveWorkbook.Saved = true;
                obj.Quit();
                // Mở tệp Excel sau khi xuất
                System.Diagnostics.Process.Start(filePath);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Kiêm tra tệp đang tồn tại hoặc lỗi: " + ex.Message);
            }
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                exportExcel(dataGridViewConform, @"D:\", "FileExcelUngVienPhuHop");
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
