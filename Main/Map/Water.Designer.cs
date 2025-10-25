namespace Main.Map
{
    partial class Water
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
            this.timerGameLoop = new System.Windows.Forms.Timer(this.components);
            this.timerRain = new System.Windows.Forms.Timer(this.components);
            this.timerFall = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timerGameLoop
            // 
            this.timerGameLoop.Interval = 16; // Khoảng 60 FPS
            this.timerGameLoop.Tick += new System.EventHandler(this.timerGameLoop_Tick);
            // 
            // timerRain
            // 
            this.timerRain.Interval = 3000; // 3 giây
            this.timerRain.Tick += new System.EventHandler(this.timerRain_Tick);
            // 
            // timerFall
            // 
            this.timerFall.Interval = 500; // 0.5 giây (sẽ được override trong logic)
            this.timerFall.Tick += new System.EventHandler(this.timerFall_Tick);
            // 
            // Water
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480); // Kích thước chuẩn 25x15 tiles (800x480)
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Water";
            this.Text = "Water";
            this.Load += new System.EventHandler(this.Water_Load);

            this.ResumeLayout(false);

        }

        #endregion

        // KHAI BÁO CÁC TIMER ĐỂ SỬA LỖI CS0103
        private System.Windows.Forms.Timer timerGameLoop;
        private System.Windows.Forms.Timer timerRain;
        private System.Windows.Forms.Timer timerFall;
    }
}
