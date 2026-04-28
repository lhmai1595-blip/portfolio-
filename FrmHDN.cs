using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization; // Cần để xử lý ngày tháng, tiền tệ
using System.IO; // Cần để Xuất Excel
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChuongTrinhQuanLiSach
{

    public partial class FrmHDN : Form
    {

        KetNoi kn = new KetNoi();
        BindingSource bds = new BindingSource();
        public FrmHDN()
        {
            InitializeComponent();
        }
        private void LoadAndBindData(string query)
        {
            // a. Lấy dữ liệu từ CSDL
            DataTable dt = kn.ExecuteQuery(query);

            // b. Gán vào BindingSource
            bds.DataSource = dt;

            // c. Gán vào DataGridView
            dgvHDN.DataSource = bds;

            // (Nếu có BindingNavigator: bindingNavigator1.BindingSource = bds;)

            // d. Xóa và tạo lại Binding cho các TextBox
            txtSoHDN.DataBindings.Clear();
            txtNgayNhap.DataBindings.Clear();
            txtTongTien.DataBindings.Clear();
            txtThueVAT.DataBindings.Clear();
            txtMaNV.DataBindings.Clear();
            txtMaNCC.DataBindings.Clear();

            // Lưu ý: Tên trường "SoHDN", "MaNCC"... phải đúng y hệt trong SQL Server
            txtSoHDN.DataBindings.Add("Text", bds, "SoHDN", true, DataSourceUpdateMode.OnPropertyChanged);
            txtNgayNhap.DataBindings.Add("Text", bds, "NgayNhap", true, DataSourceUpdateMode.OnPropertyChanged);
            txtTongTien.DataBindings.Add("Text", bds, "TongTien", true, DataSourceUpdateMode.OnPropertyChanged);
            txtThueVAT.DataBindings.Add("Text", bds, "ThueVAT", true, DataSourceUpdateMode.OnPropertyChanged);
            txtMaNV.DataBindings.Add("Text", bds, "MaNV", true, DataSourceUpdateMode.OnPropertyChanged);
            txtMaNCC.DataBindings.Add("Text", bds, "MaNCC", true, DataSourceUpdateMode.OnPropertyChanged);

            // e. Cấu hình hiển thị cho DataGridView
            dgvHDN.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            try
            {
                dgvHDN.Columns["SoHDN"].HeaderText = "Số HĐN";
                dgvHDN.Columns["NgayNhap"].HeaderText = "Ngày Nhập";
                dgvHDN.Columns["TongTien"].HeaderText = "Tổng Tiền";
                dgvHDN.Columns["ThueVAT"].HeaderText = "Thuế VAT";
                dgvHDN.Columns["MaNV"].HeaderText = "Mã Nhân Viên";
                dgvHDN.Columns["MaNCC"].HeaderText = "Mã Nhà Cung Cấp";

                // Định dạng ngày tháng và tiền tệ
                dgvHDN.Columns["NgayNhap"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvHDN.Columns["TongTien"].DefaultCellStyle.Format = "N0"; // Dấu phẩy ngăn cách hàng nghìn
            }
            catch (Exception) { /* Bỏ qua nếu không tìm thấy cột */ }
        }
        private void HDM_Load(object sender, EventArgs e)
        {
            bindingNavigator1.BindingSource = bds;
            LoadAndBindData("SELECT * FROM HOA_DON_NHAP");
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validate
                if (string.IsNullOrWhiteSpace(txtSoHDN.Text))
                {
                    MessageBox.Show("Vui lòng nhập Số Hóa Đơn Nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSoHDN.Focus();
                    return;
                }

                // 2. Lấy dữ liệu
                string soHDN = txtSoHDN.Text.Trim();
                string maNV = txtMaNV.Text.Trim();
                string maNCC = txtMaNCC.Text.Trim();

                if (!DateTime.TryParse(txtNgayNhap.Text, out DateTime ngayNhap))
                {
                    MessageBox.Show("Ngày nhập không hợp lệ (định dạng dd/MM/yyyy).");
                    return;
                }
                string ngayNhapSQL = ngayNhap.ToString("yyyy-MM-dd");

                if (!decimal.TryParse(txtTongTien.Text, out decimal tongTien)) tongTien = 0;
                if (!float.TryParse(txtThueVAT.Text, out float thueVAT)) thueVAT = 0;

                // 3. Query INSERT
                string query = $"INSERT INTO HOA_DON_NHAP (SoHDN, NgayNhap, TongTien, ThueVAT, MaNV, MaNCC) " +
                               $"VALUES ('{soHDN}', '{ngayNhapSQL}', {tongTien.ToString(CultureInfo.InvariantCulture)}, {thueVAT.ToString(CultureInfo.InvariantCulture)}, '{maNV}', '{maNCC}')";

                // 4. Execute
                int kq = kn.ExecuteNonQuery(query);
                if (kq > 0) MessageBox.Show("Thêm hóa đơn nhập thành công!");
                else MessageBox.Show("Thêm thất bại.");

                LoadAndBindData("SELECT * FROM HOA_DON_NHAP");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("PRIMARY KEY"))
                    MessageBox.Show("Số Hóa Đơn Nhập này đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSoHDN.Text)) return;

                string soHDN = txtSoHDN.Text.Trim();
                string maNV = txtMaNV.Text.Trim();
                string maNCC = txtMaNCC.Text.Trim();

                if (!DateTime.TryParse(txtNgayNhap.Text, out DateTime ngayNhap))
                {
                    MessageBox.Show("Ngày nhập không hợp lệ.");
                    return;
                }
                string ngayNhapSQL = ngayNhap.ToString("yyyy-MM-dd");

                if (!decimal.TryParse(txtTongTien.Text, out decimal tongTien)) tongTien = 0;
                if (!float.TryParse(txtThueVAT.Text, out float thueVAT)) thueVAT = 0;

                // Query UPDATE
                string query = $"UPDATE HOA_DON_NHAP SET " +
                               $"NgayNhap = '{ngayNhapSQL}', " +
                               $"TongTien = {tongTien.ToString(CultureInfo.InvariantCulture)}, " +
                               $"ThueVAT = {thueVAT.ToString(CultureInfo.InvariantCulture)}, " +
                               $"MaNV = '{maNV}', " +
                               $"MaNCC = '{maNCC}' " +
                               $"WHERE SoHDN = '{soHDN}'";

                int kq = kn.ExecuteNonQuery(query);
                if (kq > 0) MessageBox.Show("Cập nhật thành công!");
                else MessageBox.Show("Cập nhật thất bại.");

                LoadAndBindData("SELECT * FROM HOA_DON_NHAP");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string soHDN = txtSoHDN.Text;
            if (string.IsNullOrEmpty(soHDN))
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần xóa.");
                return;
            }

            DialogResult rs = MessageBox.Show($"Bạn có chắc muốn xóa Hóa đơn nhập {soHDN}?\n(Toàn bộ Chi tiết của hóa đơn này cũng sẽ bị xóa!)",
                                              "Cảnh báo xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (rs == DialogResult.No) return;

            try
            {
                // BƯỚC 1: XÓA BẢNG CON (CHITIET_HDN) TRƯỚC
                // *** Kiểm tra tên bảng trong SQL của bạn là CHITIET_HDN hay tên khác ***
                string deleteChild = $"DELETE FROM CHITIET_HDN WHERE SoHDN = '{soHDN}'";
                kn.ExecuteNonQuery(deleteChild);

                // BƯỚC 2: XÓA BẢNG CHA (HOA_DON_NHAP)
                string deleteParent = $"DELETE FROM HOA_DON_NHAP WHERE SoHDN = '{soHDN}'";
                int kq = kn.ExecuteNonQuery(deleteParent);

                if (kq > 0) MessageBox.Show("Xóa thành công!");
                else MessageBox.Show("Xóa thất bại.");

                LoadAndBindData("SELECT * FROM HOA_DON_NHAP");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtSoHDN.Text = "";
            txtNgayNhap.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtTongTien.Text = "";
            txtThueVAT.Text = "";
            txtMaNV.Text = "";
            txtMaNCC.Text = "";

            LoadAndBindData("SELECT * FROM HOA_DON_NHAP");
            txtSoHDN.Focus();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string soHDN = txtSoHDN.Text.Trim();
                string maNV = txtMaNV.Text.Trim();
                string maNCC = txtMaNCC.Text.Trim();

                string query = "SELECT * FROM HOA_DON_NHAP WHERE 1=1";

                if (!string.IsNullOrEmpty(soHDN))
                    query += $" AND SoHDN LIKE '%{soHDN}%'";

                if (!string.IsNullOrEmpty(maNV))
                    query += $" AND MaNV LIKE '%{maNV}%'";

                if (!string.IsNullOrEmpty(maNCC))
                    query += $" AND MaNCC LIKE '%{maNCC}%'";

                LoadAndBindData(query);

                if (bds.Count == 0) MessageBox.Show("Không tìm thấy hóa đơn nào.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel File|*.xls";
            sfd.FileName = "DanhSachHoaDonNhap.xls";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.Unicode))
                    {
                        // Ghi tiêu đề cột
                        string header = "";
                        foreach (DataGridViewColumn col in dgvHDN.Columns)
                            header += col.HeaderText + "\t";
                        sw.WriteLine(header);

                        // Ghi dữ liệu dòng
                        foreach (DataGridViewRow row in dgvHDN.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                string line = "";
                                foreach (DataGridViewCell cell in row.Cells)
                                    line += (cell.Value?.ToString() ?? "") + "\t";
                                sw.WriteLine(line);
                            }
                        }
                    }
                    MessageBox.Show("Xuất Excel thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất file: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmCT_HDN f = new FrmCT_HDN();
            f.ShowDialog();
        }

        private void txtKhachHang_Click(object sender, EventArgs e)
        {

        }
    }
}
