#region License
// Advanced DataGridView
//
// Copyright (c), 2014 Davide Gironi <davide.gironi@gmail.com>
// Original work Copyright (c), 2013 Zuby <zuby@me.com>
//
// Please refer to LICENSE file for licensing information.
#endregion

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Zuby.ADGV
{
    internal partial class FormCustomFilter : Form
    {

        #region class properties

        private enum FilterType
        {
            Unknown,
            DateTime,
            TimeSpan,
            String,
            Float,
            Integer
        }

        private readonly FilterType _filterType;
        private readonly Control _valControl1;
        private readonly Control _valControl2;

        private readonly bool _filterDateAndTimeEnabled;

        #endregion


        #region constructors

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="filterDateAndTimeEnabled"></param>
        public FormCustomFilter(Type dataType, bool filterDateAndTimeEnabled)
            : base()
        {
            //initialize components
            InitializeComponent();

            //set component translations
            Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVFormTitle.ToString()];
            label_columnName.Text =
                AdvancedDataGridView.Translations[
                    AdvancedDataGridView.TranslationKey.ADGVLabelColumnNameText.ToString()];
            label_and.Text =
                AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVLabelAnd.ToString()];
            button_ok.Text =
                AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVButtonOk.ToString()];
            button_cancel.Text =
                AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVButtonCancel.ToString()];

            if (dataType == typeof(DateTime))
                _filterType = FilterType.DateTime;
            else if (dataType == typeof(TimeSpan))
                _filterType = FilterType.TimeSpan;
            else if (dataType == typeof(int) || dataType == typeof(long) || dataType == typeof(short) ||
                     dataType == typeof(uint) || dataType == typeof(ulong) || dataType == typeof(ushort) ||
                     dataType == typeof(byte) || dataType == typeof(sbyte))
                _filterType = FilterType.Integer;
            else if (dataType == typeof(float) || dataType == typeof(double) || dataType == typeof(decimal))
                _filterType = FilterType.Float;
            else if (dataType == typeof(string))
                _filterType = FilterType.String;
            else
                _filterType = FilterType.Unknown;

            _filterDateAndTimeEnabled = filterDateAndTimeEnabled;

            switch (_filterType)
            {
                case FilterType.DateTime:
                    _valControl1 = new DateTimePicker();
                    _valControl2 = new DateTimePicker();
                    if (_filterDateAndTimeEnabled)
                    {
                        var dt = Thread.CurrentThread.CurrentCulture.DateTimeFormat;

                        (_valControl1 as DateTimePicker).CustomFormat = dt.ShortDatePattern + " " + "HH:mm";
                        (_valControl2 as DateTimePicker).CustomFormat = dt.ShortDatePattern + " " + "HH:mm";
                        (_valControl1 as DateTimePicker).Format = DateTimePickerFormat.Custom;
                        (_valControl2 as DateTimePicker).Format = DateTimePickerFormat.Custom;
                    }
                    else
                    {
                        (_valControl1 as DateTimePicker).Format = DateTimePickerFormat.Short;
                        (_valControl2 as DateTimePicker).Format = DateTimePickerFormat.Short;
                    }

                    comboBox_filterType.Items.AddRange(new string[]
                    {
                        AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVEquals.ToString()],
                        AdvancedDataGridView.Translations[
                            AdvancedDataGridView.TranslationKey.ADGVDoesNotEqual.ToString()],
                        AdvancedDataGridView.Translations[
                            AdvancedDataGridView.TranslationKey.ADGVEarlierThan.ToString()],
                        AdvancedDataGridView.Translations[
                            AdvancedDataGridView.TranslationKey.ADGVEarlierThanOrEqualTo.ToString()],
                        AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVLaterThan.ToString()],
                        AdvancedDataGridView.Translations[
                            AdvancedDataGridView.TranslationKey.ADGVLaterThanOrEqualTo.ToString()],
                        AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVBetween.ToString()]
                    });
                    break;

                case FilterType.TimeSpan:
                    _valControl1 = new TextBox();
                    _valControl2 = new TextBox();
                    comboBox_filterType.Items.AddRange(new string[]
                    {
                        AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVContains.ToString()],
                        AdvancedDataGridView.Translations[
                            AdvancedDataGridView.TranslationKey.ADGVDoesNotContain.ToString()]
                    });
                    break;

                case FilterType.Integer:
                case FilterType.Float:
                    _valControl1 = new TextBox();
                    _valControl2 = new TextBox();
                    _valControl1.TextChanged += valControl_TextChanged;
                    _valControl2.TextChanged += valControl_TextChanged;
                    comboBox_filterType.Items.AddRange(new string[]
                    {
                        AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVEquals.ToString()],
                        AdvancedDataGridView.Translations[
                            AdvancedDataGridView.TranslationKey.ADGVDoesNotEqual.ToString()],
                        AdvancedDataGridView.Translations[
                            AdvancedDataGridView.TranslationKey.ADGVGreaterThan.ToString()],
                        AdvancedDataGridView.Translations[
                            AdvancedDataGridView.TranslationKey.ADGVGreaterThanOrEqualTo.ToString()],
                        AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVLessThan.ToString()],
                        AdvancedDataGridView.Translations[
                            AdvancedDataGridView.TranslationKey.ADGVLessThanOrEqualTo.ToString()],
                        AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVBetween.ToString()]
                    });
                    _valControl1.Tag = true;
                    _valControl2.Tag = true;
                    button_ok.Enabled = false;
                    break;

                default:
                    _valControl1 = new TextBox();
                    _valControl2 = new TextBox();
                    comboBox_filterType.Items.AddRange(new string[]
                    {
                        AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVEquals.ToString()],
                        AdvancedDataGridView.Translations[
                            AdvancedDataGridView.TranslationKey.ADGVDoesNotEqual.ToString()],
                        AdvancedDataGridView.Translations
                            [AdvancedDataGridView.TranslationKey.ADGVBeginsWith.ToString()],
                        AdvancedDataGridView.Translations[
                            AdvancedDataGridView.TranslationKey.ADGVDoesNotBeginWith.ToString()],
                        AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVEndsWith.ToString()],
                        AdvancedDataGridView.Translations[
                            AdvancedDataGridView.TranslationKey.ADGVDoesNotEndWith.ToString()],
                        AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVContains.ToString()],
                        AdvancedDataGridView.Translations[
                            AdvancedDataGridView.TranslationKey.ADGVDoesNotContain.ToString()]
                    });
                    break;
            }

            comboBox_filterType.SelectedIndex = 0;

            _valControl1.Name = "valControl1";
            _valControl1.Location = new System.Drawing.Point(30, 66);
            _valControl1.Size = new System.Drawing.Size(166, 20);
            _valControl1.TabIndex = 4;
            _valControl1.Visible = true;
            _valControl1.KeyDown += valControl_KeyDown;

            _valControl2.Name = "valControl2";
            _valControl2.Location = new System.Drawing.Point(30, 108);
            _valControl2.Size = new System.Drawing.Size(166, 20);
            _valControl2.TabIndex = 5;
            _valControl2.Visible = false;
            _valControl2.VisibleChanged += valControl2_VisibleChanged;
            _valControl2.KeyDown += valControl_KeyDown;

            Controls.Add(_valControl1);
            Controls.Add(_valControl2);

            errorProvider.SetIconAlignment(_valControl1, ErrorIconAlignment.MiddleRight);
            errorProvider.SetIconPadding(_valControl1, -18);
            errorProvider.SetIconAlignment(_valControl2, ErrorIconAlignment.MiddleRight);
            errorProvider.SetIconPadding(_valControl2, -18);
        }

        /// <summary>
        /// Form loaders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormCustomFilter_Load(object sender, EventArgs e)
        {
        }

        #endregion


        #region public filter methods

        /// <summary>
        /// Get the Filter string
        /// </summary>
        public string FilterString { get; private set; }

        /// <summary>
        /// Get the Filter string description
        /// </summary>
        public string FilterStringDescription { get; private set; }

        #endregion


        #region filter builder

        /// <summary>
        /// Build a Filter string
        /// </summary>
        /// <param name="filterType"></param>
        /// <param name="filterDateAndTimeEnabled"></param>
        /// <param name="filterTypeConditionText"></param>
        /// <param name="control1"></param>
        /// <param name="control2"></param>
        /// <returns></returns>
        private string BuildCustomFilter(FilterType filterType, bool filterDateAndTimeEnabled,
            string filterTypeConditionText, Control control1, Control control2)
        {
            var column = "[{0}] ";

            if (filterType == FilterType.Unknown)
                column = "Convert([{0}], 'System.String') ";

            var filterString = column;

            switch (filterType)
            {
                case FilterType.DateTime:
                    var dt = ((DateTimePicker) control1).Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);

                    if (filterTypeConditionText ==
                        AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVEquals.ToString()])
                        filterString = "Convert([{0}], 'System.String') LIKE '%" +
                                       Convert.ToString((filterDateAndTimeEnabled ? dt : dt.Date),
                                           CultureInfo.CurrentCulture) + "%'";
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVEarlierThan.ToString()])
                        filterString += "< '" + Convert.ToString((filterDateAndTimeEnabled ? dt : dt.Date),
                                            CultureInfo.CurrentCulture) + "'";
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVEarlierThanOrEqualTo.ToString()])
                        filterString += "<= '" + Convert.ToString((filterDateAndTimeEnabled ? dt : dt.Date),
                                            CultureInfo.CurrentCulture) + "'";
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVLaterThan.ToString()])
                        filterString += "> '" + Convert.ToString((filterDateAndTimeEnabled ? dt : dt.Date),
                                            CultureInfo.CurrentCulture) + "'";
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVLaterThanOrEqualTo.ToString()])
                        filterString += ">= '" + Convert.ToString((filterDateAndTimeEnabled ? dt : dt.Date),
                                            CultureInfo.CurrentCulture) + "'";
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVBetween.ToString()])
                    {
                        var dt1 = ((DateTimePicker) control2).Value;
                        dt1 = new DateTime(dt1.Year, dt1.Month, dt1.Day, dt1.Hour, dt1.Minute, 0);
                        filterString += ">= '" + Convert.ToString((filterDateAndTimeEnabled ? dt : dt.Date),
                                            CultureInfo.CurrentCulture) + "'";
                        filterString += " AND " + column + "<= '" +
                                        Convert.ToString((filterDateAndTimeEnabled ? dt1 : dt1.Date),
                                            CultureInfo.CurrentCulture) + "'";
                    }
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVDoesNotEqual.ToString()])
                        filterString = "Convert([{0}], 'System.String') NOT LIKE '%" +
                                       Convert.ToString((filterDateAndTimeEnabled ? dt : dt.Date),
                                           CultureInfo.CurrentCulture) + "%'";

                    break;

                case FilterType.TimeSpan:
                    try
                    {
                        var ts = TimeSpan.Parse(control1.Text);

                        if (filterTypeConditionText ==
                            AdvancedDataGridView.Translations[
                                AdvancedDataGridView.TranslationKey.ADGVContains.ToString()])
                        {
                            filterString = "(Convert([{0}], 'System.String') LIKE '%P" +
                                           (ts.Days > 0 ? ts.Days + "D" : "") + (ts.TotalHours > 0 ? "T" : "") +
                                           (ts.Hours > 0 ? ts.Hours + "H" : "") +
                                           (ts.Minutes > 0 ? ts.Minutes + "M" : "") +
                                           (ts.Seconds > 0 ? ts.Seconds + "S" : "") + "%')";
                        }
                        else if (filterTypeConditionText ==
                                 AdvancedDataGridView.Translations[
                                     AdvancedDataGridView.TranslationKey.ADGVDoesNotContain.ToString()])
                        {
                            filterString = "(Convert([{0}], 'System.String') NOT LIKE '%P" +
                                           (ts.Days > 0 ? ts.Days + "D" : "") + (ts.TotalHours > 0 ? "T" : "") +
                                           (ts.Hours > 0 ? ts.Hours + "H" : "") +
                                           (ts.Minutes > 0 ? ts.Minutes + "M" : "") +
                                           (ts.Seconds > 0 ? ts.Seconds + "S" : "") + "%')";
                        }
                    }
                    catch (FormatException)
                    {
                        filterString = null;
                    }

                    break;

                case FilterType.Integer:
                case FilterType.Float:

                    var num = control1.Text;

                    if (filterType == FilterType.Float)
                        num = num.Replace(",", ".");

                    if (filterTypeConditionText ==
                        AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVEquals.ToString()])
                        filterString += "= " + num;
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVBetween.ToString()])
                        filterString += ">= " + num + " AND " + column + "<= " + (filterType == FilterType.Float
                                            ? control2.Text.Replace(",", ".")
                                            : control2.Text);
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVDoesNotEqual.ToString()])
                        filterString += "<> " + num;
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVGreaterThan.ToString()])
                        filterString += "> " + num;
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVGreaterThanOrEqualTo.ToString()])
                        filterString += ">= " + num;
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVLessThan.ToString()])
                        filterString += "< " + num;
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVLessThanOrEqualTo.ToString()])
                        filterString += "<= " + num;
                    break;

                default:
                    var txt = FormatFilterString(control1.Text);
                    if (filterTypeConditionText ==
                        AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVEquals.ToString()])
                        filterString += "LIKE '" + txt + "'";
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVDoesNotEqual.ToString()])
                        filterString += "NOT LIKE '" + txt + "'";
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVBeginsWith.ToString()])
                        filterString += "LIKE '" + txt + "%'";
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVEndsWith.ToString()])
                        filterString += "LIKE '%" + txt + "'";
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVDoesNotBeginWith.ToString()])
                        filterString += "NOT LIKE '" + txt + "%'";
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVDoesNotEndWith.ToString()])
                        filterString += "NOT LIKE '%" + txt + "'";
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVContains.ToString()])
                        filterString += "LIKE '%" + txt + "%'";
                    else if (filterTypeConditionText ==
                             AdvancedDataGridView.Translations[
                                 AdvancedDataGridView.TranslationKey.ADGVDoesNotContain.ToString()])
                        filterString += "NOT LIKE '%" + txt + "%'";
                    break;
            }

            return filterString;
        }

        /// <summary>
        /// Format a text Filter string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string FormatFilterString(string text)
        {
            var result = "";
            string[] replace = {"%", "[", "]", "*", "\"", "\\"};

            foreach (var c in text)
            {
                var s = c.ToString();
                if (replace.Contains(s))
                    result += "[" + s + "]";
                else
                    result += s;
            }

            return result.Replace("'", "''");
        }


        #endregion


        #region buttons events

        /// <summary>
        /// Button Cancel Clieck
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_cancel_Click(object sender, EventArgs e)
        {
            FilterStringDescription = null;
            FilterString = null;
            Close();
        }

        /// <summary>
        /// Button OK Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ok_Click(object sender, EventArgs e)
        {
            if ((_valControl1.Visible && _valControl1.Tag != null && ((bool) _valControl1.Tag)) ||
                (_valControl2.Visible && _valControl2.Tag != null && ((bool) _valControl2.Tag)))
            {
                button_ok.Enabled = false;
                return;
            }

            var filterString = BuildCustomFilter(_filterType, _filterDateAndTimeEnabled, comboBox_filterType.Text,
                _valControl1, _valControl2);

            if (!string.IsNullOrEmpty(filterString))
            {
                FilterString = filterString;
                FilterStringDescription =
                    string.Format(
                        AdvancedDataGridView.Translations[
                            AdvancedDataGridView.TranslationKey.ADGVFilterStringDescription.ToString()],
                        comboBox_filterType.Text, _valControl1.Text);
                if (_valControl2.Visible)
                    FilterStringDescription += " " + label_and.Text + " \"" + _valControl2.Text + "\"";
                DialogResult = DialogResult.OK;
            }
            else
            {
                FilterString = null;
                FilterStringDescription = null;
                DialogResult = DialogResult.Cancel;
            }

            Close();
        }

        #endregion


        #region changed status events

        /// <summary>
        /// Changed condition type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_filterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _valControl2.Visible = comboBox_filterType.Text ==
                                   AdvancedDataGridView.Translations[
                                       AdvancedDataGridView.TranslationKey.ADGVBetween.ToString()];
            button_ok.Enabled = !(_valControl1.Visible && _valControl1.Tag != null && ((bool) _valControl1.Tag)) ||
                                (_valControl2.Visible && _valControl2.Tag != null && ((bool) _valControl2.Tag));
        }

        /// <summary>
        /// Changed control2 visibility
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valControl2_VisibleChanged(object sender, EventArgs e)
        {
            label_and.Visible = _valControl2.Visible;
        }

        /// <summary>
        /// Changed a control Text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valControl_TextChanged(object sender, EventArgs e)
        {
            var hasErrors = false;
            switch (_filterType)
            {
                case FilterType.Integer:
                    hasErrors = !long.TryParse((sender as TextBox).Text, out _);
                    break;

                case FilterType.Float:
                    hasErrors = !double.TryParse((sender as TextBox).Text, out _);
                    break;
            }

            (sender as Control).Tag = hasErrors || (sender as TextBox).Text.Length == 0;

            if (hasErrors && (sender as TextBox).Text.Length > 0)
                errorProvider.SetError((sender as Control),
                    AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVInvalidValue.ToString()]);
            else
                errorProvider.SetError((sender as Control), "");

            button_ok.Enabled = !(_valControl1.Visible && _valControl1.Tag != null && ((bool) _valControl1.Tag)) ||
                                (_valControl2.Visible && _valControl2.Tag != null && ((bool) _valControl2.Tag));
        }

        /// <summary>
        /// KeyDown on a control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (sender == _valControl1)
                {
                    if (_valControl2.Visible)
                        _valControl2.Focus();
                    else
                        button_ok_Click(button_ok, new EventArgs());
                }
                else
                {
                    button_ok_Click(button_ok, new EventArgs());
                }

                e.SuppressKeyPress = false;
                e.Handled = true;
            }
        }

        #endregion

    }
}