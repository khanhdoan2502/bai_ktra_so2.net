using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Bài_3
{
    public partial class frmAsyncReport : Form
    {
        private CancellationTokenSource cts; // Dùng để hủy tiến trình ngầm khi tắt Form

        public frmAsyncReport()
        {
            InitializeComponent();
        }

        // Khi mở Form -> Chạy hàm quét dữ liệu ngầm liên tục
        private void frmAsyncReport_Load(object sender, EventArgs e)
        {
            cts = new CancellationTokenSource();
            StartRealtimeSync(cts.Token);
        }

        // -------------------------------------------------------------
        // KỊCH BẢN B: TẢI DỮ LIỆU NẶNG KHÔNG ĐƠ UI (ASYNC / AWAIT)
        // -------------------------------------------------------------
        private async void btnLoadReport_Click(object sender, EventArgs e)
        {
            btnLoadReport.Enabled = false; // Khóa nút bấm lại 

            // Báo Form cha bật thanh ProgressBar loading lên
            frmMain parent = (frmMain)this.MdiParent;
            parent?.ToggleProgressBar(true);

            try
            {
                // await Task.Run: Ném việc tải dữ liệu nặng sang một luồng khác chạy ngầm.
                // UI ở ngoài vẫn rảnh rỗi nên không bị đơ giật.
                DataTable data = await Task.Run(() =>
                {
                    return FetchHeavyData(); // Gọi hàm mô phỏng bên dưới
                });

                // Sau khi chạy ngầm xong (khoảng 4s), nó tự quay lại đây đổ dữ liệu lên Grid
                dgvReport.DataSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                btnLoadReport.Enabled = true;
                parent?.ToggleProgressBar(false); // Tắt loading
            }
        }

        // Hàm giả lập việc chọc vào SQL truy xuất hàng triệu dòng (Mất 4 giây)
        private DataTable FetchHeavyData()
        {
            Thread.Sleep(4000); // Bắt luồng này ngủ 4 giây để giả lập độ trễ

            DataTable dt = new DataTable();
            dt.Columns.Add("Tháng");
            dt.Columns.Add("Doanh Thu");
            dt.Rows.Add("Tháng 1", "500,000,000");
            dt.Rows.Add("Tháng 2", "650,000,000");
            dt.Rows.Add("Tháng 3", "720,000,000");
            return dt;
        }

        // -------------------------------------------------------------
        // KỊCH BẢN C: CROSS-THREAD INVOKE (Cập nhật giao diện an toàn)
        // -------------------------------------------------------------
        private async void StartRealtimeSync(CancellationToken token)
        {
            try
            {
                int orderCount = 0;
                while (!token.IsCancellationRequested) // Chạy lặp liên tục nếu Form chưa đóng
                {
                    await Task.Delay(2000, token); // Đợi 2 giây mới quét 1 lần
                    orderCount++;
                    string message = $"Cập nhật: Có {orderCount} đơn mới lúc {DateTime.Now:HH:mm:ss}";

                    // KHÔNG THỂ GÁN TRỰC TIẾP LÊN LABEL TỪ LUỒNG NÀY ĐƯỢC (Sẽ lỗi Cross-Thread)
                    // Phải gọi hàm SafeUpdateLabel bên dưới
                    SafeUpdateLabel(message);
                }
            }
            catch (TaskCanceledException) { /* Bỏ qua lỗi khi cố tình hủy Task */ }
        }

        private void SafeUpdateLabel(string text)
        {
            // InvokeRequired kiểm tra xem luồng hiện tại có khác luồng chính của UI không
            if (lblRealtimeData.InvokeRequired)
            {
                // Nhờ UI luồng chính gán chữ hộ
                lblRealtimeData.Invoke(new Action(() => lblRealtimeData.Text = text));
            }
            else
            {
                lblRealtimeData.Text = text;
            }
        }

        // Phải hủy tiến trình quét dữ liệu ngầm khi tắt Form
        private void frmAsyncReport_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }
        }
    }
}
