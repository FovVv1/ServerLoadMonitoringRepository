using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServerLoadMonitoringDataModels;
using ServerLoadMonitoringDataModels.Enums;

namespace ServerLoadMonitoring.Helpers {
	public class JobViewModel : Job, INotifyPropertyChanged {
		#region OnPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string prop = "") {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
		#endregion


		public string AgentName { get; set; }

		[JsonIgnore]
		public int Priority
		{
			get
			{
				switch (Status)
				{
					case EJobStatus.Error:
						return 1;
					case EJobStatus.Stopped:
						return 2;
					case EJobStatus.Waiting:
						return 3;
					case EJobStatus.Started:
						return 4;
					default:
						return 0;
				}
			}
		}

		
	}
}
