namespace UISmartLock
{
    partial class Form2
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
            this.PicBoxAdmin = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SaveTBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SavePathBtn = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.PicBoxAdmin)).BeginInit();
            this.SuspendLayout();
            // 
            // PicBoxAdmin
            // 
            this.PicBoxAdmin.BackColor = System.Drawing.Color.White;
            this.PicBoxAdmin.Location = new System.Drawing.Point(0, 0);
            this.PicBoxAdmin.Name = "PicBoxAdmin";
            this.PicBoxAdmin.Size = new System.Drawing.Size(300, 250);
            this.PicBoxAdmin.TabIndex = 1;
            this.PicBoxAdmin.TabStop = false;
            this.PicBoxAdmin.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PicBox_MouseDown);
            this.PicBoxAdmin.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PicBox_MouseMove);
            this.PicBoxAdmin.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PicBox_MouseUp);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 256);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 46);
            this.button1.TabIndex = 5;
            this.button1.Text = "Save Fixed Keys";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SaveTBox
            // 
            this.SaveTBox.Location = new System.Drawing.Point(12, 321);
            this.SaveTBox.Name = "SaveTBox";
            this.SaveTBox.Size = new System.Drawing.Size(202, 20);
            this.SaveTBox.TabIndex = 6;
            this.SaveTBox.Text = "D:\\TestKey";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 305);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Working directory:";
            // 
            // SavePathBtn
            // 
            this.SavePathBtn.Location = new System.Drawing.Point(220, 319);
            this.SavePathBtn.Name = "SavePathBtn";
            this.SavePathBtn.Size = new System.Drawing.Size(75, 23);
            this.SavePathBtn.TabIndex = 8;
            this.SavePathBtn.Text = "Change";
            this.SavePathBtn.UseVisualStyleBackColor = true;
            this.SavePathBtn.Click += new System.EventHandler(this.SavePathBtn_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 408);
            this.Controls.Add(this.SavePathBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SaveTBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.PicBoxAdmin);
            this.Name = "Form2";
            this.Text = "Properties";
            ((System.ComponentModel.ISupportInitialize)(this.PicBoxAdmin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PicBoxAdmin;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox SaveTBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SavePathBtn;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}