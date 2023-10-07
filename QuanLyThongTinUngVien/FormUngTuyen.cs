using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyThongTinUngVien
{
    public partial class FormUngTuyen : Form
    {
        public FormUngTuyen()
        {
            InitializeComponent();
        }

        private void tìmỨngViênPhùHợpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTimUngVien formTimUngVien = new FormTimUngVien();
            formTimUngVien.ShowDialog();
        }
    }
}
