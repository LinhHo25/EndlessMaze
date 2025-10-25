namespace Main
{
    partial class frmLoadPlay
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lvSavedGames = new System.Windows.Forms.ListView();
            this.colID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colProgress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnBack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(0, 20, 0, 10);
            this.lblTitle.Size = new System.Drawing.Size(900, 70);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "DANH SÁCH LƯỢT CHƠI ĐÃ LƯU";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lvSavedGames
            // 
            this.lvSavedGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colID,
            this.colTitle,
            this.colTime,
            this.colProgress});
            this.lvSavedGames.FullRowSelect = true;
            this.lvSavedGames.GridLines = true;
            this.lvSavedGames.HideSelection = false;
            this.lvSavedGames.Location = new System.Drawing.Point(20, 80);
            this.lvSavedGames.Name = "lvSavedGames";
            this.lvSavedGames.Size = new System.Drawing.Size(860, 450);
            this.lvSavedGames.TabIndex = 1;
            this.lvSavedGames.UseCompatibleStateImageBehavior = false;
            this.lvSavedGames.View = System.Windows.Forms.View.Details;
            this.lvSavedGames.DoubleClick += new System.EventHandler(this.ListViewSaves_DoubleClick);
            // 
            // colID
            // 
            this.colID.Text = "ID";
            this.colID.Width = 50;
            // 
            // colTitle
            // 
            this.colTitle.Text = "Tiêu đề";
            this.colTitle.Width = 250;
            // 
            // colTime
            // 
            this.colTime.Text = "Thời gian lưu";
            this.colTime.Width = 150;
            // 
            // colProgress
            // 
            this.colProgress.Text = "Tiến độ";
            this.colProgress.Width = 400;
            // 
            // btnBack
            // 
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.Location = new System.Drawing.Point(360, 550);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(180, 45);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "QUAY LẠI";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click); // Cần tạo sự kiện này trong frmLoadPlay.cs
            // 
            // frmLoadPlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 610);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.lvSavedGames);
            this.Controls.Add(this.lblTitle);
            this.Name = "frmLoadPlay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tải Lượt Chơi";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmLoadPlay_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ListView lvSavedGames;
        private System.Windows.Forms.ColumnHeader colID;
        private System.Windows.Forms.ColumnHeader colTitle;
        private System.Windows.Forms.ColumnHeader colTime;
        private System.Windows.Forms.ColumnHeader colProgress;
        private System.Windows.Forms.Button btnBack;
    }
}
