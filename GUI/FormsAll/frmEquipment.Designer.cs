namespace GUI.FormAll // SỬA LỖI 1: Đổi namespace từ Main thành GUI.FormAll
{
    partial class frmEquipment
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblWeapon = new System.Windows.Forms.Label();
            this.picWeapon = new System.Windows.Forms.PictureBox();
            this.picArmor = new System.Windows.Forms.PictureBox();
            this.lblArmor = new System.Windows.Forms.Label();
            this.grpEquipmentSlots = new System.Windows.Forms.GroupBox();
            // SỬA LỖI 2: Thêm các control bị thiếu
            this.lblHelmet = new System.Windows.Forms.Label();
            this.picHelmet = new System.Windows.Forms.PictureBox();
            this.lblAccessory = new System.Windows.Forms.Label();
            this.picAccessory = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picWeapon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picArmor)).BeginInit();
            // SỬA LỖI 2: Thêm các control bị thiếu
            ((System.ComponentModel.ISupportInitialize)(this.picHelmet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAccessory)).BeginInit();
            this.grpEquipmentSlots.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Location = new System.Drawing.Point(16, 11);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(501, 37);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "EQUIPMENT";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(480, 11);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(40, 37);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblWeapon
            // 
            this.lblWeapon.AutoSize = true;
            this.lblWeapon.Location = new System.Drawing.Point(93, 37);
            this.lblWeapon.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWeapon.Name = "lblWeapon";
            this.lblWeapon.Size = new System.Drawing.Size(93, 16);
            this.lblWeapon.TabIndex = 2;
            this.lblWeapon.Text = "Vũ khí: (Trống)";
            // 
            // picWeapon
            // 
            this.picWeapon.Location = new System.Drawing.Point(13, 37);
            this.picWeapon.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.picWeapon.Name = "picWeapon";
            this.picWeapon.Size = new System.Drawing.Size(67, 62);
            this.picWeapon.TabIndex = 3;
            this.picWeapon.TabStop = false;
            // 
            // picArmor
            // 
            this.picArmor.Location = new System.Drawing.Point(13, 111);
            this.picArmor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.picArmor.Name = "picArmor";
            this.picArmor.Size = new System.Drawing.Size(67, 62);
            this.picArmor.TabIndex = 5;
            this.picArmor.TabStop = false;
            // 
            // lblArmor
            // 
            this.lblArmor.AutoSize = true;
            this.lblArmor.Location = new System.Drawing.Point(93, 111);
            this.lblArmor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblArmor.Name = "lblArmor";
            this.lblArmor.Size = new System.Drawing.Size(104, 16);
            this.lblArmor.TabIndex = 4;
            this.lblArmor.Text = "Áo giáp: (Trống)";
            // 
            // grpEquipmentSlots
            // 
            this.grpEquipmentSlots.Controls.Add(this.picWeapon);
            this.grpEquipmentSlots.Controls.Add(this.lblWeapon);
            this.grpEquipmentSlots.Controls.Add(this.lblArmor);
            this.grpEquipmentSlots.Controls.Add(this.picArmor);
            // SỬA LỖI 2: Thêm control vào groupbox
            this.grpEquipmentSlots.Controls.Add(this.picHelmet);
            this.grpEquipmentSlots.Controls.Add(this.lblHelmet);
            this.grpEquipmentSlots.Controls.Add(this.lblAccessory);
            this.grpEquipmentSlots.Controls.Add(this.picAccessory);
            this.grpEquipmentSlots.Location = new System.Drawing.Point(16, 52);
            this.grpEquipmentSlots.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpEquipmentSlots.Name = "grpEquipmentSlots";
            this.grpEquipmentSlots.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpEquipmentSlots.Size = new System.Drawing.Size(501, 300); // Tăng kích thước
            this.grpEquipmentSlots.TabIndex = 10;
            this.grpEquipmentSlots.TabStop = false;
            this.grpEquipmentSlots.Text = "Trang bị đang mặc";
            // 
            // SỬA LỖI 2: Định nghĩa các control bị thiếu
            // 
            // picHelmet
            // 
            this.picHelmet.Location = new System.Drawing.Point(13, 185);
            this.picHelmet.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.picHelmet.Name = "picHelmet";
            this.picHelmet.Size = new System.Drawing.Size(67, 62);
            this.picHelmet.TabIndex = 7;
            this.picHelmet.TabStop = false;
            // 
            // lblHelmet
            // 
            this.lblHelmet.AutoSize = true;
            this.lblHelmet.Location = new System.Drawing.Point(93, 185);
            this.lblHelmet.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHelmet.Name = "lblHelmet";
            this.lblHelmet.Size = new System.Drawing.Size(83, 16);
            this.lblHelmet.TabIndex = 6;
            this.lblHelmet.Text = "Mũ: (Trống)";
            // 
            // picAccessory
            // 
            this.picAccessory.Location = new System.Drawing.Point(13, 259);
            this.picAccessory.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.picAccessory.Name = "picAccessory";
            this.picAccessory.Size = new System.Drawing.Size(67, 62);
            this.picAccessory.TabIndex = 9;
            this.picAccessory.TabStop = false;
            // 
            // lblAccessory
            // 
            this.lblAccessory.AutoSize = true;
            this.lblAccessory.Location = new System.Drawing.Point(93, 259);
            this.lblAccessory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAccessory.Name = "lblAccessory";
            this.lblAccessory.Size = new System.Drawing.Size(110, 16);
            this.lblAccessory.TabIndex = 8;
            this.lblAccessory.Text = "Phụ kiện: (Trống)";
            // 
            // frmEquipment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 370); // Tăng kích thước
            this.Controls.Add(this.grpEquipmentSlots);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmEquipment";
            this.Text = "frmEquipment";
            ((System.ComponentModel.ISupportInitialize)(this.picWeapon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picArmor)).EndInit();
            // SỬA LỖI 2: Thêm các control bị thiếu
            ((System.ComponentModel.ISupportInitialize)(this.picHelmet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAccessory)).EndInit();
            this.grpEquipmentSlots.ResumeLayout(false);
            this.grpEquipmentSlots.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblWeapon;
        private System.Windows.Forms.PictureBox picWeapon;
        private System.Windows.Forms.PictureBox picArmor;
        private System.Windows.Forms.Label lblArmor;
        private System.Windows.Forms.GroupBox grpEquipmentSlots;
        // SỬA LỖI 2: Khai báo các biến control bị thiếu
        private System.Windows.Forms.Label lblHelmet;
        private System.Windows.Forms.PictureBox picHelmet;
        private System.Windows.Forms.Label lblAccessory;
        private System.Windows.Forms.PictureBox picAccessory;
    }
}
