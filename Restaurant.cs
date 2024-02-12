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
            CancellationTokenSource cts = HelperMethods.GenerateCTS();
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
                Thread.Sleep(1);
                if (cts.Token.IsCancellationRequested)
                {
                    HelperMethods.DispoteCTS(cts);
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
                Thread.Sleep(1);
                if (cts.Token.IsCancellationRequested)
                {
                    HelperMethods.DispoteCTS(cts);
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
