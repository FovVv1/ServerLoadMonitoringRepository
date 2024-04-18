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
using ElControls.ELHotKey;
using ElControls.Interface;
using NLog;
using ServerLoadMonitoring.Helpers;
using WarehouseСontexts.ContextSelector;

namespace ServerLoadMonitoring {
	/// <summary>
	/// Interaction logic for ServerLoadMonitoringAdditional.xaml
	/// </summary>
	public partial class ServerLoadMonitoringAdditional:IMonitoringElUserControl,IDisposable,IElHotKey {


		public ContextSelector contextSelector { get; set; }

		public ServerLoadMonitoringAdditional() {
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
            //tmp.CalendarFrom = CalendarFrom;
            //tmp.CalendarTo = CalendarTo;
            //tmp.GridTask = GridServerLoadMonitoringsData;

			Base.DataContext = tmp;
			Base.Visibility = Visibility.Visible;
            //Список контролов на загрузку настроек
            tmp.CommandLoadSettingControlOnOpen.Execute(null);
            //******************************************
        }

        public void OnElHotKey(KeyEventArgs e)
        {

            if (e.Key == Key.F5 && Base.DataContext is ServerLoadMonitoringDataViewModel getAllSupply)
                if (getAllSupply.ActiveTab == 0)
                    getAllSupply.CommandGetTaskList.Execute(null);
            //if (e.Key == Key.F && Base.DataContext is ServerLoadMonitoringDataViewModel search)
            //    if (search.ActiveTab == 0) GridServerLoadMonitoringsData.ShowSearchPanel=true;

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
	}
}
