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
    public class DayButtonStyleSelector : StyleSelector
    {
        public Style SpecialStyleDisabledDay { get; set; }
        public Style SpecialStyleDraftScheme { get; set; }
        public Style SpecialStyleEnabledScheme { get; set; }
        public Style SpecialStyleActiveScheme { get; set; }

        public static bool ModeNewScheme { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {

            CalendarButtonContent content = item as CalendarButtonContent;

            if (content != null)
            {
                if (content.ButtonType == CalendarButtonType.Date)
                {
                    if (content.Date.DayOfWeek == DayOfWeek.Saturday || content.Date.DayOfWeek == DayOfWeek.Sunday) return SpecialStyleEnabledScheme;
                    return SpecialStyleActiveScheme;
                }
            }
            return base.SelectStyle(item, container);
        }
    }

}
