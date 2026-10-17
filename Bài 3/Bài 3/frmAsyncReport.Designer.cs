namespace Bài_3
{
    partial class frmAsyncReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnLoadReport = new Button();
            dgvReport = new DataGridView();
            lblRealtimeData = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvReport).BeginInit();
            SuspendLayout();
            // 
            // btnLoadReport
            // 
            btnLoadReport.Location = new Point(45, 48);
            btnLoadReport.Name = "btnLoadReport";
            btnLoadReport.Size = new Size(185, 29);
            btnLoadReport.TabIndex = 0;
            btnLoadReport.Text = "Tải Báo Cáo Doanh Thu";
            btnLoadReport.UseVisualStyleBackColor = true;
            // 
            // dgvReport
            // 
            dgvReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvReport.Location = new Point(306, 48);
            dgvReport.Name = "dgvReport";
            dgvReport.RowHeadersWidth = 51;
            dgvReport.Size = new Size(300, 188);
            dgvReport.TabIndex = 1;
            // 
            // lblRealtimeData
            // 
            lblRealtimeData.AutoSize = true;
            lblRealtimeData.Location = new Point(39, 379);
            lblRealtimeData.Name = "lblRealtimeData";
            lblRealtimeData.Size = new Size(123, 20);
            lblRealtimeData.TabIndex = 2;
            lblRealtimeData.Text = "Đang chờ dữ liệu";
            // 
            // frmAsyncReport
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblRealtimeData);
            Controls.Add(dgvReport);
            Controls.Add(btnLoadReport);
            Name = "frmAsyncReport";
            Text = "frmAsyncReport";
            FormClosing += frmAsyncReport_FormClosing;
            Load += frmAsyncReport_Load;
            Click += btnLoadReport_Click;
            ((System.ComponentModel.ISupportInitialize)dgvReport).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnLoadReport;
        private DataGridView dgvReport;
        private Label lblRealtimeData;
    }
}