namespace Main
{
    partial class frmItem
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ListView lvInventory; // Đổi tên
        private System.Windows.Forms.ColumnHeader colItemName;
        private System.Windows.Forms.ColumnHeader colQuantity;
        private System.Windows.Forms.Button btnUseItem; // Nút mới
        private System.Windows.Forms.Button btnClose; // Nút mới

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
            this.lvInventory = new System.Windows.Forms.ListView();
            this.colItemName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colQuantity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnUseItem = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.lblTitle.Size = new System.Drawing.Size(384, 50);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "TÚI ĐỒ (ITEM)";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lvInventory
            // 
            this.lvInventory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(195)))), ((int)(((byte)(154)))));
            this.lvInventory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colItemName,
            this.colQuantity});
            this.lvInventory.Font = new System.Drawing.Font("Arial", 10F);
            this.lvInventory.FullRowSelect = true;
            this.lvInventory.GridLines = true;
            this.lvInventory.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvInventory.HideSelection = false;
            this.lvInventory.Location = new System.Drawing.Point(12, 53);
            this.lvInventory.MultiSelect = false;
            this.lvInventory.Name = "lvInventory";
            this.lvInventory.Size = new System.Drawing.Size(360, 250);
            this.lvInventory.TabIndex = 1;
            this.lvInventory.UseCompatibleStateImageBehavior = false;
            this.lvInventory.View = System.Windows.Forms.View.Details;
            // 
            // colItemName
            // 
            this.colItemName.Text = "Vật phẩm";
            this.colItemName.Width = 250;
            // 
            // colQuantity
            // 
            this.colQuantity.Text = "Số lượng";
            this.colQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colQuantity.Width = 100;
            // 
            // btnUseItem
            // 
            this.btnUseItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(143)))), ((int)(((byte)(118)))));
            this.btnUseItem.FlatAppearance.BorderSize = 0;
            this.btnUseItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUseItem.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.btnUseItem.ForeColor = System.Drawing.Color.White;
            this.btnUseItem.Location = new System.Drawing.Point(12, 309);
            this.btnUseItem.Name = "btnUseItem";
            this.btnUseItem.Size = new System.Drawing.Size(170, 40);
            this.btnUseItem.TabIndex = 2;
            this.btnUseItem.Text = "SỬ DỤNG";
            this.btnUseItem.UseVisualStyleBackColor = false;
            this.btnUseItem.Click += new System.EventHandler(this.btnUseItem_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Gray;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(202, 309);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(170, 40);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "ĐÓNG";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(179)))), ((int)(((byte)(132)))));
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnUseItem);
            this.Controls.Add(this.lvInventory);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmItem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Túi đồ";
            this.Load += new System.EventHandler(this.frmItem_Load);
            this.ResumeLayout(false);
        }
        #endregion
    }
}

