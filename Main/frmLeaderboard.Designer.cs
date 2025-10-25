namespace Main
{
    partial class frmLeaderboard
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabKhamPha = new System.Windows.Forms.TabPage();
            this.dgvKhamPha = new System.Windows.Forms.DataGridView();
            this.colKhamPha_TenTaiKhoan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colKhamPha_ThoiGian = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colKhamPha_VatPham = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colKhamPha_TenMap = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colKhamPha_SoMap = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPhieuLuu = new System.Windows.Forms.TabPage();
            this.dgvPhieuLuu = new System.Windows.Forms.DataGridView();
            this.colPhieuLuu_TenTaiKhoan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPhieuLuu_ThoiGian = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPhieuLuu_QuaiGiet = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPhieuLuu_TenMap = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPhieuLuu_SoMap = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelTabs = new System.Windows.Forms.Panel();
            this.btnTabPhieuLuu = new System.Windows.Forms.Button();
            this.btnTabKhamPha = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabMain.SuspendLayout();
            this.tabKhamPha.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKhamPha)).BeginInit();
            this.tabPhieuLuu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPhieuLuu)).BeginInit();
            this.panelTabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabMain.Controls.Add(this.tabKhamPha);
            this.tabMain.Controls.Add(this.tabPhieuLuu);
            this.tabMain.Location = new System.Drawing.Point(12, 110);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(760, 439);
            this.tabMain.TabIndex = 0;
            // 
            // tabKhamPha
            // 
            this.tabKhamPha.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.tabKhamPha.Controls.Add(this.dgvKhamPha);
            this.tabKhamPha.Location = new System.Drawing.Point(4, 22);
            this.tabKhamPha.Name = "tabKhamPha";
            this.tabKhamPha.Padding = new System.Windows.Forms.Padding(3);
            this.tabKhamPha.Size = new System.Drawing.Size(752, 413);
            this.tabKhamPha.TabIndex = 0;
            this.tabKhamPha.Text = "Khám Phá";
            // 
            // dgvKhamPha
            // 
            this.dgvKhamPha.AllowUserToAddRows = false;
            this.dgvKhamPha.AllowUserToDeleteRows = false;
            this.dgvKhamPha.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvKhamPha.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvKhamPha.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvKhamPha.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvKhamPha.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colKhamPha_TenTaiKhoan,
            this.colKhamPha_ThoiGian,
            this.colKhamPha_VatPham,
            this.colKhamPha_TenMap,
            this.colKhamPha_SoMap});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvKhamPha.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvKhamPha.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvKhamPha.Location = new System.Drawing.Point(3, 3);
            this.dgvKhamPha.Name = "dgvKhamPha";
            this.dgvKhamPha.ReadOnly = true;
            this.dgvKhamPha.RowTemplate.Height = 28;
            this.dgvKhamPha.Size = new System.Drawing.Size(746, 407);
            this.dgvKhamPha.TabIndex = 0;
            // 
            // colKhamPha_TenTaiKhoan
            // 
            this.colKhamPha_TenTaiKhoan.FillWeight = 120F;
            this.colKhamPha_TenTaiKhoan.HeaderText = "Tên tài khoản";
            this.colKhamPha_TenTaiKhoan.Name = "colKhamPha_TenTaiKhoan";
            this.colKhamPha_TenTaiKhoan.ReadOnly = true;
            // 
            // colKhamPha_ThoiGian
            // 
            this.colKhamPha_ThoiGian.FillWeight = 110F;
            this.colKhamPha_ThoiGian.HeaderText = "Thời gian qua màn";
            this.colKhamPha_ThoiGian.Name = "colKhamPha_ThoiGian";
            this.colKhamPha_ThoiGian.ReadOnly = true;
            // 
            // colKhamPha_VatPham
            // 
            this.colKhamPha_VatPham.FillWeight = 120F;
            this.colKhamPha_VatPham.HeaderText = "Số vật phẩm đã tìm được";
            this.colKhamPha_VatPham.Name = "colKhamPha_VatPham";
            this.colKhamPha_VatPham.ReadOnly = true;
            // 
            // colKhamPha_TenMap
            // 
            this.colKhamPha_TenMap.FillWeight = 80F;
            this.colKhamPha_TenMap.HeaderText = "Tên Map";
            this.colKhamPha_TenMap.Name = "colKhamPha_TenMap";
            this.colKhamPha_TenMap.ReadOnly = true;
            // 
            // colKhamPha_SoMap
            // 
            this.colKhamPha_SoMap.FillWeight = 80F;
            this.colKhamPha_SoMap.HeaderText = "Số Map đã vượt";
            this.colKhamPha_SoMap.Name = "colKhamPha_SoMap";
            this.colKhamPha_SoMap.ReadOnly = true;
            // 
            // tabPhieuLuu
            // 
            this.tabPhieuLuu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.tabPhieuLuu.Controls.Add(this.dgvPhieuLuu);
            this.tabPhieuLuu.Location = new System.Drawing.Point(4, 22);
            this.tabPhieuLuu.Name = "tabPhieuLuu";
            this.tabPhieuLuu.Padding = new System.Windows.Forms.Padding(3);
            this.tabPhieuLuu.Size = new System.Drawing.Size(752, 413);
            this.tabPhieuLuu.TabIndex = 1;
            this.tabPhieuLuu.Text = "Phiêu Lưu";
            // 
            // dgvPhieuLuu
            // 
            this.dgvPhieuLuu.AllowUserToAddRows = false;
            this.dgvPhieuLuu.AllowUserToDeleteRows = false;
            this.dgvPhieuLuu.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPhieuLuu.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPhieuLuu.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvPhieuLuu.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPhieuLuu.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPhieuLuu_TenTaiKhoan,
            this.colPhieuLuu_ThoiGian,
            this.colPhieuLuu_QuaiGiet,
            this.colPhieuLuu_TenMap,
            this.colPhieuLuu_SoMap});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPhieuLuu.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvPhieuLuu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPhieuLuu.Location = new System.Drawing.Point(3, 3);
            this.dgvPhieuLuu.Name = "dgvPhieuLuu";
            this.dgvPhieuLuu.ReadOnly = true;
            this.dgvPhieuLuu.RowTemplate.Height = 28;
            this.dgvPhieuLuu.Size = new System.Drawing.Size(746, 407);
            this.dgvPhieuLuu.TabIndex = 1;
            // 
            // colPhieuLuu_TenTaiKhoan
            // 
            this.colPhieuLuu_TenTaiKhoan.FillWeight = 120F;
            this.colPhieuLuu_TenTaiKhoan.HeaderText = "Tên tài khoản";
            this.colPhieuLuu_TenTaiKhoan.Name = "colPhieuLuu_TenTaiKhoan";
            this.colPhieuLuu_TenTaiKhoan.ReadOnly = true;
            // 
            // colPhieuLuu_ThoiGian
            // 
            this.colPhieuLuu_ThoiGian.FillWeight = 110F;
            this.colPhieuLuu_ThoiGian.HeaderText = "Thời gian qua màn";
            this.colPhieuLuu_ThoiGian.Name = "colPhieuLuu_ThoiGian";
            this.colPhieuLuu_ThoiGian.ReadOnly = true;
            // 
            // colPhieuLuu_QuaiGiet
            // 
            this.colPhieuLuu_QuaiGiet.FillWeight = 120F;
            this.colPhieuLuu_QuaiGiet.HeaderText = "Số lượng quái giết";
            this.colPhieuLuu_QuaiGiet.Name = "colPhieuLuu_QuaiGiet";
            this.colPhieuLuu_QuaiGiet.ReadOnly = true;
            // 
            // colPhieuLuu_TenMap
            // 
            this.colPhieuLuu_TenMap.FillWeight = 80F;
            this.colPhieuLuu_TenMap.HeaderText = "Tên Map";
            this.colPhieuLuu_TenMap.Name = "colPhieuLuu_TenMap";
            this.colPhieuLuu_TenMap.ReadOnly = true;
            // 
            // colPhieuLuu_SoMap
            // 
            this.colPhieuLuu_SoMap.FillWeight = 80F;
            this.colPhieuLuu_SoMap.HeaderText = "Số Map đã vượt";
            this.colPhieuLuu_SoMap.Name = "colPhieuLuu_SoMap";
            this.colPhieuLuu_SoMap.ReadOnly = true;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Cyan;
            this.lblTitle.Location = new System.Drawing.Point(278, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(236, 37);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "BẢNG XẾP HẠNG";
            // 
            // panelTabs
            // 
            this.panelTabs.Controls.Add(this.btnTabPhieuLuu);
            this.panelTabs.Controls.Add(this.btnTabKhamPha);
            this.panelTabs.Location = new System.Drawing.Point(12, 60);
            this.panelTabs.Name = "panelTabs";
            this.panelTabs.Size = new System.Drawing.Size(400, 44);
            this.panelTabs.TabIndex = 2;
            // 
            // btnTabPhieuLuu
            // 
            this.btnTabPhieuLuu.FlatAppearance.BorderSize = 0;
            this.btnTabPhieuLuu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabPhieuLuu.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTabPhieuLuu.ForeColor = System.Drawing.Color.Gray;
            this.btnTabPhieuLuu.Location = new System.Drawing.Point(163, 3);
            this.btnTabPhieuLuu.Name = "btnTabPhieuLuu";
            this.btnTabPhieuLuu.Size = new System.Drawing.Size(154, 38);
            this.btnTabPhieuLuu.TabIndex = 1;
            this.btnTabPhieuLuu.Text = "Chế độ Phiêu lưu";
            this.btnTabPhieuLuu.UseVisualStyleBackColor = true;
            this.btnTabPhieuLuu.Click += new System.EventHandler(this.btnTabPhieuLuu_Click);
            // 
            // btnTabKhamPha
            // 
            this.btnTabKhamPha.FlatAppearance.BorderSize = 0;
            this.btnTabKhamPha.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabKhamPha.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTabKhamPha.ForeColor = System.Drawing.Color.Cyan;
            this.btnTabKhamPha.Location = new System.Drawing.Point(3, 3);
            this.btnTabKhamPha.Name = "btnTabKhamPha";
            this.btnTabKhamPha.Size = new System.Drawing.Size(154, 38);
            this.btnTabKhamPha.TabIndex = 0;
            this.btnTabKhamPha.Text = "Chế độ Khám phá";
            this.btnTabKhamPha.UseVisualStyleBackColor = true;
            this.btnTabKhamPha.Click += new System.EventHandler(this.btnTabKhamPha_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(732, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(40, 30);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmLeaderboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panelTabs);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.tabMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmLeaderboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bảng Xếp Hạng";
            this.Load += new System.EventHandler(this.frmLeaderboard_Load);
            this.tabMain.ResumeLayout(false);
            this.tabKhamPha.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvKhamPha)).EndInit();
            this.tabPhieuLuu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPhieuLuu)).EndInit();
            this.panelTabs.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabKhamPha;
        private System.Windows.Forms.TabPage tabPhieuLuu;
        private System.Windows.Forms.DataGridView dgvKhamPha;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelTabs;
        private System.Windows.Forms.Button btnTabPhieuLuu;
        private System.Windows.Forms.Button btnTabKhamPha;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgvPhieuLuu;
        private System.Windows.Forms.DataGridViewTextBoxColumn colKhamPha_TenTaiKhoan;
        private System.Windows.Forms.DataGridViewTextBoxColumn colKhamPha_ThoiGian;
        private System.Windows.Forms.DataGridViewTextBoxColumn colKhamPha_VatPham;
        private System.Windows.Forms.DataGridViewTextBoxColumn colKhamPha_TenMap;
        private System.Windows.Forms.DataGridViewTextBoxColumn colKhamPha_SoMap;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPhieuLuu_TenTaiKhoan;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPhieuLuu_ThoiGian;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPhieuLuu_QuaiGiet;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPhieuLuu_TenMap;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPhieuLuu_SoMap;
    }
}

