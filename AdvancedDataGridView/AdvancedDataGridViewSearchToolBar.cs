#region License
// Advanced DataGridView
//
// Copyright (c), 2014 Davide Gironi <davide.gironi@gmail.com>
// Original work Copyright (c), 2013 Zuby <zuby@me.com>
//
// Please refer to LICENSE file for licensing information.
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Zuby.ADGV
{

    [System.ComponentModel.DesignerCategory("")]
    public partial class AdvancedDataGridViewSearchToolBar : ToolStrip
    {

        #region public events

        public event AdvancedDataGridViewSearchToolBarSearchEventHandler Search;

        #endregion


        #region class properties

        private DataGridViewColumnCollection _columnsList = null;

        private const bool ButtonCloseEnabled = false;

        #endregion


        #region translations

        /// <summary>
        /// Available translation keys
        /// </summary>
        public enum TranslationKey
        {
            ADGVSTBLabelSearch,
            ADGVSTBButtonFromBegin,
            ADGVSTBButtonCaseSensitiveToolTip,
            ADGVSTBButtonSearchToolTip,
            ADGVSTBButtonCloseToolTip,
            ADGVSTBButtonWholeWordToolTip,
            ADGVSTBComboBoxColumnsAll,
            ADGVSTBTextBoxSearchToolTip
        }

        /// <summary>
        /// Internationalization strings
        /// </summary>
        public static Dictionary<string, string> Translations = new Dictionary<string, string>()
        {
            { TranslationKey.ADGVSTBLabelSearch.ToString(), "Search:" },
            { TranslationKey.ADGVSTBButtonFromBegin.ToString(), "From Begin" },
            { TranslationKey.ADGVSTBButtonCaseSensitiveToolTip.ToString(), "Case Sensitivity" },
            { TranslationKey.ADGVSTBButtonSearchToolTip.ToString(), "Find Next" },
            { TranslationKey.ADGVSTBButtonCloseToolTip.ToString(), "Hide" },
            { TranslationKey.ADGVSTBButtonWholeWordToolTip.ToString(), "Search only Whole Word" },
            { TranslationKey.ADGVSTBComboBoxColumnsAll.ToString(), "(All Columns)" },
            { TranslationKey.ADGVSTBTextBoxSearchToolTip.ToString(), "Value for Search" }
        };

        /// <summary>
        /// Used to check if components translations has to be updated
        /// </summary>
        private Dictionary<string, string> _translationsRefreshComponentTranslationsCheck = new Dictionary<string, string>() { };

        #endregion


        #region constructor

        /// <summary>
        /// AdvancedDataGridViewSearchToolBar constructor
        /// </summary>
        public AdvancedDataGridViewSearchToolBar()
        {
            //initialize components
            InitializeComponent();

            RefreshComponentTranslations();

            //set default values
            if (!ButtonCloseEnabled)
                this.Items.RemoveAt(0);
            comboBox_columns.SelectedIndex = 0;
        }

        #endregion


        #region translations methods

        /// <summary>
        /// Set translation dictionary
        /// </summary>
        /// <param name="translations"></param>
        public static void SetTranslations(IDictionary<string, string> translations)
        {
            //set localization strings
            if (translations != null)
            {
                foreach (KeyValuePair<string, string> translation in translations)
                {
                    if (Translations.ContainsKey(translation.Key))
                        Translations[translation.Key] = translation.Value;
                }
            }
        }

        /// <summary>
        /// Get translation dictionary
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, string> GetTranslations()
        {
            return Translations;
        }

        /// <summary>
        /// Load translations from file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static IDictionary<string, string> LoadTranslationsFromFile(string filename)
        {
            IDictionary<string, string> ret = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(filename))
            {
                //deserialize the file
                try
                {
                    string jsontext = File.ReadAllText(filename);
                    Dictionary<string, string> translations = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(jsontext);
                    foreach (KeyValuePair<string, string> translation in translations)
                    {
                        if (!ret.ContainsKey(translation.Key) && Translations.ContainsKey(translation.Key))
                            ret.Add(translation.Key, translation.Value);
                    }
                }
                catch { }
            }

            //add default translations if not in files
            foreach (KeyValuePair<string, string> translation in GetTranslations())
            {
                if (!ret.ContainsKey(translation.Key))
                    ret.Add(translation.Key, translation.Value);
            }

            return ret;
        }

        /// <summary>
        /// Update components translations
        /// </summary>
        private void RefreshComponentTranslations()
        {
            this.comboBox_columns.BeginUpdate();
            this.comboBox_columns.Items.Clear();
            this.comboBox_columns.Items.AddRange(new object[] { Translations[TranslationKey.ADGVSTBComboBoxColumnsAll.ToString()] });
            if (_columnsList != null)
                foreach (DataGridViewColumn c in _columnsList)
                    if (c.Visible)
                        this.comboBox_columns.Items.Add(c.HeaderText);
            this.comboBox_columns.SelectedIndex = 0;
            this.comboBox_columns.EndUpdate();
            this.button_close.ToolTipText = Translations[TranslationKey.ADGVSTBButtonCloseToolTip.ToString()];
            this.label_search.Text = Translations[TranslationKey.ADGVSTBLabelSearch.ToString()];
            this.textBox_search.ToolTipText = Translations[TranslationKey.ADGVSTBTextBoxSearchToolTip.ToString()];
            this.button_frombegin.ToolTipText = Translations[TranslationKey.ADGVSTBButtonFromBegin.ToString()];
            this.button_casesensitive.ToolTipText = Translations[TranslationKey.ADGVSTBButtonCaseSensitiveToolTip.ToString()];
            this.button_search.ToolTipText = Translations[TranslationKey.ADGVSTBButtonSearchToolTip.ToString()];
            this.button_wholeword.ToolTipText = Translations[TranslationKey.ADGVSTBButtonWholeWordToolTip.ToString()];
            this.textBox_search.Text = textBox_search.ToolTipText;
        }

        #endregion


        #region button events

        /// <summary>
        /// button Search Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void button_search_Click(object sender, EventArgs e)
        {
            if (textBox_search.TextLength > 0 && textBox_search.Text != textBox_search.ToolTipText && Search != null)
            {
                DataGridViewColumn c = null;
                if (comboBox_columns.SelectedIndex > 0 && _columnsList != null && _columnsList.GetColumnCount(DataGridViewElementStates.Visible) > 0)
                {
                    DataGridViewColumn[] cols = _columnsList.Cast<DataGridViewColumn>().Where(col => col.Visible).ToArray<DataGridViewColumn>();

                    if (cols.Length == comboBox_columns.Items.Count - 1)
                    {
                        if (cols[comboBox_columns.SelectedIndex - 1].HeaderText == comboBox_columns.SelectedItem.ToString())
                            c = cols[comboBox_columns.SelectedIndex - 1];
                    }
                }

                AdvancedDataGridViewSearchToolBarSearchEventArgs args = new AdvancedDataGridViewSearchToolBarSearchEventArgs(
                    textBox_search.Text,
                    c,
                    button_casesensitive.Checked,
                    button_wholeword.Checked,
                    button_frombegin.Checked
                );
                Search(this, args);
            }
        }

        /// <summary>
        /// button Close Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void button_close_Click(object sender, EventArgs e)
        {
            Hide();
        }

        #endregion


        #region textbox search events

        /// <summary>
        /// textBox Search TextChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBox_search_TextChanged(object sender, EventArgs e)
        {
            button_search.Enabled = textBox_search.TextLength > 0 && textBox_search.Text != textBox_search.ToolTipText;
        }


        /// <summary>
        /// textBox Search Enter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBox_search_Enter(object sender, EventArgs e)
        {
            if (textBox_search.Text == textBox_search.ToolTipText && textBox_search.ForeColor == Color.LightGray)
                textBox_search.Text = "";
            else
                textBox_search.SelectAll();

            textBox_search.ForeColor = SystemColors.WindowText;
        }

        /// <summary>
        /// textBox Search Leave event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBox_search_Leave(object sender, EventArgs e)
        {
            if (textBox_search.Text.Trim() == "")
            {
                textBox_search.Text = textBox_search.ToolTipText;
                textBox_search.ForeColor = Color.LightGray;
            }
        }


        /// <summary>
        /// textBox Search KeyDown event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBox_search_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox_search.TextLength > 0 && textBox_search.Text != textBox_search.ToolTipText && e.KeyData == Keys.Enter)
            {
                button_search_Click(button_search, new EventArgs());
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        #endregion


        #region public methods

        /// <summary>
        /// Set Columns to search in
        /// </summary>
        /// <param name="columns"></param>
        public void SetColumns(DataGridViewColumnCollection columns)
        {
            _columnsList = columns;
            comboBox_columns.BeginUpdate();
            comboBox_columns.Items.Clear();
            comboBox_columns.Items.AddRange(new object[] { Translations[TranslationKey.ADGVSTBComboBoxColumnsAll.ToString()] });
            if (_columnsList != null)
                foreach (DataGridViewColumn c in _columnsList)
                    if (c.Visible)
                        comboBox_columns.Items.Add(c.HeaderText);
            comboBox_columns.SelectedIndex = 0;
            comboBox_columns.EndUpdate();
        }

        #endregion


        #region resize events

        /// <summary>
        /// Resize event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeMe(object sender, EventArgs e)
        {
            SuspendLayout();
            int w1 = 150;
            int w2 = 150;
            int oldW = comboBox_columns.Width + textBox_search.Width;
            foreach (ToolStripItem c in Items)
            {
                c.Overflow = ToolStripItemOverflow.Never;
                c.Visible = true;
            }

            int width = PreferredSize.Width - oldW + w1 + w2;
            if (Width < width)
            {
                label_search.Visible = false;
                GetResizeBoxSize(PreferredSize.Width - oldW + w1 + w2, ref w1, ref w2);
                width = PreferredSize.Width - oldW + w1 + w2;

                if (Width < width)
                {
                    button_casesensitive.Overflow = ToolStripItemOverflow.Always;
                    GetResizeBoxSize(PreferredSize.Width - oldW + w1 + w2, ref w1, ref w2);
                    width = PreferredSize.Width - oldW + w1 + w2;
                }

                if (Width < width)
                {
                    button_wholeword.Overflow = ToolStripItemOverflow.Always;
                    GetResizeBoxSize(PreferredSize.Width - oldW + w1 + w2, ref w1, ref w2);
                    width = PreferredSize.Width - oldW + w1 + w2;
                }

                if (Width < width)
                {
                    button_frombegin.Overflow = ToolStripItemOverflow.Always;
                    separator_search.Visible = false;
                    GetResizeBoxSize(PreferredSize.Width - oldW + w1 + w2, ref w1, ref w2);
                    width = PreferredSize.Width - oldW + w1 + w2;
                }

                if (Width < width)
                {
                    comboBox_columns.Overflow = ToolStripItemOverflow.Always;
                    textBox_search.Overflow = ToolStripItemOverflow.Always;
                    w1 = 150;
                    w2 = Math.Max(Width - PreferredSize.Width - textBox_search.Margin.Left - textBox_search.Margin.Right, 75);
                    textBox_search.Overflow = ToolStripItemOverflow.Never;
                    width = PreferredSize.Width - textBox_search.Width + w2;
                }
                if (Width < width)
                {
                    button_search.Overflow = ToolStripItemOverflow.Always;
                    w2 = Math.Max(Width - PreferredSize.Width + textBox_search.Width, 75);
                    width = PreferredSize.Width - textBox_search.Width + w2;
                }
                if (Width < width)
                {
                    button_close.Overflow = ToolStripItemOverflow.Always;
                    textBox_search.Margin = new Padding(8, 2, 8, 2);
                    w2 = Math.Max(Width - PreferredSize.Width + textBox_search.Width, 75);
                    width = PreferredSize.Width - textBox_search.Width + w2;
                }

                if (Width < width)
                {
                    w2 = Math.Max(Width - PreferredSize.Width + textBox_search.Width, 20);
                    width = PreferredSize.Width - textBox_search.Width + w2;
                }
                if (width > Width)
                {
                    textBox_search.Overflow = ToolStripItemOverflow.Always;
                    textBox_search.Margin = new Padding(0, 2, 8, 2);
                    w2 = 150;
                }
            }
            else
            {
                GetResizeBoxSize(width, ref w1, ref w2);
            }

            if (comboBox_columns.Width != w1)
                comboBox_columns.Width = w1;
            if (textBox_search.Width != w2)
                textBox_search.Width = w2;

            ResumeLayout();
        }

        /// <summary>
        /// Get a Resize Size for a box
        /// </summary>
        /// <param name="width"></param>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        private void GetResizeBoxSize(int width, ref int w1, ref int w2)
        {
            int dif = (int)Math.Round((width - Width) / 2.0, 0, MidpointRounding.AwayFromZero);

            int oldW1 = w1;
            int oldW2 = w2;
            if (Width < width)
            {
                w1 = Math.Max(w1 - dif, 75);
                w2 = Math.Max(w2 - dif, 75);
            }
            else
            {
                w1 = Math.Min(w1 - dif, 150);
                w2 += Width - width + oldW1 - w1;
            }
        }

        #endregion


        #region paint events

        /// <summary>
        /// On Paint event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //check if translations are changed and update components
            if (!((_translationsRefreshComponentTranslationsCheck == Translations) || (_translationsRefreshComponentTranslationsCheck.Count == Translations.Count && !_translationsRefreshComponentTranslationsCheck.Except(Translations).Any())))
            {
                _translationsRefreshComponentTranslationsCheck = Translations;
                RefreshComponentTranslations();
            }

            base.OnPaint(e);
        }

        #endregion

    }

    public delegate void AdvancedDataGridViewSearchToolBarSearchEventHandler(object sender, AdvancedDataGridViewSearchToolBarSearchEventArgs e);
    public class AdvancedDataGridViewSearchToolBarSearchEventArgs : EventArgs
    {
        public string ValueToSearch { get; private set; }
        public DataGridViewColumn ColumnToSearch { get; private set; }
        public bool CaseSensitive { get; private set; }
        public bool WholeWord { get; private set; }
        public bool FromBegin { get; private set; }

        public AdvancedDataGridViewSearchToolBarSearchEventArgs(string Value, DataGridViewColumn Column, bool Case, bool Whole, bool fromBegin)
        {
            ValueToSearch = Value;
            ColumnToSearch = Column;
            CaseSensitive = Case;
            WholeWord = Whole;
            FromBegin = fromBegin;
        }
    }
}
