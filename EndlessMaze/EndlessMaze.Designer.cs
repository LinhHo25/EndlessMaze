namespace EndlessMaze
{
    partial class EndlessMaze
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
            this.pnlMainMenu = new System.Windows.Forms.Panel();
            this.lblGameTitle = new System.Windows.Forms.Label();
            this.btnStartGame = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.pbGameCanvas = new System.Windows.Forms.PictureBox();
            this.pnlGameUI = new System.Windows.Forms.Panel();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblLevel = new System.Windows.Forms.Label();
            this.lblScore = new System.Windows.Forms.Label();
            this.lblPlayerHP = new System.Windows.Forms.Label();
            this.progPlayerHP = new System.Windows.Forms.ProgressBar();
            this.pnlGameOver = new System.Windows.Forms.Panel();
            this.lblFinalTime = new System.Windows.Forms.Label();
            this.lblFinalScore = new System.Windows.Forms.Label();
            this.btnBackToMenu = new System.Windows.Forms.Button();
            this.lblGameOverTitle = new System.Windows.Forms.Label();
            this.pnlMainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGameCanvas)).BeginInit();
            this.pnlGameUI.SuspendLayout();
            this.pnlGameOver.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMainMenu
            // 
            this.pnlMainMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlMainMenu.Controls.Add(this.btnExit);
            this.pnlMainMenu.Controls.Add(this.btnStartGame);
            this.pnlMainMenu.Controls.Add(this.lblGameTitle);
            this.pnlMainMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMainMenu.Location = new System.Drawing.Point(0, 0);
            this.pnlMainMenu.Name = "pnlMainMenu";
            this.pnlMainMenu.Size = new System.Drawing.Size(960, 640);
            this.pnlMainMenu.TabIndex = 0;
            // 
            // lblGameTitle
            // 
            this.lblGameTitle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblGameTitle.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGameTitle.ForeColor = System.Drawing.Color.White;
            this.lblGameTitle.Location = new System.Drawing.Point(280, 100);
            this.lblGameTitle.Name = "lblGameTitle";
            this.lblGameTitle.Size = new System.Drawing.Size(400, 80);
            this.lblGameTitle.TabIndex = 0;
            this.lblGameTitle.Text = "ENDLESS MAZE";
            this.lblGameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnStartGame
            // 
            this.btnStartGame.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnStartGame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnStartGame.FlatAppearance.BorderSize = 0;
            this.btnStartGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartGame.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartGame.ForeColor = System.Drawing.Color.White;
            this.btnStartGame.Location = new System.Drawing.Point(380, 250);
            this.btnStartGame.Name = "btnStartGame";
            this.btnStartGame.Size = new System.Drawing.Size(200, 60);
            this.btnStartGame.TabIndex = 1;
            this.btnStartGame.Text = "Bắt Đầu";
            this.btnStartGame.UseVisualStyleBackColor = false;
            this.btnStartGame.Click += new System.EventHandler(this.btnStartGame_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(380, 330);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(200, 60);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Thoát Game";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // pbGameCanvas
            // 
            this.pbGameCanvas.BackColor = System.Drawing.Color.Black;
            this.pbGameCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbGameCanvas.Location = new System.Drawing.Point(0, 80);
            this.pbGameCanvas.Name = "pbGameCanvas";
            this.pbGameCanvas.Size = new System.Drawing.Size(960, 560);
            this.pbGameCanvas.TabIndex = 1;
            this.pbGameCanvas.TabStop = false;
            this.pbGameCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.pbGameCanvas_Paint);
            // 
            // pnlGameUI
            // 
            this.pnlGameUI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.pnlGameUI.Controls.Add(this.progPlayerHP);
            this.pnlGameUI.Controls.Add(this.lblPlayerHP);
            this.pnlGameUI.Controls.Add(this.lblScore);
            this.pnlGameUI.Controls.Add(this.lblLevel);
            this.pnlGameUI.Controls.Add(this.lblTime);
            this.pnlGameUI.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGameUI.Location = new System.Drawing.Point(0, 0);
            this.pnlGameUI.Name = "pnlGameUI";
            this.pnlGameUI.Size = new System.Drawing.Size(960, 80);
            this.pnlGameUI.TabIndex = 2;
            // 
            // lblTime
            // 
            this.lblTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTime.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.ForeColor = System.Drawing.Color.White;
            this.lblTime.Location = new System.Drawing.Point(820, 25);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(120, 30);
            this.lblTime.TabIndex = 0;
            this.lblTime.Text = "Time: 00:00";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblLevel
            // 
            this.lblLevel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblLevel.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLevel.ForeColor = System.Drawing.Color.White;
            this.lblLevel.Location = new System.Drawing.Point(430, 15);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(100, 30);
            this.lblLevel.TabIndex = 1;
            this.lblLevel.Text = "Màn: 1";
            this.lblLevel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblScore
            // 
            this.lblScore.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblScore.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScore.ForeColor = System.Drawing.Color.White;
            this.lblScore.Location = new System.Drawing.Point(430, 45);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(100, 30);
            this.lblScore.TabIndex = 2;
            this.lblScore.Text = "Điểm: 0";
            this.lblScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPlayerHP
            // 
            this.lblPlayerHP.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlayerHP.ForeColor = System.Drawing.Color.White;
            this.lblPlayerHP.Location = new System.Drawing.Point(20, 15);
            this.lblPlayerHP.Name = "lblPlayerHP";
            this.lblPlayerHP.Size = new System.Drawing.Size(200, 25);
            this.lblPlayerHP.TabIndex = 3;
            this.lblPlayerHP.Text = "HP: 100 / 100";
            // 
            // progPlayerHP
            // 
            this.progPlayerHP.Location = new System.Drawing.Point(24, 45);
            this.progPlayerHP.Name = "progPlayerHP";
            this.progPlayerHP.Size = new System.Drawing.Size(250, 20);
            this.progPlayerHP.TabIndex = 4;
            // 
            // pnlGameOver
            // 
            this.pnlGameOver.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlGameOver.Controls.Add(this.lblGameOverTitle);
            this.pnlGameOver.Controls.Add(this.btnBackToMenu);
            this.pnlGameOver.Controls.Add(this.lblFinalScore);
            this.pnlGameOver.Controls.Add(this.lblFinalTime);
            this.pnlGameOver.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGameOver.Location = new System.Drawing.Point(0, 0);
            this.pnlGameOver.Name = "pnlGameOver";
            this.pnlGameOver.Size = new System.Drawing.Size(960, 640);
            this.pnlGameOver.TabIndex = 3;
            // 
            // lblFinalTime
            // 
            this.lblFinalTime.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblFinalTime.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFinalTime.ForeColor = System.Drawing.Color.White;
            this.lblFinalTime.Location = new System.Drawing.Point(280, 260);
            this.lblFinalTime.Name = "lblFinalTime";
            this.lblFinalTime.Size = new System.Drawing.Size(400, 40);
            this.lblFinalTime.TabIndex = 0;
            this.lblFinalTime.Text = "Thời gian: 00:00";
            this.lblFinalTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFinalScore
            // 
            this.lblFinalScore.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblFinalScore.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFinalScore.ForeColor = System.Drawing.Color.White;
            this.lblFinalScore.Location = new System.Drawing.Point(280, 200);
            this.lblFinalScore.Name = "lblFinalScore";
            this.lblFinalScore.Size = new System.Drawing.Size(400, 40);
            this.lblFinalScore.TabIndex = 1;
            this.lblFinalScore.Text = "Điểm: 0";
            this.lblFinalScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnBackToMenu
            // 
            this.btnBackToMenu.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnBackToMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnBackToMenu.FlatAppearance.BorderSize = 0;
            this.btnBackToMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackToMenu.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackToMenu.ForeColor = System.Drawing.Color.White;
            this.btnBackToMenu.Location = new System.Drawing.Point(380, 350);
            this.btnBackToMenu.Name = "btnBackToMenu";
            this.btnBackToMenu.Size = new System.Drawing.Size(200, 60);
            this.btnBackToMenu.TabIndex = 2;
            this.btnBackToMenu.Text = "Về Menu";
            this.btnBackToMenu.UseVisualStyleBackColor = false;
            this.btnBackToMenu.Click += new System.EventHandler(this.btnBackToMenu_Click);
            // 
            // lblGameOverTitle
            // 
            this.lblGameOverTitle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblGameOverTitle.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGameOverTitle.ForeColor = System.Drawing.Color.IndianRed;
            this.lblGameOverTitle.Location = new System.Drawing.Point(280, 80);
            this.lblGameOverTitle.Name = "lblGameOverTitle";
            this.lblGameOverTitle.Size = new System.Drawing.Size(400, 80);
            this.lblGameOverTitle.TabIndex = 3;
            this.lblGameOverTitle.Text = "GAME OVER";
            this.lblGameOverTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EndlessMaze
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 640);
            this.Controls.Add(this.pbGameCanvas);
            this.Controls.Add(this.pnlGameUI);
            this.Controls.Add(this.pnlGameOver);
            this.Controls.Add(this.pnlMainMenu);
            this.Name = "EndlessMaze";
            this.Text = "Endless Maze";
            this.pnlMainMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbGameCanvas)).EndInit();
            this.pnlGameUI.ResumeLayout(false);
            this.pnlGameOver.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlMainMenu;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnStartGame;
        private System.Windows.Forms.Label lblGameTitle;
        private System.Windows.Forms.PictureBox pbGameCanvas;
        private System.Windows.Forms.Panel pnlGameUI;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.Label lblPlayerHP;
        private System.Windows.Forms.ProgressBar progPlayerHP;
        private System.Windows.Forms.Panel pnlGameOver;
        private System.Windows.Forms.Label lblGameOverTitle;
        private System.Windows.Forms.Button btnBackToMenu;
        private System.Windows.Forms.Label lblFinalScore;
        private System.Windows.Forms.Label lblFinalTime;
    }
}
