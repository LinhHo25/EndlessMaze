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
            this.lblMP = new System.Windows.Forms.Label();
            this.lblStrength = new System.Windows.Forms.Label();
            this.lblDefense = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.picPlayerAvatar = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picPlayerAvatar)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(376, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "STATUS";
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
            // lblHP
            // 
            this.lblHP.AutoSize = true;
            this.lblHP.Location = new System.Drawing.Point(150, 70);
            this.lblHP.Name = "lblHP";
            this.lblHP.Size = new System.Drawing.Size(53, 13);
            this.lblHP.TabIndex = 2;
            this.lblHP.Text = "HP: 0 / 0";
            // 
            // lblMP
            // 
            this.lblMP.AutoSize = true;
            this.lblMP.Location = new System.Drawing.Point(150, 100);
            this.lblMP.Name = "lblMP";
            this.lblMP.Size = new System.Drawing.Size(54, 13);
            this.lblMP.TabIndex = 3;
            this.lblMP.Text = "MP: 0 / 0";
            // 
            // lblStrength
            // 
            this.lblStrength.AutoSize = true;
            this.lblStrength.Location = new System.Drawing.Point(150, 130);
            this.lblStrength.Name = "lblStrength";
            this.lblStrength.Size = new System.Drawing.Size(69, 13);
            this.lblStrength.TabIndex = 4;
            this.lblStrength.Text = "Sức mạnh: 0";
            // 
            // lblDefense
            // 
            this.lblDefense.AutoSize = true;
            this.lblDefense.Location = new System.Drawing.Point(150, 160);
            this.lblDefense.Name = "lblDefense";
            this.lblDefense.Size = new System.Drawing.Size(69, 13);
            this.lblDefense.TabIndex = 5;
            this.lblDefense.Text = "Phòng thủ: 0";
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(150, 190);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(50, 13);
            this.lblSpeed.TabIndex = 6;
            this.lblSpeed.Text = "Tốc độ: 0";
            // 
            // picPlayerAvatar
            // 
            this.picPlayerAvatar.BackColor = System.Drawing.Color.Gray;
            this.picPlayerAvatar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPlayerAvatar.Location = new System.Drawing.Point(20, 70);
            this.picPlayerAvatar.Name = "picPlayerAvatar";
            this.picPlayerAvatar.Size = new System.Drawing.Size(100, 100);
            this.picPlayerAvatar.TabIndex = 7;
            this.picPlayerAvatar.TabStop = false;
            // 
            // frmStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 250);
            this.Controls.Add(this.picPlayerAvatar);
            this.Controls.Add(this.lblSpeed);
            this.Controls.Add(this.lblDefense);
            this.Controls.Add(this.lblStrength);
            this.Controls.Add(this.lblMP);
            this.Controls.Add(this.lblHP);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
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
        private System.Windows.Forms.Label lblMP;
        private System.Windows.Forms.Label lblStrength;
        private System.Windows.Forms.Label lblDefense;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.PictureBox picPlayerAvatar;
    }
}
