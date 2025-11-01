namespace Main 
{
    partial class frmStatus
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
            this.lblHP = new System.Windows.Forms.Label();
            this.lblStrength = new System.Windows.Forms.Label();
            this.lblDefense = new System.Windows.Forms.Label();
            this.picPlayerAvatar = new System.Windows.Forms.PictureBox();
            // SỬA LỖI 2: Thêm các control bị thiếu
            this.lblMP = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picPlayerAvatar)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Location = new System.Drawing.Point(16, 11);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(501, 37);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Trạng Thái";
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
            // lblHP
            // 
            this.lblHP.AutoSize = true;
            this.lblHP.Location = new System.Drawing.Point(200, 86);
            this.lblHP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHP.Name = "lblHP";
            this.lblHP.Size = new System.Drawing.Size(56, 16);
            this.lblHP.TabIndex = 2;
            this.lblHP.Text = "HP: 0 / 0";
            // 
            // lblStrength
            // 
            this.lblStrength.AutoSize = true;
            this.lblStrength.Location = new System.Drawing.Point(200, 150); // Sửa vị trí
            this.lblStrength.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStrength.Name = "lblStrength";
            this.lblStrength.Size = new System.Drawing.Size(79, 16);
            this.lblStrength.TabIndex = 4;
            this.lblStrength.Text = "Sức mạnh: 0";
            // 
            // lblDefense
            // 
            this.lblDefense.AutoSize = true;
            this.lblDefense.Location = new System.Drawing.Point(200, 182); // Sửa vị trí
            this.lblDefense.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDefense.Name = "lblDefense";
            this.lblDefense.Size = new System.Drawing.Size(79, 16);
            this.lblDefense.TabIndex = 5;
            this.lblDefense.Text = "Phòng thủ: 0";
            // 
            // picPlayerAvatar
            // 
            this.picPlayerAvatar.BackColor = System.Drawing.Color.Gray;
            this.picPlayerAvatar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPlayerAvatar.Location = new System.Drawing.Point(27, 86);
            this.picPlayerAvatar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.picPlayerAvatar.Name = "picPlayerAvatar";
            this.picPlayerAvatar.Size = new System.Drawing.Size(133, 123);
            this.picPlayerAvatar.TabIndex = 7;
            this.picPlayerAvatar.TabStop = false;
            // 
            // SỬA LỖI 2: Định nghĩa các control bị thiếu
            // 
            // lblMP
            // 
            this.lblMP.AutoSize = true;
            this.lblMP.Location = new System.Drawing.Point(200, 118); // Thêm vào
            this.lblMP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMP.Name = "lblMP";
            this.lblMP.Size = new System.Drawing.Size(57, 16);
            this.lblMP.TabIndex = 3;
            this.lblMP.Text = "MP: 0 / 0";
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(200, 214); // Thêm vào
            this.lblSpeed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(63, 16);
            this.lblSpeed.TabIndex = 6;
            this.lblSpeed.Text = "Tốc độ: 0";
            // 
            // frmStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 282);
            this.Controls.Add(this.picPlayerAvatar);
            this.Controls.Add(this.lblDefense);
            this.Controls.Add(this.lblStrength);
            this.Controls.Add(this.lblHP);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblTitle);
            // SỬA LỖI 2: Thêm control vào Form
            this.Controls.Add(this.lblMP);
            this.Controls.Add(this.lblSpeed);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmStatus";
            this.Text = "frmStatus";
            ((System.ComponentModel.ISupportInitialize)(this.picPlayerAvatar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblHP;
        private System.Windows.Forms.Label lblStrength;
        private System.Windows.Forms.Label lblDefense;
        private System.Windows.Forms.PictureBox picPlayerAvatar;
        // SỬA LỖI 2: Khai báo các biến control bị thiếu
        private System.Windows.Forms.Label lblMP;
        private System.Windows.Forms.Label lblSpeed;
    }
}
