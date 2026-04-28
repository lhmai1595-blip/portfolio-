using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ChuongTrinhQuanLiSach
{
    public partial class FrmQuanLiBanSach : Form
    {
        

        public FrmQuanLiBanSach()
        {
            InitializeComponent();
           

        }

     
        private void resetBtn()
        {
            btnSanPham.BackColor = Color.FromArgb(255, 131, 97, 64);
            btnHDB.BackColor = Color.FromArgb(255, 131, 97, 64);
            btnHDN.BackColor = Color.FromArgb(255, 131, 97, 64);
            btnNhanVien.BackColor = Color.FromArgb(255, 131, 97, 64);
            btnDonHang.BackColor = Color.FromArgb(255, 131, 97, 64);
            btnNCC.BackColor = Color.FromArgb(255, 131, 97, 64);
            btnBaoCao.BackColor = Color.FromArgb(255, 131, 97, 64);
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmSP f = new FrmSP();
            f.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FrmHDN f = new FrmHDN();
            f.ShowDialog();
        }

        private void FrmQuanLiBanSachcs_Load(object sender, EventArgs e)
        {

            // 1. Lấy quyền từ biến static bên Form Đăng nhập
            string quyen = FrmDangNhap.QuyenTruyCap;

            // 2. Mặc định: Hiện tất cả các nút chức năng trước
            // Để đảm bảo nếu là Admin thì thấy hết, hoặc khi đăng nhập lại không bị ẩn luôn
            btnNhanVien.Visible = true;
            btnBaoCao.Visible = true;
            btnSanPham.Visible = true;
            btnDonHang.Visible = true;
            btnKhachHang.Visible = true;
            btnNCC.Visible = true;
            btnHDN.Visible = true;
            btnHDB.Visible = true;
            quảnLýKháchHàngToolStripMenuItem.Visible = true;
            báoCáoToolStripMenuItem.Visible=true;
            quảnLýNhàCungCấpToolStripMenuItem.Visible= true;
            quảnLýĐơnHàngToolStripMenuItem.Visible=true;
            quảnLýSảnPhẩmToolStripMenuItem.Visible = true;
            báoCáoToolStripMenuItem1.Visible=true;
            hóaĐơnBánToolStripMenuItem.Visible = true;
            hóaĐươnNhậpToolStripMenuItem.Visible = true;
            // 3. Xử lý phân quyền
            switch (quyen)
            {
                case "Banhang": // Nhân viên Bán Hàng
                    btnNhanVien.Visible = false; // Không xem được nhân viên
                    btnBaoCao.Visible = false;   // Không xem báo cáo doanh thu
                    báoCáoToolStripMenuItem.Visible = false;
                    báoCáoToolStripMenuItem1.Visible = false;
                    break;

                case "Ketoan": // Kế toán
                    btnNhanVien.Visible = false; // Không quản lý nhân viên
                    báoCáoToolStripMenuItem.Visible = false;
                   
                    break;

                case "Kho": // Thủ kho
                    btnNhanVien.Visible = false;
                    btnBaoCao.Visible = false;
                    báoCáoToolStripMenuItem.Visible = false;
                    báoCáoToolStripMenuItem1.Visible = false;
                    break;

                case "Admin":
                    // Admin thấy tất cả -> Không cần làm gì vì đã enable all ở trên
                    break;
            }
        }
        private void OpenChildForm(Form childForm)
        {
            
        }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void quảnLýKháchHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmKhachHang f = new FrmKhachHang();
            f.ShowDialog();
        }

        private void báoCáoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmNhanVien f = new FrmNhanVien();
            f.ShowDialog();
        }

        private void btnNhanVien_Click(object sender, EventArgs e)
        {
            FrmNhanVien f = new FrmNhanVien();
            f.ShowDialog();
        }

        private void btnDonHang_Click(object sender, EventArgs e)
        {
            {
                FrmDonHang f = new FrmDonHang();
                f.ShowDialog();
            }

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

        private void quảnLýNhàCungCấpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmNCC f = new FrmNCC();
            f.ShowDialog();
        }

        private void btnNCC_Click(object sender, EventArgs e)
        {
            FrmNCC f = new FrmNCC();
            f.ShowDialog();
        }

        private void txtKhachHang_Click(object sender, EventArgs e)
        {
            FrmKhachHang f = new FrmKhachHang();
            f.ShowDialog();
        }

        private void quảnLýĐơnHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDonHang f = new FrmDonHang();
            f.ShowDialog();
        }

        private void quảnLýSảnPhẩmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSP f = new FrmSP();
            f.ShowDialog();
        }

        private void btnBaoCao_Click(object sender, EventArgs e)
        {
            FrmBaoCao f = new FrmBaoCao();
            f.ShowDialog();
        }

        private void báoCáoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FrmBaoCao f = new FrmBaoCao();
            f.ShowDialog();
        }

        private void hóaĐơnBánToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmHDB f = new FrmHDB();
            f.ShowDialog();
        }

        private void btnHDB_Click(object sender, EventArgs e)
        {
            FrmHDB f = new FrmHDB();
            f.ShowDialog();
        }

        private void hóaĐươnNhậpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmHDN f = new FrmHDN();
            f.ShowDialog();
        }
    }
}
