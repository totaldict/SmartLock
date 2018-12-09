namespace UISmartLock
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.PicBox = new System.Windows.Forms.PictureBox();
            this.btnChkKey = new System.Windows.Forms.Button();
            this.btnClr = new System.Windows.Forms.Button();
            this.rsltBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PicBox)).BeginInit();
            this.SuspendLayout();
            // 
            // PicBox
            // 
            this.PicBox.BackColor = System.Drawing.Color.White;
            this.PicBox.Location = new System.Drawing.Point(0, 0);
            this.PicBox.Name = "PicBox";
            this.PicBox.Size = new System.Drawing.Size(300, 250);
            this.PicBox.TabIndex = 0;
            this.PicBox.TabStop = false;
            this.PicBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PicBox_MouseDown);
            this.PicBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.PicBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PicBox_MouseUp);
            // 
            // btnChkKey
            // 
            this.btnChkKey.Location = new System.Drawing.Point(144, 254);
            this.btnChkKey.Name = "btnChkKey";
            this.btnChkKey.Size = new System.Drawing.Size(75, 23);
            this.btnChkKey.TabIndex = 1;
            this.btnChkKey.Text = "Check Key";
            this.btnChkKey.UseVisualStyleBackColor = true;
            this.btnChkKey.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnClr
            // 
            this.btnClr.Location = new System.Drawing.Point(225, 254);
            this.btnClr.Name = "btnClr";
            this.btnClr.Size = new System.Drawing.Size(75, 23);
            this.btnClr.TabIndex = 2;
            this.btnClr.Text = "Clear";
            this.btnClr.UseVisualStyleBackColor = true;
            this.btnClr.Click += new System.EventHandler(this.btnClr_Click);
            // 
            // rsltBox
            // 
            this.rsltBox.Location = new System.Drawing.Point(384, 0);
            this.rsltBox.Multiline = true;
            this.rsltBox.Name = "rsltBox";
            this.rsltBox.Size = new System.Drawing.Size(283, 254);
            this.rsltBox.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 256);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 46);
            this.button1.TabIndex = 4;
            this.button1.Text = "Save Fixed Key";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 314);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.rsltBox);
            this.Controls.Add(this.btnClr);
            this.Controls.Add(this.btnChkKey);
            this.Controls.Add(this.PicBox);
            this.Name = "Form1";
            this.Text = "SmartLock";
            ((System.ComponentModel.ISupportInitialize)(this.PicBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PicBox;
        private System.Windows.Forms.Button btnChkKey;
        private System.Windows.Forms.Button btnClr;
        private System.Windows.Forms.TextBox rsltBox;
        private System.Windows.Forms.Button button1;
    }
}

