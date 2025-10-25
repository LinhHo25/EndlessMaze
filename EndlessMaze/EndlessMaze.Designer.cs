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
            this.btnExit = new System.Windows.Forms.Button();
            this.btnStartGame = new System.Windows.Forms.Button();
            this.lblGameTitle = new System.Windows.Forms.Label();
            this.pbGameCanvas = new System.Windows.Forms.PictureBox();
            this.pnlGameUI = new System.Windows.Forms.Panel();
            this.progPlayerHP = new System.Windows.Forms.ProgressBar();
            this.lblPlayerHP = new System.Windows.Forms.Label();
            this.lblScore = new System.Windows.Forms.Label();
            this.lblLevel = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.pnlGameOver = new System.Windows.Forms.Panel();
            this.lblGameOverTitle = new System.Windows.Forms.Label();
            this.btnBackToMenu = new System.Windows.Forms.Button();
            this.lblFinalScore = new System.Windows.Forms.Label();
            this.lblFinalTime = new System.Windows.Forms.Label();
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
            this.pnlMainMenu.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlMainMenu.Name = "pnlMainMenu";
            this.pnlMainMenu.Size = new System.Drawing.Size(1280, 788);
            this.pnlMainMenu.TabIndex = 0;
            // 
            // btnExit
            // 
            this.btnExit.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(507, 406);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(267, 74);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Thoát Game";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnStartGame
            // 
            this.btnStartGame.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnStartGame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnStartGame.FlatAppearance.BorderSize = 0;
            this.btnStartGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartGame.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartGame.ForeColor = System.Drawing.Color.White;
            this.btnStartGame.Location = new System.Drawing.Point(507, 308);
            this.btnStartGame.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStartGame.Name = "btnStartGame";
            this.btnStartGame.Size = new System.Drawing.Size(267, 74);
            this.btnStartGame.TabIndex = 1;
            this.btnStartGame.Text = "Bắt Đầu";
            this.btnStartGame.UseVisualStyleBackColor = false;
            this.btnStartGame.Click += new System.EventHandler(this.btnStartGame_Click);
            // 
            // lblGameTitle
            // 
            this.lblGameTitle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblGameTitle.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGameTitle.ForeColor = System.Drawing.Color.White;
            this.lblGameTitle.Location = new System.Drawing.Point(373, 123);
            this.lblGameTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGameTitle.Name = "lblGameTitle";
            this.lblGameTitle.Size = new System.Drawing.Size(533, 98);
            this.lblGameTitle.TabIndex = 0;
            this.lblGameTitle.Text = "ENDLESS MAZE";
            this.lblGameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbGameCanvas
            // 
            this.pbGameCanvas.BackColor = System.Drawing.Color.Black;
            this.pbGameCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbGameCanvas.Location = new System.Drawing.Point(0, 98);
            this.pbGameCanvas.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pbGameCanvas.Name = "pbGameCanvas";
            this.pbGameCanvas.Size = new System.Drawing.Size(1280, 690);
            this.pbGameCanvas.TabIndex = 1;
            this.pbGameCanvas.TabStop = false;
            this.pbGameCanvas.Click += new System.EventHandler(this.pbGameCanvas_Click);
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
            this.pnlGameUI.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlGameUI.Name = "pnlGameUI";
            this.pnlGameUI.Size = new System.Drawing.Size(1280, 98);
            this.pnlGameUI.TabIndex = 2;
            // 
            // progPlayerHP
            // 
            this.progPlayerHP.Location = new System.Drawing.Point(32, 55);
            this.progPlayerHP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.progPlayerHP.Name = "progPlayerHP";
            this.progPlayerHP.Size = new System.Drawing.Size(333, 25);
            this.progPlayerHP.TabIndex = 4;
            // 
            // lblPlayerHP
            // 
            this.lblPlayerHP.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlayerHP.ForeColor = System.Drawing.Color.White;
            this.lblPlayerHP.Location = new System.Drawing.Point(27, 18);
            this.lblPlayerHP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPlayerHP.Name = "lblPlayerHP";
            this.lblPlayerHP.Size = new System.Drawing.Size(267, 31);
            this.lblPlayerHP.TabIndex = 3;
            this.lblPlayerHP.Text = "HP: 100 / 100";
            // 
            // lblScore
            // 
            this.lblScore.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblScore.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScore.ForeColor = System.Drawing.Color.White;
            this.lblScore.Location = new System.Drawing.Point(573, 55);
            this.lblScore.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(133, 37);
            this.lblScore.TabIndex = 2;
            this.lblScore.Text = "Điểm: 0";
            this.lblScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLevel
            // 
            this.lblLevel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblLevel.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLevel.ForeColor = System.Drawing.Color.White;
            this.lblLevel.Location = new System.Drawing.Point(573, 18);
            this.lblLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(133, 37);
            this.lblLevel.TabIndex = 1;
            this.lblLevel.Text = "Màn: 1";
            this.lblLevel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTime
            // 
            this.lblTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTime.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.ForeColor = System.Drawing.Color.White;
            this.lblTime.Location = new System.Drawing.Point(1093, 31);
            this.lblTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(160, 37);
            this.lblTime.TabIndex = 0;
            this.lblTime.Text = "Time: 00:00";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.pnlGameOver.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlGameOver.Name = "pnlGameOver";
            this.pnlGameOver.Size = new System.Drawing.Size(1280, 788);
            this.pnlGameOver.TabIndex = 3;
            // 
            // lblGameOverTitle
            // 
            this.lblGameOverTitle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblGameOverTitle.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGameOverTitle.ForeColor = System.Drawing.Color.IndianRed;
            this.lblGameOverTitle.Location = new System.Drawing.Point(373, 98);
            this.lblGameOverTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGameOverTitle.Name = "lblGameOverTitle";
            this.lblGameOverTitle.Size = new System.Drawing.Size(533, 98);
            this.lblGameOverTitle.TabIndex = 3;
            this.lblGameOverTitle.Text = "GAME OVER";
            this.lblGameOverTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnBackToMenu
            // 
            this.btnBackToMenu.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnBackToMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnBackToMenu.FlatAppearance.BorderSize = 0;
            this.btnBackToMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackToMenu.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackToMenu.ForeColor = System.Drawing.Color.White;
            this.btnBackToMenu.Location = new System.Drawing.Point(507, 431);
            this.btnBackToMenu.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnBackToMenu.Name = "btnBackToMenu";
            this.btnBackToMenu.Size = new System.Drawing.Size(267, 74);
            this.btnBackToMenu.TabIndex = 2;
            this.btnBackToMenu.Text = "Về Menu";
            this.btnBackToMenu.UseVisualStyleBackColor = false;
            this.btnBackToMenu.Click += new System.EventHandler(this.btnBackToMenu_Click);
            // 
            // lblFinalScore
            // 
            this.lblFinalScore.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblFinalScore.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFinalScore.ForeColor = System.Drawing.Color.White;
            this.lblFinalScore.Location = new System.Drawing.Point(373, 246);
            this.lblFinalScore.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFinalScore.Name = "lblFinalScore";
            this.lblFinalScore.Size = new System.Drawing.Size(533, 49);
            this.lblFinalScore.TabIndex = 1;
            this.lblFinalScore.Text = "Điểm: 0";
            this.lblFinalScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFinalTime
            // 
            this.lblFinalTime.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblFinalTime.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFinalTime.ForeColor = System.Drawing.Color.White;
            this.lblFinalTime.Location = new System.Drawing.Point(373, 320);
            this.lblFinalTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFinalTime.Name = "lblFinalTime";
            this.lblFinalTime.Size = new System.Drawing.Size(533, 49);
            this.lblFinalTime.TabIndex = 0;
            this.lblFinalTime.Text = "Thời gian: 00:00";
            this.lblFinalTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EndlessMaze
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 788);
            this.Controls.Add(this.pbGameCanvas);
            this.Controls.Add(this.pnlGameUI);
            this.Controls.Add(this.pnlGameOver);
            this.Controls.Add(this.pnlMainMenu);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "EndlessMaze";
            this.Text = "Endless Maze";
            this.Load += new System.EventHandler(this.EndlessMaze_Load);
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
