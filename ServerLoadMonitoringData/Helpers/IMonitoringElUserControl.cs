using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ElControls.Interface;

namespace ServerLoadMonitoring.Helpers {
	public interface IMonitoringElUserControl:IElUserControl {
		//время обновления данных 
		int RefreshTime { get; set; }
		//флаг режима автообновления данных
		bool IsAutoRefresh { get; set; }
		//Комманда обновления данных
		void RefreshData();
	}
}
