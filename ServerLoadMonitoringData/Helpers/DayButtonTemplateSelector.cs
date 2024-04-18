using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.Calendar;

namespace ServerLoadMonitoring.Helpers
{
    public class DayButtonTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate DisabledTemplate { get; set; }

        public DataTemplate BookedDayTemplate { get; set; }
        public DataTemplate SpecialHolidayTemplate { get; set; }

        public List<DateTime> BookedDays { get; set; }
        public List<DateTime> SpecialHolidays { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var calendarButton = item as CalendarButtonContent;

            if (calendarButton.ButtonType == CalendarButtonType.Date)
            {
                //if (this.BookedDays.Any(a => a.SchemeDate.Day == currDate.Day))
                //{
                //    return this.BookedDayTemplate;
                //}

                //if (this.SpecialHolidays.Any(a => a.SchemeDate.Day == currDate.Day))
                //{
                //    return this.SpecialHolidayTemplate;
                //}
            }

            return this.DefaultTemplate;
        }
    }
}
