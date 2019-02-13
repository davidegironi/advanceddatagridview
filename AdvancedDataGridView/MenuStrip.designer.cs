
namespace Zuby.ADGV
{
    partial class MenuStrip
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
            this.components = new System.ComponentModel.Container();
            this.sortASCMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortDESCMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelSortMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1MenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.cancelFilterMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customFilterLastFiltersListMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customFilterMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2MenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.customFilterLastFilter1MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customFilterLastFilter2MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customFilterLastFilter3MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customFilterLastFilter4MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customFilterLastFilter5MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3MenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.checkList = new System.Windows.Forms.TreeView();
            this.button_filter = new System.Windows.Forms.Button();
            this.button_undofilter = new System.Windows.Forms.Button();
            this.checkFilterListPanel = new System.Windows.Forms.Panel();
            this.checkFilterListButtonsPanel = new System.Windows.Forms.Panel();
            this.checkFilterListButtonsControlHost = new System.Windows.Forms.ToolStripControlHost(checkFilterListButtonsPanel);
            this.checkFilterListControlHost = new System.Windows.Forms.ToolStripControlHost(checkFilterListPanel);
            this.checkTextFilter = new System.Windows.Forms.TextBox();
            this.checkTextFilterControlHost = new System.Windows.Forms.ToolStripControlHost(checkTextFilter);
            this.resizeBoxControlHost = new System.Windows.Forms.ToolStripControlHost(new System.Windows.Forms.Control());
            this.SuspendLayout();
            //
            // MenuStrip
            //
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.AutoSize = false;
            this.Padding = new System.Windows.Forms.Padding(0);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Size = new System.Drawing.Size(287, 370);
            this.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(MenuStrip_Closed);
            this.LostFocus += new System.EventHandler(MenuStrip_LostFocus);
            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            sortASCMenuItem,
            sortDESCMenuItem,
            cancelSortMenuItem,
            toolStripSeparator1MenuItem,
            cancelFilterMenuItem,
            customFilterLastFiltersListMenuItem,
            toolStripSeparator3MenuItem,
            checkTextFilterControlHost,
            checkFilterListControlHost,
            checkFilterListButtonsControlHost,
            resizeBoxControlHost});
            //
            // sortASCMenuItem
            //
            this.sortASCMenuItem.Name = "sortASCMenuItem";
            this.sortASCMenuItem.AutoSize = false;
            this.sortASCMenuItem.Size = new System.Drawing.Size(Width - 1, 22);
            this.sortASCMenuItem.Click += new System.EventHandler(SortASCMenuItem_Click);
            this.sortASCMenuItem.MouseEnter += new System.EventHandler(SortASCMenuItem_MouseEnter);
            this.sortASCMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            //
            // sortDESCMenuItem
            //
            this.sortDESCMenuItem.Name = "sortDESCMenuItem";
            this.sortDESCMenuItem.AutoSize = false;
            this.sortDESCMenuItem.Size = new System.Drawing.Size(Width - 1, 22);
            this.sortDESCMenuItem.Click += new System.EventHandler(SortDESCMenuItem_Click);
            this.sortDESCMenuItem.MouseEnter += new System.EventHandler(SortDESCMenuItem_MouseEnter);
            this.sortDESCMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            //
            // cancelSortMenuItem
            //
            this.cancelSortMenuItem.Name = "cancelSortMenuItem";
            this.cancelSortMenuItem.Enabled = false;
            this.cancelSortMenuItem.AutoSize = false;
            this.cancelSortMenuItem.Size = new System.Drawing.Size(Width - 1, 22);
            this.cancelSortMenuItem.Text = "Clear Sort";
            this.cancelSortMenuItem.Click += new System.EventHandler(CancelSortMenuItem_Click);
            this.cancelSortMenuItem.MouseEnter += new System.EventHandler(CancelSortMenuItem_MouseEnter);
            //
            // toolStripSeparator1MenuItem
            //
            this.toolStripSeparator1MenuItem.Name = "toolStripSeparator1MenuItem";
            this.toolStripSeparator1MenuItem.Size = new System.Drawing.Size(Width - 4, 6);
            //
            // cancelFilterMenuItem
            //
            this.cancelFilterMenuItem.Name = "cancelFilterMenuItem";
            this.cancelFilterMenuItem.Enabled = false;
            this.cancelFilterMenuItem.AutoSize = false;
            this.cancelFilterMenuItem.Size = new System.Drawing.Size(Width - 1, 22);
            this.cancelFilterMenuItem.Text = "Clear Filter";
            this.cancelFilterMenuItem.Click += new System.EventHandler(CancelFilterMenuItem_Click);
            this.cancelFilterMenuItem.MouseEnter += new System.EventHandler(CancelFilterMenuItem_MouseEnter);
            //
            // toolStripMenuItem2
            //
            this.toolStripSeparator2MenuItem.Name = "toolStripSeparator2MenuItem";
            this.toolStripSeparator2MenuItem.Size = new System.Drawing.Size(149, 6);
            this.toolStripSeparator2MenuItem.Visible = false;
            //
            // customFilterMenuItem
            //
            this.customFilterMenuItem.Name = "customFilterMenuItem";
            this.customFilterMenuItem.Size = new System.Drawing.Size(152, 22);
            this.customFilterMenuItem.Text = "Add a Custom Filter";
            this.customFilterMenuItem.Click += new System.EventHandler(CustomFilterMenuItem_Click);
            //
            // customFilterLastFilter1MenuItem
            //
            this.customFilterLastFilter1MenuItem.Name = "customFilterLastFilter1MenuItem";
            this.customFilterLastFilter1MenuItem.Size = new System.Drawing.Size(152, 22);
            this.customFilterLastFilter1MenuItem.Tag = "0";
            this.customFilterLastFilter1MenuItem.Text = null;
            this.customFilterLastFilter1MenuItem.Visible = false;
            this.customFilterLastFilter1MenuItem.VisibleChanged += new System.EventHandler(CustomFilterLastFilter1MenuItem_VisibleChanged);
            this.customFilterLastFilter1MenuItem.Click += new System.EventHandler(CustomFilterLastFilterMenuItem_Click);
            this.customFilterLastFilter1MenuItem.TextChanged += new System.EventHandler(CustomFilterLastFilterMenuItem_TextChanged);
            //
            // customFilterLastFilter2MenuItem
            //
            this.customFilterLastFilter2MenuItem.Name = "customFilterLastFilter2MenuItem";
            this.customFilterLastFilter2MenuItem.Size = new System.Drawing.Size(152, 22);
            this.customFilterLastFilter2MenuItem.Tag = "1";
            this.customFilterLastFilter2MenuItem.Text = null;
            this.customFilterLastFilter2MenuItem.Visible = false;
            this.customFilterLastFilter2MenuItem.Click += new System.EventHandler(CustomFilterLastFilterMenuItem_Click);
            this.customFilterLastFilter2MenuItem.TextChanged += new System.EventHandler(CustomFilterLastFilterMenuItem_TextChanged);
            //
            // customFilterLastFilter3MenuItem
            //
            this.customFilterLastFilter3MenuItem.Name = "customFilterLastFilter3MenuItem";
            this.customFilterLastFilter3MenuItem.Size = new System.Drawing.Size(152, 22);
            this.customFilterLastFilter3MenuItem.Tag = "2";
            this.customFilterLastFilter3MenuItem.Text = null;
            this.customFilterLastFilter3MenuItem.Visible = false;
            this.customFilterLastFilter3MenuItem.Click += new System.EventHandler(CustomFilterLastFilterMenuItem_Click);
            this.customFilterLastFilter3MenuItem.TextChanged += new System.EventHandler(CustomFilterLastFilterMenuItem_TextChanged);
            //
            // customFilterLastFilter3MenuItem
            //
            this.customFilterLastFilter4MenuItem.Name = "lastfilter4MenuItem";
            this.customFilterLastFilter4MenuItem.Size = new System.Drawing.Size(152, 22);
            this.customFilterLastFilter4MenuItem.Tag = "3";
            this.customFilterLastFilter4MenuItem.Text = null;
            this.customFilterLastFilter4MenuItem.Visible = false;
            this.customFilterLastFilter4MenuItem.Click += new System.EventHandler(CustomFilterLastFilterMenuItem_Click);
            this.customFilterLastFilter4MenuItem.TextChanged += new System.EventHandler(CustomFilterLastFilterMenuItem_TextChanged);
            //
            // customFilterLastFilter5MenuItem
            //
            this.customFilterLastFilter5MenuItem.Name = "customFilterLastFilter5MenuItem";
            this.customFilterLastFilter5MenuItem.Size = new System.Drawing.Size(152, 22);
            this.customFilterLastFilter5MenuItem.Tag = "4";
            this.customFilterLastFilter5MenuItem.Text = null;
            this.customFilterLastFilter5MenuItem.Visible = false;
            this.customFilterLastFilter5MenuItem.Click += new System.EventHandler(CustomFilterLastFilterMenuItem_Click);
            this.customFilterLastFilter5MenuItem.TextChanged += new System.EventHandler(CustomFilterLastFilterMenuItem_TextChanged);
            //
            // customFilterLastFiltersListMenuItem
            //
            this.customFilterLastFiltersListMenuItem.Name = "customFilterLastFiltersListMenuItem";
            this.customFilterLastFiltersListMenuItem.AutoSize = false;
            this.customFilterLastFiltersListMenuItem.Size = new System.Drawing.Size(Width - 1, 22);
            this.customFilterLastFiltersListMenuItem.Image = Properties.Resources.ColumnHeader_Filtered;
            this.customFilterLastFiltersListMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.customFilterLastFiltersListMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            customFilterMenuItem,
            toolStripSeparator2MenuItem,
            customFilterLastFilter1MenuItem,
            customFilterLastFilter2MenuItem,
            customFilterLastFilter3MenuItem,
            customFilterLastFilter4MenuItem,
            customFilterLastFilter5MenuItem});
            this.customFilterLastFiltersListMenuItem.MouseEnter += new System.EventHandler(CustomFilterLastFiltersListMenuItem_MouseEnter);
            this.customFilterLastFiltersListMenuItem.Paint += new System.Windows.Forms.PaintEventHandler(CustomFilterLastFiltersListMenuItem_Paint);
            //
            // toolStripMenuItem3
            //
            this.toolStripSeparator3MenuItem.Name = "toolStripSeparator3MenuItem";
            this.toolStripSeparator3MenuItem.Size = new System.Drawing.Size(Width - 4, 6);
            //
            // button_filter
            //
            this.button_filter.Name = "button_filter";
            this.button_filter.BackColor = System.Windows.Forms.Button.DefaultBackColor;
            this.button_filter.UseVisualStyleBackColor = true;
            this.button_filter.Margin = new System.Windows.Forms.Padding(0);
            this.button_filter.Size = new System.Drawing.Size(75, 23);
            this.button_filter.Text = "Filter";
            this.button_filter.Click += new System.EventHandler(Button_ok_Click);
            this.button_filter.Location = new System.Drawing.Point(this.checkFilterListButtonsPanel.Width - 164, 0);
            //
            // button_undofilter
            //
            this.button_undofilter.Name = "button_undofilter";
            this.button_undofilter.BackColor = System.Windows.Forms.Button.DefaultBackColor;
            this.button_undofilter.UseVisualStyleBackColor = true;
            this.button_undofilter.Margin = new System.Windows.Forms.Padding(0);
            this.button_undofilter.Size = new System.Drawing.Size(75, 23);
            this.button_undofilter.Text = "Cancel";
            this.button_undofilter.Click += new System.EventHandler(Button_cancel_Click);
            this.button_undofilter.Location = new System.Drawing.Point(this.checkFilterListButtonsPanel.Width - 79, 0);
            //
            // resizeBoxControlHost
            //
            this.resizeBoxControlHost.Name = "resizeBoxControlHost";
            this.resizeBoxControlHost.Control.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.resizeBoxControlHost.AutoSize = false;
            this.resizeBoxControlHost.Padding = new System.Windows.Forms.Padding(0);
            this.resizeBoxControlHost.Margin = new System.Windows.Forms.Padding(Width - 45, 0, 0, 0);
            this.resizeBoxControlHost.Size = new System.Drawing.Size(10, 10);
            this.resizeBoxControlHost.Paint += new System.Windows.Forms.PaintEventHandler(ResizeBoxControlHost_Paint);
            this.resizeBoxControlHost.MouseDown += new System.Windows.Forms.MouseEventHandler(ResizeBoxControlHost_MouseDown);
            this.resizeBoxControlHost.MouseUp += new System.Windows.Forms.MouseEventHandler(ResizeBoxControlHost_MouseUp);
            this.resizeBoxControlHost.MouseMove += new System.Windows.Forms.MouseEventHandler(ResizeBoxControlHost_MouseMove);
            //
            // checkFilterListControlHost
            //
            this.checkFilterListControlHost.Name = "checkFilterListControlHost";
            this.checkFilterListControlHost.AutoSize = false;
            this.checkFilterListControlHost.Size = new System.Drawing.Size(Width - 35, 194);
            this.checkFilterListControlHost.Padding = new System.Windows.Forms.Padding(0);
            this.checkFilterListControlHost.Margin = new System.Windows.Forms.Padding(0);
            //
            // checkTextFilterControlHost
            //
            this.checkTextFilterControlHost.Name = "checkTextFilterControlHost";
            this.checkTextFilterControlHost.AutoSize = false;
            this.checkTextFilterControlHost.Size = new System.Drawing.Size(Width - 35, 20);
            this.checkTextFilterControlHost.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.checkTextFilterControlHost.Margin = new System.Windows.Forms.Padding(0);
            //
            // checkFilterListButtonsControlHost
            //
            this.checkFilterListButtonsControlHost.Name = "checkFilterListButtonsControlHost";
            this.checkFilterListButtonsControlHost.AutoSize = false;
            this.checkFilterListButtonsControlHost.Size = new System.Drawing.Size(Width - 35, 24);
            this.checkFilterListButtonsControlHost.Padding = new System.Windows.Forms.Padding(0);
            this.checkFilterListButtonsControlHost.Margin = new System.Windows.Forms.Padding(0);
            //
            // checkFilterListPanel
            //
            this.checkFilterListPanel.Name = "checkFilterListPanel";
            this.checkFilterListPanel.AutoSize = false;
            this.checkFilterListPanel.Size = checkFilterListControlHost.Size;
            this.checkFilterListPanel.Padding = new System.Windows.Forms.Padding(0);
            this.checkFilterListPanel.Margin = new System.Windows.Forms.Padding(0);
            this.checkFilterListPanel.BackColor = BackColor;
            this.checkFilterListPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.checkFilterListPanel.Controls.Add(checkList);
            //
            // checkList
            //
            this.checkList.Name = "checkList";
            this.checkList.AutoSize = false;
            this.checkList.Padding = new System.Windows.Forms.Padding(0);
            this.checkList.Margin = new System.Windows.Forms.Padding(0);
            this.checkList.Bounds = new System.Drawing.Rectangle(4, 4, this.checkFilterListPanel.Width - 8, this.checkFilterListPanel.Height - 8);
            this.checkList.StateImageList = GetCheckListStateImages();
            this.checkList.CheckBoxes = false;
            this.checkList.MouseLeave += new System.EventHandler(CheckList_MouseLeave);
            this.checkList.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(CheckList_NodeMouseClick);
            this.checkList.KeyDown += new System.Windows.Forms.KeyEventHandler(CheckList_KeyDown);
            this.checkList.MouseEnter += CheckList_MouseEnter;
            this.checkList.NodeMouseDoubleClick += CheckList_NodeMouseDoubleClick;
            //
            // checkTextFilter
            //
            this.checkTextFilter.Name = "checkTextFilter";
            this.checkTextFilter.Padding = new System.Windows.Forms.Padding(0);
            this.checkTextFilter.Margin = new System.Windows.Forms.Padding(0);
            this.checkTextFilter.Size = checkTextFilterControlHost.Size;
            this.checkTextFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkTextFilter.TextChanged += new System.EventHandler(CheckTextFilter_TextChanged);
            //
            // checkFilterListButtonsPanel
            //
            this.checkFilterListButtonsPanel.Name = "checkFilterListButtonsPanel";
            this.checkFilterListButtonsPanel.AutoSize = false;
            this.checkFilterListButtonsPanel.Size = checkFilterListButtonsControlHost.Size;
            this.checkFilterListButtonsPanel.Padding = new System.Windows.Forms.Padding(0);
            this.checkFilterListButtonsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.checkFilterListButtonsPanel.BackColor = BackColor;
            this.checkFilterListButtonsPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.checkFilterListButtonsPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
            button_filter,
            button_undofilter
            });
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem sortASCMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortDESCMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelSortMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1MenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2MenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3MenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelFilterMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customFilterLastFiltersListMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customFilterMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customFilterLastFilter1MenuItem;
        private System.Windows.Forms.ToolStripMenuItem customFilterLastFilter2MenuItem;
        private System.Windows.Forms.ToolStripMenuItem customFilterLastFilter3MenuItem;
        private System.Windows.Forms.ToolStripMenuItem customFilterLastFilter4MenuItem;
        private System.Windows.Forms.ToolStripMenuItem customFilterLastFilter5MenuItem;
        private System.Windows.Forms.TreeView checkList;
        private System.Windows.Forms.Button button_filter;
        private System.Windows.Forms.Button button_undofilter;
        private System.Windows.Forms.ToolStripControlHost checkFilterListControlHost;
        private System.Windows.Forms.ToolStripControlHost checkFilterListButtonsControlHost;
        private System.Windows.Forms.ToolStripControlHost resizeBoxControlHost;
        private System.Windows.Forms.Panel checkFilterListPanel;
        private System.Windows.Forms.Panel checkFilterListButtonsPanel;
        private System.Windows.Forms.TextBox checkTextFilter;
        private System.Windows.Forms.ToolStripControlHost checkTextFilterControlHost;
    }
}
