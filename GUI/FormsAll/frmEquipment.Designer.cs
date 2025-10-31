namespace Main // <-- SỬA LỖI: Đổi namespace từ GUI.FormAll về Main
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
            ((System.ComponentModel.ISupportInitialize)(this.picWeapon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picArmor)).BeginInit();
            this.grpEquipmentSlots.SuspendLayout();
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
            this.lblWeapon.Location = new System.Drawing.Point(70, 30);
            this.lblWeapon.Name = "lblWeapon";
            this.lblWeapon.Size = new System.Drawing.Size(79, 13);
            this.lblWeapon.TabIndex = 2;
            this.lblWeapon.Text = "Vũ khí: (Trống)";
            // 
            // picWeapon
            // 
            this.picWeapon.Location = new System.Drawing.Point(10, 30);
            this.picWeapon.Name = "picWeapon";
            this.picWeapon.Size = new System.Drawing.Size(50, 50);
            this.picWeapon.TabIndex = 3;
            this.picWeapon.TabStop = false;
            // 
            // picArmor
            // 
            this.picArmor.Location = new System.Drawing.Point(10, 90);
            this.picArmor.Name = "picArmor";
            this.picArmor.Size = new System.Drawing.Size(50, 50);
            this.picArmor.TabIndex = 5;
            this.picArmor.TabStop = false;
            // 
            // lblArmor
            // 
            this.lblArmor.AutoSize = true;
            this.lblArmor.Location = new System.Drawing.Point(70, 90);
            this.lblArmor.Name = "lblArmor";
            this.lblArmor.Size = new System.Drawing.Size(83, 13);
            this.lblArmor.TabIndex = 4;
            this.lblArmor.Text = "Áo giáp: (Trống)";
            // 
            // grpEquipmentSlots
            // 
            this.grpEquipmentSlots.Controls.Add(this.picWeapon);
            this.grpEquipmentSlots.Controls.Add(this.lblWeapon);
            this.grpEquipmentSlots.Controls.Add(this.lblArmor);
            this.grpEquipmentSlots.Controls.Add(this.picArmor);
            this.grpEquipmentSlots.Location = new System.Drawing.Point(12, 42);
            this.grpEquipmentSlots.Name = "grpEquipmentSlots";
            this.grpEquipmentSlots.Size = new System.Drawing.Size(376, 168);
            this.grpEquipmentSlots.TabIndex = 10;
            this.grpEquipmentSlots.TabStop = false;
            this.grpEquipmentSlots.Text = "Trang bị đang mặc";
            // 
            // frmEquipment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 229);
            this.Controls.Add(this.grpEquipmentSlots);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmEquipment";
            this.Text = "frmEquipment";
            ((System.ComponentModel.ISupportInitialize)(this.picWeapon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picArmor)).EndInit();
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
    }
}
