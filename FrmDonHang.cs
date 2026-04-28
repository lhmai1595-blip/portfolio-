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
    public partial class FrmDonHang : Form
    {
        KetNoi kn = new KetNoi();
        BindingSource bds = new BindingSource();
        public FrmDonHang()
        {
            InitializeComponent();
        }
        private void setbinding()
        {
            bds.DataSource = kn.LayDuLieu("DON_DAT_HANG");
            dgvDH.DataSource = bds;
            bindingNavigator1.BindingSource = bds;
            bindingNavigator1.Visible = true;
            txtMaDH.DataBindings.Add("Text", bds, "MaDDH");
            txtNgayDat.DataBindings.Add("Text", bds, "NgayDat");
            txtTriGia.DataBindings.Add("Text", bds, "TriGiaDH");
            txtNgayNhan.DataBindings.Add("Text", bds, "NgayNhan");
            txtMaKH.DataBindings.Add("Text", bds, "MaKH");
            hienthi();
        }

        private void hienthi()
        {
            if (dgvDH.Columns["NgayDat"] != null)
            {
                dgvDH.Columns["NgayDat"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }
            if (dgvDH.Columns["NgayNhan"] != null)
            {
                dgvDH.Columns["NgayNhan"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }
            if (dgvDH.CurrentRow != null)
            {
                txtMaDH.Text = dgvDH.CurrentRow.Cells[0].Value.ToString();
                txtNgayDat.Text = dgvDH.CurrentRow.Cells[1].Value.ToString();
                txtTriGia.Text = dgvDH.CurrentRow.Cells[2].Value.ToString();
                txtNgayNhan.Text = dgvDH.CurrentRow.Cells[3].Value.ToString();
                txtMaKH.Text = dgvDH.CurrentRow.Cells[4].Value.ToString();
            }
            dgvDH.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void load()
        {
            dgvDH.DataSource = kn.LayDuLieu("DON_DAT_HANG");
        }
        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void dgvDH_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvDH_SelectionChanged(object sender, EventArgs e)
        {
            hienthi();
        }

        private void FrmDonHang_Load(object sender, EventArgs e)
        {
            load();
            setbinding();
            hienthi();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                int vitri = dgvDH.CurrentRow.Index;
                if (string.IsNullOrWhiteSpace(dgvDH.Rows[vitri].Cells[0].Value.ToString()))
                {
                    MessageBox.Show("Mã đơn hàng không được trống", "Thông báo",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

               
                string MaDH = dgvDH.Rows[vitri].Cells[0].Value.ToString();

           
                DialogResult xacNhan;
                xacNhan = MessageBox.Show($"Bạn có chắc muốn thêm đơn hàng {MaDH} không?", "Xác nhận thêm",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question);

               
                if (xacNhan != DialogResult.Yes)
                {
                    return; // Dừng hàm, không làm gì cả
                }
              
                DateTime NgayDatHang = DateTime.Parse(dgvDH.Rows[vitri].Cells[1].Value.ToString());
                string TriGiaDH = dgvDH.Rows[vitri].Cells[2].Value.ToString();
                DateTime NgayNhanHang = DateTime.Parse(dgvDH.Rows[vitri].Cells[3].Value.ToString());
                string MaKH = dgvDH.Rows[vitri].Cells[4].Value.ToString();

                // Format ngày tháng cho an toàn
                string NgayDatHang_SQL = NgayDatHang.ToString("yyyy-MM-dd");
                string NgayNhanHang_SQL = NgayNhanHang.ToString("yyyy-MM-dd");

                // Thực thi câu lệnh
                kn.ExecuteNonQuery($"insert into DON_DAT_HANG values ('{MaDH}', '{NgayDatHang_SQL}', {TriGiaDH}, '{NgayNhanHang_SQL}', '{MaKH}')");

                MessageBox.Show("Thêm " + MaDH + " thành công!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Lấy dữ liệu từ các ô TextBoxes
                string MaDH = txtMaDH.Text;
                DateTime NgayDatHang = DateTime.Parse(txtNgayDat.Text);
                string TriGiaDH = txtTriGia.Text; // Lấy ra dạng chuỗi
                DateTime NgayNhanHang = DateTime.Parse(txtNgayNhan.Text);
                string MaKH = txtMaKH.Text;

                // 2. Hỏi xác nhận trước khi sửa
                DialogResult xacNhan;
                xacNhan = MessageBox.Show($"Bạn có chắc muốn cập nhật đơn hàng {MaDH} không?", "Xác nhận cập nhật",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // 3. Nếu người dùng KHÔNG bấm YES, thì dừng lại
                if (xacNhan != DialogResult.Yes)
                {
                    return; // Dừng hàm, không làm gì cả
                }

                // 4. Định dạng lại ngày tháng cho an toàn (yyyy-MM-dd)
                string NgayDatHang_SQL = NgayDatHang.ToString("yyyy-MM-dd");
                string NgayNhanHang_SQL = NgayNhanHang.ToString("yyyy-MM-dd");

                // 5. Viết câu lệnh UPDATE
                // Lưu ý: TriGiaDH là kiểu số nên không có dấu nháy ' ' bao quanh
                string query = $"UPDATE DON_DAT_HANG " +
                               $"SET NgayDat = '{NgayDatHang_SQL}', " +
                               $"    TriGiaDH = {TriGiaDH}, " +
                               $"    NgayNhan = '{NgayNhanHang_SQL}', " +
                               $"    MaKH = '{MaKH}' " +
                               $"WHERE MaDDH = '{MaDH}'"; 

                // 6. Thực thi câu lệnh
                kn.ExecuteNonQuery(query);

                // 7. Thông báo thành công và tải lại bảng
                MessageBox.Show("Cập nhật " + MaDH + " thành công!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                load(); // Gọi lại hàm load() để làm mới DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật đơn hàng: " + ex.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Lấy MaDH từ TextBox (đã được fill khi bạn click vào DataGridView)
                string MaDH = txtMaDH.Text;

                // 2. Kiểm tra xem đã chọn đơn hàng nào chưa
                if (string.IsNullOrWhiteSpace(MaDH))
                {
                    MessageBox.Show("Vui lòng chọn đơn hàng cần xóa!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 3. Hỏi xác nhận trước khi xóa (Rất quan trọng)
                DialogResult xacNhan;
                xacNhan = MessageBox.Show($"Bạn có chắc chắn muốn xóa vĩnh viễn đơn hàng {MaDH} không?", "Xác nhận xóa",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Warning); // Dùng icon Cảnh báo

                // 4. Nếu người dùng KHÔNG bấm YES, thì dừng lại
                if (xacNhan != DialogResult.Yes)
                {
                    return; // Dừng hàm, không làm gì cả
                }

                // 5. Viết câu lệnh DELETE (Lưu ý WHERE MaDDH)
                string query = $"DELETE FROM DON_DAT_HANG WHERE MaDDH = '{MaDH}'";

                // 6. Thực thi câu lệnh
                kn.ExecuteNonQuery(query);

                // 7. Thông báo thành công và tải lại bảng
                MessageBox.Show("Xóa đơn hàng " + MaDH + " thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 8. Tải lại DataGridView
                load();

                // 9. Xóa trắng các ô TextBox
                txtMaDH.Text = "";
                txtNgayDat.Text = "";
                txtTriGia.Text = "";
                txtNgayNhan.Text = "";
                txtMaKH.Text = "";
            }
            catch (Exception ex)
            {
                // Báo lỗi nếu không xóa được (ví dụ: do ràng buộc khóa ngoại)
                MessageBox.Show("Lỗi khi xóa đơn hàng: " + ex.ToString(), "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Xóa trắng các ô nhập liệu
                txtMaDH.Text = "";
                txtNgayDat.Text = "";
                txtTriGia.Text = "";
                txtNgayNhan.Text = "";
                txtMaKH.Text = "";

                // 2. Tải lại dữ liệu (giả sử hàm load() của bạn làm việc này)
                load();

                // 3. (Tùy chọn) Di chuyển con trỏ chuột về ô đầu tiên
                txtMaDH.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi làm mới dữ liệu: " + ex.Message, "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Lấy nội dung tìm kiếm
                string maDH_TimKiem = txtMaDH.Text.Trim();
                string maKH_TimKiem = txtMaKH.Text.Trim();

                // 2. Nếu không nhập gì cả, thì tải lại toàn bộ
                if (string.IsNullOrWhiteSpace(maDH_TimKiem) && string.IsNullOrWhiteSpace(maKH_TimKiem))
                {
                    MessageBox.Show("Vui lòng nhập thông tin để tìm kiếm. Đang tải lại toàn bộ danh sách.", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    load(); // Tải lại toàn bộ
                    return;
                }

                string query = "SELECT * FROM DON_DAT_HANG";
                List<string> conditions = new List<string>();

                // 3. SỬA: Chỉ tìm nếu MaDH không rỗng, và tìm CHÍNH XÁC (=)
                if (!string.IsNullOrWhiteSpace(maDH_TimKiem))
                {
                    conditions.Add($"MaDDH = N'{maDH_TimKiem}'");
                }

                // 4. SỬA: Chỉ tìm nếu MaKH không rỗng, và tìm CHÍNH XÁC (=)
                if (!string.IsNullOrWhiteSpace(maKH_TimKiem))
                {
                    conditions.Add($"MaKH = N'{maKH_TimKiem}'");
                }

                // 5. Nối các điều kiện bằng 'AND' (VÀ)
                if (conditions.Count > 0)
                {
                    // Phải khớp TẤT CẢ điều kiện (MaDH VÀ MaKH)
                    query += " WHERE " + string.Join(" AND ", conditions);
                }

                // 6. Thực thi và hiển thị
                DataTable dt = kn.ExecuteQuery(query);
                dgvDH.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy đơn hàng nào khớp.", "Kết quả tìm kiếm",
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
            // 1. Tạo đối tượng SaveFileDialog để cho phép người dùng chọn nơi lưu
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls"; // Chỉ cho phép lưu file .xls
            saveFileDialog.Title = "Chọn nơi để lưu danh sách";
            saveFileDialog.FileName = "DanhSachDonHang.xls"; // Tên file mặc định

            // 2. Hiển thị hộp thoại và kiểm tra nếu người dùng bấm "OK"
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 3. Mở một file để ghi (StreamWriter)
                    // Dùng System.Text.Encoding.Unicode để đảm bảo tiếng Việt không bị lỗi font
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, System.Text.Encoding.Unicode))
                    {
                        // -- A. GHI DÒNG TIÊU ĐỀ (HEADER) --
                        string headerLine = "";
                        foreach (DataGridViewColumn col in dgvDH.Columns)
                        {
                            // Lấy tên cột (HeaderText) và ngăn cách bằng ký tự Tab (\t)
                            headerLine += col.HeaderText + "\t";
                        }
                        sw.WriteLine(headerLine.Trim()); // Ghi dòng tiêu đề vào file

                        // -- B. GHI DỮ LIỆU CÁC DÒNG (ROWS) --
                        foreach (DataGridViewRow row in dgvDH.Rows)
                        {
                            // Bỏ qua dòng mới (dòng trống cuối cùng) của DataGridView
                            if (!row.IsNewRow)
                            {
                                string rowLine = "";
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    // Lấy giá trị của ô, nếu null thì trả về rỗng
                                    // Ngăn cách các ô bằng ký tự Tab (\t)
                                    rowLine += (cell.Value ?? "").ToString() + "\t";
                                }
                                sw.WriteLine(rowLine.Trim()); // Ghi dòng dữ liệu vào file
                            }
                        }
                    } // 'using' sẽ tự động đóng file (sw.Close())

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

        private void btnSanPham_Click(object sender, EventArgs e)
        {
            FrmSP f = new FrmSP();
            f.ShowDialog();
        }

        private void btnDonHang_Click(object sender, EventArgs e)
        {

        }

        private void btnNhanVien_Click(object sender, EventArgs e)
        {
            FrmNhanVien f = new FrmNhanVien();
            f.ShowDialog();
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

        private void btnBaoCao_Click(object sender, EventArgs e)
        {
            FrmBaoCao f = new FrmBaoCao();
            f.ShowDialog();
        }
    }
}
