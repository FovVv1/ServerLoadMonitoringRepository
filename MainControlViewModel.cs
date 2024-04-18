using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ElMessage;
using Newtonsoft.Json;
using NLog;
using ServerLoadMonitoring.Helpers;
using ServerLoadMonitoring.MetricControl;
using ServerLoadMonitoringDataModels;
using Telerik.Windows.Controls;
using Application = System.Windows.Application;
using Timer = System.Threading.Timer;

namespace ServerLoadMonitoring
{
    public class MainControlViewModel : ViewModelBase
    {
        private int _selectedIndex;
        /// <summary>Индекс выбраной страницы </summary>
		public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        //Режим киоска
        private bool isKioskMode;

        /// <summary>Kiosk Mode </summary>
        public bool IsKioskMode
        {
            get => isKioskMode;
            set
            {
                try
                {
                    var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                    var winWight = SystemParameters.MaximizedPrimaryScreenWidth - 14.0 -
                                   2 * SystemParameters.BorderWidth;
                    var winHeight = SystemParameters.MaximizedPrimaryScreenHeight - 14.0 -
                                    2 * SystemParameters.BorderWidth;
                    if (!value)
                    {
                        window.Topmost = false;
                        window.Top = 0;
                        window.Width = winWight;
                        window.Left = 0;
                        window.Height = winHeight;
                        window.WindowState = WindowState.Normal;
                        window.ResizeMode = ResizeMode.CanResize;
                        Taskbar.Show();
                        window.Focus();
                    }
                    else
                    {
                        Taskbar.Hide();

                        window.Topmost = true;

                        window.WindowState = WindowState.Normal;


                        window.WindowStartupLocation = WindowStartupLocation.Manual;

                        window.Top = -42;
                        window.Width = winWight + 2;
                        window.Left = -2;
                        window.Height = winHeight + 100;

                        window.ResizeMode = ResizeMode.NoResize;
                    }

                    window.Focus();
                    isKioskMode = value;
                }
                catch (Exception e)
                {
                    LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
                }
            }
        }

        // Таймер переключения страниц в главном меню
        private Timer _showPageTimer;
        //Блокировка автоматического обновления
        private bool _isBlockAutoUpdate;
        //public Visibility NextButton => _listSlide.IndexOf(ListSelected) < _listSlide.Count - 1 ? Visibility.Visible : Visibility.Hidden;
        //public Visibility BackButton => _listSlide.IndexOf(ListSelected) == 0 ? Visibility.Hidden : Visibility.Visible;

        /// <summary>Коллекция контролов содержимого страниц </summary>
        private ObservableCollection<MainControlModel> _includedContentList;
        public ObservableCollection<MainControlModel> IncludedContentList
        {
            get => _includedContentList;
            set
            {
                _includedContentList = value;
                OnPropertyChanged("IncludedContentList");
            }
        }


        /// <summary>Коллекция контролов содержимого страниц </summary>
        private ObservableCollection<IMonitoringElUserControl> _ControlsList;
        public ObservableCollection<IMonitoringElUserControl> ControlsList
        {
            get => _ControlsList;
            set
            {
                _ControlsList = value;
                OnPropertyChanged("ControlsList");
            }
        }

        public void ShowPage()
        {
            //IncludedContentList.FirstOrDefault(m => m.Title == "Дашборд").Visibility = Visibility.Visible;
            //IncludedContentList.FirstOrDefault(m => m.Title == "Данные отчета").Visibility = Visibility.Visible;
            //IncludedContentList.FirstOrDefault(m => m.Title == "Настройки отчета").Visibility = Visibility.Visible;

        }

        private void ShowNextPage(object obj)
        {
            try
            {
                //if (!_isBlockAutoUpdate)
                //   SelectedPage = SelectedPage.Index == Pages.Count - 1 ? Pages[0] : Pages[SelectedPage.Index + 1];
                ////Эмуляция нажатия клавиши для учетки TS только в полноэкраном режиме
                //if (isKioskMode)
                //   Application.Current.Dispatcher.Invoke(() =>
                //   {

                //      try {
                //         SendKeys.SendWait("{LEFT}");
                //      } catch (Exception e) {
                //         //LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
                //         //ignore
                //      }
                //   });




                if (isKioskMode)
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            var window = Application.Current.Windows.OfType<Window>()
                           .SingleOrDefault(x => x.IsActive);
                            var winWight = SystemParameters.MaximizedPrimaryScreenWidth - 14.0 -
                                      2 * SystemParameters.BorderWidth;
                            var winHeight = SystemParameters.MaximizedPrimaryScreenHeight - 14.0 -
                                       2 * SystemParameters.BorderWidth;

                            if (!(window is null))
                            {
                                window.Topmost = true;

                                window.WindowState = WindowState.Normal;


                                window.WindowStartupLocation = WindowStartupLocation.Manual;

                                window.Top = -42;
                                window.Width = winWight + 2;
                                window.Left = -2;
                                window.Height = winHeight + 100;

                                window.ResizeMode = ResizeMode.NoResize;

                                window.Focus();
                            }
                        }
                        catch (Exception e)
                        {
                       // LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
                       // ignored
                   }

                    });
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }

        /// <summary>
        /// Такт обработки пропусков получивших фокус по событию
        /// </summary>
        private void TickFocus(object obj)
        {
            try
            {
                //foreach (var el in PassFocusList.Values.ToList()) {
                //   el.IsTickBackground = !el.IsTickBackground;

                //   if (--el.TickCount == 0) {
                //      PassFocusList.Remove(el.CarProcessingTask);
                //      el.IsTickBackground = false;
                //      //при установленом флаге удаляем эдемент и пересчитываем количество сраниц
                //      if (el.RemoveAfterTicks) {
                //         ListOfCarsPending.Remove(el);
                //         CalculatePageCount();
                //      }
                //      SelectedPage = Pages[0];
                //   }
                //}

                ////Если нет элементов с фокусом отключаем таймер тактов и снимаем блокмровку полного обновления
                //if (PassFocusList.Count == 0) {
                //   _tickTimer.Dispose();
                //   _isBlockAutoUpdate = false;
                //}
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }

        public MainControlViewModel()
        {
            //IncludedContentList = new ObservableCollection<MainControlModel>();
            ControlsList = new ObservableCollection<IMonitoringElUserControl>();
            //Команда вызова помошника для расширения
            CommandStartHelper = new DelegateCommand(OnStartHelper);

            ControlsList.Add(new ServerLoadMonitoringData.ServerLoadMonitoringData());
            ControlsList.Add(new ServerLoadMonitoringAdditional());

            //IncludedContentList.Add(new MainControlModel(typeof(Dashboard.Dashboard)) { Image = "&#xe600;", Title = "Дашборд" });
            //IncludedContentList.Add(new MainControlModel(typeof(ServerLoadMonitoringDataModel.ServerLoadMonitoringAdditional)) { Image = "&#xe651;", Title = "Журнал заданий" });
            //IncludedContentList.Add(new MainControlModel(typeof(ServerLoadMonitoringSettings.ServerLoadMonitoringSettings)) { Image = "&#xe10f;", Title = "Настройки отчета" });

            CommandNextSlide = new DelegateCommand(OnNextSlide);
            CommandBackSlide = new DelegateCommand(OnBackSlide);
            CommandGetListOfMetrics = new DelegateCommand(OnGetListOfMetrics);
        }


        #region COMMANDS

        public ICommand CommandGetListOfMetrics { get; set; }
        private void OnGetListOfMetrics(object obj)
        {
            try
            {
                ConfigPlugin.connectionElServer.SendMessage(
                        new ElMessageClient("ServerLoadMonitoring", "GetListOfMetrics", Response_GetListOfMetrics),
                        null);
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }
        public ICommand CommandStartHelper { get; set; }
        private void OnStartHelper(object obj)
        {

            //Формируем набр параметров для удаленного запуска расширения
            var param = new Dictionary<string, object>
            {
                  {"PluginName", "Helper"}
                , {"NotLimitCopies", ""}
                , {"PageIndex", (int) 10}
            };

            ////Формируем список пользователей для назначения
            var targetListUserId = new List<int>();
            targetListUserId.Add(ConfigPlugin.connectionElServer.CurrentUser.UserID);
            param.Add("TargetListUserId", JsonConvert.SerializeObject(targetListUserId));
            ////**********************************************

            //Регистрация запуска расширения для списка последних использованых
            ConfigPlugin.connectionElServer.SendMessage(new ElMessageClient("BasicFunctions", "ShowPlugin", null), JsonConvert.SerializeObject(param));

        }

        public ICommand CommandNextSlide { get; set; }

        private void OnNextSlide(object obj)
        {
            try
            {
                ////устанавливаем время последней активности пользователя
                //_lastChangeTime = DateTime.Now;
                //var index = _listSlide.IndexOf(ListSelected);
                if (SelectedIndex < ControlsList.Count - 1)
                {
                    SelectedIndex += 1;
                }
                else
                {
                    SelectedIndex = 0;
                }

                //if (index < ListSlide.Count - 1)
                //	ListSelected = ListSlide[++index];
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }
        public ICommand CommandBackSlide { get; set; }

        private void OnBackSlide(object obj)
        {
            try
            {
                ////устанавливаем время последней активности пользователя
                //_lastChangeTime = DateTime.Now;
                //var index = _listSlide.IndexOf(ListSelected);
                //if (index != 0)
                //	ListSelected = ListSlide[--index];
                if (SelectedIndex != 0)
                {
                    SelectedIndex -= 1;
                }
                else
                {
                    SelectedIndex = ControlsList.Count - 1;
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
            }
        }

        #endregion


        #region RESPONSES
        private void Response_GetListOfMetrics(string data)
        {
            try
            {

            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
                //IsBusy = false;
            }
        }
        #endregion

    }
}
