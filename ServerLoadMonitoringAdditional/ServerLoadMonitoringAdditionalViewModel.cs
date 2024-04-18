using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Calendar;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Persistence;

namespace ServerLoadMonitoring.ServerLoadMonitoringData {
	public class ServerLoadMonitoringAdditionalViewModel:ViewModelBase {
        public RadGridView GridTask { get; set; }
		public ServerLoadMonitoringAdditionalViewModel()
        { 
            
         CommandGridToExcel=new DelegateCommand(OnGridToExcel);
         CommandGetTaskList = new DelegateCommand(OnGetTaskList);
         CommandClearFilterGrid = new DelegateCommand(OnClearFilterGrid);
         CommandToggleRightPanel = new DelegateCommand(OnToggleRightPanel);
         CommandCalendarToToday = new DelegateCommand(OnCalendarToToday);
         CommandCalendarFromToday = new DelegateCommand(OnCalendarFromToday);
            
         CommandCopyingCellClipboard = new DelegateCommand(OnCopyingCellClipboard);
         CommandCopying=new DelegateCommand(OnCopying);
        CommandClipboardMode = new DelegateCommand(OnClipboardMode);
        CommandRefreshData = new DelegateCommand(OnRefreshData);

    

            //Управление настройками грида ДЛЯ РАБОТЫ НАСТРОЕК В TAG RADGRIDVIEW ВНОСИМ НОВЫЙ GUID
            CommandSaveConfigGridView =new DelegateCommand(OnSaveConfigGridView);
         CommandLoadConfigGridView = new DelegateCommand(OnLoadConfigGridView);
         CommandResetConfigGridView = new DelegateCommand(OnResetConfigGridView);
         CommandChangeModeSaveConfig = new DelegateCommand(OnChangeModeSaveConfig);
         CommandLoadSettingControlOnOpen=new DelegateCommand(OnLoadSettingControlOnOpen);
         CommandSaveSettingControlOnClosed =new DelegateCommand(OnSaveSettingControlOnClosed);
         //***************************

            //CommandMouseDoubleClickGrid = new DelegateCommand(OnMouseDoubleClickGrid);

            TwoTabVisibility = Visibility.Collapsed;
         ActiveTab = 0;
         VisibilityRightPanel = Visibility.Collapsed;

         _selectedDateTo = DateTime.Today;
         _selectedDateFrom = DateTime.Today.AddDays(-1);
         
         IsAllSubnetUnloading = false;
         Mask = 24;
        }

        #region Properties

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

        private string _taskCodeString;

        public string TaskCodeString
        {
            get => _taskCodeString;
            set
            {
                _taskCodeString = value;
                OnPropertyChanged("TaskCodeString");
            }
        }


        private int _firstOctet;

        public int FirstOctet
        {
            get => _firstOctet;
            set
            {
                _firstOctet = value;
                OnPropertyChanged("FirstOctet");
            }
        }

        private int _secondOctet;

        public int SecondOctet
        {
            get => _secondOctet;
            set
            {
                _secondOctet = value;
                OnPropertyChanged("SecondOctet");
            }
        }

        private int _thirdOctet;

        public int ThirdOctet
        {
            get => _thirdOctet;
            set
            {
                _thirdOctet = value;
                OnPropertyChanged("ThirdOctet");
            }
        }

        private int _fourthOctet;

        public int FourOctet
        {
            get => _fourthOctet;
            set
            {
                _fourthOctet = value;
                OnPropertyChanged("FourOctet");
            }
        }

        private int _mask;

        public int Mask
        {
            get => _mask;
            set
            {
                _mask = value;
                OnPropertyChanged("Mask");
            }
        }

        private bool _isAllSubnetUnloading;

        public bool IsAllSubnetUnloading
        {
            get => _isAllSubnetUnloading;
            set
            {
                _isAllSubnetUnloading = value;
                OnPropertyChanged("IsAllSubnetUnloading");
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
                ClipboardMode = !ClipboardMode;
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }

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
                    objData[i, 0 ] = el.ID.ToString();
                    objData[i, 1 ] = el.OwnerId.ToString();
                    objData[i, 2 ] = el.ContextId;
                    objData[i, 3 ] = el.TaskCode;
                    objData[i, 4 ] = el.MaskTaskCode;
                    objData[i, 5 ] = EnumConverter.GetDescription( el.Status);
                    objData[i, 6 ] = EnumConverter.GetDescription( el.Priority);
                    objData[i, 7 ] = el.ErrorCode;
                    objData[i, 8 ] = el.UserID;
                    objData[i, 9 ] = el.DateTask;
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

            if (IsAllSubnetUnloading)
                ConfigPlugin.connectionElServer.SendMessage(
                    new ElMessageClient("BasicFunctions", "GetELogisticsAllSubnetTaskInPeriod", Response_GetListTask),
                    JsonConvert.SerializeObject(new
                    {
                        DateFrom = SelectedDateFrom,
                        DateTo = SelectedDateTo,
                        TargetTaskCode = IPAddress
                            .Parse(ipString: $"{FirstOctet}.{SecondOctet}.{ThirdOctet}.{FourOctet}").Address,
                        Mask,
                        IsLoadAttachment = false
                    }));


            else
                ConfigPlugin.connectionElServer.SendMessage(
                    new ElMessageClient("BasicFunctions", "GetELogisticsTaskInPeriod", Response_GetListTask),
                    JsonConvert.SerializeObject(new
                    {
                        DateFrom = SelectedDateFrom,
                        DateTo = SelectedDateTo,
                        TargetTaskCode = IPAddress
                            .Parse(ipString: $"{FirstOctet}.{SecondOctet}.{ThirdOctet}.{FourOctet}").Address,
                        Mask,
                        IsLoadAttachment = true
                    }));
        }

        #endregion


        #region TemplateProperty

        private int _activeTab;
      public int ActiveTab {
         get => _activeTab;
         set {
            _activeTab = value;
            OnPropertyChanged("ActiveTab");
            TwoTabVisibility = _activeTab == 1 ? Visibility.Visible : Visibility.Collapsed;
         }
      }




      private Visibility _twoTabVisibility;
      public Visibility TwoTabVisibility {
         get {
            return _twoTabVisibility;
         }
         set {
            _twoTabVisibility = value;
            OnPropertyChanged("TwoTabVisibility");
         }
      }

      private Visibility _visibilityRightPanel;
      public Visibility VisibilityRightPanel {
         get {
            return _visibilityRightPanel;
         }
         set {
            _visibilityRightPanel = value;
            OnPropertyChanged("VisibilityRightPanel");
         }
      }

      private int _WidthRightPanel;

      public int WidthRightPanel {
         get {
            if (_WidthRightPanel == 0)
               _WidthRightPanel = 550;
            return _WidthRightPanel;
         }
         set {
            _WidthRightPanel = value;
            OnPropertyChanged("WidthRightPanel");
         }

      }

      private int _selectedIndexRightPanel;
      public int SelectedIndexRightPanel {
         get {

            return _selectedIndexRightPanel;
         }
         set {
            //if (value == 1)
            //   CommandGetSKUInSupply.Execute(null);//Получаем данные по текущей поставке

            _selectedIndexRightPanel = value;
            OnPropertyChanged("SelectedIndexRightPanel");
         }
      }


      private Visibility _VisibleEditButton;
      public Visibility VisibleEditButton {
         get {
            return _VisibleEditButton;
         }
         set {
            _VisibleEditButton = value;
            OnPropertyChanged("VisibleEditButton");
         }
      }

      //Индикатор свернутого состояния левой панели
      private Visibility _VisibilityLeftPanel;
      public Visibility VisibilityLeftPanel {
         get {
            return _VisibilityLeftPanel;
         }
         set {
            _VisibilityLeftPanel = value;
            OnPropertyChanged("VisibilityLeftPanel");
         }
      }

      //видимость кнопки разворачивания левой панели
      public Visibility VisibilityLeftButton {
         get {
            if (VisibilityLeftPanel == Visibility.Visible) {
               return Visibility.Collapsed;
            } else {
               return Visibility.Visible;
            }
         }
      }

      private bool _isBusy;
      public bool IsBusy {
         get => _isBusy;
         set {
            _isBusy = value;
            OnPropertyChanged("IsBusy");
         }
      }


      #endregion

      #region TemplateCommands

      public ICommand CommandToggleRightPanel { get; set; }
      private void OnToggleRightPanel(object obj) {

         if (obj != null || VisibilityRightPanel == Visibility.Visible) {
            int.TryParse(obj?.ToString(), out var index);
            SelectedIndexRightPanel = index;
            WidthRightPanel = 550;
            VisibilityRightPanel = Visibility.Collapsed;

         } else {
            WidthRightPanel = 40;
            VisibilityRightPanel = Visibility.Visible;

         }
      }

      public ICommand CommandClearFilterGrid { get; set; }
      private void OnClearFilterGrid(object obj) {
         if (obj is RadGridView gv) {
            gv.FilterDescriptors.SuspendNotifications();
            foreach (Telerik.Windows.Controls.GridViewColumn column in gv.Columns) {
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
      public void OnMouseDoubleClickGrid(object obj) {
         if (!(obj is MouseButtonEventArgs e))
            return;
         if (!(e.OriginalSource is FrameworkElement originalSender))
            return;
         var row = originalSender.ParentOfType<GridViewRow>();
         if (row != null) {
            //CommandGetUsers.Execute(row.DataContext);
            ConfigPlugin.connectionElServer.SendMessage(
              new ElMessageClient("Administration", "GetUsers", Response_MouseDoubleClickGrid),
              "");
         }
      }

        #endregion

        #region ServerResponses

     
        private void Response_GetListTask(string data) {
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
