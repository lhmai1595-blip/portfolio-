using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Excel = Microsoft.Office.Interop.Excel;

namespace ChuongTrinhQuanLiSach
{
    public partial class FrmNCC : Form
    {
        

        KetNoi kn = new KetNoi();
        BindingSource bds = new BindingSource();
        public FrmNCC()
        {
            InitializeComponent();
        }
        public FrmNCC(string Username, string Password, string Quyen)
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
            dgvNCC.DataSource = bds;

            // (Giả sử bạn có 1 bindingNavigator tên là 'bindingNavigator1')
            bindingNavigator1.BindingSource = bds;

            // 4. Gỡ và cài đặt lại Binding cho các TextBox
            txtMaNCC.DataBindings.Clear();
            txtTenNCC.DataBindings.Clear();
            txtDiaChi.DataBindings.Clear();
            txtDienThoai.DataBindings.Clear();

            // 5. Thêm binding mới
            txtMaNCC.DataBindings.Add("Text", bds, "MaNCC", true, DataSourceUpdateMode.OnPropertyChanged);
            txtTenNCC.DataBindings.Add("Text", bds, "TenNCC", true, DataSourceUpdateMode.OnPropertyChanged);
            txtDiaChi.DataBindings.Add("Text", bds, "DiaChiNCC", true, DataSourceUpdateMode.OnPropertyChanged);
            txtDienThoai.DataBindings.Add("Text", bds,"SdtNCC", true, DataSourceUpdateMode.OnPropertyChanged);

            // 6. Cấu hình DataGridView
            dgvNCC.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            try
            {
                // Đặt tên cột Tiếng Việt
                dgvNCC.Columns["MaNCC"].HeaderText = "Mã NCC";
                dgvNCC.Columns["TenNCC"].HeaderText = "Tên NCC";
                dgvNCC.Columns["DiaChiNCC"].HeaderText = "Địa Chỉ";
                dgvNCC.Columns["SdtNCC"].HeaderText = "Điện Thoại";
            }
            catch (Exception) { /* Bỏ qua nếu lỗi */ }
        }
        private void txtDienThoai_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            //{
            //    e.Handled = true;
            //}
        }

        private void dgvNCC_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
        private void load()
        {
            dgvNCC.DataSource = kn.LayDuLieu("NHA_CUNG_CAP");
        }
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            // Xóa trắng các ô
            txtMaNCC.Text = "";
            txtTenNCC.Text = "";
            txtDiaChi.Text = "";
            txtDienThoai.Text = "";

            // Tải lại toàn bộ dữ liệu
            LoadAndBindData("SELECT * FROM NHA_CUNG_CAP");

            // Kích hoạt lại các nút và ô Mã
            txtMaNCC.Enabled = true;
            btnThem.Enabled = true;
            btnTimKiem.Enabled = true;
            txtMaNCC.Focus();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // 1. Validate dữ liệu
            if (string.IsNullOrWhiteSpace(txtMaNCC.Text))
            {
                MessageBox.Show("Hãy nhập mã nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtMaNCC.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTenNCC.Text))
            {
                MessageBox.Show("Hãy nhập tên nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtTenNCC.Focus();
                return;
            }

            try
            {
                // 2. Lấy dữ liệu
                string maNCC = txtMaNCC.Text.Trim();
                string tenNCC = txtTenNCC.Text.Trim();
                string diaChi = txtDiaChi.Text.Trim();
                string dienThoai = txtDienThoai.Text.Trim();

                // 3. Tạo câu query *** (SỬA TÊN CỘT Ở ĐÂY) ***
                string qr = $"INSERT INTO NHA_CUNG_CAP (MaNCC, TenNCC, DiaChiNCC, SdtNCC) " +
                              $"VALUES(N'{maNCC}', N'{tenNCC}', N'{diaChi}', N'{dienThoai}')";

                int kq = kn.ExecuteNonQuery(qr);
                if (kq > 0)
                {
                    MessageBox.Show("Thêm " + maNCC + " thành công!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Thêm không thành công.");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("PRIMARY KEY"))
                    MessageBox.Show("Mã nhà cung cấp này đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Có lỗi xảy ra khi thêm dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // 4. Tải lại
            LoadAndBindData("SELECT * FROM NHA_CUNG_CAP");
        }


        private void btnSua_Click(object sender, EventArgs e)
        {
            // 1. Validate
            if (string.IsNullOrWhiteSpace(txtTenNCC.Text))
            {
                MessageBox.Show("Tên nhà cung cấp không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtTenNCC.Focus();
                return;
            }

            try
            {
                // 2. Lấy dữ liệu
                string maNCC = txtMaNCC.Text.Trim();
                string tenNCC = txtTenNCC.Text.Trim();
                string diaChi = txtDiaChi.Text.Trim();
                string dienThoai = txtDienThoai.Text.Trim();

                // 3. Tạo câu query *** (SỬA TÊN CỘT Ở ĐÂY) ***
                string qr = $"UPDATE NHA_CUNG_CAP SET " +
                            $"TenNCC = N'{tenNCC}', " +
                            $"DiaChiNCC = N'{diaChi}', " +
                            $"SdtNCC = N'{dienThoai}' " +
                            $"WHERE MaNCC = N'{maNCC}'";

                int kq = kn.ExecuteNonQuery(qr);
                if (kq > 0)
                {
                    MessageBox.Show("Cập nhật " + maNCC + " thành công!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Sửa không thành công. Mã NCC có thể không tồn tại.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi sửa dữ liệu: " + ex.Message);
            }

            // 4. Tải lại
            LoadAndBindData("SELECT * FROM NHA_CUNG_CAP");
            txtMaNCC.Enabled = true;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string maNCC = txtMaNCC.Text;
            if (string.IsNullOrEmpty(maNCC))
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Bạn có chắc muốn xóa nhà cung cấp {maNCC} không?\n(Mọi Hóa Đơn Nhập của NCC này cũng sẽ bị xóa!)",
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
                // 1. *** XỬ LÝ KHÓA NGOẠI ***
                // Xóa các Hóa Đơn Nhập (bảng con) của NCC này trước
                string qr_delete_child = $"DELETE FROM HOA_DON_NHAP WHERE MaNCC = N'{maNCC}'";
                kn.ExecuteNonQuery(qr_delete_child);

                // (Nếu có bảng CHITIET_HDN, bạn phải xóa CHITIET_HDN -> HOA_DON_NHAP -> NHA_CUNG_CAP)

                // 2. Xóa NCC (bảng cha)
                int kq = kn.ExecuteNonQuery($"DELETE FROM NHA_CUNG_CAP WHERE MaNCC = N'{maNCC}'");

                if (kq > 0)
                {
                    MessageBox.Show("Xóa đơn hàng " + maNCC + " thành công!", "Thông báo",
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
            LoadAndBindData("SELECT * FROM NHA_CUNG_CAP");
            txtMaNCC.Enabled = true; // Cho phép nhập lại mã
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            // Tìm kiếm dựa trên các ô thông tin
            try
            {
                string maNCC = txtMaNCC.Text.Trim();
                string tenNCC = txtTenNCC.Text.Trim();
                string diaChi = txtDiaChi.Text.Trim();
                string dienThoai = txtDienThoai.Text.Trim();

                string query = "SELECT * FROM NHA_CUNG_CAP";
                List<string> conditions = new List<string>();

                if (!string.IsNullOrWhiteSpace(maNCC))
                {
                    conditions.Add($"MaNCC LIKE N'%{maNCC}%'");
                }
                if (!string.IsNullOrWhiteSpace(tenNCC))
                {
                    conditions.Add($"TenNCC LIKE N'%{tenNCC}%'");
                }
                // *** (SỬA TÊN CỘT Ở ĐÂY) ***
                if (!string.IsNullOrWhiteSpace(diaChi))
                {
                    conditions.Add($"DiaChiNCC LIKE N'%{diaChi}%'");
                }
                // *** (SỬA TÊN CỘT Ở ĐÂY) ***
                if (!string.IsNullOrWhiteSpace(dienThoai))
                {
                    conditions.Add($"SdtNCC LIKE N'%{dienThoai}%'");
                }

                if (conditions.Count > 0)
                {
                    query += " WHERE " + string.Join(" AND ", conditions);
                }

                LoadAndBindData(query);

                if (bds.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy nhà cung cấp nào.", "Kết quả tìm kiếm",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.ToString(), "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Xuất danh sách sang Excel";
            saveFileDialog.Filter = "Sổ làm việc Excel|*.xlsx|Sổ làm việc Excel 97-2003|*.xls";
            saveFileDialog.FileName = "NhaCungCap";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Excel.Application application = new Excel.Application();
                    application.Application.Workbooks.Add(Type.Missing);
                    Excel.Workbook exBook = application.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
                    Excel.Worksheet exSheet = (Excel.Worksheet)exBook.Worksheets[1];

                    exSheet.get_Range("A1").Font.Bold = true;
                    exSheet.get_Range("A1").Value = "Nhà cung cấp";
                    application.Cells[2, 1] = "Số thứ tự";

                    for (int i = 0; i < dgvNCC.Columns.Count; i++)
                    {
                        application.Cells[2, i + 2] = dgvNCC.Columns[i].HeaderText;
                    }

                    for (int i = 0; i < dgvNCC.Rows.Count - 1; i++)
                    {
                        for (int j = 0; j < dgvNCC.Columns.Count; j++)
                        {
                            application.Cells[i + 3, 1] = (i + 1).ToString();
                            application.Cells[i + 3, j + 2] = dgvNCC.Rows[i].Cells[j].Value;
                        }
                    }

                    application.Columns.AutoFit();
                    application.ActiveWorkbook.SaveCopyAs(saveFileDialog.FileName);
                    application.ActiveWorkbook.Saved = true;

                    MessageBox.Show("Xuất danh sách sang Excel thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

       
       
        private void FrmNCC_Load(object sender, EventArgs e)
        {
            // Tải toàn bộ dữ liệu và bind lần đầu
            LoadAndBindData("SELECT * FROM NHA_CUNG_CAP");
            // Cho phép nhập liệu ngay
            txtMaNCC.Enabled = true;
            btnThem.Enabled = true;
            btnTimKiem.Enabled = true;
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

        private void txtMaNCC_TextChanged(object sender, EventArgs e)
        {

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

        private void txtKhachHang_Click(object sender, EventArgs e)
        {
            FrmKhachHang f = new FrmKhachHang();
            f.ShowDialog();
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

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnBaoCao_Click(object sender, EventArgs e)
        {
            FrmBaoCao f = new FrmBaoCao();
            f.ShowDialog();
        }
    }
}
