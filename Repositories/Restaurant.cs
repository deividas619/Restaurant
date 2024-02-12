using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Restaurant.Services;

namespace Restaurant.Repositories
{
    internal interface IReturnWorkingHours
    {
        static abstract void SetWorkingHours();
    }
    internal class Restaurant : IReturnWorkingHours
    {
        public static string Name { get; set; }
        private static List<Employee> Employees;
        public static List<string> WorkingHours = Program.database.WorkingHours;

        public Restaurant(string name, List<Employee> employees)
        {
            Name = name;
            Employees = employees;
        }
        public static void SetWorkingHours()
        {
            var optionSelected = -1;
            string newHourFrom = null;
            string newHourTo = null;

            List<string> menuOptions = new List<string>(WorkingHours.Select(x => x.ToString()).ToList());
            optionSelected = HelperMethods.MenuInteraction(menuOptions);
            string selectedDayOfWeek = menuOptions[optionSelected].Split(' ')[0];

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

                if (Regex.IsMatch(newHourFrom, "^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$"))
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

                if (Regex.IsMatch(newHourTo, "^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$"))
                {
                    break;
                }
                else
                {
                    HelperMethods.PrintError("Incorrect format!");
                }
            }
            string newWorkingHours = $"{selectedDayOfWeek} (from {newHourFrom} to {newHourTo})";
            menuOptions[optionSelected] = newWorkingHours;
            WorkingHours = menuOptions;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\nSuccessfully edited to: ");
            Console.ResetColor();
            Console.Write(menuOptions[optionSelected]);
            Program.database.WorkingHours = menuOptions;
            Program.UpdateDatabase(Program.database);
            HelperMethods.ReturnToMainMenu();
        }
    }
}
