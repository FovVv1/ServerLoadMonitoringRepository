using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using NLog;
using WarehouseСontexts.ContextSelector;

namespace ServerLoadMonitoring.Helpers {
	/// <summary>
	/// Interaction logic for JobsHeatmapControl.xaml
	/// </summary>
	public partial class JobsHeatmapControl:IMonitoringElUserControl {
		public JobsHeatmapControl() {
			InitializeComponent();
			Base.DataContext = new JobsHeatmapControlViewModel();
		}

		public ContextSelector contextSelector { get; set; }
		public int RefreshTime { get; set; }
		public bool IsAutoRefresh { get; set; }

		public void UpdateData(List<JobViewModel> jobs)
		{
			try
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					if (Base.DataContext is JobsHeatmapControlViewModel baseModel) {
						baseModel.Jobs = new ObservableCollection<JobViewModel>(jobs);
					}
				});
				
			}
			catch (Exception e)
			{
				LogManager.GetCurrentClassLogger().Error(e.ToString().Replace("\r\n", ""));
			}
		}

		public void RefreshData()
		{
			

		}
	}
}
