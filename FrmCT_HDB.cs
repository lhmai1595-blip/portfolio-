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
    public partial class FrmCT_HDB : Form
    {
        KetNoi kn = new KetNoi();
        BindingSource bds = new BindingSource();
        public FrmCT_HDB()
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
            dgvChiTietHDB.DataSource = bds;

            // 4. Gỡ Binding cũ (tránh lỗi lặp binding)
            txtSoHDB.DataBindings.Clear();
            txtMaSP.DataBindings.Clear();
            txtSoLuong.DataBindings.Clear();
            txtDonGia.DataBindings.Clear();
            txtGiamGia.DataBindings.Clear();
            txtThanhTien.DataBindings.Clear();

            // 5. Thêm Binding mới (Liên kết ô nhập với cột trong database)
            txtSoHDB.DataBindings.Add("Text", bds, "SoHDB", true, DataSourceUpdateMode.OnPropertyChanged);
            txtMaSP.DataBindings.Add("Text", bds, "MaSP", true, DataSourceUpdateMode.OnPropertyChanged);
            txtSoLuong.DataBindings.Add("Text", bds, "SoLuong", true, DataSourceUpdateMode.OnPropertyChanged);
            txtDonGia.DataBindings.Add("Text", bds, "DonGia", true, DataSourceUpdateMode.OnPropertyChanged); // Format tiền tệ nếu cần
            txtGiamGia.DataBindings.Add("Text", bds, "GiamGia", true, DataSourceUpdateMode.OnPropertyChanged);
            txtThanhTien.DataBindings.Add("Text", bds, "ThanhTien", true, DataSourceUpdateMode.OnPropertyChanged);

            // 6. Cấu hình DataGridView
            dgvChiTietHDB.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            try
            {
                dgvChiTietHDB.Columns["SoHDB"].HeaderText = "Số Hóa Đơn";
                dgvChiTietHDB.Columns["MaSP"].HeaderText = "Mã Sản Phẩm";
                dgvChiTietHDB.Columns["SoLuong"].HeaderText = "Số Lượng";
                dgvChiTietHDB.Columns["DonGia"].HeaderText = "Đơn Giá";
                dgvChiTietHDB.Columns["GiamGia"].HeaderText = "Giảm Giá";
                dgvChiTietHDB.Columns["ThanhTien"].HeaderText = "Thành Tiền";
            }
            catch (Exception) { }
        }

        // --- SỰ KIỆN LOAD FORM ---
        private void FrmChiTietHDB_Load(object sender, EventArgs e)
        {
            // Tải dữ liệu khi mở form
            LoadAndBindData("SELECT * FROM CHITIET_HDB");
            txtThanhTien.Enabled = false; // Khóa ô thành tiền
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FrmCT_HDB_Load(object sender, EventArgs e)
        {
            bindingNavigator1.BindingSource = bds;
            LoadAndBindData("SELECT * FROM CHITIET_HDB");
        }
        private void TinhThanhTien(object sender, EventArgs e)
        {
            try
            {
                double soLuong = 0, donGia = 0, giamGia = 0;

                double.TryParse(txtSoLuong.Text, out soLuong);
                double.TryParse(txtDonGia.Text, out donGia);
                double.TryParse(txtGiamGia.Text, out giamGia);

                double thanhTien = (soLuong * donGia) - giamGia;

                // Cập nhật vào ô text, vì đã binding nên nó sẽ tự update vào bds nếu đúng mode
                txtThanhTien.Text = thanhTien.ToString();
            }
            catch { }
        }
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtSoHDB.Text = "";
            txtMaSP.Text = "";
            txtSoLuong.Text = "0";
            txtDonGia.Text = "0";
            txtGiamGia.Text = "0";
            txtThanhTien.Text = "0";
            txtSoHDB.Focus();
            LoadAndBindData("SELECT * FROM CHITIET_HDB");
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                string soHDB = txtSoHDB.Text.Trim();
                string maSP = txtMaSP.Text.Trim();

                // Kiểm tra rỗng
                if (string.IsNullOrEmpty(soHDB) || string.IsNullOrEmpty(maSP))
                {
                    MessageBox.Show("Số HĐ và Mã SP không được để trống.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy giá trị số
                string soLuong = string.IsNullOrEmpty(txtSoLuong.Text) ? "0" : txtSoLuong.Text;
                string donGia = string.IsNullOrEmpty(txtDonGia.Text) ? "0" : txtDonGia.Text;
                string giamGia = string.IsNullOrEmpty(txtGiamGia.Text) ? "0" : txtGiamGia.Text;
                string thanhTien = txtThanhTien.Text; // Đã tính ở hàm trên

                // Query Insert
                string qr = $"INSERT INTO CHITIET_HDB (SoHDB, MaSP, SoLuong, DonGia, GiamGia, ThanhTien) " +
                            $"VALUES(N'{soHDB}', N'{maSP}', {soLuong}, {donGia}, {giamGia}, {thanhTien})";

                int kq = kn.ExecuteNonQuery(qr);
                if (kq > 0)
                    MessageBox.Show("Thêm chi tiết thành công!", "Thông báo");
                else
                    MessageBox.Show("Thêm thất bại. Có thể trùng khóa chính.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm: " + ex.Message);
            }

            LoadAndBindData("SELECT * FROM CHITIET_HDB");
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                string soHDB = txtSoHDB.Text.Trim();
                string maSP = txtMaSP.Text.Trim();

                // Lấy giá trị mới
                string soLuong = txtSoLuong.Text;
                string donGia = txtDonGia.Text;
                string giamGia = txtGiamGia.Text;
                string thanhTien = txtThanhTien.Text;

                // Query Update (Chú ý điều kiện WHERE phải có cả SoHDB và MaSP)
                string qr = $"UPDATE CHITIET_HDB SET " +
                            $"SoLuong = {soLuong}, " +
                            $"DonGia = {donGia}, " +
                            $"GiamGia = {giamGia}, " +
                            $"ThanhTien = {thanhTien} " +
                            $"WHERE SoHDB = N'{soHDB}' AND MaSP = N'{maSP}'";

                int kq = kn.ExecuteNonQuery(qr);
                if (kq > 0)
                    MessageBox.Show("Cập nhật thành công!", "Thông báo");
                else
                    MessageBox.Show("Không tìm thấy dòng dữ liệu để sửa.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa: " + ex.Message);
            }

            LoadAndBindData("SELECT * FROM CHITIET_HDB");
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string soHDB = txtSoHDB.Text;
            string maSP = txtMaSP.Text;

            if (string.IsNullOrEmpty(soHDB) || string.IsNullOrEmpty(maSP))
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa.", "Thông báo");
                return;
            }

            DialogResult result = MessageBox.Show(
               $"Bạn có chắc muốn xóa SP {maSP} trong Hóa đơn {soHDB} không?",
               "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    string qr = $"DELETE FROM CHITIET_HDB WHERE SoHDB = N'{soHDB}' AND MaSP = N'{maSP}'";
                    int kq = kn.ExecuteNonQuery(qr);

                    if (kq > 0) MessageBox.Show("Xóa thành công!");
                    else MessageBox.Show("Xóa thất bại.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa: " + ex.Message);
                }
                LoadAndBindData("SELECT * FROM CHITIET_HDB");
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string soHDB = txtSoHDB.Text.Trim();
                if (string.IsNullOrWhiteSpace(soHDB))
                {
                    MessageBox.Show("Nhập số Hóa đơn để tìm chi tiết.", "Thông báo");
                    return;
                }

                string query = $"SELECT * FROM CHITIET_HDB WHERE SoHDB LIKE N'%{soHDB}%'";
                LoadAndBindData(query);

                if (bds.Count == 0) MessageBox.Show("Không tìm thấy chi tiết nào.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Chọn nơi lưu chi tiết hóa đơn";
            saveFileDialog.FileName = "ChiTietHDB.xls";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.Unicode))
                    {
                        // Viết tiêu đề cột
                        string headerLine = "";
                        foreach (DataGridViewColumn col in dgvChiTietHDB.Columns)
                        {
                            headerLine += col.HeaderText + "\t";
                        }
                        sw.WriteLine(headerLine.Trim());

                        // Viết dữ liệu dòng
                        foreach (DataGridViewRow row in dgvChiTietHDB.Rows)
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
                    MessageBox.Show("Xuất file Excel thành công!", "Thông báo");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất file: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmHDB f = new FrmHDB();
            f.ShowDialog();
        }
    }
}
