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
    public partial class FrmDangNhap : Form
    {
        public static string QuyenTruyCap = "";
        public static string TenNhanVien = "";
        KetNoi kn = new KetNoi();
        public FrmDangNhap()
        {
            InitializeComponent();
        }
        private bool checkValidation()
        {
            if (txtTenDangNhap.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập tên người dùng", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtTenDangNhap.Focus();

                return false;
            }

            if (txtMatKhau.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập mật khẩu", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtMatKhau.Focus();

                return false;
            }

            return true;
        }
        private void txtDangnhap_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string taiKhoan = txtTenDangNhap.Text.Trim();
            string matKhau = txtMatKhau.Text.Trim();

            if (string.IsNullOrEmpty(taiKhoan) || string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tài khoản và Mật khẩu!", "Thông báo");
                return;
            }

            try
            {
                // Lưu ý: ID_User của bạn có tiếng Việt nên bắt buộc phải có chữ N đằng trước chuỗi
                // Câu lệnh lấy cả Quyền truy cập
                string query = $"SELECT * FROM TAI_KHOAN_NHAN_VIEN WHERE ID_User = N'{taiKhoan}' AND PassWord = N'{matKhau}'";

                DataTable dt = kn.ExecuteQuery(query);

                if (dt.Rows.Count > 0)
                {
                    // 1. Lấy quyền từ dữ liệu trả về
                    QuyenTruyCap = dt.Rows[0]["QuyenTruyCap"].ToString();
                    TenNhanVien = dt.Rows[0]["ID_User"].ToString(); // Lấy tên để hiển thị xin chào

                    MessageBox.Show($"Đăng nhập thành công!\nXin chào: {TenNhanVien}\nQuyền: {QuyenTruyCap}", "Thông báo");

                    // 2. Mở Form Main
                    FrmQuanLiBanSach f = new FrmQuanLiBanSach();
                    this.Hide(); // Ẩn form đăng nhập
                    f.ShowDialog();

                    // Khi Form Main đóng thì hiện lại Form đăng nhập hoặc thoát luôn tùy logic của bạn
                    this.Show();
                }
                else
                {
                    MessageBox.Show("Tên tài khoản hoặc mật khẩu không chính xác!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
        }

        private void cboHienMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            if (txtMatKhau.PasswordChar == '*')
            {
                txtMatKhau.PasswordChar = '\0';
            }
            else
            {
                txtMatKhau.PasswordChar = '*';
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn thoát không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void txtTenDangNhap_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnDangNhap_Click(sender, e);
            }
        }

        private void txtMatKhau_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnDangNhap_Click(sender, e);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void FrmDangNhap_Load(object sender, EventArgs e)
        {
            txtMatKhau.PasswordChar = '*';
        }
    }
}
