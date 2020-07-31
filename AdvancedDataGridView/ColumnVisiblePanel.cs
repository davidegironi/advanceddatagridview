using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Zuby
{
    /// <summary>
    /// Usage: Called in the CellMouseClick event of the DataGridView
    /// private void dgv1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    /// {
    ///     if (e.RowIndex == -1 && e.Button == MouseButtons.Right)
    ///     {
    ///         dgv1ColPanel.InitColumns(e.ColumnIndex);
    ///         dgv1ColPanel.Location = PointToClient(Cursor.Position);
    ///         dgv1ColPanel.Visible = true;
    ///     }
    ///     else
    ///     {
    ///         dgv1ColPanel.Visible = false;
    ///         dgv1ColPanel.ClearColumns();
    ///     }
    /// }
    /// </summary>
    public partial class ColumnVisiblePanel : UserControl
    {
        public ColumnVisiblePanel()
        {
            InitializeComponent();
        }

        public DataGridView DataGridView { get; set; }

        private void DGVColumnPanel_Load(object sender, EventArgs e)
        {
        }

        public void InitColumns(int curColumnIndex)
        {
            flp1.Controls.Clear();

            if (DataGridView != default(DataGridView))
            {
                //Add hide current column option
                if (curColumnIndex > -1 && DataGridView.Columns[curColumnIndex].HeaderText.Length > 0)
                {
                    CheckBox chkHideCur = new CheckBox();
                    chkHideCur.Text = $"Hide {DataGridView.Columns[curColumnIndex].HeaderText}";
                    chkHideCur.Name = DataGridView.Columns[curColumnIndex].Name;
                    chkHideCur.AutoSize = true;
                    chkHideCur.CheckState = CheckState.Indeterminate;
                    chkHideCur.CheckedChanged += ChkHideCurOrShowAll_CheckedChanged;
                    chkHideCur.Click += ChkHideCur_Click;
                    chkHideCur.MouseEnter += CheckBox_MouseEnter;
                    chkHideCur.MouseLeave += CheckBox_MouseLeave;

                    flp1.Controls.Add(chkHideCur);
                }

                //Add show all option
                CheckBox chkShowAll = new CheckBox();
                chkShowAll.Text = "Show all";
                chkShowAll.Name = "ShowAll";
                chkShowAll.AutoSize = true;
                chkShowAll.CheckState = CheckState.Indeterminate;
                chkShowAll.CheckedChanged += ChkHideCurOrShowAll_CheckedChanged;
                chkShowAll.Click += ChkShowAll_Click;
                chkShowAll.MouseEnter += CheckBox_MouseEnter;
                chkShowAll.MouseLeave += CheckBox_MouseLeave;

                flp1.Controls.Add(chkShowAll);

                //Add a line
                Label lblSplitLine = new Label();
                lblSplitLine.AutoSize = false;
                lblSplitLine.BorderStyle = BorderStyle.FixedSingle;
                lblSplitLine.Text = string.Empty;
                lblSplitLine.Width = 0;  //Reset the Width later
                lblSplitLine.Height = 1;
                flp1.Controls.Add(lblSplitLine);

                //Add all columns option
                //The display order of the columns of the DataGridView is inconsistent with the internal Index (foreach traversal order).
                //Here, the CheckBox is added according to the display order.
                CheckBox[] checkBoxes = new CheckBox[DataGridView.Columns.Count];
                foreach (DataGridViewColumn item in DataGridView.Columns)
                {
                    //If the column HeaderText is empty, ignore.
                    //So if some columns need to be hidden permanently, just set HeaderText to empty.
                    if (item.HeaderText.Length > 0)
                    {
                        CheckBox checkBox = new CheckBox();
                        checkBox.Text = item.HeaderText;
                        checkBox.Name = item.Name;
                        checkBox.AutoSize = true;
                        checkBox.Checked = item.Visible;
                        checkBox.CheckedChanged += CheckBox_CheckedChanged;
                        checkBox.MouseEnter += CheckBox_MouseEnter;
                        checkBox.MouseLeave += CheckBox_MouseLeave;

                        //flp1.Controls.Add(checkBox);
                        checkBoxes[item.DisplayIndex] = checkBox;

                    }
                }
                flp1.Controls.AddRange(checkBoxes);
            }

            CalThisSize();
        }

        private void ChkShowAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn item in DataGridView.Columns)
            {
                if (item.HeaderText.Length > 0)
                {
                    item.Visible = true;
                }
            }

            this.Visible = false;
        }

        private void ChkHideCur_Click(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            DataGridView.Columns[checkBox.Name].Visible = false;

            this.Visible = false;
        }

        private void ChkHideCurOrShowAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            checkBox.CheckState = CheckState.Indeterminate;
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            DataGridView.Columns[checkBox.Name].Visible = checkBox.Checked;
        }

        private void CheckBox_MouseEnter(object sender, EventArgs e)
        {
            ((CheckBox)sender).BackColor = SystemColors.ControlLightLight;
        }

        private void CheckBox_MouseLeave(object sender, EventArgs e)
        {
            ((CheckBox)sender).BackColor = Color.Empty;
        }

        private void CalThisSize()
        {
            int thisWidth = flp1.Margin.Horizontal + flp1.Padding.Horizontal;
            int thisHeight = flp1.Margin.Vertical + flp1.Padding.Vertical;

            //Calculate the Width and Height
            foreach (Control item in flp1.Controls)
            {
                thisWidth = Math.Max(thisWidth, item.Width + item.Margin.Horizontal);
                thisHeight += item.Height + item.Margin.Vertical;
            }

            //Reset the Width of the label.
            //For performance, also can directly set the Width of the control with index 2.
            foreach (Control item in flp1.Controls)
            {
                if (item is Label)
                {
                    item.Width = this.Width - item.Margin.Horizontal;
                }
            }

            this.Width = Math.Max(thisWidth, 120);  //Width not less than 120
            this.Height = thisHeight;
        }

        public void ClearColumns()
        {
            flp1.Controls.Clear();
        }
    }
}
