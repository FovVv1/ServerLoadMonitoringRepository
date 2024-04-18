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
using ElDataModels.BasicFunctions;
using ElMessage;
using Newtonsoft.Json;

namespace ServerLoadMonitoring
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl : UserControl,IDisposable,IElHotKey
    {
		public MainControl(ElPluginClientSettings setting) {
			ConfigPlugin.connectionElServer = setting.ConnectionElServer;
			ConfigPlugin.BasePath = setting.PluginFolderPath;
			ConfigPlugin.TargetContext = null;
			ConfigPlugin.GlobalModel = new MainControlViewModel();


			

        
				//Регестрируем идентификатор процесса
            ConfigPlugin.ProcessId = (int)setting.Parameters["PluginId"];

            if(setting.Parameters.ContainsKey("SecurityKey"))
            ConfigPlugin.SetSecurityKey((ulong)setting.Parameters["SecurityKey"]);

            if (setting.Parameters.ContainsKey("EL_APPDATA"))
	            ConfigPlugin.ELAppData = (string)setting.Parameters["EL_APPDATA"];
        

         ConfigPlugin.connectionElServer.SendMessage(new ElMessageClient("BasicFunctions", "GetUserSettings", Response_StartViewModel),
	         JsonConvert.SerializeObject(new { UserId = ConfigPlugin.connectionElServer.CurrentUser.UserID, PluginName = "ServerLoadMonitoring" }));



        

		}


		private void Response_StartViewModel(string data) {
			Dictionary<string, UserSetting> _param;
			try {
				_param = JsonConvert.DeserializeObject<List<UserSetting>>(data).ToDictionary(u => u.KeyName);
			} catch {
				_param = new Dictionary<string, UserSetting>();
			}


			

			

			if (_param.ContainsKey("IsRefreshDataEnabled"))
            //Разрешение на обновление данных
            ConfigPlugin.IsRefreshDataEnabled = bool.TryParse(_param["IsRefreshDataEnabled"].Value, out var isRefreshDataEnabled);

         

         Application.Current.Dispatcher.Invoke(() =>
         {
	         InitializeComponent();
	         Base.DataContext = ConfigPlugin.GlobalModel;

                if (_param.ContainsKey("FullScreen"))
                    ConfigPlugin.GlobalModel.IsKioskMode = true;
            });
        }


      #region DISPOSE

      private bool _disposed = false;

        // реализация интерфейса IDisposable.
        public void Dispose()
        {
            Dispose(true);
            // подавляем финализацию
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {

                foreach (var viewModel in ConfigPlugin.GlobalModel.ControlsList)
                {
	                //if (viewModel is MainControlModel tmpss)
		                if (viewModel is IDisposable dis)
			                dis.Dispose();
                }
            }
            // освобождаем неуправляемые объекты
            _disposed = true;
        }

        // Деструктор
        ~MainControl()
        {
            Dispose(false);
        }
        #endregion
        public void OnElHotKey(KeyEventArgs e)
        {
            //Вызов пощи для расширения
            if (e.Key == Key.F1)
            {
                ConfigPlugin.GlobalModel.CommandStartHelper.Execute(null);
                return;
            }

            
            //Отключение полноэкранного режима
            if (e.Key == Key.B)
            {
                ConfigPlugin.GlobalModel.IsKioskMode = !ConfigPlugin.GlobalModel.IsKioskMode;
                return;
            } 

            //Предыдущий слайд
            if (e.Key == Key.Left)
            {
                ConfigPlugin.GlobalModel.CommandBackSlide.Execute(null);
                return;
            } 
            
            //Следующий слайд
            if (e.Key == Key.Right)
            {
                ConfigPlugin.GlobalModel.CommandNextSlide.Execute(null);
                return;
            }




            if (ConfigPlugin.GlobalModel.ControlsList[ConfigPlugin.GlobalModel.SelectedIndex] is IElHotKey vm) vm.OnElHotKey(e);

        }

        public void OnElScanData(List<KeyEventArgs> listKey)
        {
            if (ConfigPlugin.GlobalModel.ControlsList[ConfigPlugin.GlobalModel.SelectedIndex] is IElHotKey vm) vm.OnElScanData(listKey);
        }

	}
}
