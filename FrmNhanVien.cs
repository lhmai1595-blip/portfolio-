using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChuongTrinhQuanLiSach
{
    public partial class FrmNhanVien : Form
    {
        KetNoi kn = new KetNoi();
        BindingSource bds = new BindingSource();


        public FrmNhanVien()
        {
            InitializeComponent();
        }

        private void LoadAndBindData(string query)
        {
            // 1. Lấy dữ liệu
            DataTable dt = kn.ExecuteQuery(query);

            // 2. Gán cho BindingSource
            bds.DataSource = dt;

            // 3. Gán cho DataGridView
            dgvNV.DataSource = bds;

            // 4. Gỡ và cài đặt lại Binding cho các TextBox
            txtMaNV.DataBindings.Clear();
            txtTenNV.DataBindings.Clear();
            txtEmail.DataBindings.Clear();
            txtDienThoai.DataBindings.Clear();
            txtMaCV.DataBindings.Clear();

            // 5. Thêm binding mới
            txtMaNV.DataBindings.Add("Text", bds, "MaNV", true, DataSourceUpdateMode.OnPropertyChanged);
            txtTenNV.DataBindings.Add("Text", bds, "HoTenNV", true, DataSourceUpdateMode.OnPropertyChanged);
            txtEmail.DataBindings.Add("Text", bds, "EmailNV", true, DataSourceUpdateMode.OnPropertyChanged);
            txtDienThoai.DataBindings.Add("Text", bds, "SDTNV", true, DataSourceUpdateMode.OnPropertyChanged);
            txtMaCV.DataBindings.Add("Text", bds, "MaCV", true, DataSourceUpdateMode.OnPropertyChanged);

            // 6. Cấu hình DataGridView
            dgvNV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            try
            {
                // Đặt tên cột Tiếng Việt
                dgvNV.Columns["MaNV"].HeaderText = "Mã Nhân Viên";
                dgvNV.Columns["HoTenNV"].HeaderText = "Họ Tên";
                dgvNV.Columns["EmailNV"].HeaderText = "Email";
                dgvNV.Columns["SDTNV"].HeaderText = "Số Điện Thoại";
                dgvNV.Columns["MaCV"].HeaderText = "Mã Chức Vụ";
            }
            catch (Exception) {  }
        }
            private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Chọn nơi để lưu danh sách";
            saveFileDialog.FileName = "DanhSachNhanVien.xls";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, System.Text.Encoding.Unicode))
                    {
                        string headerLine = "";
                        foreach (DataGridViewColumn col in dgvNV.Columns)
                        {
                            headerLine += col.HeaderText + "\t";
                        }
                        sw.WriteLine(headerLine.Trim());

                        foreach (DataGridViewRow row in dgvNV.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                string rowLine = "";
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    rowLine += (cell.Value ?? "").ToString() + "\t";
                                }
                                sw.WriteLine(rowLine.Trim());
                            }
                        }
                    }

                    MessageBox.Show("Xuất file Excel thành công!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất file Excel: " + ex.ToString(), "Lỗi",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Lấy dữ liệu
                string maNV = txtMaNV.Text.Trim();
                string hoTen = txtTenNV.Text.Trim();
                string email = txtEmail.Text.Trim();
                string sdt = txtDienThoai.Text.Trim();
                string maCV = txtMaCV.Text.Trim();

                // 2. Tạo câu query
                string qr = $"UPDATE NHAN_VIEN SET " +
                            $"HoTenNV = N'{hoTen}', " +
                            $"EmailNV = N'{email}', " +
                            $"SDTNV = N'{sdt}', " +
                            $"MaCV = N'{maCV}' " +
                            $"WHERE MaNV = N'{maNV}'";

                int kq = kn.ExecuteNonQuery(qr);
                if (kq > 0)
                {
                    MessageBox.Show("Cập nhật thông tin cho " + maNV + " thành công!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information); ;
                }
                else
                {
                    MessageBox.Show("Sửa không thành công. Mã nhân viên có thể không tồn tại.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi sửa dữ liệu: " + ex.Message);
            }

            // 3. Tải lại
            LoadAndBindData("SELECT * FROM NHAN_VIEN");
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string maNV = txtMaNV.Text;
            if (string.IsNullOrEmpty(maNV))
            {
                MessageBox.Show("Vui lòng chọn nhân viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Bạn có chắc muốn xóa nhân viên {maNV} không?\n(Mọi tài khoản của nhân viên này cũng sẽ bị xóa!)",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.No)
            {
                return;
            }

            try
            {
                // 1. XÓA BẢNG CON (TAI_KHOAN_NHAN_VIEN) TRƯỚC
                // Đây là bước xử lý lỗi khóa ngoại (REFERENCE constraint)
                string qr_delete_child = $"DELETE FROM TAI_KHOAN_NHAN_VIEN WHERE MaNV = N'{maNV}'";
                kn.ExecuteNonQuery(qr_delete_child);

                // 2. Xóa nhân viên (bảng cha)
                int kq = kn.ExecuteNonQuery($"DELETE FROM NHAN_VIEN WHERE MaNV = N'{maNV}'");

                if (kq > 0)
                {
                    MessageBox.Show("Xóa khách hàng " + maNV + " thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Xóa không thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // 3. Tải lại
            LoadAndBindData("SELECT * FROM NHAN_VIEN");
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            // Xóa trắng các ô
            txtMaNV.Text = "";
            txtTenNV.Text = "";
            txtEmail.Text = "";
            txtDienThoai.Text = "";
            txtMaCV.Text = "";

            // (Nếu có ô tìm kiếm riêng thì thêm dòng)
            // txtTimKiem.Text = ""; 

            LoadAndBindData("SELECT * FROM NHAN_VIEN");
            txtMaNV.Focus();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Lấy dữ liệu
                string maNV = txtMaNV.Text.Trim();
                string hoTen = txtTenNV.Text.Trim();
                string email = txtEmail.Text.Trim();
                string sdt = txtDienThoai.Text.Trim();
                string maCV = txtMaCV.Text.Trim();

                if (string.IsNullOrEmpty(maNV))
                {
                    MessageBox.Show("Mã nhân viên không được để trống.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Tạo câu query
                string qr = $"INSERT INTO NHAN_VIEN (MaNV, HoTenNV, EmailNV, SDTNV, MaCV) " +
                              $"VALUES(N'{maNV}', N'{hoTen}', N'{email}', N'{sdt}', N'{maCV}')";

                int kq = kn.ExecuteNonQuery(qr);
                if (kq > 0)
                {
                    MessageBox.Show("Thêm " + maNV + " thành công!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Thêm không thành công.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi thêm dữ liệu: " + ex.Message);
            }

            // 3. Tải lại
            LoadAndBindData("SELECT * FROM NHAN_VIEN");
        }

        private void txtDienThoai_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTenNV_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            // Tìm kiếm dựa trên các ô thông tin
            try
            {
                string maNV = txtMaNV.Text.Trim();
                string hoTen = txtTenNV.Text.Trim();

                if (string.IsNullOrWhiteSpace(maNV) && string.IsNullOrWhiteSpace(hoTen))
                {
                    MessageBox.Show("Vui lòng nhập Mã NV hoặc Họ Tên để tìm kiếm.", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = "SELECT * FROM NHAN_VIEN WHERE ";
                List<string> conditions = new List<string>();

                if (!string.IsNullOrWhiteSpace(maNV))
                {
                    conditions.Add($"MaNV = N'{maNV}'");
                }
                if (!string.IsNullOrWhiteSpace(hoTen))
                {
                    conditions.Add($"HoTenNV LIKE N'%{hoTen}%'");
                }

                query += string.Join(" AND ", conditions);

                LoadAndBindData(query);

                if (bds.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy nhân viên nào.", "Kết quả tìm kiếm",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.ToString(), "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtMaNV_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvNV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
       
            
        }

        private void txtKhachHang_Click(object sender, EventArgs e)
        {
            FrmKhachHang f = new FrmKhachHang();
            f.ShowDialog();
        }

        private void btnBaoCao_Click(object sender, EventArgs e)
        {
            FrmBaoCao f = new FrmBaoCao();
            f.ShowDialog();
        }

        private void btnHDN_Click(object sender, EventArgs e)
        {

        }

        private void btnHDB_Click(object sender, EventArgs e)
        {

        }

        private void btnNCC_Click(object sender, EventArgs e)
        {

        }

        private void btnNhanVien_Click(object sender, EventArgs e)
        {

        }

        private void btnDonHang_Click(object sender, EventArgs e)
        {
            FrmDonHang f = new FrmDonHang();
            f.ShowDialog();
        }

        private void btnSanPham_Click(object sender, EventArgs e)
        {
            FrmSP f = new FrmSP();
            f.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void FrmNhanVien_Load(object sender, EventArgs e)
        {

            // Gán BindingNavigator cho BindingSource
            bindingNavigator1.BindingSource = bds;

            // Tải toàn bộ dữ liệu và bind lần đầu
            LoadAndBindData("SELECT * FROM NHAN_VIEN");
        }

        private void txtDienThoai_Press(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn thoát chương trình không?",
                                          "Xác nhận thoát",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit(); // Thoát toàn bộ chương trình
            }
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn đăng xuất không?",
                                          "Xác nhận thoát",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                FrmDangNhap f = new FrmDangNhap();
                f.ShowDialog();
            }
        }
    }
}
