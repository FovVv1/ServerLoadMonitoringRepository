using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ElControls.Interface;
using Telerik.Windows.Controls;

namespace ServerLoadMonitoring {
	public class MainControlModel:ViewModelBase {
		public MainControlModel(Type controlName) {
			_typeControl = controlName;
		}
		private readonly Type _typeControl;
		private UserControl _content;
        /// <summary>Содержимое текущей страницы </summary>
		public UserControl Content 
        {
			get {
				//Проверяем установленый для контента usercontrol и при его существовании целевой контекст
				if (!(_content is IElUserControl tmp)) _content = (UserControl)Activator.CreateInstance(_typeControl);
				return _content;
			}
			set {
				_content = value;
				OnPropertyChanged("Content");
			}
		}
		/// <summary>Глиф текущей страницы на левой панели </summary>
		private string _image;
		public string Image {
			get => _image.Length == 0 ? "&#xe11d;" : _image;
            set
            { _image = value;
		      OnPropertyChanged("Image");
			}
		}

		private string _title;
        /// <summary>Наименование  страницы</summary>
		public string Title {
			get => _title;
            set {
				_title = value;
				OnPropertyChanged("Title");
			}
		}
        //Видимость страницы в левой панели
		private Visibility _visibility;
		public Visibility Visibility {
			get => _visibility;
            set {
                if (this._visibility == value) return;
                _visibility = value;
				OnPropertyChanged("Visibility");
            }
		}
	}
}
