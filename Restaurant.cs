using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Restaurant
{
    internal interface IReturnWorkingHours
    {
        static abstract void SetWorkingHours();
    }
    internal class Restaurant : IReturnWorkingHours
    {
        public static void SetWorkingHours()
        {
            var optionSelected = -1;

            List<string> menuOptions =
            [
                "Mon (from 10:00 to 22:00)",
                "Tue (from 10:00 to 22:00)",
                "Wed (from 10:00 to 22:00)",
                "Thu (from 10:00 to 22:00)",
                "Fri (from 10:00 to 22:00)",
                "Sat (from 10:00 to 22:00)",
                "Sun (day off)"
            ];

            optionSelected = HelperMethods.MenuInteraction(menuOptions);

            Console.WriteLine(menuOptions[optionSelected]);
        }
    }
}
