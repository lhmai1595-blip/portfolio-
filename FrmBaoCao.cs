using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Reporting.WinForms;

namespace ChuongTrinhQuanLiSach
{
    public partial class FrmBaoCao : Form
    {

        KetNoi kn = new KetNoi();
        ReportViewer rpt = new ReportViewer();
        public FrmBaoCao()
        {
            InitializeComponent();
            rpt.Name = "reportViewer1";
            rpt.Dock = DockStyle.Fill;

            // Add biến rpt toàn cục vào panel
            pnlHienThi.Controls.Add(rpt);

            rpt.RefreshReport();




        }

        private void ReportViewer_Setup()
        {

            rpt.Padding = new Padding(0, 150, 0, 0);

        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void txtMaDH_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void FrmBaoCao_Load(object sender, EventArgs e)
        {
            // Cấu hình ComboBox Loại Báo Cáo
            cboLoaiBaoCao.Items.Clear();
            cboLoaiBaoCao.Items.Add("Báo cáo doanh thu theo tháng");             // Index 0
            cboLoaiBaoCao.Items.Add("Báo cáo chi trả NCC năm 2023");                  // Index 1
            cboLoaiBaoCao.Items.Add("Báo cáo 3 sản phẩm bán chạy nhất "); // Index 2 (MỚI THÊM)
            cboLoaiBaoCao.SelectedIndex = 0;

            // Load tháng
            cboThang.Items.Clear();
            for (int i = 1; i <= 12; i++) cboThang.Items.Add(i.ToString());

            rpt.Padding = new Padding(0, 10, 0, 0);
        }




        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Sửa lại: Dùng đúng tên cboLoaiBaoCao thay vì comboBox1
            int selectedIndex = cboLoaiBaoCao.SelectedIndex;


            // Xóa dữ liệu cũ trong các ô
            txtKH.Clear();
            txtNam.Clear();
            cboThang.SelectedIndex = -1;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = "";
            string reportPath = "";
            string tenDataSet = "DataSet1"; // Giá trị mặc định

            try
            {
                // ============================================================
                // PHẦN 1: XÁC ĐỊNH LOẠI BÁO CÁO VÀ TẠO CÂU SQL
                // ============================================================

                // --- TRƯỜNG HỢP 1: BÁO CÁO DOANH THU (Index 0) ---
                if (cboLoaiBaoCao.SelectedIndex == 0)
                {
                    if (string.IsNullOrEmpty(cboThang.Text) || string.IsNullOrEmpty(txtNam.Text))
                    {
                        MessageBox.Show("Vui lòng chọn Tháng và Năm!");
                        return;
                    }

                    sql = "SELECT " +
                          "ROW_NUMBER() OVER(ORDER BY NgayNhap) AS STT, " +
                          "NgayNhap AS NgayNhap, " +
                          "TongTien AS TongSoTien " +
                          "FROM HOA_DON_BAN " +
                          $"WHERE MONTH(NgayNhap) = {cboThang.Text} AND YEAR(NgayNhap) = {txtNam.Text}";

                    reportPath = "Report.rdlc";
                    tenDataSet = "DataSet1";
                }

                // --- TRƯỜNG HỢP 2: CHI TRẢ NHÀ CUNG CẤP 2023 ---
                else if (cboLoaiBaoCao.Text.Contains("NCC"))
                {
                    // SQL MỚI: Dùng SUM và GROUP BY để gộp tiền
                    sql = "SELECT " +
                          "ROW_NUMBER() OVER(ORDER BY SUM(hdn.TongTien) DESC) AS STT, " + // Sắp xếp theo ông nào trả nhiều tiền nhất lên đầu
                          "ncc.MaNCC AS MaNCC, " +    // Lấy thêm mã để hiển thị
                          "ncc.TenNCC AS TenNCC, " +
                          "SUM(hdn.TongTien) AS TongTien " + // <--- QUAN TRỌNG: Dùng hàm SUM để cộng dồn
                          "FROM HOA_DON_NHAP hdn " +
                          "JOIN NHA_CUNG_CAP ncc ON hdn.MaNCC = ncc.MaNCC " +
                          "WHERE YEAR(hdn.NgayNhap) = 2023 " +
                          "GROUP BY ncc.MaNCC, ncc.TenNCC"; // <--- QUAN TRỌNG: Phải Group theo Mã và Tên thì mới gộp được

                    reportPath = "BaoCaoChiTraNCC.rdlc";
                    tenDataSet = "DataSet2";
                }

                // --- TRƯỜNG HỢP 3: TOP 3 SẢN PHẨM BÁN CHẠY ---
                else if (cboLoaiBaoCao.Text.Contains("3 sản phẩm"))
                {
                    sql = "SELECT TOP 3 " +
                          "sp.TenSP AS TenSP, " + // Lưu ý: Trong DataSet3 cột này tên là TenHang hay TenSP? Sửa cho khớp
                          "SUM(ct.SoLuong) AS SoLuong " + // Lưu ý: Trong DataSet3 cột này tên là SoLuong hay TongSoLuong?
                          "FROM CHITIET_HDB ct " +
                          "JOIN SAN_PHAM sp ON ct.MaSP = sp.MaSP " +
                          "GROUP BY sp.TenSP " +
                          "ORDER BY SUM(ct.SoLuong) DESC";

                    reportPath = "BaoCaoTop3.rdlc";
                    tenDataSet = "DataSet3"; // Dùng DataSet3
                }
                else
                {
                    return; // Không chọn gì thì thoát
                }

                // ============================================================
                // PHẦN 2: THỰC THI VÀ HIỂN THỊ (ĐÃ SỬA LỖI)
                // ============================================================

                // 1. Lấy dữ liệu từ SQL
                DataTable dt = kn.ExecuteQuery(sql);

                // 2. Xóa dữ liệu cũ trên ReportViewer
                rpt.LocalReport.DataSources.Clear();

                if (dt.Rows.Count > 0)
                {
                    // --- SỬA QUAN TRỌNG TẠI ĐÂY ---
                    // Dùng biến reportPath (thay vì "DoanhThuBH.rdlc")
                    rpt.LocalReport.ReportPath = reportPath;

                    // Dùng biến tenDataSet (thay vì "DataSet1") để nó tự động đổi thành DataSet2, DataSet3...
                    ReportDataSource rds = new ReportDataSource(tenDataSet, dt);

                    rpt.LocalReport.DataSources.Add(rds);
                    rpt.RefreshReport();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy dữ liệu nào cho báo cáo này!");
                    rpt.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
    
        private void button2_Click(object sender, EventArgs e)
        {
            cboLoaiBaoCao.SelectedIndex = 0;
            txtKH.Clear();
            cboThang.SelectedIndex = -1;
            txtNam.Clear();
            rpt.Clear(); // Xóa trắng báo cáo



        }

        // Các nút điều hướng menu bên trái
       

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

        private void txtKhachHang_Click(object sender, EventArgs e)
        {
            FrmKhachHang f = new FrmKhachHang();
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

        private void txtKH_TextChanged(object sender, EventArgs e)
        {

        }

        private void cboThang_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblNhapNam_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
