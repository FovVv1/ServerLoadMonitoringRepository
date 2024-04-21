using ServerLoadMonitoringDataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ServerLoadMonitoring.MetricControl
{
    public class MetricDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is ServerUtilization)
                {
                    var metric = (ServerUtilization)value;

                    if (parameter != null && parameter.ToString() == "CpuUsage")
                        return metric.CpuUsage.ToString()+"%";
                    if (parameter != null && parameter.ToString() == "UsedMemoryPercents")
                        return metric.UsedMemoryPercents.ToString() + "%";
                    if (parameter != null && parameter.ToString() == "processorName")
                        return metric.processorName.ToString();
                    if (parameter != null && parameter.ToString() == "maxClockSpeed")
                        return metric.maxClockSpeed.ToString();
                    if (parameter != null && parameter.ToString() == "currentClockSpeed")
                        return metric.currentClockSpeed.ToString()+"MHz";
                    if (parameter != null && parameter.ToString() == "UpTime")
                        return string.Format("{0}д {1}ч {2}м {3}с ", metric.UpTime.Days, metric.UpTime.Hours, metric.UpTime.Minutes, metric.UpTime.Seconds);
                    if (parameter != null && parameter.ToString() == "Processes")
                        return metric.Processes.ToString();
                    if (parameter != null && parameter.ToString() == "InstalledMemory")
                        return string.Format("{0:F2} ГБ", metric.InstalledMemory);
                    if (parameter != null && parameter.ToString() == "UsedMemory")
                        return string.Format("{0:F2} ГБ", metric.UsedMemory);
                    if (parameter != null && parameter.ToString() == "availableMemory")
                        return string.Format("{0:F2} ГБ", (metric.InstalledMemory - metric.UsedMemory));
                    if (parameter != null && parameter.ToString() == "RefreshingData")
                        return string.Format("{0:HH:mm:ss}", metric.RefreshingData);
                }
                else if (value is DatabaseUtilization)
                {
                    var metric = (DatabaseUtilization)value;

                    if (parameter != null && parameter.ToString() == "TotalDatabaseSizeBytes")
                        return metric.TotalDatabaseSizeBytes.ToString();
                    if (parameter != null && parameter.ToString() == "UsedSpaceBytes")
                        return metric.UsedSpaceBytes.ToString();
                    if (parameter != null && parameter.ToString() == "FreeSpaceBytes")
                        return metric.FreeSpaceBytes.ToString();
                    if (parameter != null && parameter.ToString() == "TransactionLogSizeBytes")
                        return metric.TransactionLogSizeBytes.ToString();
                    if (parameter != null && parameter.ToString() == "UsedTransactionLogSpaceBytes")
                        return metric.UsedTransactionLogSpaceBytes.ToString();
                    if (parameter != null && parameter.ToString() == "FreeTransactionLogSpaceBytes")
                        return metric.FreeTransactionLogSpaceBytes.ToString();
                    if (parameter != null && parameter.ToString() == "NumberOfStoredProcedures")
                        return metric.NumberOfStoredProcedures.ToString();
                    if (parameter != null && parameter.ToString() == "RefreshingData")
                        return string.Format("{0:HH:mm:ss}", metric.RefreshingData);
                }
                else if (value is StorageUtilization)
                {
                    return "StorageUtilization без кода в конвертере,т.к. на него забили. Жаль(нет)";
                }

                return "Объкт не подходит конвертеру";
            }
            catch
            {
                return "Ошибка";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
