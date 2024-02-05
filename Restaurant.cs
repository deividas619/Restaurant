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
            var optionSelected = -1; //edit with placeholders using default time
            string defaultHourFrom = "10:00";
            string newHourFrom = null;
            string defaultHourTo = "22:00";
            string newHourTo = null;

            List<string> menuOptions =
            [
                $"Mon (from {defaultHourFrom} to {defaultHourTo})",
                $"Tue (from {defaultHourFrom} to {defaultHourTo})",
                $"Wed (from {defaultHourFrom} to {defaultHourTo})",
                $"Thu (from {defaultHourFrom} to {defaultHourTo})",
                $"Fri (from {defaultHourFrom} to {defaultHourTo})",
                $"Sat (from {defaultHourFrom} to {defaultHourTo})",
                $"Sun (day off)"
            ];

            optionSelected = HelperMethods.MenuInteraction(menuOptions);

            Console.WriteLine($"\nEditing: {menuOptions[optionSelected]}");
            while (true)
            {
                Console.Write("Enter new hours 'from' in 23:59 format: ");
                newHourFrom = Console.ReadLine();
                if (HelperMethods._cancellation.Token.IsCancellationRequested)
                {
                    Console.Write("\nCancelled... ");
                    HelperMethods.ProceedIn(3);
                    HelperMethods._cancellation.Cancel();
                    return;
                }

                if (Regex.IsMatch(newHourFrom, @"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$"))
                {
                    break;
                }
                else
                {
                    HelperMethods.PrintError("Incorrect format!");
                }
            }

            while (true)
            {
                Console.Write("Enter new hours 'to' in 23:59 format: ");
                newHourTo = Console.ReadLine();
                if (HelperMethods._cancellation.Token.IsCancellationRequested)
                {
                    Console.Write("\nCancelled... ");
                    HelperMethods.ProceedIn(3);
                    HelperMethods._cancellation.Cancel();
                    return;
                }

                if (Regex.IsMatch(newHourTo, @"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$"))
                {
                    break;
                }
                else
                {
                    HelperMethods.PrintError("Incorrect format!");
                }
            }

            menuOptions[optionSelected] = menuOptions[optionSelected].Replace(defaultHourFrom, newHourFrom).Replace(defaultHourTo, newHourTo);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Successfully edited to: ");
            Console.ResetColor();
            Console.Write(menuOptions[optionSelected]);
        }
    }
}
