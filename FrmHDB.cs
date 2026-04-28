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
    public partial class FrmHDB : Form
    {
        KetNoi kn = new KetNoi();
        BindingSource bds = new BindingSource();
        public FrmHDB()
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

            dgvHDB.DataSource = bds;

            
            // Giả sử cái thanh đó tên là bindingNavigator1 (tên mặc định)
            if (bindingNavigator1 != null)
            {
                bindingNavigator1.BindingSource = bds;
            }

            // d. Xóa và tạo lại Binding cho các TextBox (Liên kết ô nhập với bảng)
            txtSoHDB.DataBindings.Clear();
            txtNgayNhap.DataBindings.Clear();
            txtTongTien.DataBindings.Clear();
            txtThueVAT.DataBindings.Clear();
            txtMaNV.DataBindings.Clear();

            // Lưu ý: Tên cột "SoHDB", "NgayBan"... phải đúng y hệt trong SQL Server
            txtSoHDB.DataBindings.Add("Text", bds, "SoHDB", true, DataSourceUpdateMode.OnPropertyChanged);
            txtNgayNhap.DataBindings.Add("Text", bds, "NgayNhap", true, DataSourceUpdateMode.OnPropertyChanged); // Nếu DB là NgayNhap thì sửa thành "NgayNhap"
            txtTongTien.DataBindings.Add("Text", bds, "TongTien", true, DataSourceUpdateMode.OnPropertyChanged);
            txtThueVAT.DataBindings.Add("Text", bds, "ThueVAT", true, DataSourceUpdateMode.OnPropertyChanged);
            txtMaNV.DataBindings.Add("Text", bds, "MaNV", true, DataSourceUpdateMode.OnPropertyChanged);

            // e. Cấu hình hiển thị cho DataGridView
            dgvHDB.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            try
            {
                dgvHDB.Columns["SoHDB"].HeaderText = "Số HĐB";
                dgvHDB.Columns["NgayNhap"].HeaderText = "Ngày Lập";
                dgvHDB.Columns["TongTien"].HeaderText = "Tổng Tiền";
                dgvHDB.Columns["ThueVAT"].HeaderText = "Thuế VAT";
                dgvHDB.Columns["MaNV"].HeaderText = "Mã Nhân Viên";

                // Định dạng ngày tháng và tiền tệ cho đẹp
                dgvHDB.Columns["NgayNhap"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvHDB.Columns["TongTien"].DefaultCellStyle.Format = "N0"; // Số có dấu phẩy ngăn cách hàng nghìn
            }
            catch (Exception) { /* Bỏ qua nếu không tìm thấy cột */ }
        }

      
        // 2. Cập nhật nút THÊM (Chống lỗi rỗng và trùng mã)
        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validate (Kiểm tra dữ liệu đầu vào)
                if (string.IsNullOrWhiteSpace(txtSoHDB.Text))
                {
                    MessageBox.Show("Vui lòng nhập Số Hóa Đơn Bán.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSoHDB.Focus();
                    return;
                }

                // 2. Lấy và Xử lý dữ liệu
                string soHDB = txtSoHDB.Text.Trim();
                string maNV = txtMaNV.Text.Trim();

                // Xử lý ngày tháng (dd/MM/yyyy hoặc yyyy-MM-dd tùy máy, nên dùng DateTime.Parse)
                if (!DateTime.TryParse(txtNgayNhap.Text, out DateTime ngayBan))
                {
                    MessageBox.Show("Ngày nhập không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string ngayBanSQL = ngayBan.ToString("yyyy-MM-dd");

                // Xử lý số (Tiền, Thuế)
                if (!decimal.TryParse(txtTongTien.Text, out decimal tongTien)) tongTien = 0;
                if (!float.TryParse(txtThueVAT.Text, out float thueVAT)) thueVAT = 0;

                // 3. Tạo câu lệnh INSERT
                // Lưu ý: TongTien và ThueVAT là số nên không có dấu nháy đơn ' '
                string query = $"INSERT INTO HOA_DON_BAN (SoHDB, NgayNhap, TongTien, ThueVAT, MaNV) " +
                               $"VALUES ('{soHDB}', '{ngayBanSQL}', {tongTien.ToString(CultureInfo.InvariantCulture)}, {thueVAT.ToString(CultureInfo.InvariantCulture)}, '{maNV}')";

                // 4. Thực thi
                int kq = kn.ExecuteNonQuery(query);
                if (kq > 0) MessageBox.Show("Thêm hóa đơn thành công!");
                else MessageBox.Show("Thêm thất bại.");

                // 5. Tải lại dữ liệu
                LoadAndBindData("SELECT * FROM HOA_DON_BAN");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("PRIMARY KEY"))
                    MessageBox.Show("Số Hóa Đơn này đã tồn tại.", "Lỗi trùng mã", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Lỗi: " + ex.Message);
            }
        }


       
     
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dgvHDB_SelectionChanged(object sender, EventArgs e)
        {
            
        }

       

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSoHDB.Text)) return;

                string soHDB = txtSoHDB.Text.Trim();
                string maNV = txtMaNV.Text.Trim();

                if (!DateTime.TryParse(txtNgayNhap.Text, out DateTime ngayBan))
                {
                    MessageBox.Show("Ngày không hợp lệ.");
                    return;
                }
                string ngayBanSQL = ngayBan.ToString("yyyy-MM-dd");

                if (!decimal.TryParse(txtTongTien.Text, out decimal tongTien)) tongTien = 0;
                if (!float.TryParse(txtThueVAT.Text, out float thueVAT)) thueVAT = 0;

                // Câu lệnh UPDATE
                string query = $"UPDATE HOA_DON_BAN SET " +
                               $"NgayNhap = '{ngayBanSQL}', " +
                               $"TongTien = {tongTien.ToString(CultureInfo.InvariantCulture)}, " +
                               $"ThueVAT = {thueVAT.ToString(CultureInfo.InvariantCulture)}, " +
                               $"MaNV = '{maNV}' " +
                               $"WHERE SoHDB = '{soHDB}'";

                int kq = kn.ExecuteNonQuery(query);
                if (kq > 0) MessageBox.Show("Cập nhật thành công!");
                else MessageBox.Show("Cập nhật thất bại.");

                LoadAndBindData("SELECT * FROM HOA_DON_BAN");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string soHDB = txtSoHDB.Text;
            if (string.IsNullOrEmpty(soHDB))
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần xóa.");
                return;
            }

            DialogResult rs = MessageBox.Show($"Bạn có chắc muốn xóa Hóa đơn {soHDB}?\n(Toàn bộ Chi tiết của hóa đơn này cũng sẽ bị xóa!)",
                                              "Cảnh báo xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (rs == DialogResult.No) return;

            try
            {
                // BƯỚC 1: XÓA BẢNG CON (CHITIET_HDB) TRƯỚC
                // *** Bạn cần kiểm tra xem bảng chi tiết trong SQL tên là gì (ví dụ: CHITIET_HDB hay CHITIET_HOADON_BAN) ***
                string deleteChild = $"DELETE FROM CHITIET_HDB WHERE SoHDB = '{soHDB}'";
                kn.ExecuteNonQuery(deleteChild);

                // BƯỚC 2: XÓA BẢNG CHA (HOA_DON_BAN)
                string deleteParent = $"DELETE FROM HOA_DON_BAN WHERE SoHDB = '{soHDB}'";
                int kq = kn.ExecuteNonQuery(deleteParent);

                if (kq > 0) MessageBox.Show("Xóa thành công!");
                else MessageBox.Show("Xóa thất bại.");

                LoadAndBindData("SELECT * FROM HOA_DON_BAN");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtSoHDB.Text = "";
            txtNgayNhap.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtTongTien.Text = "0";
            txtThueVAT.Text = "0";
            txtMaNV.Text = "";

            LoadAndBindData("SELECT * FROM HOA_DON_BAN");
            txtSoHDB.Enabled = true;
            btnThem.Enabled = true;
            btnTimKiem.Enabled = true;
           
            txtSoHDB.Focus();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string soHDB = txtSoHDB.Text.Trim();
            string maNV = txtMaNV.Text.Trim();

            if (string.IsNullOrEmpty(soHDB) && string.IsNullOrEmpty(maNV))
            {
                MessageBox.Show("Nhập Số HĐB hoặc Mã NV để tìm!");
                return;
            }

            string query = "SELECT * FROM HOA_DON_BAN WHERE 1=1 "; // Mẹo 1=1 để nối chuỗi dễ hơn

            if (!string.IsNullOrEmpty(soHDB))
                query += $" AND SoHDB LIKE '%{soHDB}%'";

            if (!string.IsNullOrEmpty(maNV))
                query += $" AND MaNV LIKE '%{maNV}%'";

            LoadAndBindData(query);
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
            saveFileDialog.FileName = "DanhSachHoaDonBan.xls";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.Unicode))
                    {
                        // Tiêu đề cột
                        for (int i = 0; i < dgvHDB.Columns.Count; i++)
                        {
                            sw.Write(dgvHDB.Columns[i].HeaderText + "\t");
                        }
                        sw.WriteLine();

                        // Dữ liệu dòng
                        foreach (DataGridViewRow row in dgvHDB.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                for (int i = 0; i < dgvHDB.Columns.Count; i++)
                                {
                                    sw.Write((row.Cells[i].Value ?? "").ToString() + "\t");
                                }
                                sw.WriteLine();
                            }
                        }
                    }
                    MessageBox.Show("Xuất Excel thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất Excel: " + ex.Message);
                }
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

        private void btnNCC_Click(object sender, EventArgs e)
        {
            FrmNCC f = new FrmNCC();
            f.ShowDialog();
        }

        private void btnHDB_Click(object sender, EventArgs e)
        {

        }

        private void btnBaoCao_Click(object sender, EventArgs e)
        {
            FrmBaoCao f = new FrmBaoCao();
            f.ShowDialog();
        }

        private void FrmHDB_Load(object sender, EventArgs e)
        {
            bindingNavigator1.BindingSource = bds;
            // Tải toàn bộ dữ liệu và bind lần đầu
            LoadAndBindData("SELECT * FROM HOA_DON_BAN");
            // Cho phép nhập liệu ngay
            
           
        }

        private void dgvHDB_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void bindingNavigator1_RefreshItems(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmCT_HDB f = new FrmCT_HDB();
            f.ShowDialog();
        }
    }
}
