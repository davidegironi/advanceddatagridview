
namespace Zuby.ADGV
{
    partial class AdvancedDataGridViewSearchToolBar
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
            this.button_close = new System.Windows.Forms.ToolStripButton();
            this.label_search = new System.Windows.Forms.ToolStripLabel();
            this.comboBox_columns = new System.Windows.Forms.ToolStripComboBox();
            this.textBox_search = new System.Windows.Forms.ToolStripTextBox();
            this.button_frombegin = new System.Windows.Forms.ToolStripButton();
            this.button_casesensitive = new System.Windows.Forms.ToolStripButton();
            this.button_search = new System.Windows.Forms.ToolStripButton();
            this.button_wholeword = new System.Windows.Forms.ToolStripButton();
            this.separator_search = new System.Windows.Forms.ToolStripSeparator();
            this.SuspendLayout();
            // 
            // button_close
            // 
            this.button_close.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button_close.Image = global::Zuby.Properties.Resources.SearchToolBar_ButtonCaseSensitive;
            this.button_close.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.button_close.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button_close.Name = "button_close";
            this.button_close.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.button_close.Size = new System.Drawing.Size(23, 24);
            this.button_close.Click += new System.EventHandler(this.button_close_Click);
            // 
            // label_search
            // 
            this.label_search.Name = "label_search";
            this.label_search.Size = new System.Drawing.Size(45, 15);

            // 
            // comboBox_columns
            // 
            this.comboBox_columns.AutoSize = false;
            this.comboBox_columns.AutoToolTip = true;
            this.comboBox_columns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_columns.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.comboBox_columns.IntegralHeight = false;
            this.comboBox_columns.Margin = new System.Windows.Forms.Padding(0, 2, 8, 2);
            this.comboBox_columns.MaxDropDownItems = 12;
            this.comboBox_columns.Name = "comboBox_columns";
            this.comboBox_columns.Size = new System.Drawing.Size(150, 23);
            // 
            // textBox_search
            // 
            this.textBox_search.AutoSize = false;
            this.textBox_search.ForeColor = System.Drawing.Color.LightGray;
            this.textBox_search.Margin = new System.Windows.Forms.Padding(0, 2, 8, 2);
            this.textBox_search.Name = "textBox_search";
            this.textBox_search.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.textBox_search.Size = new System.Drawing.Size(100, 23);
            this.textBox_search.Enter += new System.EventHandler(this.textBox_search_Enter);
            this.textBox_search.Leave += new System.EventHandler(this.textBox_search_Leave);
            this.textBox_search.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_search_KeyDown);
            this.textBox_search.TextChanged += new System.EventHandler(this.textBox_search_TextChanged);
            // 
            // button_frombegin
            // 
            this.button_frombegin.CheckOnClick = true;
            this.button_frombegin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button_frombegin.Image = global::Zuby.Properties.Resources.SearchToolBar_ButtonFromBegin;
            this.button_frombegin.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.button_frombegin.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button_frombegin.Name = "button_frombegin";
            this.button_frombegin.Size = new System.Drawing.Size(23, 20);
            // 
            // button_casesensitive
            // 
            this.button_casesensitive.CheckOnClick = true;
            this.button_casesensitive.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button_casesensitive.Image = global::Zuby.Properties.Resources.SearchToolBar_ButtonCaseSensitive;
            this.button_casesensitive.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.button_casesensitive.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button_casesensitive.Name = "button_casesensitive";
            this.button_casesensitive.Size = new System.Drawing.Size(23, 20);
            // 
            // button_search
            // 
            this.button_search.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button_search.Image = global::Zuby.Properties.Resources.SearchToolBar_ButtonSearch;
            this.button_search.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.button_search.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button_search.Name = "button_search";
            this.button_search.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.button_search.Size = new System.Drawing.Size(23, 24);
            this.button_search.Click += new System.EventHandler(this.button_search_Click);
            // 
            // button_wholeword
            // 
            this.button_wholeword.CheckOnClick = true;
            this.button_wholeword.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button_wholeword.Image = global::Zuby.Properties.Resources.SearchToolBar_ButtonWholeWord;
            this.button_wholeword.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.button_wholeword.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button_wholeword.Margin = new System.Windows.Forms.Padding(1, 1, 1, 2);
            this.button_wholeword.Name = "button_wholeword";
            this.button_wholeword.Size = new System.Drawing.Size(23, 20);
            // 
            // separator_search
            // 
            this.separator_search.AutoSize = false;
            this.separator_search.Name = "separator_search";
            this.separator_search.Size = new System.Drawing.Size(10, 25);
            // 
            // AdvancedDataGridViewSearchToolBar
            // 
            this.AllowMerge = false;
            this.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.button_close,
            this.label_search,
            this.comboBox_columns,
            this.textBox_search,
            this.button_frombegin,
            this.button_wholeword,
            this.button_casesensitive,
            this.separator_search,
            this.button_search});
            this.MaximumSize = new System.Drawing.Size(0, 27);
            this.MinimumSize = new System.Drawing.Size(0, 27);
            this.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.Size = new System.Drawing.Size(0, 27);
            this.Resize += new System.EventHandler(this.ResizeMe);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripButton button_close;
        private System.Windows.Forms.ToolStripLabel label_search;
        private System.Windows.Forms.ToolStripComboBox comboBox_columns;
        private System.Windows.Forms.ToolStripTextBox textBox_search;
        private System.Windows.Forms.ToolStripButton button_frombegin;
        private System.Windows.Forms.ToolStripButton button_casesensitive;
        private System.Windows.Forms.ToolStripButton button_search;
        private System.Windows.Forms.ToolStripButton button_wholeword;
        private System.Windows.Forms.ToolStripSeparator separator_search;
    }
}
