using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Zuby.ADGV;

namespace AdvancedDataGridViewSample
{
    public partial class FormMain : Form
    {
        private DataTable _dataTable = null;
        private DataSet _dataSet = null;

        private SortedDictionary<int, string> _filtersaved = new SortedDictionary<int, string>();
        private SortedDictionary<int, string> _sortsaved = new SortedDictionary<int, string>();

        private bool _testtranslations = false;
        private bool _testtranslationsFromFile = false;

        private static int DisplayItemsCounter = 100;

        private static bool MemoryTestEnabled = true;
        private const int MemoryTestFormsNum = 100;
        private bool _memorytest = false;
        private object[][] _inrows = new object[][] { };

        private Timer _memorytestclosetimer = null;
        private Timer _timermemoryusage = null;

        private static bool CollectGarbageOnTimerMemoryUsageUpdate = true;

        public FormMain()
        {
            InitializeComponent();

            //set timers
            _memorytestclosetimer = new Timer(components)
            {
                Interval = 10
            };
            _timermemoryusage = new Timer(components)
            {
                Interval = 2000
            };

            //trigger the memory usage show
            _timermemoryusage_Tick(null, null);

            //set localization strings
            Dictionary<string, string> translations = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> translation in AdvancedDataGridView.Translations)
            {
                if (!translations.ContainsKey(translation.Key))
                    translations.Add(translation.Key, "." + translation.Value);
            }
            foreach (KeyValuePair<string, string> translation in AdvancedDataGridViewSearchToolBar.Translations)
            {
                if (!translations.ContainsKey(translation.Key))
                    translations.Add(translation.Key, "." + translation.Value);
            }
            if (_testtranslations)
            {
                AdvancedDataGridView.SetTranslations(translations);
                AdvancedDataGridViewSearchToolBar.SetTranslations(translations);
            }
            if (_testtranslationsFromFile)
            {
                AdvancedDataGridView.SetTranslations(AdvancedDataGridView.LoadTranslationsFromFile("lang.json"));
                AdvancedDataGridViewSearchToolBar.SetTranslations(AdvancedDataGridViewSearchToolBar.LoadTranslationsFromFile("lang.json"));
            }

            //set filter and sort saved
            _filtersaved.Add(0, "");
            _sortsaved.Add(0, "");
            comboBox_filtersaved.DataSource = new BindingSource(_filtersaved, null);
            comboBox_filtersaved.DisplayMember = "Key";
            comboBox_filtersaved.ValueMember = "Value";
            comboBox_filtersaved.SelectedIndex = -1;
            comboBox_sortsaved.DataSource = new BindingSource(_sortsaved, null);
            comboBox_sortsaved.DisplayMember = "Key";
            comboBox_sortsaved.ValueMember = "Value";
            comboBox_sortsaved.SelectedIndex = -1;

            //set memory test button
            button_memorytest.Enabled = MemoryTestEnabled;

            //initialize dataset
            _dataTable = new DataTable();
            _dataSet = new DataSet();

            //initialize bindingsource
            bindingSource_main.DataSource = _dataSet;

            //initialize datagridview
            advancedDataGridView_main.SetDoubleBuffered();
            advancedDataGridView_main.DataSource = bindingSource_main;

            //set bindingsource
            SetTestData();
        }

        public FormMain(bool memorytest, object[][] inrows)
            : this()
        {
            _memorytest = memorytest;
            _inrows = inrows;
        }

        private void button_load_Click(object sender, EventArgs e)
        {
            //add test data to bindsource
            AddTestData();
        }

        private void SetTestData()
        {
            _dataTable = _dataSet.Tables.Add("TableTest");
            _dataTable.Columns.Add("int", typeof(int));
            _dataTable.Columns.Add("decimal", typeof(decimal));
            _dataTable.Columns.Add("double", typeof(double));
            _dataTable.Columns.Add("date", typeof(DateTime));
            _dataTable.Columns.Add("datetime", typeof(DateTime));
            _dataTable.Columns.Add("string", typeof(string));
            _dataTable.Columns.Add("boolean", typeof(bool));
            _dataTable.Columns.Add("guid", typeof(Guid));
            _dataTable.Columns.Add("image", typeof(Bitmap));
            _dataTable.Columns.Add("timespan", typeof(TimeSpan));

            bindingSource_main.DataMember = _dataTable.TableName;

            advancedDataGridViewSearchToolBar_main.SetColumns(advancedDataGridView_main.Columns);
        }

        private void AddTestData()
        {
            Random r = new Random();
            Image[] sampleimages = new Image[2];
            sampleimages[0] = Image.FromFile(Path.Combine(Application.StartupPath, "flag-green_24.png"));
            sampleimages[1] = Image.FromFile(Path.Combine(Application.StartupPath, "flag-red_24.png"));

            int maxMinutes = (int)((TimeSpan.FromHours(20) - TimeSpan.FromHours(10)).TotalMinutes);

            if (_inrows.Length == 0)
            {
                for (int i = 0; i < DisplayItemsCounter; i++)
                {
                    object[] newrow = new object[] {
                        i,
                        Math.Round((decimal)i*2/3, 6),
                        Math.Round(i % 2 == 0 ? (double)i*2/3 : (double)i/2, 6),
                        DateTime.Today.AddHours(i*2).AddHours(i%2 == 0 ?i*10+1:0).AddMinutes(i%2 == 0 ?i*10+1:0).AddSeconds(i%2 == 0 ?i*10+1:0).AddMilliseconds(i%2 == 0 ?i*10+1:0).Date,
                        DateTime.Today.AddHours(i*2).AddHours(i%2 == 0 ?i*10+1:0).AddMinutes(i%2 == 0 ?i*10+1:0).AddSeconds(i%2 == 0 ?i*10+1:0).AddMilliseconds(i%2 == 0 ?i*10+1:0),
                        i*2 % 3 == 0 ? null : i.ToString()+" str",
                        i % 2 == 0 ? true:false,
                        Guid.NewGuid(),
                        sampleimages[r.Next(0, 2)],
                        TimeSpan.FromHours(10).Add(TimeSpan.FromMinutes(r.Next(maxMinutes)))
                    };

                    _dataTable.Rows.Add(newrow);
                }
            }
            else
            {
                for (int i = 0; i < _inrows.Length; i++)
                {
                    _dataTable.Rows.Add(_inrows[i]);
                }
            }

        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //add test data to bindsource
            AddTestData();

            //setup datagridview
            advancedDataGridView_main.SetFilterDateAndTimeEnabled(advancedDataGridView_main.Columns["datetime"], true);
            advancedDataGridView_main.SetSortEnabled(advancedDataGridView_main.Columns["guid"], false);
            advancedDataGridView_main.SetFilterChecklistEnabled(advancedDataGridView_main.Columns["guid"], false);
            advancedDataGridView_main.SortASC(advancedDataGridView_main.Columns["datetime"]);
            advancedDataGridView_main.SortDESC(advancedDataGridView_main.Columns["double"]);
            advancedDataGridView_main.SetTextFilterRemoveNodesOnSearch(advancedDataGridView_main.Columns["double"], false);
            advancedDataGridView_main.SetChecklistTextFilterRemoveNodesOnSearchMode(advancedDataGridView_main.Columns["decimal"], false);
            advancedDataGridView_main.SetFilterChecklistEnabled(advancedDataGridView_main.Columns["double"], false);
            advancedDataGridView_main.SetFilterCustomEnabled(advancedDataGridView_main.Columns["timespan"], false);
            advancedDataGridView_main.CleanSort(advancedDataGridView_main.Columns["datetime"]);
            advancedDataGridView_main.SetFilterChecklistTextFilterTextChangedDelayNodes(advancedDataGridView_main.Columns["string"], 10);
            advancedDataGridView_main.SetFilterChecklistTextFilterTextChangedDelayMs(advancedDataGridView_main.Columns["string"], 500);

            //memory test
            if (!_memorytest)
            {
                //set timer memory usage
                _timermemoryusage.Enabled = true;
                _timermemoryusage.Tick += _timermemoryusage_Tick;
            }
            else
            {
                panel_top.Visible = false;

                _memorytestclosetimer.Enabled = true;
                _memorytestclosetimer.Tick += _memorytestclosetimer_Tick;

                foreach (DataGridViewColumn column in advancedDataGridView_main.Columns)
                    advancedDataGridView_main.ShowMenuStrip(column);
            }
        }

        private void advancedDataGridView_main_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e)
        {
            //eventually set the FilterString here
            //if e.Cancel is set to true one have to update the datasource here using
            //bindingSource_main.Filter = advancedDataGridView_main.FilterString;
            //otherwise it will be updated by the component

            //sample use of the override string filter
            string stringcolumnfilter = textBox_strfilter.Text;
            if (!String.IsNullOrEmpty(stringcolumnfilter))
                e.FilterString += (!String.IsNullOrEmpty(e.FilterString) ? " AND " : "") + String.Format("string LIKE '%{0}%'", stringcolumnfilter.Replace("'", "''"));

            textBox_filter.Text = e.FilterString;
        }

        private void advancedDataGridView_main_SortStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.SortEventArgs e)
        {
            //eventually set the SortString here
            //if e.Cancel is set to true one have to update the datasource here
            //bindingSource_main.Sort = advancedDataGridView_main.SortString;
            //otherwise it will be updated by the component

            textBox_sort.Text = e.SortString;
        }

        private void textBox_strfilter_TextChanged(object sender, EventArgs e)
        {
            //trigger the filter string changed function when text is changed
            advancedDataGridView_main.TriggerFilterStringChanged();
        }

        private void bindingSource_main_ListChanged(object sender, ListChangedEventArgs e)
        {
            textBox_total.Text = bindingSource_main.List.Count.ToString();
        }

        private void button_savefilters_Click(object sender, EventArgs e)
        {
            _filtersaved.Add((comboBox_filtersaved.Items.Count - 1) + 1, advancedDataGridView_main.FilterString);
            comboBox_filtersaved.DataSource = new BindingSource(_filtersaved, null);
            comboBox_filtersaved.SelectedIndex = comboBox_filtersaved.Items.Count - 1;
            _sortsaved.Add((comboBox_sortsaved.Items.Count - 1) + 1, advancedDataGridView_main.SortString);
            comboBox_sortsaved.DataSource = new BindingSource(_sortsaved, null);
            comboBox_sortsaved.SelectedIndex = comboBox_sortsaved.Items.Count - 1;
        }

        private void button_setsavedfilter_Click(object sender, EventArgs e)
        {
            if (comboBox_filtersaved.SelectedIndex != -1 && comboBox_sortsaved.SelectedIndex != -1)
                advancedDataGridView_main.LoadFilterAndSort(comboBox_filtersaved.SelectedValue.ToString(), comboBox_sortsaved.SelectedValue.ToString());
        }

        private void button_unloadfilters_Click(object sender, EventArgs e)
        {
            advancedDataGridView_main.CleanFilterAndSort();
            comboBox_filtersaved.SelectedIndex = -1;
            comboBox_sortsaved.SelectedIndex = -1;
        }

        private void advancedDataGridViewSearchToolBar_main_Search(object sender, Zuby.ADGV.AdvancedDataGridViewSearchToolBarSearchEventArgs e)
        {
            bool restartsearch = true;
            int startColumn = 0;
            int startRow = 0;
            if (!e.FromBegin)
            {
                bool endcol = advancedDataGridView_main.CurrentCell.ColumnIndex + 1 >= advancedDataGridView_main.ColumnCount;
                bool endrow = advancedDataGridView_main.CurrentCell.RowIndex + 1 >= advancedDataGridView_main.RowCount;

                if (endcol && endrow)
                {
                    startColumn = advancedDataGridView_main.CurrentCell.ColumnIndex;
                    startRow = advancedDataGridView_main.CurrentCell.RowIndex;
                }
                else
                {
                    startColumn = endcol ? 0 : advancedDataGridView_main.CurrentCell.ColumnIndex + 1;
                    startRow = advancedDataGridView_main.CurrentCell.RowIndex + (endcol ? 1 : 0);
                }
            }
            DataGridViewCell c = advancedDataGridView_main.FindCell(
                e.ValueToSearch,
                e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
                startRow,
                startColumn,
                e.WholeWord,
                e.CaseSensitive);
            if (c == null && restartsearch)
                c = advancedDataGridView_main.FindCell(
                    e.ValueToSearch,
                    e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
                    0,
                    0,
                    e.WholeWord,
                    e.CaseSensitive);
            if (c != null)
                advancedDataGridView_main.CurrentCell = c;
        }


        private void _timermemoryusage_Tick(object sender, EventArgs e)
        {
            if (CollectGarbageOnTimerMemoryUsageUpdate)
                GC.Collect();
            toolStripStatusLabel_memory.Text = String.Format("Memory Usage: {0}Mb", GC.GetTotalMemory(false) / (1024 * 1024));
        }

        private void button_memorytest_Click(object sender, EventArgs e)
        {
            //build random data
            Random r = new Random();
            Image[] sampleimages = new Image[2];
            sampleimages[0] = Image.FromFile(Path.Combine(Application.StartupPath, "flag-green_24.png"));
            sampleimages[1] = Image.FromFile(Path.Combine(Application.StartupPath, "flag-red_24.png"));
            int maxMinutes = (int)((TimeSpan.FromHours(20) - TimeSpan.FromHours(10)).TotalMinutes);
            object[][] testrows = new object[100][];
            for (int i = 0; i < 100; i++)
            {
                object[] newrow = new object[] {
                        i,
                        Math.Round((decimal)i*2/3, 6),
                        Math.Round(i % 2 == 0 ? (double)i*2/3 : (double)i/2, 6),
                        DateTime.Today.AddHours(i*2).AddHours(i%2 == 0 ?i*10+1:0).AddMinutes(i%2 == 0 ?i*10+1:0).AddSeconds(i%2 == 0 ?i*10+1:0).AddMilliseconds(i%2 == 0 ?i*10+1:0).Date,
                        DateTime.Today.AddHours(i*2).AddHours(i%2 == 0 ?i*10+1:0).AddMinutes(i%2 == 0 ?i*10+1:0).AddSeconds(i%2 == 0 ?i*10+1:0).AddMilliseconds(i%2 == 0 ?i*10+1:0),
                        i*2 % 3 == 0 ? null : i.ToString()+" str",
                        i % 2 == 0 ? true:false,
                        Guid.NewGuid(),
                        sampleimages[r.Next(0, 2)],
                        TimeSpan.FromHours(10).Add(TimeSpan.FromMinutes(r.Next(maxMinutes)))
                    };

                testrows.SetValue(newrow, i);
            }

            //show the forms
            for (int i = 0; i < MemoryTestFormsNum; i++)
            {
                FormMain formtest = new FormMain(true, testrows);
                formtest.Show();
                //wait for the form to be disposed
                while (!formtest.IsDisposed)
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        private void _memorytestclosetimer_Tick(object sender, EventArgs e)
        {
            _dataTable.Rows.Clear();
            this.Close();
        }
    }
}
