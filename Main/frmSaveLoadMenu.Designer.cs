namespace Main
{
    partial class frmSaveLoadMenu
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
            this.lvSaveSlots = new System.Windows.Forms.ListView();
            this.colSlotName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTimestamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnSave = new System.Windows.Forms.Button(); // ĐỔI TÊN ĐỂ KHỚP CS0103
            this.btnLoad = new System.Windows.Forms.Button(); // ĐỔI TÊN ĐỂ KHỚP CS0103
            this.btnBack = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label(); // ĐỔI TÊN ĐỂ KHỚP CS0103
            this.lblTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lvSaveSlots
            // 
            this.lvSaveSlots.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSaveSlots.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSlotName,
            this.colTimestamp});
            this.lvSaveSlots.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvSaveSlots.FullRowSelect = true;
            this.lvSaveSlots.GridLines = true;
            this.lvSaveSlots.HideSelection = false;
            this.lvSaveSlots.Location = new System.Drawing.Point(12, 60);
            this.lvSaveSlots.MultiSelect = false;
            this.lvSaveSlots.Name = "lvSaveSlots";
            this.lvSaveSlots.Size = new System.Drawing.Size(560, 280);
            this.lvSaveSlots.TabIndex = 0;
            this.lvSaveSlots.UseCompatibleStateImageBehavior = false;
            this.lvSaveSlots.View = System.Windows.Forms.View.Details;
            this.lvSaveSlots.DoubleClick += new System.EventHandler(this.lvSaveSlots_DoubleClick); // SỬA LỖI CS1061
            // 
            // colSlotName
            // 
            this.colSlotName.Text = "Tên Lượt Lưu";
            this.colSlotName.Width = 350;
            // 
            // colTimestamp
            // 
            this.colTimestamp.Text = "Thời Gian Lưu";
            this.colTimestamp.Width = 200;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(12, 380);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(180, 50);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "LƯU GAME";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSaveGame_Click); // SỬA LỖI CS1061
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnLoad.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoad.Location = new System.Drawing.Point(202, 380);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(180, 50);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "TẢI GAME";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoadGame_Click); // SỬA LỖI CS1061
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.Location = new System.Drawing.Point(392, 380);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(180, 50);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "QUAY LẠI";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.Location = new System.Drawing.Point(12, 350);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(117, 17);
            this.lblInfo.TabIndex = 4;
            this.lblInfo.Text = "Đang tải danh sách...";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(190, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(205, 30);
            this.lblTitle.TabIndex = 5;
            this.lblTitle.Text = "LƯU / TẢI LƯỢT CHƠI";
            // 
            // frmSaveLoadMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 441);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lvSaveSlots);
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "frmSaveLoadMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Lưu / Tải Game";
            this.Load += new System.EventHandler(this.frmSaveLoadMenu_Load); // SỬA LỖI CS1061
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvSaveSlots;
        private System.Windows.Forms.ColumnHeader colSlotName;
        private System.Windows.Forms.ColumnHeader colTimestamp;
        private System.Windows.Forms.Button btnSave; // ĐÃ ĐỔI TÊN
        private System.Windows.Forms.Button btnLoad; // ĐÃ ĐỔI TÊN
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label lblInfo; // ĐÃ ĐỔI TÊN
        private System.Windows.Forms.Label lblTitle;
    }
}

