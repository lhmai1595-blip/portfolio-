using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace ChuongTrinhQuanLiSach
{
    public partial class FrmTruyVanSQL : Form
    {
        KetNoi kn = new KetNoi();
        public FrmTruyVanSQL()
        {
            InitializeComponent();
        }

        private void FrmTruyVanSQL_Load(object sender, EventArgs e)
        {

        }

        private void btnThucThi_Click(object sender, EventArgs e)
        {
            // Lấy câu lệnh SQL người dùng nhập
            string sql = txtQuery.Text.Trim();

            // Kiểm tra nếu chưa nhập gì
            if (string.IsNullOrEmpty(sql))
            {
                MessageBox.Show("Vui lòng nhập câu lệnh SQL!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Kiểm tra xem người dùng đang muốn SELECT (Lấy dữ liệu) hay INSERT/UPDATE/DELETE
                // Cách đơn giản nhất để bắt chước hình ảnh là dùng ExecuteQuery cho SELECT

                if (sql.ToLower().StartsWith("select"))
                {
                    // 2. Gọi hàm ExecuteQuery từ class KetNoi của bạn
                    DataTable dt = kn.ExecuteQuery(sql);

                    // 3. Đổ dữ liệu lên lưới
                    dgvKetQua.DataSource = dt;

                    // 4. Hiện thông báo giống trong hình
                    MessageBox.Show("Thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Trường hợp lệnh INSERT, UPDATE, DELETE
                    int rowChanged = kn.ExecuteNonQuery(sql);

                    // Nếu muốn reset lưới (vì lệnh này không trả về bảng)
                    dgvKetQua.DataSource = null;

                    MessageBox.Show($"Thành công. Số dòng bị ảnh hưởng: {rowChanged}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // Nếu gõ sai lệnh SQL thì báo lỗi
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem bảng có dữ liệu không
            if (dgvKetQua.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Mở hộp thoại lưu file
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Documents (*.xlsx)|*.xlsx";
            sfd.FileName = "KetQuaTruyVan.xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    MessageBox.Show("Đang xuất dữ liệu, vui lòng chờ...", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Gọi hàm xuất Excel
                    ToExcel(dgvKetQua, sfd.FileName);

                    MessageBox.Show("Xuất file thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Hàm xử lý việc xuất Excel
        private void ToExcel(DataGridView dgv, string fileName)
        {
            // Tạo đối tượng Excel
            Excel.Application excelApp = new Excel.Application();
            excelApp.Application.Workbooks.Add(Type.Missing);

            // Lưu cột (Header)
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                excelApp.Cells[1, i + 1] = dgv.Columns[i].HeaderText;
            }

            // Lưu dữ liệu (Rows)
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                for (int j = 0; j < dgv.Columns.Count; j++)
                {
                    if (dgv.Rows[i].Cells[j].Value != null)
                    {
                        excelApp.Cells[i + 2, j + 1] = dgv.Rows[i].Cells[j].Value.ToString();
                    }
                }
            }

            // Căn chỉnh cột cho đẹp
            excelApp.Columns.AutoFit();

            // Lưu file
            excelApp.ActiveWorkbook.SaveCopyAs(fileName);
            excelApp.ActiveWorkbook.Saved = true;

            // Đóng Excel và giải phóng bộ nhớ (Quan trọng)
            excelApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        }
    }
}
    
