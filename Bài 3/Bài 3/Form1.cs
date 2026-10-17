using System;
using System.Windows.Forms;

namespace Bài_3
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        // HÀM QUAN TRỌNG: Mở form con và chống mở nhiều cửa sổ giống nhau
        private void OpenChildForm<T>() where T : Form, new()
        {
            // Quét xem trong các Form con đang mở, có form nào cùng loại không
            foreach (Form frm in this.MdiChildren)
            {
                if (frm is T)
                {
                    frm.Activate(); // Nếu có rồi thì lôi nó lên trên cùng
                    return;
                }
            }

            // Nếu chưa có, tạo form mới, chỉ định Form Cha (MdiParent) và hiển thị
            T newForm = new T();
            newForm.MdiParent = this;
            newForm.Show();
        }

        // Hàm này để Form con (báo cáo) có thể gọi ngược lên Form cha để bật/tắt ProgressBar
        public void ToggleProgressBar(bool isRunning)
        {
            prgLoading.Visible = isRunning;
        }

        // --- Gắn sự kiện Click cho các nút Menu ---

        private void mnuThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mnuSanPham_Click(object sender, EventArgs e)
        {
            OpenChildForm<frmProductCatalog>();
        }

        private void mnuDoanhThu_Click(object sender, EventArgs e)
        {
             OpenChildForm<frmAsyncReport>(); 
        }

        // --- Xếp cửa sổ ---
        private void mnuCascade_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        private void mnuTileHor_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void mnuTileVer_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }
    }
}