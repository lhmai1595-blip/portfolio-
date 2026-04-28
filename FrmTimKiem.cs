using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChuongTrinhQuanLiSach
{
    public partial class FrmTimKiem : Form
    {
        KetNoi kn = new KetNoi();
        BindingSource bds = new BindingSource();
        public FrmTimKiem()
        {
            InitializeComponent();
        }
        private void LoadDS()
        {
            dgvKH.DataSource = kn.LayDuLieu("KHACH_HANG");
            dgvKH.DataSource = bds;
        }

        private void setdata()
        {
            dgvKH.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            try
            {
                // Đặt tên cột Tiếng Việt
                dgvKH.Columns["MaKH"].HeaderText = "Mã Khách Hàng";
                dgvKH.Columns["TenKH"].HeaderText = "Tên Khách Hàng";
                dgvKH.Columns["DiaChiKH"].HeaderText = "Địa Chỉ";
                dgvKH.Columns["SdtKH"].HeaderText = "Điện Thoại";
                dgvKH.Columns["EmailKH"].HeaderText = "Email";
                dgvKH.Columns["STKKH"].HeaderText = "Số Tài Khoản";
                dgvKH.Columns["NgaySinhKH"].HeaderText = "Ngày Sinh";
                // Định dạng ngày
                dgvKH.Columns["NgaySinhKH"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }
            catch (Exception) { /* Bỏ qua nếu lỗi */ }
        }

        private void setbinding()
        {
            bds.DataSource = kn.LayDuLieu("KHACH_HANG");
            dgvKH.DataSource = bds;
            bindingNavigator1.BindingSource = bds;
            bindingNavigator1.Visible = true;
        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void FrmTimKiem_Load(object sender, EventArgs e)
        {
            LoadDS();
            setbinding();
            setdata();

        }

        private void btnDanhsach_Click(object sender, EventArgs e)
        {
            LoadDS();
            setbinding();

        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTT.Text))
            {
                MessageBox.Show("Hãy nhập thông tin cần tìm theo", "Thông báo",
               MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string qr = "SELECT * FROM KHACH_HANG WHERE 1=1 ";
            if (raMa.Checked && !string.IsNullOrEmpty(txtTT.Text))
            {
                qr += $"AND MaKH LIKE N'%{txtTT.Text}%'";
            }
            if (raTen.Checked && !string.IsNullOrEmpty(txtTT.Text))
            {
                qr += $"AND TenKH LIKE N'%{txtTT.Text}%'";
            }
            dgvKH.DataSource = kn.ExecuteQuery(qr);
        }
    }
    
}
