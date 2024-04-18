using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerLoadMonitoringDataModels.Enums;
using Telerik.Windows.Controls;

namespace ServerLoadMonitoring.Helpers {
	public class JobsHeatmapControlViewModel:ViewModelBase {

		public JobsHeatmapControlViewModel()
		{

		}

		private ObservableCollection<JobViewModel> _Jobs;
		public ObservableCollection<JobViewModel> Jobs {
			get => _Jobs;
			set {
				_Jobs = value;
				OnPropertyChanged("Jobs");
				OnPropertyChanged("Messages");
			}
		}

		public ObservableCollection<string> Messages
		{
			get
			{
				
				var lastMessages = new ObservableCollection<string>();
				var jobs = _Jobs?.ToList().Where(i => i.Status != EJobStatus.OK)?.OrderBy(i => i.Priority).ToList();

				if (jobs != null && jobs.Any())
				{
					lastMessages.Add(jobs[0].Description + ": " + jobs[0].LastMessage);
					if (jobs.Count() >= 2)
					{
						lastMessages.Add(jobs[1].Description + ": " + jobs[1].LastMessage);
					}
					
					//lastMessages.ad
				}


				return lastMessages;
			}
		}
	}
}
