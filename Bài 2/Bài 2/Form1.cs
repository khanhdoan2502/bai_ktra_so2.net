using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Bài_2
{
    public partial class Form1 : Form
    {
        private BindingList<Product> productList;
        private BindingSource bindingSource;
        private int nextId = 1;
        private string currentImagePath = ""; // Lưu đường dẫn ảnh tạm thời khi chọn

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetupDataGridView(); // Phải tắt tự động sinh cột và vẽ cột bằng tay TRƯỚC
            InitData();          
            UpdateStatus();
        }

        #region 1. Khởi tạo & Cấu hình
        private void InitData()
        {
            cboCategory.Items.Clear();
            cboCategory.Items.AddRange(new string[] { "Điện thoại", "Laptop", "Phụ kiện", "Gia dụng" });

            productList = new BindingList<Product>();
            bindingSource = new BindingSource { DataSource = productList };
            dgvProducts.DataSource = bindingSource;

            // Gắn sự kiện để hỏi người dùng trước khi đóng Form
            this.FormClosing += Form1_FormClosing;
        }

        private void SetupDataGridView()
        {
            dgvProducts.Columns.Clear();
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.ReadOnly = true;
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "Mã SP", Width = 50 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Tên sản phẩm" });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Category", HeaderText = "Loại" });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Price", HeaderText = "Đơn giá", DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Số lượng", Width = 80 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ImagePath", HeaderText = "File Ảnh" });
        }
        #endregion

        #region 2. StatusStrip & Timer
        // Hàm cập nhật thanh trạng thái (Gọi hàm này mỗi khi Thêm/Sửa/Xóa/Import)
        private void UpdateStatus()
        {
            int totalQty = productList.Sum(p => p.Quantity);
            decimal totalVal = productList.Sum(p => p.Quantity * p.Price);

            lblTotalQuantity.Text = $"Tổng SP: {totalQty}";
            lblTotalValue.Text = $"Tổng giá trị: {totalVal:N0} VNĐ";
        }

        // Sự kiện Tick của Timer (Hiển thị đồng hồ thời gian thực)
        private void timerClock_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
        #endregion

        #region 3. Xử lý Ảnh (OpenFileDialog)
        private void btnChooseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn ảnh sản phẩm";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"; // Chỉ lọc file ảnh

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    currentImagePath = ofd.FileName;
                    picProductImage.Image = Image.FromFile(currentImagePath);
                }
            }
        }
        #endregion

        #region 4. Các chức năng CRUD cơ bản
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (ValidateData())
            {
                Product newProduct = new Product
                {
                    Id = nextId++,
                    Name = txtName.Text.Trim(),
                    Category = cboCategory.SelectedItem?.ToString(),
                    Price = decimal.Parse(txtPrice.Text),
                    Quantity = (int)numQuantity.Value,
                    ImagePath = currentImagePath
                };

                productList.Add(newProduct);
                btnClear_Click(null, null);
                UpdateStatus();
            }
        }
        private bool ValidateData()
        {
            bool isValid = true;
            errorProvider1.Clear(); // Xóa các lỗi cũ trước khi kiểm tra lại

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errorProvider1.SetError(txtName, "Vui lòng nhập tên sản phẩm!");
                isValid = false;
            }

            if (cboCategory.SelectedIndex == -1)
            {
                errorProvider1.SetError(cboCategory, "Vui lòng chọn loại sản phẩm!");
                isValid = false;
            }

            // Dùng TryParse để kiểm tra xem chữ nhập vào có chuyển thành số được không
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                errorProvider1.SetError(txtPrice, "Đơn giá phải là số hợp lệ và >= 0!");
                isValid = false;
            }

            return isValid;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current is Product currentProduct)
            {
                currentProduct.Name = txtName.Text.Trim();
                currentProduct.Category = cboCategory.SelectedItem.ToString();
                currentProduct.Price = decimal.Parse(txtPrice.Text);
                currentProduct.Quantity = (int)numQuantity.Value;
                currentProduct.ImagePath = currentImagePath;

                bindingSource.ResetCurrentItem();
                UpdateStatus();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current != null && MessageBox.Show("Xóa sản phẩm này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                bindingSource.RemoveCurrent();
                btnClear_Click(null, null);
                UpdateStatus();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtId.Clear();
            txtName.Clear();
            cboCategory.SelectedIndex = -1;
            txtPrice.Clear();
            numQuantity.Value = 0;
            currentImagePath = "";
            picProductImage.Image = null; // Xóa ảnh hiển thị
        }

        // Đổ dữ liệu ngược từ Grid lên Form (Bao gồm cả load ảnh)
        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && bindingSource.Current is Product p)
            {
                txtId.Text = p.Id.ToString();
                txtName.Text = p.Name;
                cboCategory.SelectedItem = p.Category;
                txtPrice.Text = p.Price.ToString();
                numQuantity.Value = p.Quantity;
                currentImagePath = p.ImagePath;

                // Load ảnh từ file nếu đường dẫn tồn tại
                if (!string.IsNullOrEmpty(p.ImagePath) && File.Exists(p.ImagePath))
                {
                    picProductImage.Image = Image.FromFile(p.ImagePath);
                }
                else
                {
                    picProductImage.Image = null;
                }
            }
        }
        #endregion

        #region 5. File I/O (Export / Import CSV)
        // Gắn hàm này vào sự kiện Click của Menu "Export CSV"
        private void exportCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (productList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.Title = "Lưu báo cáo tồn kho";
                sfd.FileName = "BaoCaoTonKho.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(sfd.FileName))
                        {
                            // Ghi dòng Tiêu đề (Header)
                            sw.WriteLine("Id,Name,Category,Price,Quantity,ImagePath");

                            // Ghi dữ liệu
                            foreach (var p in productList)
                            {
                                sw.WriteLine($"{p.Id},{p.Name},{p.Category},{p.Price},{p.Quantity},{p.ImagePath}");
                            }
                        }
                        MessageBox.Show("Xuất file thành công!", "Thông báo");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi lưu file: " + ex.Message, "Lỗi");
                    }
                }
            }
        }

        // Gắn hàm này vào sự kiện Click của Menu "Import CSV"
        private void importCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV files (*.csv)|*.csv";
                ofd.Title = "Chọn file dữ liệu để nạp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        productList.Clear(); // Xóa dữ liệu cũ
                        string[] lines = File.ReadAllLines(ofd.FileName);

                        // Bỏ qua dòng đầu tiên (Header), bắt đầu từ i = 1
                        for (int i = 1; i < lines.Length; i++)
                        {
                            string[] data = lines[i].Split(',');
                            if (data.Length >= 5)
                            {
                                Product p = new Product
                                {
                                    Id = int.Parse(data[0]),
                                    Name = data[1],
                                    Category = data[2],
                                    Price = decimal.Parse(data[3]),
                                    Quantity = int.Parse(data[4]),
                                    ImagePath = data.Length == 6 ? data[5] : ""
                                };
                                productList.Add(p);

                                // Cập nhật nextId để khi Thêm mới không bị trùng
                                if (p.Id >= nextId) nextId = p.Id + 1;
                            }
                        }
                        UpdateStatus();
                        MessageBox.Show("Nhập dữ liệu thành công!", "Thông báo");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi định dạng file CSV: " + ex.Message, "Lỗi");
                    }
                }
            }
        }
        #endregion

        #region 6. Xử lý thoát an toàn
        // Gắn vào Menu Exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close(); // Lệnh này sẽ kích hoạt sự kiện FormClosing bên dưới
        }

        // Sự kiện FormClosing
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát phần mềm?",
                                                  "Xác nhận thoát",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true; // Hủy lệnh đóng Form
            }
        }
        #endregion
    }
}