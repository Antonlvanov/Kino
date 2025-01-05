using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Kino.Forms.Admin
{
    partial class AdminForm : Form
    {
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustMainElements();

            AdjustControlPanel();

            AdjustColumnControlFont();
            AdjustColumnControlsSize();
            AdjustInputFieldsWidth();
            AdjustLabelWidth();
            Console.WriteLine($"Размер окна: {this.Size.Width}x{this.Size.Height}");
            //Console.WriteLine($"Размер ColumnsControlPanel: {ColumnsControlPanel.Size.Width}x{ColumnsControlPanel.Size.Height}");
            //Console.WriteLine($"Размер ControlPanel: {ControlPanel.Size.Width}x{ControlPanel.Size.Height}");
        }

        public void AdjustLabelWidth()
        {
            // measure text
            int maxWidth = 0;
            foreach (FlowLayoutPanel columnPanel in ColumnsControlPanel.Controls)
            {
                foreach (Label lb in columnPanel.Controls.OfType<Label>())
                {
                    int labelWidth = TextRenderer.MeasureText(lb.Text, lb.Font).Width;
                    if (labelWidth > maxWidth)
                    {
                        maxWidth = labelWidth;
                    }
                }
            }
            // set every label width based on widest label width
            foreach (FlowLayoutPanel columnPanel in ColumnsControlPanel.Controls)
            {
                foreach (Label lb in columnPanel.Controls.OfType<Label>())
                {
                    lb.Width = maxWidth;
                }
            }
        }

        public void AdjustColumnControlFont()
        {
            foreach (Control ColumnPanel in ColumnsControlPanel.Controls)
            {
                foreach (Control control in ColumnPanel.Controls)
                {
                    // half height
                    control.Height = ColumnPanel.Height/2;
                    // padding based on heigh
                    int verticalMargin = (ColumnPanel.Height - control.Height) / 2;
                    control.Margin = new Padding(0, verticalMargin, 0, 0);
                    //font size
                    float desiredFontSize = ColumnPanel.Height / 4f;
                    if (desiredFontSize > 18f) { desiredFontSize = 18f; }
                    control.Font = new Font(control.Font.FontFamily, desiredFontSize);
                }
            }
        }

        public void AdjustInputFieldsWidth()
        {
            int inputMinWidth = 0;
            int inputMaxWidth = 0;
            foreach (Control columnPanel in ColumnsControlPanel.Controls)
            {
                foreach (Control control in columnPanel.Controls)
                {
                    if (control is Label) { 
                        inputMaxWidth = columnPanel.Width - control.Width - columnPanel.Width/5;
                        inputMinWidth = (columnPanel.Width - control.Width);
                    }
                    if (control is DateTimePicker)
                    {
                        int contentWidth = TextRenderer.MeasureText(control.Text, control.Font).Width;
                        int width = contentWidth + 20;
                        if (contentWidth != 0)
                        {
                            control.Width = width;
                        }
                    }
                    if (control is ComboBox comboBox)
                    {
                        int maxWidth = 0;
                        using (Graphics g = comboBox.CreateGraphics())
                        {
                            foreach (var item in comboBox.Items)
                            {
                                string text = comboBox.GetItemText(item);
                                int itemWidth = TextRenderer.MeasureText(g, text, comboBox.Font).Width;
                                maxWidth = Math.Max(maxWidth, itemWidth);
                            }
                        }
                        int width = maxWidth + SystemInformation.VerticalScrollBarWidth;
                        comboBox.Width = Math.Max(comboBox.Width, width);
                    }
                    if (control is TextBox)
                    {
                        control.Width = inputMaxWidth;
                    }
                }
            }
        }

        public void AdjustControlPanel()
        {
            ColumnsControlPanel.Height = ControlPanel.Height-ButtonPanel.Height;
            ColumnsControlPanel.Width = ControlPanel.Width;
        }
        public void AdjustColumnControlsSize()
        {
            foreach (Control ColumnPanel in ColumnsControlPanel.Controls)
            {
                ColumnPanel.Width = ColumnsControlPanel.Width;
                ColumnPanel.Height = (ColumnsControlPanel.Height - ButtonPanel.Height) / ColumnsControlPanel.Controls.Count;
            }
        }
        public void AdjustMainElements()
        {
            DataGridView.Height = this.ClientSize.Height / 3;
            AdjustTablesListSize();
        }
        public void AdjustTablesListSize()
        {
            if (TableList.Items.Count > 0)
            {
                int maxWidth = TableList.Items.Cast<ListViewItem>()
                                  .Max(item => TextRenderer.MeasureText(item.Text, TableList.Font).Width);
                MainPanel.ColumnStyles[0].Width = (float)maxWidth / MainPanel.Width * 100;
            }
        }
    }
}
