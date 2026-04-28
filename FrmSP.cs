using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChuongTrinhQuanLiSach
{
    public partial class FrmSP : Form
    {

        KetNoi kn = new KetNoi();
        BindingSource bds = new BindingSource();
        public FrmSP()
        {
            InitializeComponent();
        }

        // *** HÀM MỚI (THAY THẾ setbinding, load, hienthi) ***
        // Hàm này sẽ tải dữ liệu VÀ liên kết (bind)
        private void LoadAndBindData(string query)
        {
            // 1. Lấy dữ liệu theo query
            DataTable dt = kn.ExecuteQuery(query);

            // 2. Gán dữ liệu cho BindingSource
            bds.DataSource = dt;

            // 3. Gán BindingSource cho DataGridView
            dgvSP.DataSource = bds;

            // 4. *** QUAN TRỌNG: GỠ BỎ VÀ CÀI ĐẶT LẠI BINDING CHO TEXTBOX ***
            // Điều này đảm bảo các ô text "trên" luôn kết nối với "dưới"
            txtMaSP.DataBindings.Clear();
            txtTenSP.DataBindings.Clear();
            txtGiaBan.DataBindings.Clear();
            txtGiaNhap.DataBindings.Clear();
            txtSLTK.DataBindings.Clear();
            txtMoTaSP.DataBindings.Clear();

            // 5. Thêm binding mới
            // (true, DataSourceUpdateMode.OnPropertyChanged)
            // Sẽ tự động xóa trắng TextBox nếu không tìm thấy kết quả (khi tìm kiếm)
            txtMaSP.DataBindings.Add("Text", bds, "MaSP", true, DataSourceUpdateMode.OnPropertyChanged);
            txtTenSP.DataBindings.Add("Text", bds, "TenSP", true, DataSourceUpdateMode.OnPropertyChanged);
            txtGiaBan.DataBindings.Add("Text", bds, "GiaBan", true, DataSourceUpdateMode.OnPropertyChanged);
            txtGiaNhap.DataBindings.Add("Text", bds, "GiaNhap", true, DataSourceUpdateMode.OnPropertyChanged);
            txtSLTK.DataBindings.Add("Text", bds, "SLTonKho", true, DataSourceUpdateMode.OnPropertyChanged);
            txtMoTaSP.DataBindings.Add("Text", bds, "MoTaSP", true, DataSourceUpdateMode.OnPropertyChanged);

            // 6. Cấu hình DataGridView
            dgvSP.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            try
            {
                // Đặt lại tên cột Tiếng Việt (vì dữ liệu mới tải)
                dgvSP.Columns["MaSP"].HeaderText = "Mã Sản Phẩm";
                dgvSP.Columns["TenSP"].HeaderText = "Tên Sản Phẩm";
                dgvSP.Columns["GiaNhap"].HeaderText = "Giá Nhập";
                dgvSP.Columns["GiaBan"].HeaderText = "Giá Bán";
                dgvSP.Columns["SLTonKho"].HeaderText = "SL Tồn Kho";
                dgvSP.Columns["MoTaSP"].HeaderText = "Mô Tả";
            }
            catch (Exception)
            {
                // Bỏ qua nếu lỗi (ví dụ: khi tìm không ra kết quả, không có cột nào)
            }
        }

        // *** ĐÃ XÓA HÀM setbinding(), hienthi(), load() ***

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void FrmSP_Load(object sender, EventArgs e)
        {
            // Gán BindingNavigator cho BindingSource (chỉ làm 1 lần)
            bindingNavigator1.BindingSource = bds;

            // Tải toàn bộ dữ liệu và bind lần đầu
            LoadAndBindData("SELECT * FROM SAN_PHAM");
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // 1. Lấy và .Trim() dữ liệu
            string maSP = txtMaSP.Text.Trim();
            string tenSP = txtTenSP.Text.Trim();
            string moTa = txtMoTaSP.Text.Trim();

            // 2. Validate và Parse
            if (!decimal.TryParse(txtGiaNhap.Text, out decimal giaNhap) ||
                !decimal.TryParse(txtGiaBan.Text, out decimal giaBan) ||
                !int.TryParse(txtSLTK.Text, out int slTonKho))
            {
                MessageBox.Show("Giá bán, Giá nhập và SL Tồn Kho phải là số hợp lệ.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Tạo câu query
            string qr = $"INSERT INTO SAN_PHAM (MaSP, TenSP, GiaNhap, GiaBan, SLTonKho, MoTaSP) " +
                          $"VALUES('{maSP}', N'{tenSP}', {giaNhap.ToString(CultureInfo.InvariantCulture)}, {giaBan.ToString(CultureInfo.InvariantCulture)}, {slTonKho}, N'{moTa}')";

            try
            {
                int kq = kn.ExecuteNonQuery(qr);
                if (kq > 0)
                {
                    MessageBox.Show("Thêm thành công");
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

            // 4. SỬA LỖI: Tải lại bằng hàm mới
            LoadAndBindData("SELECT * FROM SAN_PHAM");
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            // 1. Lấy và .Trim() dữ liệu
            string maSP = txtMaSP.Text.Trim();
            string tenSP = txtTenSP.Text.Trim();
            string moTa = txtMoTaSP.Text.Trim();

            // 2. Validate và Parse
            if (!decimal.TryParse(txtGiaNhap.Text, out decimal giaNhap) ||
                !decimal.TryParse(txtGiaBan.Text, out decimal giaBan) ||
                !int.TryParse(txtSLTK.Text, out int slTonKho))
            {
                MessageBox.Show("Giá bán, Giá nhập và SL Tồn Kho phải là số hợp lệ.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Tạo câu query
            string qr = $"UPDATE SAN_PHAM SET TenSP = N'{tenSP}', GiaNhap = {giaNhap.ToString(CultureInfo.InvariantCulture)}, GiaBan = {giaBan.ToString(CultureInfo.InvariantCulture)}, SLTonKho = {slTonKho}, MoTaSP = N'{moTa}' WHERE MaSP = '{maSP}'";

            int kq = kn.ExecuteNonQuery(qr);
            if (kq > 0)
            {
                MessageBox.Show("Sửa thành công");
            }
            else
            {
                MessageBox.Show("Sửa không thành công. Vui lòng kiểm tra lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // 4. SỬA LỖI: Tải lại bằng hàm mới
            LoadAndBindData("SELECT * FROM SAN_PHAM");
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa sản phẩm này không?\n(Mọi chi tiết đơn hàng liên quan cũng sẽ bị xóa)",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
            {
                return;
            }

            string maSPCanXoa = txtMaSP.Text;

            // 1. Xóa bản ghi con (nếu có)
            string qr_delete_child = $"DELETE FROM CHITIET_DDH WHERE MaSP = '{maSPCanXoa}'";
            kn.ExecuteNonQuery(qr_delete_child);

            // 2. Xóa bản ghi cha
            string qr_delete_parent = $"DELETE FROM SAN_PHAM WHERE MaSP = '{maSPCanXoa}'";
            int kq = kn.ExecuteNonQuery(qr_delete_parent);

            // 3. Thông báo
            if (kq > 0)
            {
                MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Xóa không thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // 4. SỬA LỖI: Tải lại bằng hàm mới
            LoadAndBindData("SELECT * FROM SAN_PHAM");
        }

        // Nút Đăng xuất
        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn đăng xuất không?",
                                                "Xác nhận thoát",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // SỬA LỖI LOGIC: Ẩn form này, mở form đăng nhập
                // Sau khi form đăng nhập tắt, form này cũng tự tắt theo
                this.Hide();
                FrmDangNhap f = new FrmDangNhap();
                f.ShowDialog();
                this.Close(); // Đóng form hiện tại sau khi form đăng nhập đóng
            }
        }

        // Nút Thoát
        private void button9_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn thoát chương trình không?",
                                                "Xác nhận thoát",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. SỬA LỖI: Lấy nội dung từ ô txtTimKiem
                // (Bạn cần thêm 1 TextBox tên là txtTimKiem vào form)
                string maSP_TimKiem = txtMaSP.Text.Trim();

                // 2. Nếu không nhập gì, báo lỗi
                if (string.IsNullOrWhiteSpace(maSP_TimKiem))
                {
                    MessageBox.Show("Vui lòng nhập Mã sản phẩm vào ô tìm kiếm.", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 3. Xây dựng câu truy vấn
                string query = $"SELECT * FROM SAN_PHAM WHERE MaSP = N'{maSP_TimKiem}'";

                // 4. SỬA LỖI: Gọi hàm mới để tải và bind
                LoadAndBindData(query);

                // 5. Thông báo nếu tìm không thấy
                if (bds.Count == 0) // bds.Count là số dòng tìm thấy
                {
                    MessageBox.Show("Không tìm thấy sản phẩm nào có mã này.", "Kết quả tìm kiếm",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.ToString(), "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. SỬA LỖI: Xóa trắng ô tìm kiếm (nếu có)
                if (txtMaSP != null)
                {
                    txtMaSP.Text = "";
                }

                // 2. SỬA LỖI: Tải lại toàn bộ dữ liệu
                LoadAndBindData("SELECT * FROM SAN_PHAM");

                // 3. Di chuyển con trỏ chuột về ô đầu tiên
                txtMaSP.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi làm mới dữ liệu: " + ex.Message, "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            // 1. Tạo đối tượng SaveFileDialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Chọn nơi để lưu danh sách";
            saveFileDialog.FileName = "DanhSachSanPham.xls";

            // 2. Hiển thị hộp thoại và kiểm tra
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 3. Mở file để ghi (StreamWriter)
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, System.Text.Encoding.Unicode))
                    {
                        // -- A. GHI DÒNG TIÊU ĐỀ (HEADER) --
                        string headerLine = "";
                        foreach (DataGridViewColumn col in dgvSP.Columns)
                        {
                            headerLine += col.HeaderText + "\t";
                        }
                        sw.WriteLine(headerLine.Trim());

                        // -- B. GHI DỮ LIỆU CÁC DÒNG (ROWS) --
                        foreach (DataGridViewRow row in dgvSP.Rows)
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
                    } // 'using' sẽ tự động đóng file

                    // 4. Thông báo thành công
                    MessageBox.Show("Xuất file Excel thành công!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // 5. Báo lỗi nếu có sự cố
                    MessageBox.Show("Lỗi khi xuất file Excel: " + ex.ToString(), "Lỗi",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtKhachHang_Click(object sender, EventArgs e)
        {
            FrmKhachHang f = new FrmKhachHang();
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