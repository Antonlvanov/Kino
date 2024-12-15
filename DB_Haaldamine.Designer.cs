using System.Windows.Forms;

namespace Kino
{
    partial class DB_Haaldamine
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
            this.tableListBox = new System.Windows.Forms.ListBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.uuenda_btn = new System.Windows.Forms.Button();
            this.kustuta_btn = new System.Windows.Forms.Button();
            this.lisa_btn = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.filter_btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableListBox
            // 
            this.tableListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.tableListBox.FormattingEnabled = true;
            this.tableListBox.ItemHeight = 25;
            this.tableListBox.Location = new System.Drawing.Point(12, 23);
            this.tableListBox.Name = "tableListBox";
            this.tableListBox.Size = new System.Drawing.Size(199, 279);
            this.tableListBox.TabIndex = 0;
            this.tableListBox.DoubleClick += new System.EventHandler(this.vali_btn_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 318);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(819, 203);
            this.dataGridView1.TabIndex = 20;
            this.dataGridView1.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_RowHeaderMouseClick);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(217, 21);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(381, 251);
            this.flowLayoutPanel1.TabIndex = 21;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // uuenda_btn
            // 
            this.uuenda_btn.AutoSize = true;
            this.uuenda_btn.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uuenda_btn.Location = new System.Drawing.Point(427, 273);
            this.uuenda_btn.Name = "uuenda_btn";
            this.uuenda_btn.Size = new System.Drawing.Size(80, 29);
            this.uuenda_btn.TabIndex = 25;
            this.uuenda_btn.Text = "Uuenda";
            this.uuenda_btn.UseVisualStyleBackColor = true;
            // 
            // kustuta_btn
            // 
            this.kustuta_btn.AutoSize = true;
            this.kustuta_btn.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kustuta_btn.Location = new System.Drawing.Point(341, 273);
            this.kustuta_btn.Name = "kustuta_btn";
            this.kustuta_btn.Size = new System.Drawing.Size(80, 29);
            this.kustuta_btn.TabIndex = 24;
            this.kustuta_btn.Text = "Kustuta";
            this.kustuta_btn.UseVisualStyleBackColor = true;
            // 
            // lisa_btn
            // 
            this.lisa_btn.AutoSize = true;
            this.lisa_btn.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lisa_btn.Location = new System.Drawing.Point(220, 273);
            this.lisa_btn.Name = "lisa_btn";
            this.lisa_btn.Size = new System.Drawing.Size(115, 29);
            this.lisa_btn.TabIndex = 23;
            this.lisa_btn.Text = "Lisa andmed";
            this.lisa_btn.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(604, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(227, 251);
            this.pictureBox1.TabIndex = 27;
            this.pictureBox1.TabStop = false;
            // 
            // filter_btn
            // 
            this.filter_btn.AutoSize = true;
            this.filter_btn.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filter_btn.Location = new System.Drawing.Point(513, 273);
            this.filter_btn.Name = "filter_btn";
            this.filter_btn.Size = new System.Drawing.Size(69, 29);
            this.filter_btn.TabIndex = 28;
            this.filter_btn.Text = "Filter";
            this.filter_btn.UseVisualStyleBackColor = true;
            this.filter_btn.Click += new System.EventHandler(this.filter_btn_Click);
            // 
            // DB_Haaldamine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 533);
            this.Controls.Add(this.filter_btn);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.uuenda_btn);
            this.Controls.Add(this.kustuta_btn);
            this.Controls.Add(this.lisa_btn);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.tableListBox);
            this.Name = "DB_Haaldamine";
            this.Text = "Andmebaasi haaldamine";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox tableListBox;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button uuenda_btn;
        private System.Windows.Forms.Button kustuta_btn;
        private System.Windows.Forms.Button lisa_btn;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Button filter_btn;
    }
}