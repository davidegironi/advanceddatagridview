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
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Zuby.ADGV
{

    [System.ComponentModel.DesignerCategory("")]
    public class AdvancedDataGridView : DataGridView
    {

        #region public events

        public class SortEventArgs : EventArgs
        {
            public string SortString { get; set; }
            public bool Cancel { get; set; }

            public SortEventArgs()
            {
                SortString = null;
                Cancel = false;
            }
        }

        public class FilterEventArgs : EventArgs
        {
            public string FilterString { get; set; }
            public bool Cancel { get; set; }

            public FilterEventArgs()
            {
                FilterString = null;
                Cancel = false;
            }
        }

        public event EventHandler<SortEventArgs> SortStringChanged;

        public event EventHandler<FilterEventArgs> FilterStringChanged;

        #endregion


        #region translations

        /// <summary>
        /// Available translation keys
        /// </summary>
        public enum TranslationKey
        {
            ADGVSortDateTimeASC,
            ADGVSortDateTimeDESC,
            ADGVSortBoolASC,
            ADGVSortBoolDESC,
            ADGVSortNumASC,
            ADGVSortNumDESC,
            ADGVSortTextASC,
            ADGVSortTextDESC,
            ADGVAddCustomFilter,
            ADGVCustomFilter,
            ADGVClearFilter,
            ADGVClearSort,
            ADGVButtonFilter,
            ADGVButtonUndofilter,
            ADGVNodeSelectAll,
            ADGVNodeSelectEmpty,
            ADGVNodeSelectTrue,
            ADGVNodeSelectFalse,
            ADGVFilterChecklistDisable,
            ADGVEquals,
            ADGVDoesNotEqual,
            ADGVEarlierThan,
            ADGVEarlierThanOrEqualTo,
            ADGVLaterThan,
            ADGVLaterThanOrEqualTo,
            ADGVBetween,
            ADGVGreaterThan,
            ADGVGreaterThanOrEqualTo,
            ADGVLessThan,
            ADGVLessThanOrEqualTo,
            ADGVBeginsWith,
            ADGVDoesNotBeginWith,
            ADGVEndsWith,
            ADGVDoesNotEndWith,
            ADGVContains,
            ADGVDoesNotContain,
            ADGVInvalidValue,
            ADGVFilterStringDescription,
            ADGVFormTitle,
            ADGVLabelColumnNameText,
            ADGVLabelAnd,
            ADGVButtonOk,
            ADGVButtonCancel
        }

        /// <summary>
        /// Internationalization strings
        /// </summary>
        public static Dictionary<string, string> Translations = new Dictionary<string, string>()
        {
            { TranslationKey.ADGVSortDateTimeASC.ToString(), "Sort Oldest to Newest" },
            { TranslationKey.ADGVSortDateTimeDESC.ToString(), "Sort Newest to Oldest" },
            { TranslationKey.ADGVSortBoolASC.ToString(), "Sort by False/True" },
            { TranslationKey.ADGVSortBoolDESC.ToString(), "Sort by True/False" },
            { TranslationKey.ADGVSortNumASC.ToString(), "Sort Smallest to Largest" },
            { TranslationKey.ADGVSortNumDESC.ToString(), "Sort Largest to Smallest" },
            { TranslationKey.ADGVSortTextASC.ToString(), "Sort А to Z" },
            { TranslationKey.ADGVSortTextDESC.ToString(), "Sort Z to A" },
            { TranslationKey.ADGVAddCustomFilter.ToString(), "Add a Custom Filter" },
            { TranslationKey.ADGVCustomFilter.ToString(), "Custom Filter" },
            { TranslationKey.ADGVClearFilter.ToString(), "Clear Filter" },
            { TranslationKey.ADGVClearSort.ToString(), "Clear Sort" },
            { TranslationKey.ADGVButtonFilter.ToString(), "Filter" },
            { TranslationKey.ADGVButtonUndofilter.ToString(), "Cancel" },
            { TranslationKey.ADGVNodeSelectAll.ToString(), "(Select All)" },
            { TranslationKey.ADGVNodeSelectEmpty.ToString(), "(Blanks)" },
            { TranslationKey.ADGVNodeSelectTrue.ToString(), "True" },
            { TranslationKey.ADGVNodeSelectFalse.ToString(), "False" },
            { TranslationKey.ADGVFilterChecklistDisable.ToString(), "Filter list is disabled" },
            { TranslationKey.ADGVEquals.ToString(), "equals" },
            { TranslationKey.ADGVDoesNotEqual.ToString(), "does not equal" },
            { TranslationKey.ADGVEarlierThan.ToString(), "earlier than" },
            { TranslationKey.ADGVEarlierThanOrEqualTo.ToString(), "earlier than or equal to" },
            { TranslationKey.ADGVLaterThan.ToString(), "later than"},
            { TranslationKey.ADGVLaterThanOrEqualTo.ToString(), "later than or equal to" },
            { TranslationKey.ADGVBetween.ToString(), "between" },
            { TranslationKey.ADGVGreaterThan.ToString(), "greater than" },
            { TranslationKey.ADGVGreaterThanOrEqualTo.ToString(), "greater than or equal to" },
            { TranslationKey.ADGVLessThan.ToString(), "less than" },
            { TranslationKey.ADGVLessThanOrEqualTo.ToString(), "less than or equal to" },
            { TranslationKey.ADGVBeginsWith.ToString(), "begins with" },
            { TranslationKey.ADGVDoesNotBeginWith.ToString(), "does not begin with" },
            { TranslationKey.ADGVEndsWith.ToString(), "ends with" },
            { TranslationKey.ADGVDoesNotEndWith.ToString(), "does not end with" },
            { TranslationKey.ADGVContains.ToString(), "contains" },
            { TranslationKey.ADGVDoesNotContain.ToString(), "does not contain" },
            { TranslationKey.ADGVInvalidValue.ToString(), "Invalid Value" },
            { TranslationKey.ADGVFilterStringDescription.ToString(), "Show rows where value {0} \"{1}\"" },
            { TranslationKey.ADGVFormTitle.ToString(), "Custom Filter" },
            { TranslationKey.ADGVLabelColumnNameText.ToString(), "Show rows where value" },
            { TranslationKey.ADGVLabelAnd.ToString(), "And" },
            { TranslationKey.ADGVButtonOk.ToString(), "OK" },
            { TranslationKey.ADGVButtonCancel.ToString(), "Cancel" }
        };

        #endregion


        #region class properties

        private List<string> _sortOrderList = new List<string>();
        private List<string> _filterOrderList = new List<string>();
        private List<string> _filteredColumns = new List<string>();

        private bool _loadedFilter = false;
        private string _sortString = null;
        private string _filterString = null;

        #endregion


        #region constructors

        /// <summary>
        /// AdvancedDataGridView constructor
        /// </summary>
        public AdvancedDataGridView()
        { }

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

        #endregion


        #region Helper methods

        /// <summary>
        /// Set AdvancedDataGridView the Double Buffered
        /// </summary>
        public void SetDoubleBuffered()
        {
            this.DoubleBuffered = true;
        }

        #endregion


        #region public Filter and Sort methods

        /// <summary>
        /// Disable a Filter and Sort on a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        public void DisableFilterAndSort(DataGridViewColumn column)
        {
            if (Columns.Contains(column))
            {
                ColumnHeaderCell cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    if (cell.FilterAndSortEnabled == true && (cell.SortString.Length > 0 || cell.FilterString.Length > 0))
                    {
                        CleanFilter(true);
                        cell.FilterAndSortEnabled = false;
                    }
                    else
                        cell.FilterAndSortEnabled = false;
                    _filterOrderList.Remove(column.Name);
                    _sortOrderList.Remove(column.Name);
                    _filteredColumns.Remove(column.Name);
                }
            }
        }

        /// <summary>
        /// Enable a Filter and Sort on a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        public void EnableFilterAndSort(DataGridViewColumn column)
        {
            if (Columns.Contains(column))
            {
                ColumnHeaderCell cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    if (!cell.FilterAndSortEnabled && (cell.FilterString.Length > 0 || cell.SortString.Length > 0))
                        CleanFilter(true);

                    cell.FilterAndSortEnabled = true;
                    _filteredColumns.Remove(column.Name);

                    SetFilterDateAndTimeEnabled(column, cell.IsFilterDateAndTimeEnabled);
                    SetSortEnabled(column, cell.IsSortEnabled);
                    SetFilterEnabled(column, cell.IsFilterEnabled);
                }
                else
                {
                    column.SortMode = DataGridViewColumnSortMode.Programmatic;
                    cell = new ColumnHeaderCell(column.HeaderCell, true);
                    cell.SortChanged += new ColumnHeaderCellEventHandler(Cell_SortChanged);
                    cell.FilterChanged += new ColumnHeaderCellEventHandler(Cell_FilterChanged);
                    cell.FilterPopup += new ColumnHeaderCellEventHandler(Cell_FilterPopup);
                    column.MinimumWidth = cell.MinimumSize.Width;
                    if (ColumnHeadersHeight < cell.MinimumSize.Height)
                        ColumnHeadersHeight = cell.MinimumSize.Height;
                    column.HeaderCell = cell;
                }
            }
        }

        /// <summary>
        /// Enabled or disable Filter and Sort capabilities on a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="enabled"></param>
        public void SetFilterAndSortEnabled(DataGridViewColumn column, bool enabled)
        {
            if (enabled)
                EnableFilterAndSort(column);
            else
                DisableFilterAndSort(column);
        }

        /// <summary>
        /// Disable a Filter checklist on a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        public void DisableFilterChecklist(DataGridViewColumn column)
        {
            if (Columns.Contains(column))
            {
                ColumnHeaderCell cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    cell.SetFilterChecklistEnabled(false);
                }
            }
        }

        /// <summary>
        /// Enable a Filter checklist on a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        public void EnableFilterChecklist(DataGridViewColumn column)
        {
            if (Columns.Contains(column))
            {
                ColumnHeaderCell cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    cell.SetFilterChecklistEnabled(true);
                }
            }
        }

        /// <summary>
        /// Enabled or disable Filter checklist capabilities on a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="enabled"></param>
        public void SetFilterChecklistEnabled(DataGridViewColumn column, bool enabled)
        {
            if (enabled)
                EnableFilterChecklist(column);
            else
                DisableFilterChecklist(column);
        }

        /// <summary>
        /// Disable a Filter custom on a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        public void DisableFilterCustom(DataGridViewColumn column)
        {
            if (Columns.Contains(column))
            {
                ColumnHeaderCell cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    cell.SetFilterCustomEnabled(false);
                }
            }
        }

        /// <summary>
        /// Enable a Filter custom on a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        public void EnableFilterCustom(DataGridViewColumn column)
        {
            if (Columns.Contains(column))
            {
                ColumnHeaderCell cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    cell.SetFilterCustomEnabled(true);
                }
            }
        }

        /// <summary>
        /// Enabled or disable Filter custom capabilities on a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="enabled"></param>
        public void SetFilterCustomEnabled(DataGridViewColumn column, bool enabled)
        {
            if (enabled)
                EnableFilterCustom(column);
            else
                DisableFilterCustom(column);
        }

        /// <summary>
        /// Load a Filter and Sort preset
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sorting"></param>
        public void LoadFilterAndSort(string filter, string sorting)
        {
            foreach (ColumnHeaderCell c in FilterableCells)
                c.SetLoadedMode(true);

            _filteredColumns.Clear();

            _filterOrderList.Clear();
            _sortOrderList.Clear();

            if (filter != null)
                FilterString = filter;
            if (sorting != null)
                SortString = sorting;

            _loadedFilter = true;
        }

        /// <summary>
        /// Clean Filter and Sort
        /// </summary>
        public void CleanFilterAndSort()
        {
            foreach (ColumnHeaderCell c in FilterableCells)
                c.SetLoadedMode(false);

            _filteredColumns.Clear();
            _filterOrderList.Clear();
            _sortOrderList.Clear();

            _loadedFilter = false;

            CleanFilter();
            CleanSort();
        }

        /// <summary>
        /// Set the NOTIN Logic for checkbox filter
        /// </summary>
        /// <param name="enabled"></param>
        public void SetMenuStripFilterNOTINLogic(bool enabled)
        {
            foreach (ColumnHeaderCell c in FilterableCells)
                c.IsMenuStripFilterNOTINLogicEnabled = enabled;
        }

        /// <summary>
        /// Get or Set Filter and Sort status
        /// </summary>
        public bool FilterAndSortEnabled
        {
            get
            {
                return _filterAndSortEnabled;
            }
            set
            {
                _filterAndSortEnabled = value;
            }
        }
        private bool _filterAndSortEnabled = true;

        #endregion


        #region public Sort methods

        /// <summary>
        /// Get the Sort string
        /// </summary>
        public string SortString
        {
            get
            {
                return (!String.IsNullOrEmpty(_sortString) ? _sortString : "");
            }
            private set
            {
                string old = value;
                if (old != _sortString)
                {
                    _sortString = value;

                    TriggerSortStringChanged();
                }
            }
        }

        /// <summary>
        /// Trigger the sort string changed method
        /// </summary>
        public void TriggerSortStringChanged()
        {
            //call event handler if one is attached
            SortEventArgs sortEventArgs = new SortEventArgs
            {
                SortString = _sortString,
                Cancel = false
            };
            if (SortStringChanged != null)
                SortStringChanged.Invoke(this, sortEventArgs);
            //sort datasource
            if (sortEventArgs.Cancel == false)
            {
                BindingSource datasource = this.DataSource as BindingSource;
                if (datasource != null)
                    datasource.Sort = sortEventArgs.SortString;
            }
        }

        /// <summary>
        /// Enabled or disable Sort capabilities for a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="enabled"></param>
        public void SetSortEnabled(DataGridViewColumn column, bool enabled)
        {
            if (Columns.Contains(column))
            {
                ColumnHeaderCell cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    cell.SetSortEnabled(enabled);
                }
            }
        }

        /// <summary>
        /// Sort ASC
        /// </summary>
        public void SortASC(DataGridViewColumn column)
        {
            if (Columns.Contains(column))
            {
                ColumnHeaderCell cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    cell.SortASC();
                }
            }
        }

        /// <summary>
        /// Sort ASC
        /// </summary>
        public void SortDESC(DataGridViewColumn column)
        {
            if (Columns.Contains(column))
            {
                ColumnHeaderCell cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    cell.SortDESC();
                }
            }
        }

        /// <summary>
        /// Clean all Sort on all columns
        /// </summary>
        /// <param name="fireEvent"></param>
        public void CleanSort(bool fireEvent)
        {
            foreach (ColumnHeaderCell c in FilterableCells)
                c.CleanSort();
            _sortOrderList.Clear();

            if (fireEvent)
                SortString = null;
            else
                _sortString = null;
        }
        public void CleanSort()
        {
            CleanSort(true);
        }

        #endregion


        #region public Filter methods

        /// <summary>
        /// Get the Filter string
        /// </summary>
        public string FilterString
        {
            get
            {
                return (!String.IsNullOrEmpty(_filterString) ? _filterString : "");
            }
            private set
            {
                string old = value;
                if (old != _filterString)
                {
                    _filterString = value;

                    TriggerFilterStringChanged();
                }
            }
        }

        /// <summary>
        /// Trigger the filter string changed method
        /// </summary>
        public void TriggerFilterStringChanged()
        {
            //call event handler if one is attached
            FilterEventArgs filterEventArgs = new FilterEventArgs
            {
                FilterString = _filterString,
                Cancel = false
            };
            if (FilterStringChanged != null)
                FilterStringChanged.Invoke(this, filterEventArgs);
            //sort datasource
            if (filterEventArgs.Cancel == false)
            {
                BindingSource datasource = this.DataSource as BindingSource;
                if (datasource != null)
                    datasource.Filter = filterEventArgs.FilterString;
            }
        }

        /// <summary>
        /// Set FilterDateAndTime status for a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="enabled"></param>
        public void SetFilterDateAndTimeEnabled(DataGridViewColumn column, bool enabled)
        {
            if (Columns.Contains(column))
            {
                ColumnHeaderCell cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    cell.IsFilterDateAndTimeEnabled = enabled;
                }
            }
        }

        /// <summary>
        /// Enable or disable Filter capabilities for a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="enabled"></param>
        public void SetFilterEnabled(DataGridViewColumn column, bool enabled)
        {
            if (Columns.Contains(column))
            {
                ColumnHeaderCell cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    cell.SetFilterEnabled(enabled);
                }
            }
        }

        /// <summary>
        /// Enable or disable Text filter on checklist remove node mode for a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="enabled"></param>
        public void SetChecklistTextFilterRemoveNodesOnSearchMode(DataGridViewColumn column, bool enabled)
        {
            if (Columns.Contains(column))
            {
                ColumnHeaderCell cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    cell.SetChecklistTextFilterRemoveNodesOnSearchMode(enabled);
                }
            }
        }

        /// <summary>
        /// Clean Filter on all columns
        /// </summary>
        /// <param name="fireEvent"></param>
        public void CleanFilter(bool fireEvent)
        {
            foreach (ColumnHeaderCell c in FilterableCells)
            {
                c.CleanFilter();
            }
            _filterOrderList.Clear();

            if (fireEvent)
                FilterString = null;
            else
                _filterString = null;
        }
        public void CleanFilter()
        {
            CleanFilter(true);
        }

        /// <summary>
        /// Set the text filter search nodes behaviour
        /// </summary>
        public void SetTextFilterRemoveNodesOnSearch(DataGridViewColumn column, bool enabled)
        {
            if (Columns.Contains(column))
            {
                ColumnHeaderCell cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                    cell.DoesTextFilterRemoveNodesOnSearch = enabled;
            }
        }

        /// <summary>
        /// Get the text filter search nodes behaviour
        /// </summary>
        public Nullable<bool> GetTextFilterRemoveNodesOnSearch(DataGridViewColumn column)
        {
            Nullable<bool> ret = null;
            if (Columns.Contains(column))
            {
                ColumnHeaderCell cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                    ret = cell.DoesTextFilterRemoveNodesOnSearch;
            }
            return ret;
        }

        #endregion


        #region public Find methods

        /// <summary>
        /// Find the Cell with the given value
        /// </summary>
        /// <param name="valueToFind"></param>
        /// <param name="columnName"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <param name="isWholeWordSearch"></param>
        /// <param name="isCaseSensitive"></param>
        /// <returns></returns>
        public DataGridViewCell FindCell(string valueToFind, string columnName, int rowIndex, int columnIndex, bool isWholeWordSearch, bool isCaseSensitive)
        {
            if (valueToFind != null && RowCount > 0 && ColumnCount > 0 && (columnName == null || (Columns.Contains(columnName) && Columns[columnName].Visible)))
            {
                rowIndex = Math.Max(0, rowIndex);

                if (!isCaseSensitive)
                    valueToFind = valueToFind.ToLower();

                if (columnName != null)
                {
                    int c = Columns[columnName].Index;
                    if (columnIndex > c)
                        rowIndex++;
                    for (int r = rowIndex; r < RowCount; r++)
                    {
                        string value = Rows[r].Cells[c].FormattedValue.ToString();
                        if (!isCaseSensitive)
                            value = value.ToLower();

                        if ((!isWholeWordSearch && value.Contains(valueToFind)) || value.Equals(valueToFind))
                            return Rows[r].Cells[c];
                    }
                }
                else
                {
                    columnIndex = Math.Max(0, columnIndex);

                    for (int r = rowIndex; r < RowCount; r++)
                    {
                        for (int c = columnIndex; c < ColumnCount; c++)
                        {
                            if (!Rows[r].Cells[c].Visible)
                                continue;

                            string value = Rows[r].Cells[c].FormattedValue.ToString();
                            if (!isCaseSensitive)
                                value = value.ToLower();

                            if ((!isWholeWordSearch && value.Contains(valueToFind)) || value.Equals(valueToFind))
                                return Rows[r].Cells[c];
                        }

                        columnIndex = 0;
                    }
                }
            }

            return null;
        }

        #endregion


        #region cells methods

        /// <summary>
        /// Get all columns
        /// </summary>
        private IEnumerable<ColumnHeaderCell> FilterableCells
        {
            get
            {
                return from DataGridViewColumn c in Columns
                       where c.HeaderCell != null && c.HeaderCell is ColumnHeaderCell
                       select (c.HeaderCell as ColumnHeaderCell);
            }
        }

        #endregion


        #region column events

        /// <summary>
        /// Overriden  OnColumnAdded event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            e.Column.SortMode = DataGridViewColumnSortMode.Programmatic;
            ColumnHeaderCell cell = new ColumnHeaderCell(e.Column.HeaderCell, FilterAndSortEnabled);
            cell.SortChanged += new ColumnHeaderCellEventHandler(Cell_SortChanged);
            cell.FilterChanged += new ColumnHeaderCellEventHandler(Cell_FilterChanged);
            cell.FilterPopup += new ColumnHeaderCellEventHandler(Cell_FilterPopup);
            e.Column.MinimumWidth = cell.MinimumSize.Width;
            if (ColumnHeadersHeight < cell.MinimumSize.Height)
                ColumnHeadersHeight = cell.MinimumSize.Height;
            e.Column.HeaderCell = cell;

            base.OnColumnAdded(e);
        }

        /// <summary>
        /// Overridden OnColumnRemoved event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnColumnRemoved(DataGridViewColumnEventArgs e)
        {
            _filteredColumns.Remove(e.Column.Name);
            _filterOrderList.Remove(e.Column.Name);
            _sortOrderList.Remove(e.Column.Name);

            ColumnHeaderCell cell = e.Column.HeaderCell as ColumnHeaderCell;
            if (cell != null)
            {
                cell.SortChanged -= Cell_SortChanged;
                cell.FilterChanged -= Cell_FilterChanged;
                cell.FilterPopup -= Cell_FilterPopup;
            }
            base.OnColumnRemoved(e);
        }

        #endregion


        #region rows events

        /// <summary>
        /// Overridden OnRowsAdded event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
        {
            _filteredColumns.Clear();
            base.OnRowsAdded(e);
        }

        /// <summary>
        /// Overridden OnRowsRemoved event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e)
        {
            _filteredColumns.Clear();
            base.OnRowsRemoved(e);
        }

        #endregion


        #region cell events

        /// <summary>
        /// Overridden OnCellValueChanged event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
        {
            _filteredColumns.Remove(Columns[e.ColumnIndex].Name);
            base.OnCellValueChanged(e);
        }

        #endregion


        #region filter events

        /// <summary>
        /// Build the complete Filter string
        /// </summary>
        /// <returns></returns>
        private string BuildFilterString()
        {
            StringBuilder sb = new StringBuilder("");
            string appx = "";

            foreach (string filterOrder in _filterOrderList)
            {
                DataGridViewColumn Column = Columns[filterOrder];

                if (Column != null)
                {
                    ColumnHeaderCell cell = Column.HeaderCell as ColumnHeaderCell;
                    if (cell != null)
                    {
                        if (cell.FilterAndSortEnabled && cell.ActiveFilterType != MenuStrip.FilterType.None)
                        {
                            sb.AppendFormat(appx + "(" + cell.FilterString + ")", Column.DataPropertyName);
                            appx = " AND ";
                        }
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// FilterPopup event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_FilterPopup(object sender, ColumnHeaderCellEventArgs e)
        {
            if (Columns.Contains(e.Column))
            {
                MenuStrip filterMenu = e.FilterMenu;
                DataGridViewColumn column = e.Column;

                System.Drawing.Rectangle rect = GetCellDisplayRectangle(column.Index, -1, true);

                if (_filteredColumns.Contains(column.Name))
                    filterMenu.Show(this, rect.Left, rect.Bottom, false);
                else
                {
                    _filteredColumns.Add(column.Name);
                    if (_filterOrderList.Count() > 0 && _filterOrderList.Last() == column.Name)
                        filterMenu.Show(this, rect.Left, rect.Bottom, true);
                    else
                        filterMenu.Show(this, rect.Left, rect.Bottom, MenuStrip.GetValuesForFilter(this, column.Name));
                }
            }
        }

        /// <summary>
        /// FilterChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_FilterChanged(object sender, ColumnHeaderCellEventArgs e)
        {
            if (Columns.Contains(e.Column))
            {
                MenuStrip filterMenu = e.FilterMenu;
                DataGridViewColumn column = e.Column;

                _filterOrderList.Remove(column.Name);
                if (filterMenu.ActiveFilterType != MenuStrip.FilterType.None)
                    _filterOrderList.Add(column.Name);

                FilterString = BuildFilterString();

                if (_loadedFilter)
                {
                    _loadedFilter = false;
                    foreach (ColumnHeaderCell c in FilterableCells.Where(f => f.MenuStrip != filterMenu))
                        c.SetLoadedMode(false);
                }
            }
        }

        #endregion


        #region sort events

        /// <summary>
        /// Build the complete Sort string
        /// </summary>
        /// <returns></returns>
        private string BuildSortString()
        {
            StringBuilder sb = new StringBuilder("");
            string appx = "";

            foreach (string sortOrder in _sortOrderList)
            {
                DataGridViewColumn column = Columns[sortOrder];

                if (column != null)
                {
                    ColumnHeaderCell cell = column.HeaderCell as ColumnHeaderCell;
                    if (cell != null)
                    {
                        if (cell.FilterAndSortEnabled && cell.ActiveSortType != MenuStrip.SortType.None)
                        {
                            sb.AppendFormat(appx + cell.SortString, column.DataPropertyName);
                            appx = ", ";
                        }
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// SortChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_SortChanged(object sender, ColumnHeaderCellEventArgs e)
        {
            if (Columns.Contains(e.Column))
            {
                MenuStrip filterMenu = e.FilterMenu;
                DataGridViewColumn column = e.Column;

                _sortOrderList.Remove(column.Name);
                if (filterMenu.ActiveSortType != MenuStrip.SortType.None)
                    _sortOrderList.Add(column.Name);
                SortString = BuildSortString();
            }
        }

        #endregion

    }
}