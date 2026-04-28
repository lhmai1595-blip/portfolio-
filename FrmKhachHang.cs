using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChuongTrinhQuanLiSach
{
    public partial class FrmKhachHang : Form
    {
        KetNoi kn = new KetNoi();
        BindingSource bds = new BindingSource();
        public FrmKhachHang()
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
            // *** THAY 'dgvKH' BẰNG TÊN DATAGRIDVIEW CỦA BẠN ***
            dgvKH.DataSource = bds;

            // 4. Gỡ và cài đặt lại Binding cho các TextBox
            txtMaKH.DataBindings.Clear();
            txtTenKH.DataBindings.Clear();
            txtDiaChi.DataBindings.Clear();
            txtNgaySinh.DataBindings.Clear();
            txtEmail.DataBindings.Clear();
            txtDienThoai.DataBindings.Clear();
            txtSTK.DataBindings.Clear();

            // 5. Thêm binding mới
            txtMaKH.DataBindings.Add("Text", bds, "MaKH", true, DataSourceUpdateMode.OnPropertyChanged);
            txtTenKH.DataBindings.Add("Text", bds, "TenKH", true, DataSourceUpdateMode.OnPropertyChanged);
            txtDiaChi.DataBindings.Add("Text", bds, "DiaChiKH", true, DataSourceUpdateMode.OnPropertyChanged);
            txtNgaySinh.DataBindings.Add("Text", bds, "NgaySinhKH", true, DataSourceUpdateMode.OnPropertyChanged);
            txtEmail.DataBindings.Add("Text", bds, "EmailKH", true, DataSourceUpdateMode.OnPropertyChanged);
            txtDienThoai.DataBindings.Add("Text", bds, "SdtKH", true, DataSourceUpdateMode.OnPropertyChanged);
            txtSTK.DataBindings.Add("Text", bds, "STKKH", true, DataSourceUpdateMode.OnPropertyChanged);

            // 6. Cấu hình DataGridView
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
        private void FrmKhachHang_Load(object sender, EventArgs e)
        {
            // Gán BindingNavigator cho BindingSource (chỉ làm 1 lần)
            // *** THAY 'bindingNavigator1' BẰNG TÊN BINDINGNAVIGATOR CỦA BẠN ***
            bindingNavigator1.BindingSource = bds;

            // Tải toàn bộ dữ liệu và bind lần đầu
            LoadAndBindData("SELECT * FROM KHACH_HANG");
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Lấy dữ liệu
                string maKH = txtMaKH.Text.Trim();
                string tenKH = txtTenKH.Text.Trim();
                string diaChi = txtDiaChi.Text.Trim();
                string sdt = txtDienThoai.Text.Trim();
                string email = txtEmail.Text.Trim();
                string stk = txtSTK.Text.Trim();

                // 2. Xử lý ngày sinh
                if (!DateTime.TryParse(txtNgaySinh.Text, out DateTime ngaySinh))
                {
                    MessageBox.Show("Ngày sinh không hợp lệ.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string ngaySinh_SQL = ngaySinh.ToString("yyyy-MM-dd");

                // 3. Tạo câu query
                string qr = $"INSERT INTO KHACH_HANG (MaKH, TenKH, DiaChiKH, SdtKH, EmailKH, STKKH, NgaySinhKH) " +
                              $"VALUES(N'{maKH}', N'{tenKH}', N'{diaChi}', N'{sdt}', N'{email}', N'{stk}', '{ngaySinh_SQL}')";

                int kq = kn.ExecuteNonQuery(qr);
                if (kq > 0)
                {
                    MessageBox.Show("Thêm " + maKH + " thành công!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Thêm không thành công");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi thêm dữ liệu: " + ex.Message);
            }

            // 4. Tải lại
            LoadAndBindData("SELECT * FROM KHACH_HANG");
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Lấy dữ liệu
                string maKH = txtMaKH.Text.Trim();
                string tenKH = txtTenKH.Text.Trim();
                string diaChi = txtDiaChi.Text.Trim();
                string sdt = txtDienThoai.Text.Trim();
                string email = txtEmail.Text.Trim();
                string stk = txtSTK.Text.Trim();

                // 2. Xử lý ngày sinh
                if (!DateTime.TryParse(txtNgaySinh.Text, out DateTime ngaySinh))
                {
                    MessageBox.Show("Ngày sinh không hợp lệ.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string ngaySinh_SQL = ngaySinh.ToString("yyyy-MM-dd");

                // 3. Tạo câu query
                string qr = $"UPDATE KHACH_HANG SET " +
                            $"TenKH = N'{tenKH}', " +
                            $"DiaChiKH = N'{diaChi}', " +
                            $"SdtKH = N'{sdt}', " +
                            $"EmailKH = N'{email}', " +
                            $"STKKH = N'{stk}', " +
                            $"NgaySinhKH = '{ngaySinh_SQL}' " +
                            $"WHERE MaKH = N'{maKH}'";

                int kq = kn.ExecuteNonQuery(qr);
                if (kq > 0)
                {
                    MessageBox.Show("Cập nhật " + maKH + " thành công!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Sửa không thành công. Mã khách hàng có thể không tồn tại.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi sửa dữ liệu: " + ex.Message);
            }

            // 4. Tải lại
            LoadAndBindData("SELECT * FROM KHACH_HANG");
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string maKH = txtMaKH.Text;
            if (string.IsNullOrEmpty(maKH))
            {
                MessageBox.Show("Vui lòng chọn khách hàng để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Bạn có chắc muốn xóa khách hàng {maKH} không?\n(Mọi Đơn Hàng, Chi Tiết Đơn Hàng VÀ Phiếu Thanh Toán của khách này cũng sẽ bị xóa!)",
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
                // 1. XÓA BẢNG CHÁU (CHITIET_DDH)
                string qr_delete_grandchild = $"DELETE FROM CHITIET_DDH " +
                                              $"WHERE MaDDH IN (SELECT MaDDH FROM DON_DAT_HANG WHERE MaKH = N'{maKH}')";
                kn.ExecuteNonQuery(qr_delete_grandchild);

                // 2. Xóa các đơn hàng (bảng con)
                string qr_delete_child1 = $"DELETE FROM DON_DAT_HANG WHERE MaKH = N'{maKH}'";
                kn.ExecuteNonQuery(qr_delete_child1);

                // 3. XÓA PHIẾU THANH TOÁN (bảng con khác) - *** CODE MỚI THÊM VÀO ***
                string qr_delete_child2 = $"DELETE FROM PHIEU_THANH_TOAN WHERE MaKH = N'{maKH}'";
                kn.ExecuteNonQuery(qr_delete_child2);

                // 4. Xóa khách hàng (bảng cha)
                int kq = kn.ExecuteNonQuery($"DELETE FROM KHACH_HANG WHERE MaKH = N'{maKH}'");

                if (kq > 0)
                {
                    MessageBox.Show("Xóa khách hàng " + maKH + " thành công!", "Thông báo",
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

            // 5. Tải lại dữ liệu
            LoadAndBindData("SELECT * FROM KHACH_HANG");
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            // (Nút này là nút "Hủy Tìm" hoặc "Hiện Tất Cả")
            // *** THAY 'txtTimKiem' BẰNG TÊN Ô TÌM KIẾM CỦA BẠN ***
            txtMaKH.Text = "";

            LoadAndBindData("SELECT * FROM KHACH_HANG");
            txtMaKH.Focus();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    // *** THAY 'txtTimKiem' BẰNG TÊN Ô TÌM KIẾM CỦA BẠN ***
            //    string timKiem = txtMaKH.Text.Trim();

            //    if (string.IsNullOrWhiteSpace(timKiem))
            //    {
            //        MessageBox.Show("Vui lòng nhập Mã hoặc Tên khách hàng vào ô tìm kiếm.", "Thông báo",
            //                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return;
            //    }

            //    // Tìm theo MÃ hoặc TÊN
            //    string query = $"SELECT * FROM KHACH_HANG WHERE MaKH = N'{timKiem}' OR TenKH LIKE N'%{timKiem}%'";

            //    LoadAndBindData(query);

            //    if (bds.Count == 0)
            //    {
            //        MessageBox.Show("Không tìm thấy khách hàng nào.", "Kết quả tìm kiếm",
            //                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Lỗi khi tìm kiếm: " + ex.ToString(), "Lỗi",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            {
                FrmTimKiem f = new FrmTimKiem();
                f.ShowDialog();
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Chọn nơi để lưu danh sách";
            saveFileDialog.FileName = "DanhSachKhachHang.xls";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, System.Text.Encoding.Unicode))
                    {
                        string headerLine = "";
                        // *** THAY 'dgvKH' BẰNG TÊN DATAGRIDVIEW CỦA BẠN ***
                        foreach (DataGridViewColumn col in dgvKH.Columns)
                        {
                            headerLine += col.HeaderText + "\t";
                        }
                        sw.WriteLine(headerLine.Trim());

                        // *** THAY 'dgvKH' BẰNG TÊN DATAGRIDVIEW CỦA BẠN ***
                        foreach (DataGridViewRow row in dgvKH.Rows)
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

        private void button9_Click(object sender, EventArgs e)
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

        private void button8_Click(object sender, EventArgs e)
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

        private void btnSanPham_Click(object sender, EventArgs e)
        {
            FrmSP f = new FrmSP();
            f.ShowDialog();
        }

        private void btnDonHang_Click(object sender, EventArgs e)
        {
            FrmDonHang f = new FrmDonHang();
            f.ShowDialog();
        }

        private void btnNhanVien_Click(object sender, EventArgs e)
        {
            FrmNhanVien f = new FrmNhanVien();
            f.ShowDialog();
        }

        private void btnBaoCao_Click(object sender, EventArgs e)
        {
            FrmBaoCao f = new FrmBaoCao();
            f.ShowDialog();
        }
    }
}
    

