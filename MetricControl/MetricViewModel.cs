using ElMessage;
using Newtonsoft.Json;
using NLog;
using ServerLoadMonitoringDataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Telerik.Windows.Controls;

namespace ServerLoadMonitoring.MetricControl
{
    public class MetricViewModel : ViewModelBase
    {
        private SolidColorBrush _cpuStrokeColor = new SolidColorBrush(Colors.Green);

        public SolidColorBrush cpuStrokeColor
        {
            get { return _cpuStrokeColor; }
            set
            {
                if (_cpuStrokeColor != value)
                {
                    _cpuStrokeColor = value;
                    OnPropertyChanged("cpuStrokeColor");
                }
            }
        }

        private SolidColorBrush _ramStrokeColor = new SolidColorBrush(Colors.Yellow);

        public SolidColorBrush RAMStrokeColor
        {
            get { return _ramStrokeColor; }
            set
            {
                if (_ramStrokeColor != value)
                {
                    _ramStrokeColor = value;
                    OnPropertyChanged("RAMStrokeColor");
                }
            }
        }

        private DispatcherTimer timer;

        

        private void Timer_Tick(object sender, EventArgs e)
        {
            GetMetrics();
        }

        private DispatcherTimer timerChangeStrokeColor;


        private void Timer_TickChangeRAMStrokeColor(object sender, EventArgs e)
        {
           
                if (LastMetric.UsedMemoryPercents > 90)
                {
                    if (RAMStrokeColor.Color == Colors.Yellow)
                    {
                        RAMStrokeColor.Color = Colors.Red;
                    }
                    else { RAMStrokeColor.Color = Colors.Yellow; }
                }
                else if (RAMStrokeColor.Color != Colors.Yellow)
                {
                    RAMStrokeColor.Color = Colors.Yellow;
                }
                if (LastMetric.CpuUsage > 90)
                {
                    if (cpuStrokeColor.Color == Colors.Green)
                    {
                        cpuStrokeColor.Color = Colors.Red;
                    }
                    else { cpuStrokeColor.Color = Colors.Green; }
                }
                else if (cpuStrokeColor.Color != Colors.Green)
                {
                    cpuStrokeColor.Color = Colors.Green;
                }
        }


        public MetricViewModel()
        {
            Metrics = new ObservableCollection<IMetric>();
        }
        public MetricViewModel(MetricsControlConfig config)
        {
            Metrics = new ObservableCollection<IMetric>();
            this.Config = config;


            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(2);
            this.timer.Tick += Timer_Tick;
            this.timer.Start();
       
            if (config.metricType == ServerLoadMonitoringDataModels.Enums.MetricType.ServerUtilization)
            {
                LastMetric = new ServerUtilization();
            }
            if (config.metricType == ServerLoadMonitoringDataModels.Enums.MetricType.DatabaseUtilization)
            {
                LastMetric = new DatabaseUtilization();
            }
            this.timerChangeStrokeColor = new DispatcherTimer();
            this.timerChangeStrokeColor.Interval = TimeSpan.FromSeconds(1);
            this.timerChangeStrokeColor.Tick += Timer_TickChangeRAMStrokeColor;
            this.timerChangeStrokeColor.Start();
            
        }

        ReaderWriterLockSlim rwLockMetrics = new ReaderWriterLockSlim();

        MetricsControlConfig Config;

        private ObservableCollection<IMetric> _Metrics;
        public ObservableCollection<IMetric> Metrics
        {
            get 
            {
                return _Metrics;
            }
            set
            {
                _Metrics = value;
                OnPropertyChanged("Metrics");
            }
        }
        private IMetric _LastMetric;
        public IMetric LastMetric
        {
            get => _LastMetric;
            set
            {
                _LastMetric = value;
                OnPropertyChanged("LastMetric");
            }
        }

        private int _LastReadedMetricPlacement;

        public int LastReadedMetricPlacement
        {
            get => _LastReadedMetricPlacement;
            set
            {
                _LastReadedMetricPlacement = value;
                OnPropertyChanged("LastReadedMetricPlacement");
            }
        }
        public void GetMetrics()
        {
            ConfigPlugin.connectionElServer.SendMessage(
                        new ElMessageClient("ServerLoadMonitoring", "GetReadyMetrics", Response_CommandReadReadyMetrics),
                        JsonConvert.SerializeObject(new { LastReadedMetricPlacement = LastReadedMetricPlacement, type = Config.metricType, Ip = Config.metricIp }));
        }
        public void Response_CommandReadReadyMetrics(string data)
        {
            try {
                var definition = new { json = "", type = "" };
                var tmp = JsonConvert.DeserializeAnonymousType(data, definition);
                ObservableCollection<IMetric> newCollection = new ObservableCollection<IMetric>();
                if (tmp.type == "ServerUtilization")
                {
                    var collection = JsonConvert.DeserializeObject<ObservableCollection<ServerUtilization>>(tmp.json);

                    foreach (ServerUtilization serverUtilization in collection)
                    {
                        // Приводим элемент к типу IMetric
                        IMetric metric = serverUtilization;

                        // Добавляем элемент в новую коллекцию
                        newCollection.Add(metric);
                    }
                    if (newCollection.Count > 0)
                        UpdateMetricCollectors(newCollection);
                }
                else if (tmp.type == "DatabaseUtilization")
                {
                    var collection = JsonConvert.DeserializeObject<ObservableCollection<DatabaseUtilization>>(tmp.json);
                    foreach (DatabaseUtilization DatabaseUtilization in collection)
                    {
                        // Приводим элемент к типу IMetric
                        IMetric metric = DatabaseUtilization;

                        // Добавляем элемент в новую коллекцию
                        newCollection.Add(metric);
                    }
                    if (newCollection.Count > 0)
                        UpdateMetricCollectors(newCollection);
                }
                else
                {
                    var collection = JsonConvert.DeserializeObject<ObservableCollection<StorageUtilization>>(tmp.json);
                    foreach (StorageUtilization StorageUtilization in collection)
                    {
                        // Приводим элемент к типу IMetric
                        IMetric metric = StorageUtilization;

                        // Добавляем элемент в новую коллекцию
                        newCollection.Add(metric);
                    }
                    if(newCollection.Count > 0)
                        UpdateMetricCollectors(newCollection);
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));

            }
        }

        private void UpdateMetricCollectors(ObservableCollection<IMetric> MetricsListProperty)
        {
            try
            {

                LastMetric = MetricsListProperty.Last();
                if (LastMetric != null)
                { LastReadedMetricPlacement = LastMetric.Placement; }
                    // Создаем копию коллекции Metrics для безопасной итерации
                var metricsCopy = new List<IMetric>(Metrics);

                foreach (var Metric in MetricsListProperty)
                {
                    if (metricsCopy.Count > 60)
                    {
                        metricsCopy.RemoveAt(0);
                        metricsCopy.Add(Metric);
                    }
                    else
                    {
                        metricsCopy.Add(Metric);
                    }
                }

                // Обновляем коллекцию Metrics
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Metrics = new ObservableCollection<IMetric>(metricsCopy);
                });
             
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));

            }
        }


      
    }
}

