namespace Main
{
    partial class frmMainGame // <-- Tên lớp đã được sửa
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
            this.components = new System.ComponentModel.Container();
            this.gameTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // gameTimer
            // 
            this.gameTimer.Enabled = true; // Đảm bảo Timer tự động chạy
            this.gameTimer.Interval = 30; // 33 FPS
            this.gameTimer.Tick += new System.EventHandler(this.GameTimer_Tick);
            // 
            // frmMainGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1856, 854); // Kích thước mới
            this.DoubleBuffered = true; // Quan trọng để giảm nhấp nháy khi vẽ game
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle; // Khóa kích thước form
            this.MaximizeBox = false; // Không cho phép phóng to
            this.MinimizeBox = false; // Không cho phép thu nhỏ
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "frmMainGame";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmMainGame";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmMainGame_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMainGame_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMainGame_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmMainGame_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmMainGame_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer gameTimer;
    }
}
