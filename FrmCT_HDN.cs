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
    public partial class FrmCT_HDN : Form
    {

        KetNoi kn = new KetNoi();
        BindingSource bds = new BindingSource();
        public FrmCT_HDN()
        {
            InitializeComponent();
        }
        private void LoadAndBindData(string query)
        {
            try
            {
                // 1. Lấy dữ liệu
                DataTable dt = kn.ExecuteQuery(query);

                // 2. Gán cho BindingSource
                bds.DataSource = dt;

                // 3. Gán cho DataGridView
                dgvChiTietHDN.DataSource = bds;

                // 4. Gỡ Binding cũ
                txtSoHDN.DataBindings.Clear();
                txtMaSP.DataBindings.Clear();
                txtSoLuong.DataBindings.Clear();
                txtDonGia.DataBindings.Clear();
                txtGiamGia.DataBindings.Clear();
                txtThanhTien.DataBindings.Clear();

                // 5. Thêm Binding mới (Lưu ý: SoHDN thay vì SoHDB)
                txtSoHDN.DataBindings.Add("Text", bds, "SoHDN", true, DataSourceUpdateMode.Never);
                txtMaSP.DataBindings.Add("Text", bds, "MaSP", true, DataSourceUpdateMode.Never);
                txtSoLuong.DataBindings.Add("Text", bds, "SoLuong", true, DataSourceUpdateMode.Never);
                txtDonGia.DataBindings.Add("Text", bds, "DonGia", true, DataSourceUpdateMode.Never);
                txtGiamGia.DataBindings.Add("Text", bds, "GiamGia", true, DataSourceUpdateMode.Never);
                txtThanhTien.DataBindings.Add("Text", bds, "ThanhTien", true, DataSourceUpdateMode.Never);

                // 6. Cấu hình DataGridView
                dgvChiTietHDN.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dgvChiTietHDN.Columns["SoHDN"].HeaderText = "Số HĐ Nhập";
                dgvChiTietHDN.Columns["MaSP"].HeaderText = "Mã Sản Phẩm";
                dgvChiTietHDN.Columns["SoLuong"].HeaderText = "Số Lượng";
                dgvChiTietHDN.Columns["DonGia"].HeaderText = "Đơn Giá Nhập";
                dgvChiTietHDN.Columns["GiamGia"].HeaderText = "Giảm Giá";
                dgvChiTietHDN.Columns["ThanhTien"].HeaderText = "Thành Tiền";
            }
            catch (Exception) { }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            FrmHDN f = new FrmHDN();
            f.ShowDialog();
        }
        private void TinhThanhTien(object sender, EventArgs e)
        {
            try
            {
                decimal soLuong = 0, donGia = 0, giamGia = 0;

                decimal.TryParse(txtSoLuong.Text, out soLuong);
                decimal.TryParse(txtDonGia.Text, out donGia);
                decimal.TryParse(txtGiamGia.Text, out giamGia);

                decimal thanhTien = (soLuong * donGia) - giamGia;
                txtThanhTien.Text = thanhTien.ToString("0.##");
            }
            catch { }
        }
        private void FrmCT_HDN_Load(object sender, EventArgs e)
        {
            // Gán BindingNavigator nếu có
            bindingNavigator1.BindingSource = bds;

            LoadAndBindData("SELECT * FROM CHITIET_HDN");
            txtThanhTien.Enabled = false;

            // Gán sự kiện tính tiền tự động
            this.txtSoLuong.TextChanged += new EventHandler(TinhThanhTien);
            this.txtDonGia.TextChanged += new EventHandler(TinhThanhTien);
            this.txtGiamGia.TextChanged += new EventHandler(TinhThanhTien);
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtSoHDN.Text = "";
            txtMaSP.Text = "";
            txtSoLuong.Text = "0";
            txtDonGia.Text = "0";
            txtGiamGia.Text = "0";
            txtThanhTien.Text = "0";
            txtSoHDN.Focus();
            LoadAndBindData("SELECT * FROM CHITIET_HDN");
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                string soHDN = txtSoHDN.Text.Trim(); // Lưu ý biến soHDN
                string maSP = txtMaSP.Text.Trim();

                if (string.IsNullOrEmpty(soHDN) || string.IsNullOrEmpty(maSP))
                {
                    MessageBox.Show("Số HĐ Nhập và Mã SP không được để trống.", "Cảnh báo");
                    return;
                }

                string soLuong = string.IsNullOrEmpty(txtSoLuong.Text) ? "0" : txtSoLuong.Text;
                string donGia = string.IsNullOrEmpty(txtDonGia.Text) ? "0" : txtDonGia.Text;
                string giamGia = string.IsNullOrEmpty(txtGiamGia.Text) ? "0" : txtGiamGia.Text;
                string thanhTien = txtThanhTien.Text;

                // Insert vào CHITIET_HDN
                string qr = $"INSERT INTO CHITIET_HDN (SoHDN, MaSP, SoLuong, DonGia, GiamGia, ThanhTien) " +
                            $"VALUES(N'{soHDN}', N'{maSP}', {soLuong}, {donGia}, {giamGia}, {thanhTien})";

                int kq = kn.ExecuteNonQuery(qr);
                if (kq > 0)
                {
                    MessageBox.Show("Thêm chi tiết nhập thành công!");
                    LoadAndBindData("SELECT * FROM CHITIET_HDN");
                }
                else MessageBox.Show("Thêm thất bại (Trùng khóa chính).");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm: " + ex.Message);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                string soHDN = txtSoHDN.Text.Trim();
                string maSP = txtMaSP.Text.Trim();

                string soLuong = txtSoLuong.Text;
                string donGia = txtDonGia.Text;
                string giamGia = txtGiamGia.Text;
                string thanhTien = txtThanhTien.Text;

                // Update bảng CHITIET_HDN
                string qr = $"UPDATE CHITIET_HDN SET " +
                            $"SoLuong = {soLuong}, " +
                            $"DonGia = {donGia}, " +
                            $"GiamGia = {giamGia}, " +
                            $"ThanhTien = {thanhTien} " +
                            $"WHERE SoHDN = N'{soHDN}' AND MaSP = N'{maSP}'";

                int kq = kn.ExecuteNonQuery(qr);
                if (kq > 0)
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadAndBindData("SELECT * FROM CHITIET_HDN");
                }
                else MessageBox.Show("Không tìm thấy dữ liệu để sửa.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi sửa: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string soHDN = txtSoHDN.Text;
            string maSP = txtMaSP.Text;

            if (string.IsNullOrEmpty(soHDN) || string.IsNullOrEmpty(maSP))
            {
                MessageBox.Show("Chọn dòng cần xóa trước.");
                return;
            }

            DialogResult result = MessageBox.Show(
               $"Xóa SP {maSP} khỏi HĐ Nhập {soHDN}?",
               "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Delete bảng CHITIET_HDN
                    string qr = $"DELETE FROM CHITIET_HDN WHERE SoHDN = N'{soHDN}' AND MaSP = N'{maSP}'";
                    int kq = kn.ExecuteNonQuery(qr);

                    if (kq > 0)
                    {
                        MessageBox.Show("Xóa thành công!");
                        LoadAndBindData("SELECT * FROM CHITIET_HDN");
                        btnLamMoi_Click(sender, e);
                    }
                    else MessageBox.Show("Xóa thất bại.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa: " + ex.Message);
                }
            }

        }



        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string soHDN = txtSoHDN.Text.Trim();
                if (string.IsNullOrWhiteSpace(soHDN))
                {
                    MessageBox.Show("Nhập số HĐ Nhập để tìm.");
                    LoadAndBindData("SELECT * FROM CHITIET_HDN");
                    return;
                }

                string query = $"SELECT * FROM CHITIET_HDN WHERE SoHDN LIKE N'%{soHDN}%'";
                LoadAndBindData(query);

                if (bds.Count == 0) MessageBox.Show("Không tìm thấy.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm: " + ex.Message);
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Lưu file chi tiết nhập";
            saveFileDialog.FileName = "ChiTietHDN.xls";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.Unicode))
                    {
                        string headerLine = "";
                        foreach (DataGridViewColumn col in dgvChiTietHDN.Columns)
                            headerLine += col.HeaderText + "\t";
                        sw.WriteLine(headerLine.Trim());

                        foreach (DataGridViewRow row in dgvChiTietHDN.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                string rowLine = "";
                                foreach (DataGridViewCell cell in row.Cells)
                                    rowLine += (cell.Value ?? "").ToString() + "\t";
                                sw.WriteLine(rowLine.Trim());
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
    }
    }
