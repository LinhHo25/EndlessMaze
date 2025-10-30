namespace Main
{
    partial class frmMenu
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
            this.lblPaused = new System.Windows.Forms.Label();
            this.btnItem = new System.Windows.Forms.Button();
            this.btnStatus = new System.Windows.Forms.Button();
            this.btnEquipment = new System.Windows.Forms.Button();
            this.btnSaveLoad = new System.Windows.Forms.Button();
            this.btnMainMenu = new System.Windows.Forms.Button();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.lblVolume = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPaused
            // 
            this.lblPaused.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPaused.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold);
            this.lblPaused.Location = new System.Drawing.Point(0, 0);
            this.lblPaused.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPaused.Name = "lblPaused";
            this.lblPaused.Padding = new System.Windows.Forms.Padding(0, 12, 0, 12);
            this.lblPaused.Size = new System.Drawing.Size(379, 62);
            this.lblPaused.TabIndex = 0;
            this.lblPaused.Text = "PAUSED";
            this.lblPaused.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
  
            // 
            // btnItem
            // 
            this.btnItem.Location = new System.Drawing.Point(56, 74);
            this.btnItem.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnItem.Name = "btnItem";
            this.btnItem.Size = new System.Drawing.Size(267, 49);
            this.btnItem.TabIndex = 1;
            this.btnItem.Text = "ITEM";
            this.btnItem.UseVisualStyleBackColor = true;
            this.btnItem.Click += new System.EventHandler(this.btnItem_Click);
            // 
            // btnStatus
            // 
            this.btnStatus.Location = new System.Drawing.Point(56, 130);
            this.btnStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStatus.Name = "btnStatus";
            this.btnStatus.Size = new System.Drawing.Size(267, 49);
            this.btnStatus.TabIndex = 2;
            this.btnStatus.Text = "STATUS";
            this.btnStatus.UseVisualStyleBackColor = true;
            this.btnStatus.Click += new System.EventHandler(this.btnStatus_Click);
            // 
            // btnEquipment
            // 
            this.btnEquipment.Location = new System.Drawing.Point(56, 187);
            this.btnEquipment.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnEquipment.Name = "btnEquipment";
            this.btnEquipment.Size = new System.Drawing.Size(267, 49);
            this.btnEquipment.TabIndex = 3;
            this.btnEquipment.Text = "EQUIPMENT";
            this.btnEquipment.UseVisualStyleBackColor = true;
            this.btnEquipment.Click += new System.EventHandler(this.btnEquipment_Click);
            // 
            // btnSaveLoad
            // 
            this.btnSaveLoad.Location = new System.Drawing.Point(56, 244);
            this.btnSaveLoad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSaveLoad.Name = "btnSaveLoad";
            this.btnSaveLoad.Size = new System.Drawing.Size(267, 49);
            this.btnSaveLoad.TabIndex = 4;
            this.btnSaveLoad.Text = "SAVE / LOAD";
            this.btnSaveLoad.UseVisualStyleBackColor = true;
            this.btnSaveLoad.Click += new System.EventHandler(this.btnSaveLoad_Click);
            // 
            // btnMainMenu
            // 
            this.btnMainMenu.Location = new System.Drawing.Point(56, 300);
            this.btnMainMenu.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnMainMenu.Name = "btnMainMenu";
            this.btnMainMenu.Size = new System.Drawing.Size(267, 49);
            this.btnMainMenu.TabIndex = 5;
            this.btnMainMenu.Text = "MAIN MENU";
            this.btnMainMenu.UseVisualStyleBackColor = true;
            this.btnMainMenu.Click += new System.EventHandler(this.btnMainMenu_Click);
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.Location = new System.Drawing.Point(56, 390);
            this.trackBarVolume.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Size = new System.Drawing.Size(267, 56);
            this.trackBarVolume.TabIndex = 6;
            this.trackBarVolume.Value = 5;
            this.trackBarVolume.Scroll += new System.EventHandler(this.trackBarVolume_Scroll);
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.Location = new System.Drawing.Point(52, 370);
            this.lblVolume.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(66, 16);
            this.lblVolume.TabIndex = 7;
            this.lblVolume.Text = "Âm lượng:";
            // 
            // frmMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 460);
            this.Controls.Add(this.lblVolume);
            this.Controls.Add(this.trackBarVolume);
            this.Controls.Add(this.btnMainMenu);
            this.Controls.Add(this.btnSaveLoad);
            this.Controls.Add(this.btnEquipment);
            this.Controls.Add(this.btnStatus);
            this.Controls.Add(this.btnItem);
            this.Controls.Add(this.lblPaused);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pause Menu";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenu_FormClosed);
          
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPaused; // Đảm bảo tên đúng
        private System.Windows.Forms.Button btnItem;
        private System.Windows.Forms.Button btnStatus;
        private System.Windows.Forms.Button btnEquipment;
        private System.Windows.Forms.Button btnSaveLoad;
        private System.Windows.Forms.Button btnMainMenu;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.Label lblVolume;
    }
}

