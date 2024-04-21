using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ElControls.ElEnumControl;
using ElControls.ElExportToExcel;
using ElControls.RadGridViewConfig;
using ElDataModels.TaskManager;
using ElMessage;
using Newtonsoft.Json;
using NLog;
using ServerLoadMonitoring.Helpers;
using ServerLoadMonitoringDataModels;
using ServerLoadMonitoringDataModels.Enums;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Calendar;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Persistence;
using Telerik.Windows.Controls.ChartView;
using Telerik.Windows.Data;
using System.Windows.Data;
using System.Globalization;
using System.Management.Automation.Runspaces;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using ValidationResult = System.Windows.Controls.ValidationResult;
using System.Threading;
using System.Windows.Media.TextFormatting;
using ServerLoadMonitoring.MetricControl;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;
//using System.Windows.Forms;

namespace ServerLoadMonitoring
{

    public class ServerLoadMonitoringDataViewModel : ViewModelBase
    {

        public RadGridView GridTask { get; set; }
        public JobsHeatmapControl HeatmapControl { get; set; }
        public ServerLoadMonitoringDataViewModel()
        {

            CommandGridToExcel = new DelegateCommand(OnGridToExcel);
            CommandGetTaskList = new DelegateCommand(OnGetTaskList);
            CommandClearFilterGrid = new DelegateCommand(OnClearFilterGrid);
            CommandToggleRightPanel = new DelegateCommand(OnToggleRightPanel);
            CommandCalendarToToday = new DelegateCommand(OnCalendarToToday);
            CommandCalendarFromToday = new DelegateCommand(OnCalendarFromToday);

            CommandCopyingCellClipboard = new DelegateCommand(OnCopyingCellClipboard);
            CommandCopying = new DelegateCommand(OnCopying);
            CommandClipboardMode = new DelegateCommand(OnClipboardMode);
            CommandRefreshData = new DelegateCommand(OnRefreshData);
            CommandGetMonitoringData = new DelegateCommand(OnGetMonitoringData);

            CommandUpdateListOfMetricsSource = new DelegateCommand(OnUpdateListOfMetricsSource);
            CommandUpdateReadyMetrics = new DelegateCommand(OnUpdateReadyMetrics);
            CommandReadReadyMetrics = new DelegateCommand(OnReadReadyMetrics);
            CommandAllMetricsProcceses = new DelegateCommand(OnAllMetricsProcceses);

            //Управление настройками грида ДЛЯ РАБОТЫ НАСТРОЕК В TAG RADGRIDVIEW ВНОСИМ НОВЫЙ GUID
            CommandSaveConfigGridView = new DelegateCommand(OnSaveConfigGridView);
            CommandLoadConfigGridView = new DelegateCommand(OnLoadConfigGridView);
            CommandResetConfigGridView = new DelegateCommand(OnResetConfigGridView);
            CommandChangeModeSaveConfig = new DelegateCommand(OnChangeModeSaveConfig);
            CommandLoadSettingControlOnOpen = new DelegateCommand(OnLoadSettingControlOnOpen);
            CommandSaveSettingControlOnClosed = new DelegateCommand(OnSaveSettingControlOnClosed);
            CommandGetTasksFromTaskManager = new DelegateCommand(OnGetTasksFromTaskManager);
            //***************************
            //CommandMouseDoubleClickGrid = new DelegateCommand(OnMouseDoubleClickGrid);

            TwoTabVisibility = Visibility.Collapsed;
            ActiveTab = 0;
            VisibilityRightPanel = Visibility.Collapsed;

            _selectedDateTo = DateTime.Today;
            _selectedDateFrom = DateTime.Today.AddDays(-1);

            //IsAllSubnetUnloading = false;
            //Mask = 24;
        }

        #region Properties

        private ObservableCollection<TaskCountItem> _TasksCount = new ObservableCollection<TaskCountItem>();
        public ObservableCollection<TaskCountItem> TasksCount
        {
            get => _TasksCount;
            set
            {
                _TasksCount = value;
                OnPropertyChanged("TasksCount");
            }
        }

        private ErrorCountItem _AllLogsErrorsCount = new ErrorCountItem();
        public ErrorCountItem AllLogsErrorsCount
        {
            get => _AllLogsErrorsCount;
            set
            {
                _AllLogsErrorsCount = value;
                OnPropertyChanged("AllLogsErrorsCount");
            }
        }

        private ObservableCollection<ErrorCountItem> _LogsErrorCounts = new ObservableCollection<ErrorCountItem>();
        public ObservableCollection<ErrorCountItem> LogsErrorCounts
        {
            get => _LogsErrorCounts;
            set
            {
                _LogsErrorCounts = value;
                OnPropertyChanged("LogsErrorCounts");
            }
        }

        private ObservableCollection<Metric> _MetricControlsList;
        public ObservableCollection<Metric> MetricControlsList
        {
            get => _MetricControlsList;
            set
            {
                _MetricControlsList = value;
                OnPropertyChanged("MetricControlsList");
            }
        }

        private ObservableCollection<MetricsControlConfig> _MetricsResources;
        public ObservableCollection<MetricsControlConfig> MetricsResources
        {
            get => _MetricsResources;
            set
            {
                _MetricsResources = value;
                OnPropertyChanged("MetricsResources");
            }
        }






        private bool tmpTest = true;
        private bool IpChecked = false;
        private DispatcherTimer timer;

        private void Timer_Tick(object sender, EventArgs e)
        {

            OnUpdateReadyMetrics(null);
            //ServerMetricsControl[0].GetMetrics();
            //OnReadReadyMetrics(null);
        }

        private Dictionary<string, int> _LastReadedMetricPlacement;
        public Dictionary<string, int> LastReadedMetricPlacement
        {
            get => _LastReadedMetricPlacement;
            set
            {
                _LastReadedMetricPlacement = value;
                OnPropertyChanged("LastReadedMetricPlacement");
            }
        }

        




        private ObservableCollection<ElTask> _ListServerLoadMonitoringsData;
        public ObservableCollection<ElTask> ListServerLoadMonitoringsData
        {
            get => _ListServerLoadMonitoringsData;
            set
            {
                _ListServerLoadMonitoringsData = value;
                OnPropertyChanged("ListServerLoadMonitoringsData");
            }
        }


        private BackupMonitorData _lastLoadedBackup { get; set; }


        private ElTask _SelectedServerLoadMonitoringData;

        public ElTask SelectedServerLoadMonitoringData
        {
            get
            {
                return _SelectedServerLoadMonitoringData;
            }
            set
            {
                _SelectedServerLoadMonitoringData = value;

                OnPropertyChanged("SelectedServerLoadMonitoringData");
            }
        }
        //ListServerLoadMonitoringsData
        //SelectedServerLoadMonitoringData

        private DateTime _selectedDateFrom;

        public DateTime SelectedDateFrom
        {
            get => _selectedDateFrom;
            set
            {
                _selectedDateFrom = value;
                if (_selectedDateTo < _selectedDateFrom) SelectedDateTo = value;
                OnPropertyChanged("SelectedDateFrom");
            }
        }




        private bool _clipboardMode;
        /// <summary>
        /// Режим работы буффера обмена грида        /// </summary>
        public bool ClipboardMode
        {
            get => _clipboardMode;
            set
            {
                _clipboardMode = value;
                OnPropertyChanged("ClipboardMode");
            }
        }


        private DateTime _selectedDateTo;

        public DateTime SelectedDateTo
        {
            get => _selectedDateTo;
            set
            {
                _selectedDateTo = value;
                if (_selectedDateTo < _selectedDateFrom) SelectedDateFrom = value;
                OnPropertyChanged("SelectedDateTo");
            }
        }
        public RadCalendar CalendarFrom { get; set; }
        public RadCalendar CalendarTo { get; set; }

        #endregion

        /// <summary>Обработка копирования строки грида</summary>

        #region GridSettings

        //Сохранение конфигурации грида
        public ICommand CommandSaveConfigGridView { get; set; }

        public void OnSaveConfigGridView(object obj)
        {
            if (obj is RadGridView grid)
            {
                try
                {
                    if (grid.Tag is RadGridViewAdditionalConfigurator config)
                    {
                        var appdataPath = ConfigPlugin.ELAppData;
                        if (!Directory.Exists(appdataPath + @"GridViewSettings")) return;

                        var manager = new PersistenceManager();

                        var mode = File.Exists(appdataPath + $@"GridViewSettings\{config.ConfigurationId}.auto") ? ".auto" : ".manu";

                        using (var fstream = new FileStream(appdataPath + $@"GridViewSettings\{config.ConfigurationId}{mode}", FileMode.Create))
                        {
                            manager.Save(grid).CopyTo(fstream);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
                }
            }
        }

        //Загрузка конфигурации грида
        public ICommand CommandLoadConfigGridView { get; set; }

        public void OnLoadConfigGridView(object obj)
        {
            if (obj is RadGridView grid)
            {
                try
                {
                    if (grid.Tag is RadGridViewAdditionalConfigurator config)
                    {
                        var appdataPath = ConfigPlugin.ELAppData;
                        if (!Directory.Exists(appdataPath + @"GridViewSettings")) return;

                        var mode = "";
                        if (File.Exists(appdataPath + $@"GridViewSettings\{config.ConfigurationId}.auto"))
                        {
                            mode = ".auto";
                            config.IsAutoSaveMode = true;
                        }
                        else
                        {
                            config.IsAutoSaveMode = false;
                            mode = ".manu";
                        }

                        if (!File.Exists(appdataPath + $@"GridViewSettings\{config.ConfigurationId}{mode}"))
                        {
                            //ErrorString = "Нет сохраненных конфигураций";
                            return;
                        }



                        using (FileStream fstream = new FileStream(@appdataPath + $@"GridViewSettings\{config.ConfigurationId}{mode}", FileMode.OpenOrCreate))
                        {
                            byte[] array = new byte[fstream.Length];
                            fstream.Read(array, 0, (int)fstream.Length);


                            Stream stream = new MemoryStream(array.Length);
                            stream.Write(array, 0, array.Length);
                            stream.Position = 0L;
                            PersistenceManager manager = new PersistenceManager();
                            manager.Load(grid, stream);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
                }
            }
        }

        //Сброс конфигурации грида (стандартная кнопка)
        public ICommand CommandResetConfigGridView { get; set; }

        public void OnResetConfigGridView(object obj)

        {
            try
            {
                if (obj is RadGridView grid)
                {
                    if (grid.Tag is RadGridViewAdditionalConfigurator config)
                    {
                        var appdataPath = ConfigPlugin.ELAppData;
                        if (!Directory.Exists(appdataPath + @"GridViewSettings")) return;
                        var isNotConfig = false;
                        if (File.Exists(appdataPath + $@"GridViewSettings\{config.ConfigurationId}.manu"))
                        {
                            File.Delete(appdataPath + $@"GridViewSettings\{config.ConfigurationId}.manu");
                            isNotConfig = true;
                        }
                        if (File.Exists(appdataPath + $@"GridViewSettings\{config.ConfigurationId}.auto"))
                        {
                            File.Delete(appdataPath + $@"GridViewSettings\{config.ConfigurationId}.auto");
                            isNotConfig = true;
                        }

                        // ErrorString = isNotConfig ? "Возврат к стандартным параметрам произойдет при следующем запуске" : "У текущего грида нет сохраненных конфигураций";
                    }
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }

        //Загрузка конфигурации при старте VM
        public ICommand CommandLoadSettingControlOnOpen { get; set; }

        public void OnLoadSettingControlOnOpen(object obj)
        {
            try
            {
                var appdataPath = ConfigPlugin.ELAppData;
                if (!Directory.Exists(appdataPath + @"GridViewSettings")) return;
                CommandLoadConfigGridView.Execute(obj);
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }

        //Загрузка сохранение конфигурации при закрытии

        public ICommand CommandSaveSettingControlOnClosed { get; set; }

        public void OnSaveSettingControlOnClosed(object obj)
        {
            try
            {
                var appdataPath = ConfigPlugin.ELAppData;
                if (!Directory.Exists(appdataPath + @"GridViewSettings")) return;

                if (obj is RadGridView grid)
                    if (grid.Tag is RadGridViewAdditionalConfigurator config)
                        if (File.Exists(appdataPath + $@"GridViewSettings\{config.ConfigurationId}.auto"))
                            CommandSaveConfigGridView.Execute(obj);
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }

        //Изменение режима сохранения настроек

        public ICommand CommandChangeModeSaveConfig { get; set; }

        public void OnChangeModeSaveConfig(object obj)
        {
            try
            {
                if (!(obj is RadGridView grid)) return;
                if (grid.Tag is RadGridViewAdditionalConfigurator config)
                {
                    var appdataPath = ConfigPlugin.ELAppData;
                    if (!Directory.Exists(appdataPath + @"GridViewSettings")) return;

                    if (!File.Exists(appdataPath + $@"GridViewSettings\{config.ConfigurationId}.auto"))
                    {
                        config.IsAutoSaveMode = true;
                        var manager = new PersistenceManager();
                        using (var fstream = new FileStream(appdataPath + $@"GridViewSettings\{config.ConfigurationId}.auto", FileMode.Create))
                        {
                            manager.Save(grid).CopyTo(fstream);
                        }
                    }
                    else
                    {
                        File.Delete(appdataPath + $@"GridViewSettings\{config.ConfigurationId}.auto");
                        config.IsAutoSaveMode = false;
                    }
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }


        #endregion


        #region COMMANDS
        Stopwatch stopwatch = new Stopwatch();
        public ICommand CommandAllMetricsProcceses { get; set; }
        bool timerr = true;
        private void OnAllMetricsProcceses(object obj)
        {
            try
            {

                if (timerr)
                {
                    this.timer = new DispatcherTimer();
                    this.timer.Interval = TimeSpan.FromSeconds(2);
                    this.timer.Tick += Timer_Tick;
                    this.timer.Start();
                    timerr = false;
                }
                MetricControlsList = new ObservableCollection<Metric>();
                foreach (var resource in MetricsResources)
                {
                    MetricControlsList.Add(new Metric(resource));
                }
                MetricControlsList.Last().LastControl();
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }

        public ICommand CommandReadReadyMetrics { get; set; }
        private void OnReadReadyMetrics(object obj)
        {
            try
            {
                ConfigPlugin.connectionElServer.SendMessage(
                        new ElMessageClient("ServerLoadMonitoring", "GetReadyMetrics", Response_CommandReadReadyMetrics),
                        JsonConvert.SerializeObject(new { LastReadedMetricPlacement = LastReadedMetricPlacement }));
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }
        public ICommand CommandUpdateListOfMetricsSource { get; set; }
        private void OnUpdateListOfMetricsSource(object obj)
        {
            try
            {
                OnPropertyChanged("ServerMetricsControl");
                ConfigPlugin.connectionElServer.SendMessage(
                    new ElMessageClient("ServerLoadMonitoring", "UpdateListOfMetricsSource", Response_CommandUpdateListOfMetricsSource),
                    null);

            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }
        public ICommand CommandUpdateReadyMetrics { get; set; }
        private void OnUpdateReadyMetrics(object obj)
        {
            try
            {
                ConfigPlugin.connectionElServer.SendMessage(
                    new ElMessageClient("ServerLoadMonitoring", "UpdateReadyMetrics", Response_CommandUpdateReadyMetrics), null);
                
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }

        public ICommand CommandGetTasksFromTaskManager { get; set; }
        private void OnGetTasksFromTaskManager(object obj)
        {
            try
            {
                ConfigPlugin.connectionElServer.SendMessage(
                    new ElMessageClient("ServerLoadMonitoring", "GetTasksFromTaskManager", Response_CommandGetTasksFromTaskManager), null);

            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }

        public ICommand CommandCopyingCellClipboard { get; set; }

        private void OnCopyingCellClipboard(object obj)
        {
            try
            {

                if (ClipboardMode) return;


                if (obj is GridViewCellClipboardEventArgs e) e.Cancel = true;
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }


        /// <summary>Копирование значения текущей ячейуи в буффер обмена</summary>
        public ICommand CommandCopying { get; set; }

        private void OnCopying(object obj)
        {
            try
            {

                if (ClipboardMode) return;

                if (obj is GridViewClipboardEventArgs e)
                {
                    e.Cancel = true;

                    if (e.OriginalSource is RadGridView grid)
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(grid.CurrentCell.Value.ToString().Trim());
                    }
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }

        /// <summary>Изменить режим копирования</summary>
        public ICommand CommandClipboardMode { get; set; }
        private void OnClipboardMode(object obj)
        {
            try
            {
                ClipboardMode = !ClipboardMode;
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }

        /// <summary>Изменить режим копирования</summary>
        public ICommand CommandRefreshData { get; set; }
        private void OnRefreshData(object obj)
        {
            try
            {
                if (ConfigPlugin.SecurityKey.HasFlag(EPermissions.IsRefreshDataEnabled))
                {
                    ConfigPlugin.connectionElServer.SendMessage(
                        new ElMessageClient("ServerLoadMonitoring", "RefreshData", Response_RefreshData),
                        JsonConvert.SerializeObject(new { ReportKey = ESettingsKeys.CurrentJobsStatus }));
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }

        /// <summary>Изменить режим копирования</summary>
        public ICommand CommandGetMonitoringData { get; set; }
        private void OnGetMonitoringData(object obj)
        {
            try
            {
                //if (ConfigPlugin.IsRefreshDataEnabled)
                //{
                ConfigPlugin.connectionElServer.SendMessage(
                    new ElMessageClient("ServerLoadMonitoring", "GetServerLoadMonitoringData", Response_GetMonitoringData),
                    JsonConvert.SerializeObject(new { ReportKey = ESettingsKeys.CurrentJobsStatus }));
                //}
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }


        /// <summary> Календарь до установить на сегодня </summary>
        public ICommand CommandCalendarToToday { get; set; }
        public void OnCalendarToToday(object obj)
        {
            try
            {
                CalendarTo.SelectedDate = DateTime.Today;
                CalendarTo.DisplayDate = DateTime.Today;
                CalendarTo.DisplayMode = DisplayMode.MonthView;
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }

        }
        /// <summary> Календарь от установить на сегодня </summary>
        public ICommand CommandCalendarFromToday { get; set; }
        public void OnCalendarFromToday(object obj)
        {
            try
            {
                CalendarFrom.SelectedDate = DateTime.Today;
                CalendarFrom.DisplayDate = DateTime.Today;
                CalendarFrom.DisplayMode = DisplayMode.MonthView;
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }

        }

        //комманда для выгрузки грида в эксэль
        public ICommand CommandGridToExcel { get; set; }
        private void OnGridToExcel(object obj)
        {
            try
            {

                object[,] objData = new Object[GridTask.Items.Count, 36];
                //Коллекция Items содержит отфильтрованные пользователем данные
                int i = 0;

                foreach (ElTask el in GridTask.Items)
                {
                    objData[i, 0] = el.ID.ToString();
                    objData[i, 1] = el.OwnerId.ToString();
                    objData[i, 2] = el.ContextId;
                    objData[i, 3] = el.TaskCode;
                    objData[i, 4] = el.MaskTaskCode;
                    objData[i, 5] = EnumConverter.GetDescription(el.Status);
                    objData[i, 6] = EnumConverter.GetDescription(el.Priority);
                    objData[i, 7] = el.ErrorCode;
                    objData[i, 8] = el.UserID;
                    objData[i, 9] = el.DateTask;
                    objData[i, 10] = el.CreateTime;
                    objData[i, 11] = el.ScheduledTime;
                    objData[i, 12] = el.DeadlineTime;
                    objData[i, 13] = el.StartTime;
                    objData[i, 14] = el.ClosingTime;
                    objData[i, 15] = el.Param0;
                    objData[i, 16] = el.Param1;
                    objData[i, 17] = el.Param2;
                    objData[i, 18] = el.Param3;
                    objData[i, 19] = el.Param4;
                    objData[i, 20] = el.Param5;
                    objData[i, 21] = el.Param6;
                    objData[i, 22] = el.Param7;
                    objData[i, 23] = el.Param8;
                    objData[i, 24] = el.Param9;
                    objData[i, 25] = el.Param10;
                    objData[i, 26] = el.Param11;
                    objData[i, 27] = el.Param12;
                    objData[i, 28] = el.Param13;
                    objData[i, 29] = el.Param14;
                    objData[i, 30] = el.Param15;
                    objData[i, 31] = el.Comment;
                    objData[i, 32] = el.ListChildrenTasks.Count;
                    objData[i, 33] = el.TaskLink.ToString();//+
                    objData[i, 34] = el.OwnerUserID;
                    objData[i, 35] = el.TaskProperties;



                    i++;
                }
                //формируем заголовок в ячейку (1,1)
                string title = $"Список задач c {SelectedDateFrom.ToShortDateString()} по {SelectedDateTo.ToShortDateString()}";
                //создаем объект документа и передаем на него масив с данными,  данных на странице Excel начнутся с ячейки [1,4]

                var tmpDoc = new CreateExcelDoc(objData, ConfigPlugin.BasePath, "ExportTaskTemplate.xlsx", title);

            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }



        }

        public ICommand CommandGetTaskList { get; set; }

        [Obsolete]
        public void OnGetTaskList(object obj)
        {
            IsBusy = true;

            //if (IsAllSubnetUnloading)
            //    ConfigPlugin.connectionElServer.SendMessage(
            //        new ElMessageClient("BasicFunctions", "GetELogisticsAllSubnetTaskInPeriod", Response_GetListTask),
            //        JsonConvert.SerializeObject(new
            //        {
            //            DateFrom = SelectedDateFrom,
            //            DateTo = SelectedDateTo,
            //            TargetTaskCode = IPAddress
            //                .Parse(ipString: $"{FirstOctet}.{SecondOctet}.{ThirdOctet}.{FourOctet}").Address,
            //            Mask,
            //            IsLoadAttachment = false
            //        }));


            //else
            //    ConfigPlugin.connectionElServer.SendMessage(
            //        new ElMessageClient("BasicFunctions", "GetELogisticsTaskInPeriod", Response_GetListTask),
            //        JsonConvert.SerializeObject(new
            //        {
            //            DateFrom = SelectedDateFrom,
            //            DateTo = SelectedDateTo,
            //            TargetTaskCode = IPAddress
            //                .Parse(ipString: $"{FirstOctet}.{SecondOctet}.{ThirdOctet}.{FourOctet}").Address,
            //            Mask,
            //            IsLoadAttachment = true
            //        }));
        }

        #endregion


        #region TemplateProperty

        private int _activeTab;
        public int ActiveTab
        {
            get => _activeTab;
            set
            {
                _activeTab = value;
                OnPropertyChanged("ActiveTab");
                TwoTabVisibility = _activeTab == 1 ? Visibility.Visible : Visibility.Collapsed;
            }
        }




        private Visibility _twoTabVisibility;
        public Visibility TwoTabVisibility
        {
            get
            {
                return _twoTabVisibility;
            }
            set
            {
                _twoTabVisibility = value;
                OnPropertyChanged("TwoTabVisibility");
            }
        }

        private Visibility _visibilityRightPanel;
        public Visibility VisibilityRightPanel
        {
            get
            {
                return _visibilityRightPanel;
            }
            set
            {
                _visibilityRightPanel = value;
                OnPropertyChanged("VisibilityRightPanel");
            }
        }

        private int _WidthRightPanel;

        public int WidthRightPanel
        {
            get
            {
                if (_WidthRightPanel == 0)
                    _WidthRightPanel = 550;
                return _WidthRightPanel;
            }
            set
            {
                _WidthRightPanel = value;
                OnPropertyChanged("WidthRightPanel");
            }

        }

        private int _selectedIndexRightPanel;
        public int SelectedIndexRightPanel
        {
            get
            {

                return _selectedIndexRightPanel;
            }
            set
            {
                //if (value == 1)
                //   CommandGetSKUInSupply.Execute(null);//Получаем данные по текущей поставке

                _selectedIndexRightPanel = value;
                OnPropertyChanged("SelectedIndexRightPanel");
            }
        }


        private Visibility _VisibleEditButton;
        public Visibility VisibleEditButton
        {
            get
            {
                return _VisibleEditButton;
            }
            set
            {
                _VisibleEditButton = value;
                OnPropertyChanged("VisibleEditButton");
            }
        }

        //Индикатор свернутого состояния левой панели
        private Visibility _VisibilityLeftPanel;
        public Visibility VisibilityLeftPanel
        {
            get
            {
                return _VisibilityLeftPanel;
            }
            set
            {
                _VisibilityLeftPanel = value;
                OnPropertyChanged("VisibilityLeftPanel");
            }
        }

        //видимость кнопки разворачивания левой панели
        public Visibility VisibilityLeftButton
        {
            get
            {
                if (VisibilityLeftPanel == Visibility.Visible)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }


        #endregion

        #region TemplateCommands

        public ICommand CommandToggleRightPanel { get; set; }
        private void OnToggleRightPanel(object obj)
        {

            if (obj != null || VisibilityRightPanel == Visibility.Visible)
            {
                int.TryParse(obj?.ToString(), out var index);
                SelectedIndexRightPanel = index;
                WidthRightPanel = 550;
                VisibilityRightPanel = Visibility.Collapsed;

            }
            else
            {
                WidthRightPanel = 40;
                VisibilityRightPanel = Visibility.Visible;

            }
        }

        public ICommand CommandClearFilterGrid { get; set; }
        private void OnClearFilterGrid(object obj)
        {
            if (obj is RadGridView gv)
            {
                gv.FilterDescriptors.SuspendNotifications();
                foreach (Telerik.Windows.Controls.GridViewColumn column in gv.Columns)
                {
                    column.ClearFilters();
                }
                gv.FilterDescriptors.ResumeNotifications();
            }

        }

        ////возврат на основную страницу журнала (стандартная кнопка)
        //public ICommand CommandBackOnMainTab { get; set; }

        //public void OnBackOnMainTab(object obj) {
        // ActiveTab = 0;
        // VisibilityBlockWindow = Visibility.Collapsed;

        // ConfigPlugin.connectionElServer.SendMessage(
        //  new ElMessageClient("WarehouseAccounting", "CancelBlockSupplyPlng", Response_CancelBlockSupplyPlng),
        //  JsonConvert.SerializeObject(SelectedSupplyPlanning.Id));
        // CommandGetAllSupply.Execute(null);
        // CommandRadarDocumentsOff.Execute(null);
        //}

        public ICommand CommandMouseDoubleClickGrid { get; set; }
        public void OnMouseDoubleClickGrid(object obj)
        {
            if (!(obj is MouseButtonEventArgs e))
                return;
            if (!(e.OriginalSource is FrameworkElement originalSender))
                return;
            var row = originalSender.ParentOfType<GridViewRow>();
            if (row != null)
            {
                //CommandGetUsers.Execute(row.DataContext);
                ConfigPlugin.connectionElServer.SendMessage(
                  new ElMessageClient("Administration", "GetUsers", Response_MouseDoubleClickGrid),
                  "");
            }
        }

        #endregion

        #region ServerResponses

        private void GetLogsInfo()
        {
            DateTime currentDate = DateTime.Now;

            // Формирование имени файла
            string fileName = currentDate.ToString("yyyy-MM-dd") + ".log";

            string filePath = ConfigPlugin.LogsFilePath + fileName;

            // Проверка наличия файла
            if (File.Exists(filePath))
            {
                // Создаем новую коллекцию для хранения обновленных данных
                var updatedErrorCounts = new ObservableCollection<ErrorCountItem>();
                AllLogsErrorsCount = new ErrorCountItem();
                // Чтение содержимого файла
                string[] lines = File.ReadAllLines(filePath);

                // Регулярное выражение для извлечения названия микросервиса
                Regex serviceNameRegex = new Regex(@"\| ([^\s|]+?)\.");

                // Обработка каждой строки
                foreach (string line in lines)
                {
                    // Проверка, содержит ли строка ошибку
                    if (line.Contains("| ERROR |"))
                    {
                        // Извлечение названия микросервиса с помощью регулярного выражения
                        Match match = serviceNameRegex.Match(line);
                        if (match.Success)
                        {
                            string serviceName = match.Groups[1].Value;

                            // Поиск элемента в коллекции по названию микросервиса
                            var errorCountItem = updatedErrorCounts.FirstOrDefault(item => item.MicroserviceName == serviceName);

                            // Если элемент не найден, добавляем новый
                            if (errorCountItem == null)
                            {
                                errorCountItem = new ErrorCountItem { MicroserviceName = serviceName, ErrorCount = 0 };
                                updatedErrorCounts.Add(errorCountItem);
                            }

                            // Увеличение счетчика ошибок для данного микросервиса
                            errorCountItem.ErrorCount++;
                            AllLogsErrorsCount.ErrorCount++;
                            errorCountItem.MicroserviceNameAndCount = serviceName + " " + errorCountItem.ErrorCount.ToString();
                        }
                    }
                }

                // Присваиваем новую коллекцию переменной LogsErrorCounts
                    LogsErrorCounts = updatedErrorCounts;

                ///
            }
            else
            {
                Console.WriteLine("Файл не найден: " + fileName);
            }
        }

        private void UpdateMetricCollectors(ObservableCollection<ServerMetricCollector> ServerMetricsListProperty, ObservableCollection<DatabaseMetricCollector> DatabaseMetricsListProperty, ObservableCollection<StorageMetricCollector> StorageMetricsListProperty)
        {

        }

        private void Response_CommandReadReadyMetrics(string data)
        {
            try
            {
            
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));

            }
        }
        private void Response_CommandUpdateListOfMetricsSource(string data)
        {
            try
            {
                GetLogsInfo();
                var definition = new { MetricsSources = new List<MetricsControlConfig>() };
                var tmp = JsonConvert.DeserializeAnonymousType(data, definition);
                MetricsResources = new ObservableCollection<MetricsControlConfig>();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var tmp2 in tmp.MetricsSources)
                    {
                        var resource = new MetricsControlConfig(tmp2.metricIp, tmp2.metricType, tmp2.metricCheckInterval);
                        MetricsResources.Add(resource);
                    }
                });
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));

            }
        }
        private void Response_CommandUpdateReadyMetrics(string data)
        {
            try
            {

            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));

            }
        }

        private void Response_CommandGetTasksFromTaskManager(string data)
        {
            try
            {

            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));

            }
        }
        private void Response_CommandAllMetricsProcceses(string data)
        {
            try
            {

            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));

            }
        }

        private void Response_RefreshData(string data)
        {
            try
            {
                //  if (JsonConvert.DeserializeObject<bool>(data))
                // {
                CommandGetMonitoringData.Execute(null);
                //   }
                //else
                //{
                // //error string
                //}
                //ListServerLoadMonitoringsData = JsonConvert.DeserializeObject<ObservableCollection<ElTask>>(data);

                //IsBusy = false;
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
                //IsBusy = false;
            }
        }

        private void Response_GetMonitoringData(string data)
        {
            try
            {

                var jobs = JsonConvert.DeserializeObject<BackupMonitorData>(JsonConvert.DeserializeObject<string>(data));
                if (jobs != null)
                {
                    _lastLoadedBackup = jobs;

                    var jobsModels = new List<JobViewModel>();

                    if (jobs.AgentsConfigs != null)
                    {
                        foreach (var agentConfig in jobs.AgentsConfigs)
                        {
                            if (agentConfig.Jobs != null)
                            {
                                foreach (var job in agentConfig.Jobs)
                                {
                                    var jobView = JsonConvert.DeserializeObject<JobViewModel>(JsonConvert.SerializeObject(job));

                                    if (jobView != null)
                                    {
                                        jobView.AgentName = agentConfig?.Name;
                                        jobsModels.Add(jobView);
                                    }
                                    else
                                    {
                                        LogManager.GetCurrentClassLogger().Error($"Ошибка чтения работы {job}");
                                    }

                                }
                            }
                        }
                    }

                    //send data to heatmap control
                    HeatmapControl.UpdateData(jobsModels);

                    //если вклюено обновление данных у этого пользователя и состояние бэкапа обновлялось больше, чем полчаса назад, то запускаем заново
                    if (ConfigPlugin.SecurityKey.HasFlag(EPermissions.IsRefreshDataEnabled) && _lastLoadedBackup.RefreshingData < DateTime.Now.AddMinutes(-30))
                    {
                        CommandRefreshData.Execute(null);
                    }
                }



                //IsBusy = false;
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
                //IsBusy = false;
            }
        }


        private void Response_GetListTask(string data)
        {
            try
            {

                ListServerLoadMonitoringsData = JsonConvert.DeserializeObject<ObservableCollection<ElTask>>(data);

                IsBusy = false;
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
                IsBusy = false;
            }
        }


        private void Response_MouseDoubleClickGrid(string data)
        {
            try
            {
                // ListUsers = JsonConvert.DeserializeObject<ObservableCollection<User>>(data);
                //if (ListUsers.Count > 0) {
                //   SelectedUser = ListUsers[0];
                //}
                //VisibilityUsers = Visibility.Visible;
            }
            catch (Exception e)
            {

            }
        }



        #endregion

        #region ContextMenu


        //public ObservableCollection<ModeltemMenu> GetContextMenuListUsers(User sender) {


        //   var itemContextMenuListOrders = new ObservableCollection<ModeltemMenu>();


        //   //Все запрещаем********************************
        //   var NotEdit = false;

        //   //**********************************************


        //   itemContextMenuListOrders.Add(new ModeltemMenu() { Header = "Закрыть", Icon = "&#xe11b;", Command = CommandClosePlugin, CommandParameter = sender, IsEnabled = false });
        //   itemContextMenuListOrders.Add(new ModeltemMenu() { Header = "Открыть", Icon = "&#xe134;", Command = CommandOpenPlugin, CommandParameter = sender, IsEnabled = false });
        //   //  if (SelectedTransportOrders.Count < 2)    //по условию данные комманды в меню блокируются
        //   //  {
        //   //   itemContextMenuListOrders.Add(new ModeltemMenu() { Header = "Перевести в статус \"Сделано\"", Icon = "&#xe817;", Command = CommandAcceptOrders, CommandParameter = sender, IsEnabled = false});
        //   //        }
        //   //  else
        //   //  {
        //   //   itemContextMenuListOrders.Add(new ModeltemMenu() { Header = "Перевести в статус \"Сделано\" ", Icon = "&#xe817;", Command = CommandAcceptOrders, CommandParameter = sender});
        //   //}
        //   //*********************************************
        //   return itemContextMenuListOrders;
        //}


        #endregion
    }
}
