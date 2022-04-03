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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Zuby.ADGV
{

    [System.ComponentModel.DesignerCategory("")]
    internal partial class MenuStrip : ContextMenuStrip
    {

        #region public enum

        /// <summary>
        /// MenuStrip Filter type
        /// </summary>
        public enum FilterType : byte
        {
            None = 0,
            Custom = 1,
            CheckList = 2,
            Loaded = 3
        }


        /// <summary>
        /// MenuStrip Sort type
        /// </summary>
        public enum SortType : byte
        {
            None = 0,
            ASC = 1,
            DESC = 2
        }

        #endregion


        #region public constants

        /// <summary>
        /// Default checklist filter node behaviour 
        /// </summary>
        public const bool DefaultCheckTextFilterRemoveNodesOnSearch = true;

        /// <summary>
        /// Default max filter checklist max nodes
        /// </summary>
        public const int DefaultMaxChecklistNodes = 10000;

        /// <summary>
        /// Default number of nodes to enable the TextChanged delay on text filter
        /// </summary>
        public const int DefaultTextFilterTextChangedDelayNodes = 1000;

        /// <summary>
        /// Number of nodes to disable the text filter TextChanged delay
        /// </summary>
        public const int TextFilterTextChangedDelayNodesDisabled = -1;

        /// <summary>
        /// Default delay milliseconds for TextChanged delay on text filter
        /// </summary>
        public const int DefaultTextFilterTextChangedDelayMs = 300;

        #endregion


        #region class properties

        private FilterType _activeFilterType = FilterType.None;
        private SortType _activeSortType = SortType.None;
        private TreeNodeItemSelector[] _loadedNodes = new TreeNodeItemSelector[] { };
        private TreeNodeItemSelector[] _startingNodes = new TreeNodeItemSelector[] { };
        private TreeNodeItemSelector[] _removedNodes = new TreeNodeItemSelector[] { };
        private TreeNodeItemSelector[] _removedsessionNodes = new TreeNodeItemSelector[] { };
        private string _sortString = null;
        private string _filterString = null;
        private static Point _resizeStartPoint = new Point(1, 1);
        private Point _resizeEndPoint = new Point(-1, -1);
        private bool _checkTextFilterChangedEnabled = true;
        private bool _checkTextFilterRemoveNodesOnSearch = DefaultCheckTextFilterRemoveNodesOnSearch;
        private int _maxChecklistNodes = DefaultMaxChecklistNodes;
        private bool _filterclick = false;
        private Timer _textFilterTextChangedTimer;
        private int _textFilterTextChangedDelayNodes = DefaultTextFilterTextChangedDelayNodes;
        private int _textFilterTextChangedDelayMs = DefaultTextFilterTextChangedDelayMs;

        #endregion


        #region costructors

        /// <summary>
        /// MenuStrip constructor
        /// </summary>
        /// <param name="dataType"></param>
        public MenuStrip(Type dataType)
            : base()
        {
            //initialize components
            InitializeComponent();

            //set component translations
            cancelSortMenuItem.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVClearSort.ToString()];
            cancelFilterMenuItem.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVClearFilter.ToString()];
            customFilterMenuItem.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVAddCustomFilter.ToString()];
            button_filter.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVButtonFilter.ToString()];
            button_undofilter.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVButtonUndofilter.ToString()];

            //set type
            DataType = dataType;

            //set components values
            if (DataType == typeof(DateTime) || DataType == typeof(TimeSpan))
            {
                customFilterLastFiltersListMenuItem.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVCustomFilter.ToString()];
                sortASCMenuItem.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVSortDateTimeASC.ToString()];
                sortDESCMenuItem.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVSortDateTimeDESC.ToString()];
                sortASCMenuItem.Image = Properties.Resources.MenuStrip_OrderASCnum;
                sortDESCMenuItem.Image = Properties.Resources.MenuStrip_OrderDESCnum;
            }
            else if (DataType == typeof(bool))
            {
                customFilterLastFiltersListMenuItem.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVCustomFilter.ToString()];
                sortASCMenuItem.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVSortBoolASC.ToString()];
                sortDESCMenuItem.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVSortBoolDESC.ToString()];
                sortASCMenuItem.Image = Properties.Resources.MenuStrip_OrderASCbool;
                sortDESCMenuItem.Image = Properties.Resources.MenuStrip_OrderDESCbool;
            }
            else if (DataType == typeof(Int32) || DataType == typeof(Int64) || DataType == typeof(Int16) ||
                DataType == typeof(UInt32) || DataType == typeof(UInt64) || DataType == typeof(UInt16) ||
                DataType == typeof(Byte) || DataType == typeof(SByte) || DataType == typeof(Decimal) ||
                DataType == typeof(Single) || DataType == typeof(Double))
            {
                customFilterLastFiltersListMenuItem.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVCustomFilter.ToString()];
                sortASCMenuItem.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVSortNumASC.ToString()];
                sortDESCMenuItem.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVSortNumDESC.ToString()];
                sortASCMenuItem.Image = Properties.Resources.MenuStrip_OrderASCnum;
                sortDESCMenuItem.Image = Properties.Resources.MenuStrip_OrderDESCnum;
            }
            else
            {
                customFilterLastFiltersListMenuItem.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVCustomFilter.ToString()];
                sortASCMenuItem.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVSortTextASC.ToString()];
                sortDESCMenuItem.Text = AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVSortTextDESC.ToString()];
                sortASCMenuItem.Image = Properties.Resources.MenuStrip_OrderASCtxt;
                sortDESCMenuItem.Image = Properties.Resources.MenuStrip_OrderDESCtxt;
            }

            //set check filter textbox
            if (DataType == typeof(DateTime) || DataType == typeof(TimeSpan) || DataType == typeof(bool))
                checkTextFilter.Enabled = false;

            //set default NOT IN logic
            IsFilterNOTINLogicEnabled = false;

            //sent enablers default
            IsSortEnabled = true;
            IsFilterEnabled = true;
            IsFilterChecklistEnabled = true;
            IsFilterDateAndTimeEnabled = true;

            //set default compoents
            customFilterLastFiltersListMenuItem.Enabled = DataType != typeof(bool);
            customFilterLastFiltersListMenuItem.Checked = ActiveFilterType == FilterType.Custom;

            //resize before hitting ResizeBox so the grip works correctly
            float scalingfactor = GetScalingFactor();
            MinimumSize = new Size(Scale(PreferredSize.Width, scalingfactor), Scale(PreferredSize.Height, scalingfactor));
            //once the size is set resize the ones that wont change      
            resizeBoxControlHost.Height = Scale(resizeBoxControlHost.Height, scalingfactor);
            resizeBoxControlHost.Width = Scale(resizeBoxControlHost.Width, scalingfactor);
            toolStripSeparator1MenuItem.Height = Scale(toolStripSeparator1MenuItem.Height, scalingfactor);
            toolStripSeparator2MenuItem.Height = Scale(toolStripSeparator2MenuItem.Height, scalingfactor);
            toolStripSeparator3MenuItem.Height = Scale(toolStripSeparator3MenuItem.Height, scalingfactor);
            sortASCMenuItem.Height = Scale(sortASCMenuItem.Height, scalingfactor);
            sortDESCMenuItem.Height = Scale(sortDESCMenuItem.Height, scalingfactor);
            cancelSortMenuItem.Height = Scale(cancelSortMenuItem.Height, scalingfactor);
            cancelFilterMenuItem.Height = Scale(cancelFilterMenuItem.Height, scalingfactor);
            customFilterMenuItem.Height = Scale(customFilterMenuItem.Height, scalingfactor);
            customFilterLastFiltersListMenuItem.Height = Scale(customFilterLastFiltersListMenuItem.Height, scalingfactor);
            checkTextFilterControlHost.Height = Scale(checkTextFilterControlHost.Height, scalingfactor);
            button_filter.Width = Scale(button_filter.Width, scalingfactor);
            button_filter.Height = Scale(button_filter.Height, scalingfactor);
            button_undofilter.Width = Scale(button_undofilter.Width, scalingfactor);
            button_undofilter.Height = Scale(button_undofilter.Height, scalingfactor);
            //resize
            ResizeBox(MinimumSize.Width, MinimumSize.Height);

            _textFilterTextChangedTimer = new Timer();
            _textFilterTextChangedTimer.Interval = _textFilterTextChangedDelayMs;
            _textFilterTextChangedTimer.Tick += new EventHandler(this.CheckTextFilterTextChangedTimer_Tick);
        }

        /// <summary>
        /// Closed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuStrip_Closed(Object sender, EventArgs e)
        {
            ResizeClean();

            if (_checkTextFilterRemoveNodesOnSearch && !_filterclick)
            {
                _loadedNodes = DuplicateNodes(_startingNodes);
            }

            _startingNodes = new TreeNodeItemSelector[] { };

            _checkTextFilterChangedEnabled = false;
            checkTextFilter.Text = "";
            _checkTextFilterChangedEnabled = true;
        }

        /// <summary>
        /// LostFocust event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuStrip_LostFocus(Object sender, EventArgs e)
        {
            if (!ContainsFocus)
                Close();
        }

        /// <summary>
        /// Control removed event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            _loadedNodes = new TreeNodeItemSelector[] { };
            _startingNodes = new TreeNodeItemSelector[] { };
            _removedNodes = new TreeNodeItemSelector[] { };
            _removedsessionNodes = new TreeNodeItemSelector[] { };
            if (_textFilterTextChangedTimer != null)
                _textFilterTextChangedTimer.Stop();

            base.OnControlRemoved(e);
        }

        /// <summary>
        /// Get all images for checkList
        /// </summary>
        /// <returns></returns>
        private static ImageList GetCheckListStateImages()
        {
            ImageList images = new ImageList();
            Bitmap unCheckImg = new Bitmap(16, 16);
            Bitmap checkImg = new Bitmap(16, 16);
            Bitmap mixedImg = new Bitmap(16, 16);

            using (Bitmap img = new Bitmap(16, 16))
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    CheckBoxRenderer.DrawCheckBox(g, new Point(0, 1), CheckBoxState.UncheckedNormal);
                    unCheckImg = (Bitmap)img.Clone();
                    CheckBoxRenderer.DrawCheckBox(g, new Point(0, 1), CheckBoxState.CheckedNormal);
                    checkImg = (Bitmap)img.Clone();
                    CheckBoxRenderer.DrawCheckBox(g, new Point(0, 1), CheckBoxState.MixedNormal);
                    mixedImg = (Bitmap)img.Clone();
                }
            }

            images.Images.Add("uncheck", unCheckImg);
            images.Images.Add("check", checkImg);
            images.Images.Add("mixed", mixedImg);

            return images;
        }

        #endregion


        #region public events

        /// <summary>
        /// The current Sorting in changed
        /// </summary>
        public event EventHandler SortChanged;

        /// <summary>
        /// The current Filter is changed
        /// </summary>
        public event EventHandler FilterChanged;

        #endregion


        #region public getter and setters

        /// <summary>
        /// Set the max checklist nodes
        /// </summary>
        public int MaxChecklistNodes
        {
            get
            {
                return _maxChecklistNodes;
            }
            set
            {
                _maxChecklistNodes = value;
            }
        }

        /// <summary>
        /// Get the current MenuStripSortType type
        /// </summary>
        public SortType ActiveSortType
        {
            get
            {
                return _activeSortType;
            }
        }

        /// <summary>
        /// Get the current MenuStripFilterType type
        /// </summary>
        public FilterType ActiveFilterType
        {
            get
            {
                return _activeFilterType;
            }
        }

        /// <summary>
        /// Get the DataType for the MenuStrip Filter
        /// </summary>
        public Type DataType { get; private set; }

        /// <summary>
        /// Get or Set the Filter Sort enabled
        /// </summary>
        public bool IsSortEnabled { get; set; }

        /// <summary>
        /// Get or Set the Filter enabled
        /// </summary>
        public bool IsFilterEnabled { get; set; }

        /// <summary>
        /// Get or Set the Filter Checklist enabled
        /// </summary>
        public bool IsFilterChecklistEnabled { get; set; }

        /// <summary>
        /// Get or Set the Filter Custom enabled
        /// </summary>
        public bool IsFilterCustomEnabled { get; set; }

        /// <summary>
        /// Get or Set the Filter DateAndTime enabled
        /// </summary>
        public bool IsFilterDateAndTimeEnabled { get; set; }

        /// <summary>
        /// Get or Set the NOT IN logic for Filter
        /// </summary>
        public bool IsFilterNOTINLogicEnabled { get; set; }

        /// <summary>
        /// Set the text filter search nodes behaviour
        /// </summary>
        public bool DoesTextFilterRemoveNodesOnSearch
        {
            get
            {
                return _checkTextFilterRemoveNodesOnSearch;
            }
            set
            {
                _checkTextFilterRemoveNodesOnSearch = value;
            }
        }

        /// <summary>
        /// Number of nodes to enable the TextChanged delay on text filter
        /// </summary>
        public int TextFilterTextChangedDelayNodes
        {
            get
            {
                return _textFilterTextChangedDelayNodes;
            }
            set
            {
                _textFilterTextChangedDelayNodes = value;
            }
        }

        /// <summary>
        /// Delay milliseconds for TextChanged delay on text filter
        /// </summary>
        public int TextFilterTextChangedDelayMs
        {
            get
            {
                return _textFilterTextChangedDelayMs;
            }
            set
            {
                _textFilterTextChangedDelayMs = value;
            }
        }

        #endregion


        #region public enablers

        /// <summary>
        /// Enabled or disable Sorting capabilities
        /// </summary>
        /// <param name="enabled"></param>
        public void SetSortEnabled(bool enabled)
        {
            IsSortEnabled = enabled;

            sortASCMenuItem.Enabled = enabled;
            sortDESCMenuItem.Enabled = enabled;
            cancelSortMenuItem.Enabled = enabled;
        }

        /// <summary>
        /// Enable or disable Filter capabilities
        /// </summary>
        /// <param name="enabled"></param>
        public void SetFilterEnabled(bool enabled)
        {
            IsFilterEnabled = enabled;

            cancelFilterMenuItem.Enabled = enabled;
            customFilterLastFiltersListMenuItem.Enabled = (enabled && DataType != typeof(bool));
            button_filter.Enabled = enabled;
            button_undofilter.Enabled = enabled;
            checkList.Enabled = enabled;
            checkTextFilter.Enabled = enabled;
        }

        /// <summary>
        /// Enable or disable Filter checklist capabilities
        /// </summary>
        /// <param name="enabled"></param>
        public void SetFilterChecklistEnabled(bool enabled)
        {
            if (!IsFilterEnabled)
                enabled = false;

            IsFilterChecklistEnabled = enabled;
            checkList.Enabled = enabled;
            checkTextFilter.ReadOnly = !enabled;

            if (!IsFilterChecklistEnabled)
            {
                ChecklistClearNodes();
                TreeNodeItemSelector disablednode = TreeNodeItemSelector.CreateNode(AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVFilterChecklistDisable.ToString()] + "            ", null, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.SelectAll);
                disablednode.NodeFont = new Font(checkList.Font, FontStyle.Bold);
                ChecklistAddNode(disablednode);
                ChecklistReloadNodes();
            }
        }

        /// <summary>
        /// Enable or disable Filter custom capabilities
        /// </summary>
        /// <param name="enabled"></param>
        public void SetFilterCustomEnabled(bool enabled)
        {
            if (!IsFilterEnabled)
                enabled = false;

            IsFilterCustomEnabled = enabled;
            customFilterMenuItem.Enabled = enabled;
            customFilterLastFiltersListMenuItem.Enabled = enabled;

            if (!IsFilterCustomEnabled)
            {
                UnCheckCustomFilters();
            }
        }

        /// <summary>
        /// Disable text filter TextChanged delay
        /// </summary>
        public void SetTextFilterTextChangedDelayNodesDisabled()
        {
            _textFilterTextChangedDelayNodes = TextFilterTextChangedDelayNodesDisabled;
        }

        #endregion


        #region preset loader

        public void SetLoadedMode(bool enabled)
        {
            customFilterMenuItem.Enabled = !enabled;
            cancelFilterMenuItem.Enabled = enabled;
            if (enabled)
            {
                _activeFilterType = FilterType.Loaded;
                _sortString = null;
                _filterString = null;
                customFilterLastFiltersListMenuItem.Checked = false;
                for (int i = 2; i < customFilterLastFiltersListMenuItem.DropDownItems.Count - 1; i++)
                {
                    (customFilterLastFiltersListMenuItem.DropDownItems[i] as ToolStripMenuItem).Checked = false;
                }

                ChecklistClearNodes();
                TreeNodeItemSelector allnode = TreeNodeItemSelector.CreateNode(AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVNodeSelectAll.ToString()] + "            ", null, CheckState.Indeterminate, TreeNodeItemSelector.CustomNodeType.SelectAll);
                allnode.NodeFont = new Font(checkList.Font, FontStyle.Bold);
                ChecklistAddNode(allnode);
                ChecklistReloadNodes();

                SetSortEnabled(false);
                SetFilterEnabled(false);
            }
            else
            {
                _activeFilterType = FilterType.None;

                SetSortEnabled(true);
                SetFilterEnabled(true);
            }
        }

        #endregion


        #region public show methods

        /// <summary>
        /// Show the menuStrip
        /// </summary>
        /// <param name="control"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="vals"></param>
        public void Show(Control control, int x, int y, IEnumerable<DataGridViewCell> vals)
        {
            _removedNodes = new TreeNodeItemSelector[] { };
            _removedsessionNodes = new TreeNodeItemSelector[] { };

            //add nodes
            BuildNodes(vals);
            //set the starting nodes
            _startingNodes = DuplicateNodes(_loadedNodes);

            if (_activeFilterType == FilterType.Custom)
                SetNodesCheckState(_loadedNodes, false);
            base.Show(control, x, y);

            _filterclick = false;

            _checkTextFilterChangedEnabled = false;
            checkTextFilter.Text = "";
            _checkTextFilterChangedEnabled = true;
        }

        /// <summary>
        /// Show the menuStrip
        /// </summary>
        /// <param name="control"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="_restoreFilter"></param>
        public void Show(Control control, int x, int y, bool _restoreFilter)
        {
            _checkTextFilterChangedEnabled = false;
            checkTextFilter.Text = "";
            _checkTextFilterChangedEnabled = true;
            if (_restoreFilter || _checkTextFilterRemoveNodesOnSearch)
            {
                //reset the starting nodes
                _startingNodes = DuplicateNodes(_loadedNodes);
            }
            //reset removed nodes
            if (_checkTextFilterRemoveNodesOnSearch)
            {
                _removedNodes = _loadedNodes.Where(n => n.CheckState == CheckState.Unchecked && n.NodeType == TreeNodeItemSelector.CustomNodeType.Default).ToArray();
                _removedsessionNodes = _removedNodes;
            }

            ChecklistReloadNodes();

            base.Show(control, x, y);

            _filterclick = false;
        }

        /// <summary>
        /// Get values used for Show method
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static IEnumerable<DataGridViewCell> GetValuesForFilter(DataGridView grid, string columnName)
        {
            var vals =
                from DataGridViewRow nulls in grid.Rows
                select nulls.Cells[columnName];

            return vals;
        }

        #endregion


        #region public sort methods

        /// <summary>
        /// Sort ASC
        /// </summary>
        public void SortASC()
        {
            SortASCMenuItem_Click(this, null);
        }

        /// <summary>
        /// Sort DESC
        /// </summary>
        public void SortDESC()
        {
            SortDESCMenuItem_Click(this, null);
        }

        /// <summary>
        /// Get the Sorting String
        /// </summary>
        public string SortString
        {
            get
            {
                return (!String.IsNullOrEmpty(_sortString) ? _sortString : "");
            }
            private set
            {
                cancelSortMenuItem.Enabled = (value != null && value.Length > 0);
                _sortString = value;
            }
        }

        /// <summary>
        /// Clean the Sorting
        /// </summary>
        public void CleanSort()
        {
            sortASCMenuItem.Checked = false;
            sortDESCMenuItem.Checked = false;
            _activeSortType = SortType.None;
            SortString = null;
        }

        #endregion


        #region public filter methods

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
                cancelFilterMenuItem.Enabled = (value != null && value.Length > 0);
                _filterString = value;
            }
        }

        /// <summary>
        /// Clean the Filter
        /// </summary>
        public void CleanFilter()
        {
            if (_checkTextFilterRemoveNodesOnSearch)
            {
                _removedNodes = new TreeNodeItemSelector[] { };
                _removedsessionNodes = new TreeNodeItemSelector[] { };
            }

            for (int i = 2; i < customFilterLastFiltersListMenuItem.DropDownItems.Count - 1; i++)
            {
                (customFilterLastFiltersListMenuItem.DropDownItems[i] as ToolStripMenuItem).Checked = false;
            }
            _activeFilterType = FilterType.None;
            SetNodesCheckState(_loadedNodes, true);
            FilterString = null;
            customFilterLastFiltersListMenuItem.Checked = false;
            button_filter.Enabled = true;
        }

        /// <summary>
        /// Set the text filter on checklist remove node mode
        /// </summary>
        /// <param name="enabled"></param>
        public void SetChecklistTextFilterRemoveNodesOnSearchMode(bool enabled)
        {
            if (_checkTextFilterRemoveNodesOnSearch != enabled)
            {
                _checkTextFilterRemoveNodesOnSearch = enabled;
                CleanFilter();
            }
        }

        #endregion


        #region checklist filter methods

        /// <summary>
        /// Clear checklist loaded nodes
        /// </summary>
        private void ChecklistClearNodes()
        {
            _loadedNodes = new TreeNodeItemSelector[] { };
        }

        /// <summary>
        /// Add a node to checklist nodes
        /// </summary>
        /// <param name="node"></param>
        private void ChecklistAddNode(TreeNodeItemSelector node)
        {
            _loadedNodes = _loadedNodes.Concat(new TreeNodeItemSelector[] { node }).ToArray();
        }

        /// <summary>
        /// Load checklist nodes
        /// </summary>
        private void ChecklistReloadNodes()
        {
            checkList.BeginUpdate();
            checkList.Nodes.Clear();
            int nodecount = 0;
            foreach (TreeNodeItemSelector node in _loadedNodes)
            {
                if (node.NodeType == TreeNodeItemSelector.CustomNodeType.Default)
                {
                    if (_maxChecklistNodes == 0)
                    {
                        if (!_removedNodes.Contains(node))
                            checkList.Nodes.Add(node);
                    }
                    else
                    {
                        if (nodecount < _maxChecklistNodes && !_removedNodes.Contains(node))
                            checkList.Nodes.Add(node);
                        else if (nodecount == _maxChecklistNodes)
                            checkList.Nodes.Add("...");
                        if (!_removedNodes.Contains(node) || nodecount == _maxChecklistNodes)
                            nodecount++;

                    }
                }
                else
                {
                    checkList.Nodes.Add(node);
                }

            }
            checkList.EndUpdate();
        }

        /// <summary>
        /// Get checklist nodes
        /// </summary>
        /// <returns></returns>
        private TreeNodeCollection ChecklistNodes()
        {
            return checkList.Nodes;
        }

        /// <summary>
        /// Set the Filter String using checkList selected Nodes
        /// </summary>
        private void SetCheckListFilter()
        {
            UnCheckCustomFilters();

            TreeNodeItemSelector selectAllNode = GetSelectAllNode();
            customFilterLastFiltersListMenuItem.Checked = false;

            if (selectAllNode != null && selectAllNode.Checked && String.IsNullOrEmpty(checkTextFilter.Text))
                CancelFilterMenuItem_Click(null, new EventArgs());
            else
            {
                string oldfilter = FilterString;
                FilterString = "";
                _activeFilterType = FilterType.CheckList;

                if (_loadedNodes.Length > 1)
                {
                    selectAllNode = GetSelectEmptyNode();
                    if (selectAllNode != null && selectAllNode.Checked)
                        FilterString = "[{0}] IS NULL";

                    if (_loadedNodes.Length > 2 || selectAllNode == null)
                    {
                        string filter = BuildNodesFilterString(
                            (IsFilterNOTINLogicEnabled && (DataType != typeof(DateTime) && DataType != typeof(TimeSpan) && DataType != typeof(bool)) ?
                                _loadedNodes.AsParallel().Cast<TreeNodeItemSelector>().Where(
                                    n => n.NodeType != TreeNodeItemSelector.CustomNodeType.SelectAll
                                        && n.NodeType != TreeNodeItemSelector.CustomNodeType.SelectEmpty
                                        && n.CheckState == CheckState.Unchecked
                                ) :
                                _loadedNodes.AsParallel().Cast<TreeNodeItemSelector>().Where(
                                    n => n.NodeType != TreeNodeItemSelector.CustomNodeType.SelectAll
                                        && n.NodeType != TreeNodeItemSelector.CustomNodeType.SelectEmpty
                                        && n.CheckState != CheckState.Unchecked
                                ))
                        );

                        if (filter.Length > 0)
                        {
                            if (FilterString.Length > 0)
                                FilterString += " OR ";

                            if (DataType == typeof(bool))
                                FilterString += "[{0}] =" + filter;
                            else if (DataType == typeof(int) || DataType == typeof(long) || DataType == typeof(short) ||
                                     DataType == typeof(uint) || DataType == typeof(ulong) || DataType == typeof(ushort) ||
                                     DataType == typeof(decimal) ||
                                     DataType == typeof(byte) || DataType == typeof(sbyte) || DataType == typeof(string))
                            {
                                if (IsFilterNOTINLogicEnabled)
                                    FilterString += "[{0}] NOT IN (" + filter + ")";
                                else
                                    FilterString += "[{0}] IN (" + filter + ")";
                            }
                            else if (DataType == typeof(Bitmap))
                            { }
                            else
                            {
                                if (IsFilterNOTINLogicEnabled)
                                    FilterString += "Convert([{0}],System.String) NOT IN (" + filter + ")";
                                else
                                    FilterString += "Convert([{0}],System.String) IN (" + filter + ")";
                            }
                        }
                    }
                }

                if (oldfilter != FilterString && FilterChanged != null)
                    FilterChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Build a Filter string based on selectd Nodes
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private string BuildNodesFilterString(IEnumerable<TreeNodeItemSelector> nodes)
        {
            StringBuilder sb = new StringBuilder("");

            string appx = ", ";

            if (nodes != null && nodes.Any())
            {
                if (DataType == typeof(DateTime))
                {
                    foreach (TreeNodeItemSelector n in nodes)
                    {
                        if (n.Checked && (n.Nodes.AsParallel().Cast<TreeNodeItemSelector>().Where(sn => sn.CheckState != CheckState.Unchecked).Count() == 0))
                        {
                            DateTime dt = (DateTime)n.Value;
                            sb.Append("'" + Convert.ToString((IsFilterDateAndTimeEnabled ? dt : dt.Date), CultureInfo.CurrentCulture) + "'" + appx);
                        }
                        else if (n.CheckState != CheckState.Unchecked && n.Nodes.Count > 0)
                        {
                            string subnode = BuildNodesFilterString(n.Nodes.AsParallel().Cast<TreeNodeItemSelector>().Where(sn => sn.CheckState != CheckState.Unchecked));
                            if (subnode.Length > 0)
                                sb.Append(subnode + appx);
                        }
                    }
                }
                else if (DataType == typeof(TimeSpan))
                {
                    foreach (TreeNodeItemSelector n in nodes)
                    {
                        if (n.Checked && (n.Nodes.AsParallel().Cast<TreeNodeItemSelector>().Where(sn => sn.CheckState != CheckState.Unchecked).Count() == 0))
                        {
                            TimeSpan ts = (TimeSpan)n.Value;
                            sb.Append("'P" +
                                (ts.Days > 0 ? ts.Days + "D" : "") +
                                (ts.TotalHours > 0 ? "T" : "") + (ts.Hours > 0 ? ts.Hours + "H" : "") +
                                (ts.Minutes > 0 ? ts.Minutes + "M" : "") +
                                (ts.Seconds > 0 ? ts.Seconds + "S" : "") + "'" +
                                appx);
                        }
                        else if (n.CheckState != CheckState.Unchecked && n.Nodes.Count > 0)
                        {
                            string subnode = BuildNodesFilterString(n.Nodes.AsParallel().Cast<TreeNodeItemSelector>().Where(sn => sn.CheckState != CheckState.Unchecked));
                            if (subnode.Length > 0)
                                sb.Append(subnode + appx);
                        }
                    }
                }
                else if (DataType == typeof(bool))
                {
                    foreach (TreeNodeItemSelector n in nodes)
                    {
                        sb.Append(n.Value.ToString());
                        break;
                    }
                }
                else if (DataType == typeof(int) || DataType == typeof(long) || DataType == typeof(short) ||
                         DataType == typeof(uint) || DataType == typeof(ulong) || DataType == typeof(ushort) ||
                         DataType == typeof(byte) || DataType == typeof(sbyte))
                {
                    foreach (TreeNodeItemSelector n in nodes)
                        sb.Append(n.Value.ToString() + appx);
                }
                else if (DataType == typeof(float) || DataType == typeof(double) || DataType == typeof(decimal))
                {
                    foreach (TreeNodeItemSelector n in nodes)
                        sb.Append(n.Value.ToString().Replace(",", ".") + appx);
                }
                else if (DataType == typeof(Bitmap))
                { }
                else
                {
                    foreach (TreeNodeItemSelector n in nodes)
                        sb.Append("'" + FormatFilterString(n.Value.ToString()) + "'" + appx);
                }
            }

            if (sb.Length > appx.Length && DataType != typeof(bool))
                sb.Remove(sb.Length - appx.Length, appx.Length);

            return sb.ToString();
        }

        /// <summary>
        /// Format a text Filter string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string FormatFilterString(string text)
        {
            return text.Replace("'", "''");
        }

        /// <summary>
        /// Add nodes to checkList
        /// </summary>
        /// <param name="vals"></param>
        private void BuildNodes(IEnumerable<DataGridViewCell> vals)
        {
            if (!IsFilterChecklistEnabled)
                return;

            ChecklistClearNodes();

            if (vals != null)
            {
                //add select all node
                TreeNodeItemSelector allnode = TreeNodeItemSelector.CreateNode(AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVNodeSelectAll.ToString()] + "            ", null, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.SelectAll);
                allnode.NodeFont = new Font(checkList.Font, FontStyle.Bold);
                ChecklistAddNode(allnode);

                if (vals.Any())
                {
                    var nonulls = vals.Where<DataGridViewCell>(c => c.Value != null && c.Value != DBNull.Value);

                    //add select empty node
                    if (vals.Count() != nonulls.Count())
                    {
                        TreeNodeItemSelector nullnode = TreeNodeItemSelector.CreateNode(AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVNodeSelectEmpty.ToString()] + "               ", null, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.SelectEmpty);
                        nullnode.NodeFont = new Font(checkList.Font, FontStyle.Bold);
                        ChecklistAddNode(nullnode);
                    }

                    //add datetime nodes
                    if (DataType == typeof(DateTime))
                    {
                        var years =
                            from year in nonulls
                            group year by ((DateTime)year.Value).Year into cy
                            orderby cy.Key ascending
                            select cy;

                        foreach (var year in years)
                        {
                            TreeNodeItemSelector yearnode = TreeNodeItemSelector.CreateNode(year.Key.ToString(), year.Key, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.DateTimeNode);
                            ChecklistAddNode(yearnode);

                            var months =
                                from month in year
                                group month by ((DateTime)month.Value).Month into cm
                                orderby cm.Key ascending
                                select cm;

                            foreach (var month in months)
                            {
                                TreeNodeItemSelector monthnode = yearnode.CreateChildNode(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month.Key), month.Key);

                                var days =
                                    from day in month
                                    group day by ((DateTime)day.Value).Day into cd
                                    orderby cd.Key ascending
                                    select cd;

                                foreach (var day in days)
                                {
                                    TreeNodeItemSelector daysnode;

                                    if (!IsFilterDateAndTimeEnabled)
                                        daysnode = monthnode.CreateChildNode(day.Key.ToString("D2"), day.First().Value);
                                    else
                                    {
                                        daysnode = monthnode.CreateChildNode(day.Key.ToString("D2"), day.Key);

                                        var hours =
                                            from hour in day
                                            group hour by ((DateTime)hour.Value).Hour into ch
                                            orderby ch.Key ascending
                                            select ch;

                                        foreach (var hour in hours)
                                        {
                                            TreeNodeItemSelector hoursnode = daysnode.CreateChildNode(hour.Key.ToString("D2") + " " + "h", hour.Key);

                                            var mins =
                                                from min in hour
                                                group min by ((DateTime)min.Value).Minute into cmin
                                                orderby cmin.Key ascending
                                                select cmin;

                                            foreach (var min in mins)
                                            {
                                                TreeNodeItemSelector minsnode = hoursnode.CreateChildNode(min.Key.ToString("D2") + " " + "m", min.Key);

                                                var secs =
                                                    from sec in min
                                                    group sec by ((DateTime)sec.Value).Second into cs
                                                    orderby cs.Key ascending
                                                    select cs;

                                                foreach (var sec in secs)
                                                {
                                                    TreeNodeItemSelector secsnode = minsnode.CreateChildNode(sec.Key.ToString("D2") + " " + "s", sec.First().Value);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //add timespan nodes
                    else if (DataType == typeof(TimeSpan))
                    {
                        var days =
                            from day in nonulls
                            group day by ((TimeSpan)day.Value).Days into cd
                            orderby cd.Key ascending
                            select cd;

                        foreach (var day in days)
                        {
                            TreeNodeItemSelector daysnode = TreeNodeItemSelector.CreateNode(day.Key.ToString("D2"), day.Key, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.DateTimeNode);
                            ChecklistAddNode(daysnode);

                            var hours =
                                from hour in day
                                group hour by ((TimeSpan)hour.Value).Hours into ch
                                orderby ch.Key ascending
                                select ch;

                            foreach (var hour in hours)
                            {
                                TreeNodeItemSelector hoursnode = daysnode.CreateChildNode(hour.Key.ToString("D2") + " " + "h", hour.Key);

                                var mins =
                                    from min in hour
                                    group min by ((TimeSpan)min.Value).Minutes into cmin
                                    orderby cmin.Key ascending
                                    select cmin;

                                foreach (var min in mins)
                                {
                                    TreeNodeItemSelector minsnode = hoursnode.CreateChildNode(min.Key.ToString("D2") + " " + "m", min.Key);

                                    var secs =
                                        from sec in min
                                        group sec by ((TimeSpan)sec.Value).Seconds into cs
                                        orderby cs.Key ascending
                                        select cs;

                                    foreach (var sec in secs)
                                    {
                                        TreeNodeItemSelector secsnode = minsnode.CreateChildNode(sec.Key.ToString("D2") + " " + "s", sec.First().Value);
                                    }
                                }
                            }
                        }
                    }

                    //add boolean nodes
                    else if (DataType == typeof(bool))
                    {
                        var values = nonulls.Where<DataGridViewCell>(c => (bool)c.Value == true);

                        if (values.Count() != nonulls.Count())
                        {
                            TreeNodeItemSelector node = TreeNodeItemSelector.CreateNode(AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVNodeSelectFalse.ToString()], false, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.Default);
                            ChecklistAddNode(node);
                        }

                        if (values.Any())
                        {
                            TreeNodeItemSelector node = TreeNodeItemSelector.CreateNode(AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVNodeSelectTrue.ToString()], true, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.Default);
                            ChecklistAddNode(node);
                        }
                    }

                    //ignore image nodes
                    else if (DataType == typeof(Bitmap))
                    { }

                    //add string nodes
                    else
                    {
                        foreach (var v in nonulls.GroupBy(c => c.Value).OrderBy(g => g.Key))
                        {
                            TreeNodeItemSelector node = TreeNodeItemSelector.CreateNode(v.First().FormattedValue.ToString(), v.Key, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.Default);
                            ChecklistAddNode(node);
                        }
                    }
                }
            }

            ChecklistReloadNodes();
        }

        /// <summary>
        /// Check if filter buttons needs to be enabled
        /// </summary>
        private void CheckFilterButtonEnabled()
        {
            button_filter.Enabled = HasNodesChecked(_loadedNodes);
        }

        /// <summary>
        /// Check if selected nodes exists
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private bool HasNodesChecked(TreeNodeItemSelector[] nodes)
        {
            bool state = false;
            if (!String.IsNullOrEmpty(checkTextFilter.Text))
            {
                state = nodes.Any(n => n.CheckState == CheckState.Checked && n.Text.ToLower().Contains(checkTextFilter.Text.ToLower()));
            }
            else
            {
                state = nodes.Any(n => n.CheckState == CheckState.Checked);
            }

            if (state)
                return state;

            foreach (TreeNodeItemSelector node in nodes)
            {
                foreach (TreeNodeItemSelector nodesel in node.Nodes)
                {
                    state = HasNodesChecked(new TreeNodeItemSelector[] { nodesel });
                    if (state)
                        break;
                }
                if (state)
                    break;
            }

            return state;
        }

        /// <summary>
        /// Check
        /// </summary>
        /// <param name="node"></param>
        private void NodeCheckChange(TreeNodeItemSelector node)
        {
            if (node.CheckState == CheckState.Checked)
                node.CheckState = CheckState.Unchecked;
            else
                node.CheckState = CheckState.Checked;

            if (node.NodeType == TreeNodeItemSelector.CustomNodeType.SelectAll)
            {
                SetNodesCheckState(_loadedNodes, node.Checked);
            }
            else
            {
                if (node.Nodes.Count > 0)
                {
                    foreach (TreeNodeItemSelector subnode in node.Nodes)
                    {
                        SetNodesCheckState(new TreeNodeItemSelector[] { subnode }, node.Checked);
                    }
                }

                //refresh nodes
                CheckState state = UpdateNodesCheckState(ChecklistNodes());
                GetSelectAllNode().CheckState = state;
            }
        }

        /// <summary>
        /// Set Nodes CheckState
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="isChecked"></param>
        private void SetNodesCheckState(TreeNodeItemSelector[] nodes, bool isChecked)
        {
            foreach (TreeNodeItemSelector node in nodes)
            {
                node.Checked = isChecked;
                if (node.Nodes != null && node.Nodes.Count > 0)
                {
                    foreach (TreeNodeItemSelector subnode in node.Nodes)
                    {
                        SetNodesCheckState(new TreeNodeItemSelector[] { subnode }, isChecked);
                    }
                }

            }
        }

        /// <summary>
        /// Update Nodes CheckState recursively
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private CheckState UpdateNodesCheckState(TreeNodeCollection nodes)
        {
            CheckState result = CheckState.Unchecked;
            bool isFirstNode = true;
            bool isAllNodesSomeCheckState = true;

            foreach (TreeNodeItemSelector n in nodes.OfType<TreeNodeItemSelector>())
            {
                if (n.NodeType == TreeNodeItemSelector.CustomNodeType.SelectAll)
                    continue;

                if (n.Nodes.Count > 0)
                {
                    n.CheckState = UpdateNodesCheckState(n.Nodes);
                }

                if (isFirstNode)
                {
                    result = n.CheckState;
                    isFirstNode = false;
                }
                else
                {
                    if (result != n.CheckState)
                        isAllNodesSomeCheckState = false;
                }
            }

            if (isAllNodesSomeCheckState)
                return result;
            else
                return CheckState.Indeterminate;
        }

        /// <summary>
        /// Get the SelectAll Node
        /// </summary>
        /// <returns></returns>
        private TreeNodeItemSelector GetSelectAllNode()
        {
            TreeNodeItemSelector result = null;
            int i = 0;
            foreach (TreeNodeItemSelector n in ChecklistNodes().OfType<TreeNodeItemSelector>())
            {
                if (n.NodeType == TreeNodeItemSelector.CustomNodeType.SelectAll)
                {
                    result = n;
                    break;
                }
                else if (i > 2)
                    break;
                else
                    i++;
            }

            return result;
        }

        /// <summary>
        /// Get the SelectEmpty Node
        /// </summary>
        /// <returns></returns>
        private TreeNodeItemSelector GetSelectEmptyNode()
        {
            TreeNodeItemSelector result = null;
            int i = 0;
            foreach (TreeNodeItemSelector n in ChecklistNodes().OfType<TreeNodeItemSelector>())
            {
                if (n.NodeType == TreeNodeItemSelector.CustomNodeType.SelectEmpty)
                {
                    result = n;
                    break;
                }
                else if (i > 2)
                    break;
                else
                    i++;
            }

            return result;
        }

        /// <summary>
        /// Duplicate Nodes
        /// </summary>
        private static TreeNodeItemSelector[] DuplicateNodes(TreeNodeItemSelector[] nodes)
        {
            TreeNodeItemSelector[] ret = new TreeNodeItemSelector[nodes.Length];
            int i = 0;
            foreach (TreeNodeItemSelector n in nodes)
            {
                ret[i] = n.Clone();
                i++;
            }
            return ret;
        }

        #endregion


        #region checklist filter events

        /// <summary>
        /// CheckList NodeMouseClick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckList_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeViewHitTestInfo HitTestInfo = checkList.HitTest(e.X, e.Y);
            if (HitTestInfo != null && HitTestInfo.Location == TreeViewHitTestLocations.StateImage)
            {
                //check the node check status
                NodeCheckChange(e.Node as TreeNodeItemSelector);
                //set filter button enabled
                CheckFilterButtonEnabled();
            }
        }

        /// <summary>
        /// CheckList KeyDown event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                //check the node check status
                NodeCheckChange(checkList.SelectedNode as TreeNodeItemSelector);
                //set filter button enabled
                CheckFilterButtonEnabled();
            }
        }

        /// <summary>
        /// CheckList NodeMouseDoubleClick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckList_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNodeItemSelector n = e.Node as TreeNodeItemSelector;
            //set the new node check status
            SetNodesCheckState(_loadedNodes, false);
            n.CheckState = CheckState.Unchecked;
            NodeCheckChange(n);
            //set filter button enabled
            CheckFilterButtonEnabled();
            //do Filter by checkList
            Button_ok_Click(this, new EventArgs());
        }

        /// <summary>
        /// CheckList MouseEnter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckList_MouseEnter(object sender, EventArgs e)
        {
            checkList.Focus();
        }

        /// <summary>
        /// CheckList MouseLeave envet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckList_MouseLeave(object sender, EventArgs e)
        {
            Focus();
        }

        /// <summary>
        /// Set the Filter by checkList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ok_Click(object sender, EventArgs e)
        {
            _filterclick = true;

            SetCheckListFilter();
            Close();
        }

        /// <summary>
        /// Undo changed by checkList 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_cancel_Click(object sender, EventArgs e)
        {
            _loadedNodes = DuplicateNodes(_startingNodes);
            Close();
        }

        #endregion


        #region filter methods

        /// <summary>
        /// UnCheck all Custom Filter presets
        /// </summary>
        private void UnCheckCustomFilters()
        {
            for (int i = 2; i < customFilterLastFiltersListMenuItem.DropDownItems.Count; i++)
            {
                (customFilterLastFiltersListMenuItem.DropDownItems[i] as ToolStripMenuItem).Checked = false;
            }
        }

        /// <summary>
        /// Set a Custom Filter
        /// </summary>
        /// <param name="filtersMenuItemIndex"></param>
        private void SetCustomFilter(int filtersMenuItemIndex)
        {
            if (_activeFilterType == FilterType.CheckList)
                SetNodesCheckState(_loadedNodes, false);

            string filterstring = customFilterLastFiltersListMenuItem.DropDownItems[filtersMenuItemIndex].Tag.ToString();
            string viewfilterstring = customFilterLastFiltersListMenuItem.DropDownItems[filtersMenuItemIndex].Text;

            //do preset jobs
            if (filtersMenuItemIndex != 2)
            {
                for (int i = filtersMenuItemIndex; i > 2; i--)
                {
                    customFilterLastFiltersListMenuItem.DropDownItems[i].Text = customFilterLastFiltersListMenuItem.DropDownItems[i - 1].Text;
                    customFilterLastFiltersListMenuItem.DropDownItems[i].Tag = customFilterLastFiltersListMenuItem.DropDownItems[i - 1].Tag;
                }

                customFilterLastFiltersListMenuItem.DropDownItems[2].Text = viewfilterstring;
                customFilterLastFiltersListMenuItem.DropDownItems[2].Tag = filterstring;
            }

            //uncheck other preset
            for (int i = 3; i < customFilterLastFiltersListMenuItem.DropDownItems.Count; i++)
            {
                (customFilterLastFiltersListMenuItem.DropDownItems[i] as ToolStripMenuItem).Checked = false;
            }

            (customFilterLastFiltersListMenuItem.DropDownItems[2] as ToolStripMenuItem).Checked = true;
            _activeFilterType = FilterType.Custom;

            //get Filter string
            string oldfilter = FilterString;
            FilterString = filterstring;

            //set CheckList nodes
            SetNodesCheckState(_loadedNodes, false);

            customFilterLastFiltersListMenuItem.Checked = true;
            button_filter.Enabled = false;

            //fire Filter changed
            if (oldfilter != FilterString && FilterChanged != null)
                FilterChanged(this, new EventArgs());
        }

        #endregion


        #region filter events

        /// <summary>
        /// Cancel Filter Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelFilterMenuItem_Click(object sender, EventArgs e)
        {
            string oldfilter = FilterString;

            //clean Filter
            CleanFilter();

            //fire Filter changed
            if (oldfilter != FilterString && FilterChanged != null)
                FilterChanged(this, new EventArgs());
        }

        /// <summary>
        /// Cancel Filter MouseEnter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelFilterMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Enabled)
                (sender as ToolStripMenuItem).Select();
        }

        /// <summary>
        /// Custom Filter Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomFilterMenuItem_Click(object sender, EventArgs e)
        {
            //ignore image nodes
            if (DataType == typeof(Bitmap))
                return;

            //open a new Custom filter window
            FormCustomFilter flt = new FormCustomFilter(DataType, IsFilterDateAndTimeEnabled);

            if (flt.ShowDialog() == DialogResult.OK)
            {
                //add the new Filter presets

                string filterString = flt.FilterString;
                string viewFilterString = flt.FilterStringDescription;

                int index = -1;

                for (int i = 2; i < customFilterLastFiltersListMenuItem.DropDownItems.Count; i++)
                {
                    if (customFilterLastFiltersListMenuItem.DropDown.Items[i].Available)
                    {
                        if (customFilterLastFiltersListMenuItem.DropDownItems[i].Text == viewFilterString && customFilterLastFiltersListMenuItem.DropDownItems[i].Tag.ToString() == filterString)
                        {
                            index = i;
                            break;
                        }
                    }
                    else
                        break;
                }

                if (index < 2)
                {
                    for (int i = customFilterLastFiltersListMenuItem.DropDownItems.Count - 2; i > 1; i--)
                    {
                        if (customFilterLastFiltersListMenuItem.DropDownItems[i].Available)
                        {
                            customFilterLastFiltersListMenuItem.DropDownItems[i + 1].Text = customFilterLastFiltersListMenuItem.DropDownItems[i].Text;
                            customFilterLastFiltersListMenuItem.DropDownItems[i + 1].Tag = customFilterLastFiltersListMenuItem.DropDownItems[i].Tag;
                        }
                    }
                    index = 2;

                    customFilterLastFiltersListMenuItem.DropDownItems[2].Text = viewFilterString;
                    customFilterLastFiltersListMenuItem.DropDownItems[2].Tag = filterString;
                }

                //set the Custom Filter
                SetCustomFilter(index);
            }
        }

        /// <summary>
        /// Custom Filter preset MouseEnter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomFilterLastFiltersListMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Enabled)
                (sender as ToolStripMenuItem).Select();
        }

        /// <summary>
        /// Custom Filter preset MouseEnter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomFilterLastFiltersListMenuItem_Paint(Object sender, PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(customFilterLastFiltersListMenuItem.Width - 12, 7, 10, 10);
            ControlPaint.DrawMenuGlyph(e.Graphics, rect, MenuGlyph.Arrow, Color.Black, Color.Transparent);
        }

        /// <summary>
        /// Custom Filter preset 1 Visibility changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomFilterLastFilter1MenuItem_VisibleChanged(object sender, EventArgs e)
        {
            toolStripSeparator2MenuItem.Visible = !customFilterLastFilter1MenuItem.Visible;
            (sender as ToolStripMenuItem).VisibleChanged -= CustomFilterLastFilter1MenuItem_VisibleChanged;
        }

        /// <summary>
        /// Custom Filter preset Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomFilterLastFilterMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuitem = sender as ToolStripMenuItem;

            for (int i = 2; i < customFilterLastFiltersListMenuItem.DropDownItems.Count; i++)
            {
                if (customFilterLastFiltersListMenuItem.DropDownItems[i].Text == menuitem.Text && customFilterLastFiltersListMenuItem.DropDownItems[i].Tag.ToString() == menuitem.Tag.ToString())
                {
                    //set current filter preset as active
                    SetCustomFilter(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Custom Filter preset TextChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomFilterLastFilterMenuItem_TextChanged(object sender, EventArgs e)
        {
            (sender as ToolStripMenuItem).Available = true;
            (sender as ToolStripMenuItem).TextChanged -= CustomFilterLastFilterMenuItem_TextChanged;
        }

        /// <summary>
        /// Text changed timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckTextFilterTextChangedTimer_Tick(object sender, EventArgs e)
        {
            Timer timer = sender as Timer;
            if (timer == null)
                return;

            CheckTextFilterHandleTextChanged(timer.Tag.ToString());

            timer.Stop();
        }

        /// <summary>
        /// Check list filter changer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckTextFilter_TextChanged(object sender, EventArgs e)
        {
            if (!_checkTextFilterChangedEnabled)
                return;

            if (_textFilterTextChangedDelayNodes != TextFilterTextChangedDelayNodesDisabled && _loadedNodes.Length > _textFilterTextChangedDelayNodes)
            {
                if (_textFilterTextChangedTimer == null)
                {
                    _textFilterTextChangedTimer = new Timer();
                    _textFilterTextChangedTimer.Tick += new EventHandler(this.CheckTextFilterTextChangedTimer_Tick);
                }
                _textFilterTextChangedTimer.Stop();
                _textFilterTextChangedTimer.Interval = _textFilterTextChangedDelayMs;
                _textFilterTextChangedTimer.Tag = checkTextFilter.Text.ToLower();
                _textFilterTextChangedTimer.Start();
            }
            else
            {
                CheckTextFilterHandleTextChanged(checkTextFilter.Text.ToLower());
            }
        }

        /// <summary>
        /// Handle check filter text changed
        /// </summary>
        /// <param name="text"></param>
        private void CheckTextFilterHandleTextChanged(string text)
        {
            TreeNodeItemSelector allnode = TreeNodeItemSelector.CreateNode(AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVNodeSelectAll.ToString()] + "            ", null, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.SelectAll);
            TreeNodeItemSelector nullnode = TreeNodeItemSelector.CreateNode(AdvancedDataGridView.Translations[AdvancedDataGridView.TranslationKey.ADGVNodeSelectEmpty.ToString()] + "               ", null, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.SelectEmpty);
            string[] removednodesText = new string[] { };
            if (_checkTextFilterRemoveNodesOnSearch)
            {
                removednodesText = _removedsessionNodes.Where(r => !String.IsNullOrEmpty(r.Text)).Select(r => r.Text.ToLower()).Distinct().ToArray();
            }
            for (int i = _loadedNodes.Length - 1; i >= 0; i--)
            {
                TreeNodeItemSelector node = _loadedNodes[i];
                if (node.Text == allnode.Text)
                {
                    node.CheckState = CheckState.Indeterminate;
                }
                else if (node.Text == nullnode.Text)
                {
                    node.CheckState = CheckState.Unchecked;
                }
                else
                {
                    if (node.Text.ToLower().Contains(text))
                        node.CheckState = CheckState.Unchecked;
                    else
                        node.CheckState = CheckState.Checked;
                    if (removednodesText.Contains(node.Text.ToLower()))
                        node.CheckState = CheckState.Checked;
                    NodeCheckChange(node);
                }
            }
            //set filter button enabled
            CheckFilterButtonEnabled();
            _removedNodes = _removedsessionNodes;
            if (_checkTextFilterRemoveNodesOnSearch)
            {
                for (int i = _loadedNodes.Length - 1; i >= 0; i--)
                {
                    TreeNodeItemSelector node = _loadedNodes[i];
                    if (!(node.Text == allnode.Text || node.Text == nullnode.Text))
                    {
                        if (!node.Text.ToLower().Contains(text))
                        {
                            _removedNodes = _removedNodes.Concat(new TreeNodeItemSelector[] { node }).ToArray();
                        }
                    }
                }
                ChecklistReloadNodes();
            }
        }

        #endregion


        #region sort events

        /// <summary>
        /// Sort ASC Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SortASCMenuItem_Click(object sender, EventArgs e)
        {
            //ignore image nodes
            if (DataType == typeof(Bitmap))
                return;

            sortASCMenuItem.Checked = true;
            sortDESCMenuItem.Checked = false;
            _activeSortType = SortType.ASC;

            //get Sort String
            string oldsort = SortString;
            SortString = "[{0}] ASC";

            //fire Sort Changed
            if (oldsort != SortString && SortChanged != null)
                SortChanged(this, new EventArgs());
        }

        /// <summary>
        /// Sort ASC MouseEnter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SortASCMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Enabled)
                (sender as ToolStripMenuItem).Select();
        }

        /// <summary>
        /// Sort DESC Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SortDESCMenuItem_Click(object sender, EventArgs e)
        {
            //ignore image nodes
            if (DataType == typeof(Bitmap))
                return;

            sortASCMenuItem.Checked = false;
            sortDESCMenuItem.Checked = true;
            _activeSortType = SortType.DESC;

            //get Sort String
            string oldsort = SortString;
            SortString = "[{0}] DESC";

            //fire Sort Changed
            if (oldsort != SortString && SortChanged != null)
                SortChanged(this, new EventArgs());
        }

        /// <summary>
        /// Sort DESC MouseEnter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SortDESCMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Enabled)
                (sender as ToolStripMenuItem).Select();
        }

        /// <summary>
        /// Cancel Sort Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelSortMenuItem_Click(object sender, EventArgs e)
        {
            string oldsort = SortString;
            //clean Sort
            CleanSort();
            //fire Sort changed
            if (oldsort != SortString && SortChanged != null)
                SortChanged(this, new EventArgs());
        }

        /// <summary>
        /// Cancel Sort MouseEnter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelSortMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Enabled)
                (sender as ToolStripMenuItem).Select();
        }

        #endregion


        #region resize methods

        /// <summary>
        /// Get the scaling factor
        /// </summary>
        /// <returns></returns>
        private float GetScalingFactor()
        {
            float ret = 1;
            using (Graphics Gscale = this.CreateGraphics())
            {
                try
                {
                    ret = Gscale.DpiX / 96.0F;
                }
                catch { };
            }
            return ret;
        }

        /// <summary>
        /// Scale an item
        /// </summary>
        /// <param name="dimesion"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        private static int Scale(int dimesion, float factor)
        {
            return (int)Math.Floor(dimesion * factor);
        }

        /// <summary>
        /// Resize the box
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// 
        private void ResizeBox(int w, int h)
        {
            sortASCMenuItem.Width = w - 1;
            sortDESCMenuItem.Width = w - 1;
            cancelSortMenuItem.Width = w - 1; ;
            cancelFilterMenuItem.Width = w - 1;
            customFilterMenuItem.Width = w - 1;
            customFilterLastFiltersListMenuItem.Width = w - 1;
            checkTextFilterControlHost.Width = w - 35;

            //scale objects using original width and height
            float scalingfactor = GetScalingFactor();
            int w2 = (int)Math.Round(w / scalingfactor, 0);
            int h2 = (int)Math.Round(h / scalingfactor, 0);
            checkFilterListControlHost.Size = new Size(Scale(w2 - 35, scalingfactor), Scale(h2 - 160 - 25, scalingfactor));
            checkFilterListPanel.Size = checkFilterListControlHost.Size;
            checkList.Bounds = new Rectangle(Scale(4, scalingfactor), Scale(4, scalingfactor), Scale(w2 - 35 - 8, scalingfactor), Scale(h2 - 160 - 25 - 8, scalingfactor));
            checkFilterListButtonsControlHost.Size = new Size(Scale(w2 - 35, scalingfactor), Scale(24, scalingfactor));
            button_filter.Location = new Point(Scale(w2 - 35 - 164, scalingfactor), 0);
            button_undofilter.Location = new Point(Scale(w2 - 35 - 79, scalingfactor), 0);
            resizeBoxControlHost.Margin = new Padding(Scale(w2 - 46, scalingfactor), 0, 0, 0);

            //get all objects height to make sure we have room for the grip
            int finalHeight =
                sortASCMenuItem.Height +
                sortDESCMenuItem.Height +
                cancelSortMenuItem.Height +
                cancelFilterMenuItem.Height +
                toolStripSeparator1MenuItem.Height +
                toolStripSeparator2MenuItem.Height +
                customFilterLastFiltersListMenuItem.Height +
                toolStripSeparator3MenuItem.Height +
                checkFilterListControlHost.Height +
                checkTextFilterControlHost.Height +
                checkFilterListButtonsControlHost.Height +
                resizeBoxControlHost.Height;

            // apply the needed height only when scaled
            if (scalingfactor == 1)
                Size = new Size(w, h);
            else
                Size = new Size(w, h + (finalHeight - h < 0 ? 0 : finalHeight - h));

        }

        /// <summary>
        /// Clean box for Resize
        /// </summary>
        private void ResizeClean()
        {
            if (_resizeEndPoint.X != -1)
            {
                Point startPoint = PointToScreen(MenuStrip._resizeStartPoint);

                Rectangle rc = new Rectangle(startPoint.X, startPoint.Y, _resizeEndPoint.X, _resizeEndPoint.Y)
                {
                    X = Math.Min(startPoint.X, _resizeEndPoint.X),
                    Width = Math.Abs(startPoint.X - _resizeEndPoint.X),

                    Y = Math.Min(startPoint.Y, _resizeEndPoint.Y),
                    Height = Math.Abs(startPoint.Y - _resizeEndPoint.Y)
                };

                ControlPaint.DrawReversibleFrame(rc, Color.Black, FrameStyle.Dashed);

                _resizeEndPoint.X = -1;
            }
        }

        #endregion


        #region resize events

        /// <summary>
        /// Resize MouseDown event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeBoxControlHost_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ResizeClean();
            }
        }

        /// <summary>
        /// Resize MouseMove event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeBoxControlHost_MouseMove(object sender, MouseEventArgs e)
        {
            if (Visible)
            {
                if (e.Button == MouseButtons.Left)
                {
                    int x = e.X;
                    int y = e.Y;

                    ResizeClean();

                    x += Width - resizeBoxControlHost.Width;
                    y += Height - resizeBoxControlHost.Height;

                    x = Math.Max(x, MinimumSize.Width - 1);
                    y = Math.Max(y, MinimumSize.Height - 1);

                    Point StartPoint = PointToScreen(MenuStrip._resizeStartPoint);
                    Point EndPoint = PointToScreen(new Point(x, y));

                    Rectangle rc = new Rectangle
                    {
                        X = Math.Min(StartPoint.X, EndPoint.X),
                        Width = Math.Abs(StartPoint.X - EndPoint.X),

                        Y = Math.Min(StartPoint.Y, EndPoint.Y),
                        Height = Math.Abs(StartPoint.Y - EndPoint.Y)
                    };

                    ControlPaint.DrawReversibleFrame(rc, Color.Black, FrameStyle.Dashed);

                    _resizeEndPoint.X = EndPoint.X;
                    _resizeEndPoint.Y = EndPoint.Y;
                }
            }
        }

        /// <summary>
        /// Resize MouseUp event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeBoxControlHost_MouseUp(object sender, MouseEventArgs e)
        {
            if (_resizeEndPoint.X != -1)
            {
                ResizeClean();

                if (Visible)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        int newWidth = e.X + Width - resizeBoxControlHost.Width;
                        int newHeight = e.Y + Height - resizeBoxControlHost.Height;

                        newWidth = Math.Max(newWidth, MinimumSize.Width);
                        newHeight = Math.Max(newHeight, MinimumSize.Height);

                        ResizeBox(newWidth, newHeight);
                    }
                }
            }
        }

        /// <summary>
        /// Resize Paint event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeBoxControlHost_Paint(Object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Properties.Resources.MenuStrip_ResizeGrip, 0, 0);
        }

        #endregion

    }
}