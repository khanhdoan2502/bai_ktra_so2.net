using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace bài_1
{
    public partial class Form1 : Form
    {
        // 1. Khai báo BindingList và BindingSource
        private BindingList<Device> deviceList;
        private BindingSource bindingSource;
        private int nextId = 1; // ID tự động tăng

        public Form1()
        {
            InitializeComponent();
        }

        // Sự kiện Form Load
        private void Form1_Load(object sender, EventArgs e)
        {
            InitData();
            SetupDataGridView();
        }

        #region 1. Khởi tạo & Cấu hình
        private void InitData()
        {
            // Nạp dữ liệu cứng cho ComboBox
            cboCategory.Items.AddRange(new string[] { "Điện thoại", "Laptop", "Máy tính bảng", "Phụ kiện" });

            // Khởi tạo danh sách lưu trữ và BindingSource
            deviceList = new BindingList<Device>();
            bindingSource = new BindingSource();

            // Gán dữ liệu vào BindingSource
            bindingSource.DataSource = deviceList;

            // Gán BindingSource vào DataGridView
            dgvDevices.DataSource = bindingSource;
        }

        private void SetupDataGridView()
        {
            dgvDevices.AutoGenerateColumns = false; // Tắt tự sinh cột tự động
            dgvDevices.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Chọn nguyên một dòng
            dgvDevices.ReadOnly = true; // Chặn sửa dữ liệu trực tiếp trên Grid
            dgvDevices.AllowUserToAddRows = false; // Ẩn dòng trắng dưới cùng
            dgvDevices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Chia đều độ rộng các cột

            // 1. Cột ID
            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Id",
                HeaderText = "Mã TB",
                Width = 80
            });

            // 2. Cột Tên
            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Name",
                HeaderText = "Tên thiết bị",
                FillWeight = 150
            });

            // 3. Cột Loại
            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Category",
                HeaderText = "Loại thiết bị"
            });

            // 4. Cột Giá (Có định dạng tiền tệ)
            DataGridViewTextBoxColumn priceCol = new DataGridViewTextBoxColumn();
            priceCol.DataPropertyName = "Price";
            priceCol.HeaderText = "Đơn giá";
            priceCol.DefaultCellStyle.Format = "C0"; // C0 = Currency (Tiền tệ không số lẻ)
            priceCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDevices.Columns.Add(priceCol);

            // 5. Cột Số lượng
            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Quantity",
                HeaderText = "Số lượng tồn",
                Width = 100
            });
        }
        #endregion

        #region 2. Kiểm tra dữ liệu (Validation)
        private bool ValidateData()
        {
            bool isValid = true;
            errorProvider1.Clear(); // Làm sạch các thông báo lỗi cũ trước khi check lại

            // 1. Kiểm tra tên trống
            if (string.IsNullOrWhiteSpace(txtDeviceName.Text))
            {
                errorProvider1.SetError(txtDeviceName, "Vui lòng nhập tên thiết bị!");
                isValid = false;
            }

            // 2. Kiểm tra chưa chọn Combo box
            if (cboCategory.SelectedIndex == -1)
            {
                errorProvider1.SetError(cboCategory, "Vui lòng chọn loại thiết bị!");
                isValid = false;
            }

            // 3. Kiểm tra định dạng số của Đơn giá
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                errorProvider1.SetError(txtPrice, "Đơn giá phải là số hợp lệ và >= 0!");
                isValid = false;
            }

            return isValid;
        }
        #endregion

        #region 3. Các sự kiện Button & GridView
        // 1. Nút THÊM MỚI
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (ValidateData())
            {
                // Tạo đối tượng mới
                Device newDevice = new Device
                {
                    Id = nextId++,
                    Name = txtDeviceName.Text.Trim(),
                    Category = cboCategory.SelectedItem.ToString(),
                    Price = decimal.Parse(txtPrice.Text),
                    Quantity = (int)numQuantity.Value
                };

                // Thêm vào danh sách (Sẽ tự động hiển thị lên Grid do dùng BindingList)
                deviceList.Add(newDevice);

                MessageBox.Show("Thêm thiết bị thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnClear_Click(null, null); // Xóa trắng Form sau khi thêm
            }
        }

        // 2. Nút CẬP NHẬT
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current == null)
            {
                MessageBox.Show("Vui lòng chọn một thiết bị trên danh sách để cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ValidateData())
            {
                // Ép kiểu dòng đang chọn hiện tại về đối tượng Device
                Device currentDevice = (Device)bindingSource.Current;

                // Cập nhật lại các thuộc tính của đối tượng đó
                currentDevice.Name = txtDeviceName.Text.Trim();
                currentDevice.Category = cboCategory.SelectedItem.ToString();
                currentDevice.Price = decimal.Parse(txtPrice.Text);
                currentDevice.Quantity = (int)numQuantity.Value;

                // Lệnh quan trọng: Báo cho BindingSource biết dữ liệu đã đổi để nó vẽ lại Grid
                bindingSource.ResetCurrentItem();

                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // 3. Nút XÓA
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current == null)
            {
                MessageBox.Show("Vui lòng chọn thiết bị cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn xóa thiết bị này không?",
                                              "Xác nhận",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                // Xóa phần tử hiện tại khỏi BindingSource (sẽ tự mất trên Grid)
                bindingSource.RemoveCurrent();
                btnClear_Click(null, null);
            }
        }

        // 4. Nút LÀM MỚI (HỦY THAO TÁC)
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtDeviceId.Clear();
            txtDeviceName.Clear();
            cboCategory.SelectedIndex = -1;
            txtPrice.Clear();
            numQuantity.Value = 0;

            errorProvider1.Clear(); // Tắt cảnh báo lỗi
            dgvDevices.ClearSelection(); // Bỏ chọn Grid
            txtDeviceName.Focus(); // Đưa con trỏ chuột về ô Tên thiết bị
        }

        // 5. Sự kiện Click vào Grid đổ dữ liệu ngược lên TextBox
        private void dgvDevices_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Chỉ xử lý nếu click vào dữ liệu hợp lệ (tránh lỗi khi click lên thanh tiêu đề cột)
            if (e.RowIndex >= 0 && bindingSource.Current != null)
            {
                Device selectedDevice = (Device)bindingSource.Current;

                txtDeviceId.Text = selectedDevice.Id.ToString();
                txtDeviceName.Text = selectedDevice.Name;
                cboCategory.SelectedItem = selectedDevice.Category;
                txtPrice.Text = selectedDevice.Price.ToString();
                numQuantity.Value = selectedDevice.Quantity;
            }
        }
        #endregion
    }
}