using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Restaurant
{
    internal class HelperMethods
    {
        public static CancellationTokenSource _cancellation;
        public static List<string> breadCrumb = new List<string>();
        public static string CurrentUser = null!;
        private static Timer _autoLogout;
        private static bool _keyPressed = false;
        
        public static void InitMainMenu(CancellationTokenSource cts)
        {
            _cancellation = cts;
            breadCrumb.Add("Main menu");
            
            var Tables = Program.database.Tables;
            var Items = Program.database.Item;
            var optionSelected = -1;

            do
            {
                List<string> menuOptions =
                [
                    "0. Exit",
                    "1. Login",
                    "2. Check table availability",
                    "3. Make table reservation",
                    "4. Place an order",
                    "5. List on-going orders",
                    "6. Close an order"
                ];
                optionSelected = MenuInteraction(menuOptions);

                switch (optionSelected)
                {
                    case 0:
                        Console.WriteLine("\nClosing the application! Goodbye...");
                        Environment.Exit(0);
                        continue;
                    case 1:
                        Login(ref CurrentUser);
                        continue;
                    case 2:
                        Table.CheckTableAvailability();
                        ReturnToMainMenu();
                        continue;
                    case 3:
                        breadCrumb.Add("Table reservation");
                        Table.MakeTableReservation();
                        breadCrumb.Remove("Table reservation");
                        continue;
                    case 4:
                        breadCrumb.Add("Place an order");
                        Order.PlaceOrder(Tables, Items);
                        breadCrumb.Remove("Place an order");
                        continue;
                    case 5:
                        Order.ListOngoingOrders();
                        continue;
                    case 6:
                        Order.CloseTable(Tables);
                        continue;
                }
            }
            while (optionSelected != 0);
        }
        public static void ProceedIn(int from)
        {
            Console.Write("Proceeding in: ");
            for (var i = from; i >= 0; i--)
            {
                Console.Write(i);
                Thread.Sleep(1000);
                Console.Write("\b \b");
                if (i == 0) Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
            }
        }
        private static void PrintWelcomeScreen()
        {
            Console.WriteLine("####################");
            Console.WriteLine($"# Welcome to JAMMY #");
            Console.WriteLine("####################");
            if (CurrentUser != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"Logged in as: ");
                Console.ResetColor();
                Console.Write(CurrentUser + "\n");
            }

            
            if (breadCrumb.Count > 0)
            {
                Console.WriteLine("");
                Console.Write(string.Join(" => ", breadCrumb));
                Console.WriteLine("\n");
            }
        }
        public static int MenuInteraction(List<string> menuOptions)
        {
            var coordinateBuffer = 3;
            if (CurrentUser != null)
            {
                coordinateBuffer = 4;
            }
            var option = 0;
            var selected = false;
            var cursorPositionColumn = 0;
            var cursorPositionRow = 4 + coordinateBuffer;

            while (!selected)
            {
                Console.Clear();
                PrintWelcomeScreen();

                Console.WriteLine("Select:");
                for (var i = 0; i < menuOptions.Count; i++)
                {
                    Console.WriteLine($"{(option == i ? "> " : "  ")}{menuOptions[i]}");
                }

                var initialCursorPosition = Console.GetCursorPosition();
                Console.SetCursorPosition(cursorPositionColumn, cursorPositionRow);
                var key = Console.ReadKey();
                _keyPressed = true;
                //_autoLogout.Change(10000, 10000);
                switch (key.Key)
                {
                    case ConsoleKey.DownArrow:
                        Console.WriteLine($"{(option == 0 ? "> " : menuOptions[option])}");
                        if (option == menuOptions.Count - 1)
                        {
                            option = 0;
                            cursorPositionRow = 4 + coordinateBuffer;
                        }
                        else
                        {
                            option++;
                            cursorPositionRow++;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        Console.WriteLine($"{(option > 0 ? "> " : menuOptions[option])}");
                        if (option == 0)
                        {
                            option = menuOptions.Count - 1;
                            cursorPositionRow = cursorPositionRow + menuOptions.Count - 1;
                        }
                        else
                        {
                            option--;
                            cursorPositionRow--;
                        }
                        break;
                    case ConsoleKey.Enter:
                        selected = true;
                        Console.SetCursorPosition(initialCursorPosition.Left, initialCursorPosition.Top);
                        break;
                }
            }
            return option;
        }
        public static void ReturnToMainMenu()
        {
            Console.WriteLine("\nTo return press 'q'");
            while (true)
            {
                var i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Q)
                {
                    return;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Incorrect input!");
                    Console.ResetColor();
                    Console.Write(" Try again...\n");
                }
            }
        }
        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(message);
            Console.ResetColor();
            Console.Write(" Try again...\n");
        }
        private static void Login(ref string CurrentUser)
        {
            var Employees = Program.database.Employees;
            string username = null;

            while (true)
            {
                Console.Write("\nUsername: ");
                username = Console.ReadLine();
                if (_cancellation.Token.IsCancellationRequested)
                {
                    Console.Write("\nCancelled... ");
                    ProceedIn(3);
                    _cancellation.Cancel();
                    return;
                }

                if (Employees.FirstOrDefault(e => e.Username == username) != null)
                {
                    var loggedInEmployee = Employees.FirstOrDefault(e => e.Username == username);
                    if (loggedInEmployee.IsManager == true && loggedInEmployee.IsEmployed == true)
                    {
                        break;
                    }
                    else if (loggedInEmployee.IsManager == true && loggedInEmployee.IsEmployed == false)
                    {
                        PrintError("User provided is a manager but the account has been disabled!");
                        continue;
                    }
                    else if (loggedInEmployee.IsManager == false && loggedInEmployee.IsEmployed == true)
                    {
                        PrintError("User provided is not a manager!");
                        continue;
                    }
                }
                else
                {
                    PrintError("User not found!");
                }
            }

            var counter = 1;
            do
            {
                Console.Write("Password: ");
                if (GetUserEncryptedPassword() == Employees.FirstOrDefault(e => e.Username == username).Password)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nLogin successful!");
                    Console.ResetColor();
                    //ProceedIn(3);
                    CurrentUser = username;
                    _keyPressed = false;
                    if (Employees.FirstOrDefault(e => e.Username == username)?.IsManager == true)
                    {
                        //StartAutoLogoutTimer(_cancellation);
                    }
                    var optionSelected = -1;
                    breadCrumb.Add("Manager menu");

                    do
                    {
                        List<string> menuOptions =
                        [
                            "0. Logout",
                            "1. Employee management",
                            "2. Item management",
                            "3. Restaurant management"
                        ];

                        optionSelected = MenuInteraction(menuOptions);

                        switch (optionSelected)
                        {
                            case 0:
                                Console.Write("\nLogging out... ");
                                Logout(_cancellation);
                                ProceedIn(3);
                                return;
                            case 1:
                                breadCrumb.Add("Employee management");
                                EmployeeManagement();
                                continue;
                            case 2:
                                breadCrumb.Add("Item management");
                                ItemManagement();
                                continue;
                            case 3:
                                breadCrumb.Add("Restaurant management");
                                RestaurantManagement();
                                continue;
                        }
                    }
                    while (optionSelected != 0);

                    break;
                }
                else
                {
                    if (counter < 3)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"\nIncorrect password!");
                        Console.ResetColor();
                        Console.Write($" {counter}/3 tries used. Try again...\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("\nToo many incorrect tries!");
                        Console.ResetColor();
                        Console.Write(" Cancelling login...\n");
                        ProceedIn(3);
                        break;
                    }
                    counter++;
                }
            }
            while (counter < 4);
        }
        private static void Logout(CancellationTokenSource _cancellation)
        {
            CurrentUser = null;
            StopAutoLogoutTimer();
            breadCrumb.Remove("Manager menu");
            return;
            //InitMainMenu(_cancellation);
        }
        public static string GetUserEncryptedPassword()
        {
            string password = null;
            while (true)
            {
                var i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (password == null)
                    {
                        continue;
                    }
                    else if (password.Length > 0)
                    {
                        password = password.Remove(password.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (i.KeyChar != '\u0000')
                {
                    password += (i.KeyChar);
                    Console.Write("*");
                }
            }
            return password;
        }
        private static void StartAutoLogoutTimer(CancellationTokenSource _cancellation)
        {
            _autoLogout = new Timer(_ =>
            {
                if (!_keyPressed)
                {
                    Console.SetCursorPosition(0, 13);
                    Console.Write("Logging out due to inactivity... ");
                    ProceedIn(3);
                    StopAutoLogoutTimer();
                    Logout(_cancellation);
                }
                _keyPressed = false;
            }, null, 10000, 10000);
        }
        private static void StopAutoLogoutTimer()
        {
            if (_autoLogout != null)
            {
                _autoLogout.Dispose();
                _autoLogout = null;
            }
        }
        private static void EmployeeManagement()
        {
            var Employees = Program.database.Employees;
            var optionSelected = -1;

            do
            {
                List<string> menuOptions =
                [
                    "0. Return",
                    "1. List all employees",
                    "2. Add an employee",
                    "3. Remove an employee",
                    "4. Edit employee info",
                ];

                optionSelected = MenuInteraction(menuOptions);

                switch (optionSelected)
                {
                    case 0:
                        breadCrumb.Remove("Employee management");
                        return;
                    case 1:
                        Employee.ListAllEmployees(Employees);
                        ReturnToMainMenu();
                        continue;
                    case 2:
                        Employee.AddEmployee(Employees);
                        continue;
                    case 3:
                        breadCrumb.Add("Remove an employee");
                        Employee.RemoveEmployee(Employees);
                        breadCrumb.Remove("Remove an employee");
                        continue;
                    case 4:
                        breadCrumb.Add("Edit employee info");
                        Employee.EditEmployeeInfo(Employees);
                        breadCrumb.Remove("Edit employee info");
                        continue;
                }
            }
            while (optionSelected != 0);
        }
        private static void RestaurantManagement()
        {
            var optionSelected = -1;

            do
            {
                List<string> menuOptions =
                [
                    "0. Return",
                    "1. Edit restaurant working hours"
                ];

                optionSelected = MenuInteraction(menuOptions);

                switch (optionSelected)
                {
                    case 0:
                        breadCrumb.Remove("Restaurant management");
                        continue;
                    case 1:
                        breadCrumb.Add("Working hours");
                        Restaurant.SetWorkingHours();
                        breadCrumb.Remove("Working hours");
                        continue;
                }
            }
            while (optionSelected != 0);
        }
        private static void ItemManagement()
        {
            var Items = Program.database.Item;
            var optionSelected = -1;

            do
            {
                List<string> menuOptions =
                [
                    "0. Return",
                    "1. List all items",
                    "2. Add an item",
                    "3. Remove an item",
                    "4. Edit item info",
                ];

                optionSelected = MenuInteraction(menuOptions);

                switch (optionSelected)
                {
                    case 0:
                        breadCrumb.Remove("Item management");
                        return;
                    case 1:
                        Item.ListAllItems(Items);
                        ReturnToMainMenu();
                        continue;
                    case 2:
                        Item.AddItem(Items);
                        continue;
                    case 3:
                        breadCrumb.Add("Remove an item");
                        Item.RemoveItem(Items);
                        breadCrumb.Remove("Remove an item");
                        continue;
                    case 4:
                        breadCrumb.Add("Edit item info");
                        Item.EditItemInfo(Items);
                        breadCrumb.Remove("Edit item info");
                        continue;
                }
            }
            while (optionSelected != 0);
        }
    }
}
