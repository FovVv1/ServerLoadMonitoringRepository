using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerLoadMonitoring.Helpers
{
	public static class TaskCodes
	{
		public static Task PassCreation = new Task() { Code = "10.10.20.1", Comment = "Создание пропуска/приезд машины", Mask = 24 };
		public static Task WaitingProcessing = new Task() { Code = "10.10.21.10", Comment = "Поставка взята в обработку (связи пропуска с поставкой)", Mask = 24 };
		public static Task WaitingPurchasers = new Task() { Code = "10.10.21.11", Comment = "Ожидает ответа закуполога (поставка отправлена на доработку)", Mask = 24 };
		public static Task SendOnUnloading = new Task() { Code = "10.10.21.12", Comment = "Автомобиль допущен к выгрузке", Mask = 24 };
		public static Task Ramping = new Task() { Code = "10.10.21.13", Comment = "Автомобиль поставлен под рампу", Mask = 24 };

		//public static Task UnloadingResult = new Task(){Code = "10.10.21.1", Comment = "Результат выгрузки (главн)", Mask = 24};
		public static Task ClosingDocument = new Task() { Code = "10.10.21.2", Comment = "Таска на закрытие 1-го документа(ТТН)", Mask = 24 };
		public static Task SiteManagersTask = new Task() { Code = "10.10.21.3", Comment = "Закрытие машины для нач участка", Mask = 24 };
		public static Task StorekeepersTask = new Task() { Code = "10.10.21.4", Comment = "Закрытие машины для товароведа", Mask = 24 };
		public static Task LoadersTask = new Task() { Code = "10.10.21.5", Comment = "Закрытие машины для грузчика", Mask = 24 };
		public static Task Order = new Task() { Code = "10.10.21.6", Comment = "Информация о заказе в поставке", Mask = 24 };
		public static Task Document = new Task() { Code = "10.10.21.7", Comment = "Информация о связанном с ним документе", Mask = 24 };
		public static Task AllDocuments = new Task() { Code = "10.10.21.8", Comment = "Задача для вложения документов", Mask = 24 };
		public static Task AllOrders = new Task() { Code = "10.10.21.9", Comment = "Задача для вложения заказов", Mask = 24 };

	}

	public struct Task
	{
		public string Code;
		public string Comment;
		public byte Mask;

		public long CodeNumber => IPAddress.Parse(Code).Address;
	}

}
