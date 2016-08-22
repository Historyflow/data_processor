namespace DBproc
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
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
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tbOut = new System.Windows.Forms.TextBox();
            this.tbSize = new System.Windows.Forms.TextBox();
            this.dgw = new System.Windows.Forms.DataGridView();
            this.page_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.page_title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.page_is_redirect = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.page_len = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.page_content_model = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.page_lang = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.page_latest = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rev_text_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.old_text = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.page_links = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgw)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(27, 431);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(171, 431);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tbOut
            // 
            this.tbOut.Location = new System.Drawing.Point(12, 231);
            this.tbOut.Multiline = true;
            this.tbOut.Name = "tbOut";
            this.tbOut.Size = new System.Drawing.Size(945, 154);
            this.tbOut.TabIndex = 2;
            // 
            // tbSize
            // 
            this.tbSize.Location = new System.Drawing.Point(12, 391);
            this.tbSize.Name = "tbSize";
            this.tbSize.Size = new System.Drawing.Size(100, 20);
            this.tbSize.TabIndex = 3;
            this.tbSize.Text = "20";
            // 
            // dgw
            // 
            this.dgw.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgw.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.page_id,
            this.page_title,
            this.page_is_redirect,
            this.page_len,
            this.page_content_model,
            this.page_lang,
            this.page_latest,
            this.rev_text_id,
            this.old_text,
            this.page_links});
            this.dgw.Location = new System.Drawing.Point(12, 22);
            this.dgw.Name = "dgw";
            this.dgw.Size = new System.Drawing.Size(945, 203);
            this.dgw.TabIndex = 4;
            this.dgw.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // page_id
            // 
            this.page_id.FillWeight = 30F;
            this.page_id.HeaderText = "page_id";
            this.page_id.Name = "page_id";
            this.page_id.Width = 30;
            // 
            // page_title
            // 
            this.page_title.FillWeight = 200F;
            this.page_title.HeaderText = "page_title";
            this.page_title.Name = "page_title";
            this.page_title.Width = 200;
            // 
            // page_is_redirect
            // 
            this.page_is_redirect.HeaderText = "page_is_redirect";
            this.page_is_redirect.Name = "page_is_redirect";
            // 
            // page_len
            // 
            this.page_len.FillWeight = 30F;
            this.page_len.HeaderText = "page_len";
            this.page_len.Name = "page_len";
            this.page_len.Width = 30;
            // 
            // page_content_model
            // 
            this.page_content_model.FillWeight = 30F;
            this.page_content_model.HeaderText = "page_content_model";
            this.page_content_model.Name = "page_content_model";
            this.page_content_model.Width = 30;
            // 
            // page_lang
            // 
            this.page_lang.FillWeight = 30F;
            this.page_lang.HeaderText = "page_lang";
            this.page_lang.Name = "page_lang";
            this.page_lang.Width = 30;
            // 
            // page_latest
            // 
            this.page_latest.FillWeight = 30F;
            this.page_latest.HeaderText = "page_latest";
            this.page_latest.Name = "page_latest";
            this.page_latest.Width = 30;
            // 
            // rev_text_id
            // 
            this.rev_text_id.FillWeight = 30F;
            this.rev_text_id.HeaderText = "rev_text_id";
            this.rev_text_id.Name = "rev_text_id";
            this.rev_text_id.Width = 30;
            // 
            // old_text
            // 
            this.old_text.FillWeight = 400F;
            this.old_text.HeaderText = "old_text";
            this.old_text.Name = "old_text";
            this.old_text.Width = 400;
            // 
            // page_links
            // 
            this.page_links.HeaderText = "page_links";
            this.page_links.Name = "page_links";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(994, 527);
            this.Controls.Add(this.dgw);
            this.Controls.Add(this.tbSize);
            this.Controls.Add(this.tbOut);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dgw)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox tbOut;
        private System.Windows.Forms.TextBox tbSize;
        private System.Windows.Forms.DataGridView dgw;
        private System.Windows.Forms.DataGridViewTextBoxColumn page_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn page_title;
        private System.Windows.Forms.DataGridViewTextBoxColumn page_is_redirect;
        private System.Windows.Forms.DataGridViewTextBoxColumn page_len;
        private System.Windows.Forms.DataGridViewTextBoxColumn page_content_model;
        private System.Windows.Forms.DataGridViewTextBoxColumn page_lang;
        private System.Windows.Forms.DataGridViewTextBoxColumn page_latest;
        private System.Windows.Forms.DataGridViewTextBoxColumn rev_text_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn old_text;
        private System.Windows.Forms.DataGridViewTextBoxColumn page_links;
    }
}

