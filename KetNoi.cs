using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Ink;

namespace ChuongTrinhQuanLiSach
{
    internal class KetNoi
    {
        private string strKn = @"Data Source=LAPTOP-KLLIECIU\SQLEXPRESS;Initial Catalog=QUANLYBANSACH;Integrated Security=True";

        public SqlConnection KetNoiCSDL()
        {
            SqlConnection conn = new SqlConnection(strKn);
            conn.Open();
            return conn;
        }
        public DataTable LayDuLieu(string tenBang)
        {
            string qr = $"SELECT * FROM [{tenBang}]";
            SqlDataAdapter adpt = new SqlDataAdapter(qr, KetNoiCSDL());
            DataTable dt = new DataTable();
            adpt.Fill(dt);
            return dt;
        }

        public DataTable LayDuLieuTuSP(string tenSP, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            // Tự động lấy chuỗi kết nối "strkn" mà bạn đã khai báo ở trên
            using (SqlConnection con = new SqlConnection(strKn))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(tenSP, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.Fill(dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Tốt hơn là dùng MessageBox, hoặc ném lỗi ra ngoài
                    // MessageBox.Show("Lỗi khi lấy dữ liệu từ SP: " + ex.Message);
                    Console.WriteLine("Lỗi khi lấy dữ liệu từ SP: " + ex.Message);
                }
            }
            return dt;
        }
        public int ExecuteNonQuery(string query)
        {
            int data = 0;
            using (SqlConnection ketnoi = new SqlConnection(strKn))
            {
                ketnoi.Open();
                SqlCommand thucthi = new SqlCommand(query, ketnoi);
                data = thucthi.ExecuteNonQuery();
                ketnoi.Close();
            }
            return data;
        }
        public DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection ketnoi = new SqlConnection(strKn))
            {
                ketnoi.Open();
                SqlCommand thucthi = new SqlCommand(query, ketnoi);
                SqlDataAdapter laydulieu = new SqlDataAdapter(thucthi);
                laydulieu.Fill(dt);
                ketnoi.Close();
            }
            return dt;
        }
    }
}

