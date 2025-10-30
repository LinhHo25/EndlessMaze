namespace Main
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
            this.picHelmet = new System.Windows.Forms.PictureBox();
            this.lblHelmet = new System.Windows.Forms.Label();
            this.picAccessory = new System.Windows.Forms.PictureBox();
            this.lblAccessory = new System.Windows.Forms.Label();
            this.grpEquipmentSlots = new System.Windows.Forms.GroupBox(); // THÊM GroupBox
            ((System.ComponentModel.ISupportInitialize)(this.picWeapon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picArmor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHelmet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAccessory)).BeginInit();
            this.grpEquipmentSlots.SuspendLayout(); // THÊM DÒNG NÀY
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(376, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "EQUIPMENT";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(360, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(30, 30);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblWeapon
            // 
            this.lblWeapon.AutoSize = true;
            this.lblWeapon.Location = new System.Drawing.Point(70, 30); // Sửa vị trí Y
            this.lblWeapon.Name = "lblWeapon";
            this.lblWeapon.Size = new System.Drawing.Size(78, 13);
            this.lblWeapon.TabIndex = 2;
            this.lblWeapon.Text = "Vũ khí: (Trống)";
            // 
            // picWeapon
            // 
            this.picWeapon.Location = new System.Drawing.Point(10, 30); // Sửa vị trí Y
            this.picWeapon.Name = "picWeapon";
            this.picWeapon.Size = new System.Drawing.Size(50, 50);
            this.picWeapon.TabIndex = 3;
            this.picWeapon.TabStop = false;
            // 
            // picArmor
            // 
            this.picArmor.Location = new System.Drawing.Point(10, 90); // Sửa vị trí Y
            this.picArmor.Name = "picArmor";
            this.picArmor.Size = new System.Drawing.Size(50, 50);
            this.picArmor.TabIndex = 5;
            this.picArmor.TabStop = false;
            // 
            // lblArmor
            // 
            this.lblArmor.AutoSize = true;
            this.lblArmor.Location = new System.Drawing.Point(70, 90); // Sửa vị trí Y
            this.lblArmor.Name = "lblArmor";
            this.lblArmor.Size = new System.Drawing.Size(81, 13);
            this.lblArmor.TabIndex = 4;
            this.lblArmor.Text = "Áo giáp: (Trống)";
            // 
            // picHelmet
            // 
            this.picHelmet.Location = new System.Drawing.Point(10, 150); // Sửa vị trí Y
            this.picHelmet.Name = "picHelmet";
            this.picHelmet.Size = new System.Drawing.Size(50, 50);
            this.picHelmet.TabIndex = 7;
            this.picHelmet.TabStop = false;
            // 
            // lblHelmet
            // 
            this.lblHelmet.AutoSize = true;
            this.lblHelmet.Location = new System.Drawing.Point(70, 150); // Sửa vị trí Y
            this.lblHelmet.Name = "lblHelmet";
            this.lblHelmet.Size = new System.Drawing.Size(63, 13);
            this.lblHelmet.TabIndex = 6;
            this.lblHelmet.Text = "Mũ: (Trống)";
            // 
            // picAccessory
            // 
            this.picAccessory.Location = new System.Drawing.Point(10, 210); // Sửa vị trí Y
            this.picAccessory.Name = "picAccessory";
            this.picAccessory.Size = new System.Drawing.Size(50, 50);
            this.picAccessory.TabIndex = 9;
            this.picAccessory.TabStop = false;
            // 
            // lblAccessory
            // 
            this.lblAccessory.AutoSize = true;
            this.lblAccessory.Location = new System.Drawing.Point(70, 210); // Sửa vị trí Y
            this.lblAccessory.Name = "lblAccessory";
            this.lblAccessory.Size = new System.Drawing.Size(87, 13);
            this.lblAccessory.TabIndex = 8;
            this.lblAccessory.Text = "Phụ kiện: (Trống)";
            // 
            // grpEquipmentSlots
            // 
            this.grpEquipmentSlots.Controls.Add(this.picWeapon); // ĐƯA VÀO GROUPBOX
            this.grpEquipmentSlots.Controls.Add(this.picAccessory);
            this.grpEquipmentSlots.Controls.Add(this.lblWeapon);
            this.grpEquipmentSlots.Controls.Add(this.lblAccessory);
            this.grpEquipmentSlots.Controls.Add(this.lblArmor);
            this.grpEquipmentSlots.Controls.Add(this.picHelmet);
            this.grpEquipmentSlots.Controls.Add(this.picArmor);
            this.grpEquipmentSlots.Controls.Add(this.lblHelmet);
            this.grpEquipmentSlots.Location = new System.Drawing.Point(12, 42); // Vị trí GroupBox
            this.grpEquipmentSlots.Name = "grpEquipmentSlots";
            this.grpEquipmentSlots.Size = new System.Drawing.Size(376, 280); // Kích thước GroupBox
            this.grpEquipmentSlots.TabIndex = 10;
            this.grpEquipmentSlots.TabStop = false;
            this.grpEquipmentSlots.Text = "Trang bị đang mặc";
            // 
            // frmEquipment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 335); // Tăng kích thước Form
            this.Controls.Add(this.grpEquipmentSlots); // THÊM GROUPBOX VÀO FORM
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmEquipment";
            this.Text = "frmEquipment";
            ((System.ComponentModel.ISupportInitialize)(this.picWeapon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picArmor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHelmet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAccessory)).EndInit();
            this.grpEquipmentSlots.ResumeLayout(false); // THÊM DÒNG NÀY
            this.grpEquipmentSlots.PerformLayout(); // THÊM DÒNG NÀY
            this.ResumeLayout(false);
            // Bỏ các control con ra khỏi form chính
            /*
            this.Controls.Remove(this.picAccessory);
            this.Controls.Remove(this.lblAccessory);
            this.Controls.Remove(this.picHelmet);
            this.Controls.Remove(this.lblHelmet);
            this.Controls.Remove(this.picArmor);
            this.Controls.Remove(this.lblArmor);
            this.Controls.Remove(this.picWeapon);
            this.Controls.Remove(this.lblWeapon);
            */

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblWeapon;
        private System.Windows.Forms.PictureBox picWeapon;
        private System.Windows.Forms.PictureBox picArmor;
        private System.Windows.Forms.Label lblArmor;
        private System.Windows.Forms.PictureBox picHelmet;
        private System.Windows.Forms.Label lblHelmet;
        private System.Windows.Forms.PictureBox picAccessory;
        private System.Windows.Forms.Label lblAccessory;
        private System.Windows.Forms.GroupBox grpEquipmentSlots; // KHAI BÁO GroupBox
    }
}

