using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChuongTrinhQuanLiSach
{
    public partial class FrmReport : Form
    {
        KetNoi kn = new KetNoi();
        ReportViewer rpt = new ReportViewer();
        public FrmReport()
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
        private void FrmReport_Load(object sender, EventArgs e)
        {
            // 1. Gọi hàm setup Padding cho ReportViewer
            ReportViewer_Setup();

            try
            {
                // 2. Chỉ định đường dẫn tới file thiết kế báo cáo
                // Nhớ chỉnh thuộc tính "Copy to Output Directory" của file Report.rdlc thành "Copy if newer" nhé
                rpt.LocalReport.ReportPath = "Report.rdlc";

                // 3. Làm sạch DataSource cũ trước khi nạp mới
                rpt.LocalReport.DataSources.Clear();

                // 4. Viết câu query lấy dữ liệu doanh thu (đặt bí danh AS khớp với tên cột trong RDLC)
                string query = @"
                    SELECT 
                        ROW_NUMBER() OVER(ORDER BY NgayNhap) AS STT,
                        NgayNhap, 
                        SUM(TongTien) AS TongSoTien
                    FROM HOA_DON_BAN
                    GROUP BY NgayNhap";

                // 5. Gọi class kết nối để lấy DataTable
                DataTable dt = kn.ExecuteQuery(query);

                // 6. Truyền dữ liệu vào Report
                // Chú ý: "DataSet1" phải giống y hệt tên Dataset mà bạn đã khai báo bên trong file Report.rdlc
                ReportDataSource rds = new ReportDataSource("DataSet1", dt);

                rpt.LocalReport.DataSources.Add(rds);

                // 7. Render/Làm mới để hiển thị báo cáo lên màn hình
                rpt.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị báo cáo: " + ex.Message, "Thông báo lỗi");
            }
        }

       

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
