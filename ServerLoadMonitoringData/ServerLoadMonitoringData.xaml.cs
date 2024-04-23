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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ElControls.ELHotKey;
using ElControls.Interface;
using NLog;
using ServerLoadMonitoring.Helpers;
using ServerLoadMonitoringDataModels;
using WarehouseСontexts.ContextSelector;


namespace ServerLoadMonitoring.ServerLoadMonitoringData {
	/// <summary>
	/// Interaction logic for ServerLoadMonitoringDataModel.xaml
	/// </summary>
	public partial class ServerLoadMonitoringData:IMonitoringElUserControl,IDisposable,IElHotKey {

        
		public ContextSelector contextSelector { get; set; }

		public ServerLoadMonitoringData() {
			InitializeComponent();
			Base.Visibility = Visibility.Hidden;
			StartVm(null, null);
        }
       
		public void Dispose()
        {

            if (Base.DataContext is ServerLoadMonitoringDataViewModel model)
            {
                model.CommandSaveSettingControlOnClosed.Execute(null);

            }
        }
		private void StartVm(object sender, EventArgs e) {
			//ConfigPlugin.SetSecurityKey(ContextSelector.SecurityKey);
			int.TryParse(ConfigPlugin.TargetContext, out var idResult);
			var tmp = new ServerLoadMonitoringDataViewModel();
			tmp.HeatmapControl = HeatmapControl;
            //tmp.CalendarFrom = CalendarFrom;
            //tmp.CalendarTo = CalendarTo;
            //tmp.GridTask = GridServerLoadMonitoringsData;

			Base.DataContext = tmp;
			Base.Visibility = Visibility.Visible;
            //Список контролов на загрузку настроек
           // tmp.CommandLoadSettingControlOnOpen.Execute(null);
            //******************************************
				tmp.CommandGetMonitoringData.Execute(null);

		  }
		public void OnElHotKey(KeyEventArgs e)
        {

            if (e.Key == Key.F5 && Base.DataContext is ServerLoadMonitoringDataViewModel refresh)
	            refresh.CommandGetMonitoringData.Execute(null);

            if (e.Key == Key.F2 && Base.DataContext is ServerLoadMonitoringDataViewModel getAllSupply)
	            getAllSupply.CommandRefreshData.Execute(null);
			

		}

        public void OnElScanData(List<KeyEventArgs> listKey)
        {

        }

		private void UIElement_OnMouseMove(object sender, MouseEventArgs mouseEventArgs) {
			double x = mouseEventArgs.GetPosition(sender as TabControl).X;


			//if (x > 545 && RightPanel.Visibility != Visibility.Visible) {
			//	if (Base.DataContext is ServerLoadMonitoringDataViewModel tmp)
			//		tmp.VisibilityRightPanel = Visibility.Visible;
			//}
			//if (0 < x && x < 500 && RightPanel.Visibility == Visibility.Visible) {
			//	if (Base.DataContext is ServerLoadMonitoringDataViewModel tmp)
			//		tmp.VisibilityRightPanel = Visibility.Collapsed;
			//}

		}

		public int RefreshTime { get; set; }
		public bool IsAutoRefresh { get; set; }
		public void RefreshData()
		{
			try
			{
				if (Base.DataContext is ServerLoadMonitoringDataViewModel model) {
					model.CommandRefreshData.Execute(null);

				}
			}
			catch (Exception e)
			{
				LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
			}
		}

        private void HeatmapControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
