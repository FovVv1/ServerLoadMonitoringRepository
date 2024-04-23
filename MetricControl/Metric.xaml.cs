using ServerLoadMonitoringDataModels;
using ServerLoadMonitoringDataModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServerLoadMonitoring.MetricControl
{
    /// <summary>
    /// Логика взаимодействия для Metric.xaml
    /// </summary>
    public partial class Metric : UserControl
    {
        public Metric()
        {
            InitializeComponent();
            Base.DataContext = new MetricViewModel();
        }
        public Metric(MetricsControlConfig config)
        {
            InitializeComponent();
            if (config.metricType == MetricType.ServerUtilization)
            {
                Server_Chart.Visibility = Visibility.Visible;
                Server_Chart.IsEnabled = true; // включаем график сервера
                ServerModul.Visibility = Visibility.Visible;
                ServerModul.IsEnabled = true;
                
            }
            else if (config.metricType == MetricType.DatabaseUtilization)
            {
                DB_Chart.Visibility = Visibility.Visible;
                DB_Chart.IsEnabled = true; // включаем график базы данных
                DatabaseModule.Visibility = Visibility.Visible;
                DatabaseModule.IsEnabled = true;
            }
            Base.DataContext = new MetricViewModel(config);
            
        }
        public void LastControl()
        {
            Thickness newThickness = new Thickness(2,2,2,2);
            BaseBorder.BorderThickness = newThickness;
        }
    
    }
}
