using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElMessage;
using ServerLoadMonitoringDataModels.Enums;

namespace ServerLoadMonitoring {
	public static class ConfigPlugin {

		public static string LogsFilePath = @"D:\ClientLogs\";

        public static ElConnectionServer connectionElServer;
		public static string BasePath;
		private static EPermissions _SecurityKey;
		public static Guid InstanceKey { get; set; }

		public static EPermissions SecurityKey {
			get { return _SecurityKey; }
		}

		public static string ELAppData { get; set; }
		

		public static MainControlViewModel GlobalModel { get; set; }

		public static string TargetContext { get; set; }
		public static int ProcessId { get; set; }

		public static bool IsRefreshDataEnabled { get; set; }

		public static void SetSecurityKey(ulong key) {
			_SecurityKey = (EPermissions)key;
			GlobalModel.ShowPage();
		}


	}
}
