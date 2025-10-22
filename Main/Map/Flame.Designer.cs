namespace Main.Map
{
    partial class Flame
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
            this.timerLava = new System.Windows.Forms.Timer(this.components);
            this.timerSweat = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timerGameLoop
            // 
            this.timerGameLoop.Interval = 16;
            this.timerGameLoop.Tick += new System.EventHandler(this.timerGameLoop_Tick);
            // 
            // timerLava
            // 
            this.timerLava.Interval = 500;
            this.timerLava.Tick += new System.EventHandler(this.timerLava_Tick);
            // 
            // timerSweat
            // 
            this.timerSweat.Interval = 1000;
            this.timerSweat.Tick += new System.EventHandler(this.timerSweat_Tick);
            // 
            // Flame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Flame";
            this.Text = "Flame";
            this.Load += new System.EventHandler(this.Flame_Load);
            
            this.ResumeLayout(false);

        }

        #endregion

        // KHAI BÁO CÁC TIMER ĐỂ SỬA LỖI CS0103
        private System.Windows.Forms.Timer timerGameLoop;
        private System.Windows.Forms.Timer timerLava;
        private System.Windows.Forms.Timer timerSweat;
    }
}

