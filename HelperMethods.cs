﻿using Microsoft.VisualBasic.FileIO;
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
        public static List<string> BreadCrumb = new List<string>();
        public static string CurrentUser = null!;
        private static Timer _autoLogout;
        private static bool _keyPressed;
        
        public static void InitMainMenu()
        {
            BreadCrumb.Add("Main menu");
            
            var Tables = Program.database.Tables;
            var Items = Program.database.Item;

            do
            {
                var optionSelected = MenuInteraction([
                    "0. Exit",
                    "1. Login",
                    "2. Check table availability",
                    "3. Make table reservation",
                    "4. Place an order",
                    "5. List on-going orders",
                    "6. Close order for a table"
                ]);

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
                        BreadCrumb.Add("Table reservation");
                        Table.MakeTableReservation();
                        BreadCrumb.Remove("Table reservation");
                        continue;
                    case 4:
                        BreadCrumb.Add("Place an order");
                        Order.PlaceOrder(Tables, Items);
                        BreadCrumb.Remove("Place an order");
                        continue;
                    case 5:
                        Order.ListOngoingOrders();
                        continue;
                    case 6:
                        BreadCrumb.Add("Close an order");
                        Order.CloseTable(Tables, BreadCrumb);
                        BreadCrumb.Remove("Close an order");
                        continue;
                }
            }
            while (true);
        }
        public static void ProceedIn(int from)
        {
            Console.Write("Proceeding in: ");
            for (var i = from; i >= 1; i--)
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
            Console.WriteLine("# Welcome to JAMMY #");
            Console.WriteLine("####################");
            if (CurrentUser != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Logged in as: ");
                Console.ResetColor();
                Console.Write(CurrentUser + "\n");
            }

            
            if (BreadCrumb.Count > 0)
            {
                Console.WriteLine("");
                Console.Write(string.Join(" => ", BreadCrumb));
                Console.WriteLine("\n");
            }
        }
        public static int MenuInteraction(List<string> menuOptions)
        {
            var coordinateBuffer = CurrentUser != null ? 4 : 3;
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
            CancellationTokenSource cts = GenerateCTS();
            var Employees = Program.database.Employees;
            string username = null;

            while (true)
            {
                Console.Write("\nUsername: ");
                username = Console.ReadLine();
                Thread.Sleep(1);
                if (cts.IsCancellationRequested) // skips the check even if ctrl + c is pressed but on 2nd iteration the if check works
                {
                    DispoteCTS(cts);
                    return;
                }

                if (Employees.Any(e => e.Username == username))
                {
                    var loggedInEmployee = Employees.FirstOrDefault(e => e.Username == username);
                    if (loggedInEmployee.IsManager && loggedInEmployee.IsEmployed)
                    {
                        break;
                    }
                    else if (loggedInEmployee is { IsManager: true, IsEmployed: false })
                    {
                        PrintError("User provided is a manager but the account has been disabled!");
                        continue;
                    }
                    else if (loggedInEmployee is { IsManager: false, IsEmployed: true })
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
                if (GetUserEncryptedPassword() == Employees.First(e => e.Username == username).Password)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nLogin successful!");
                    Console.ResetColor();
                    ProceedIn(3);
                    CurrentUser = username;
                    _keyPressed = false;
                    if (Employees.First(e => e.Username == username).IsManager == true)
                    {
                        //StartAutoLogoutTimer();
                    }
                    var optionSelected = -1;
                    BreadCrumb.Add("Manager menu");

                    do
                    {
                        optionSelected = MenuInteraction([
                            "0. Logout",
                            "1. Employee management",
                            "2. Item management",
                            "3. Restaurant management"
                        ]);

                        switch (optionSelected)
                        {
                            case 0:
                                Console.Write("\nLogging out... ");
                                Logout();
                                ProceedIn(3);
                                return;
                            case 1:
                                BreadCrumb.Add("Employee management");
                                EmployeeManagement();
                                continue;
                            case 2:
                                BreadCrumb.Add("Item management");
                                ItemManagement();
                                continue;
                            case 3:
                                BreadCrumb.Add("Restaurant management");
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
        private static void Logout()
        {
            CurrentUser = null;
            StopAutoLogoutTimer();
            BreadCrumb.Remove("Manager menu");
            //return;
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
                    if (password is { Length: > 0 })
                    {
                        password = password.Remove(password.Length - 1, 1);
                        Console.Write("\b \b");
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
        private static void StartAutoLogoutTimer()
        {
            _autoLogout = new Timer(_ =>
            {
                if (!_keyPressed)
                {
                    Console.SetCursorPosition(0, 13);
                    Console.Write("Logging out due to inactivity... ");
                    ProceedIn(3);
                    StopAutoLogoutTimer();
                    Logout();
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
                optionSelected = MenuInteraction([
                    "0. Return",
                    "1. List all employees",
                    "2. Add an employee",
                    "3. Remove an employee",
                    "4. Edit employee info"
                ]);

                switch (optionSelected)
                {
                    case 0:
                        BreadCrumb.Remove("Employee management");
                        return;
                    case 1:
                        Employee.ListAllEmployees(Employees);
                        ReturnToMainMenu();
                        continue;
                    case 2:
                        Employee.AddEmployee(Employees);
                        continue;
                    case 3:
                        BreadCrumb.Add("Remove an employee");
                        Employee.RemoveEmployee(Employees);
                        BreadCrumb.Remove("Remove an employee");
                        continue;
                    case 4:
                        BreadCrumb.Add("Edit employee info");
                        Employee.EditEmployeeInfo(Employees);
                        BreadCrumb.Remove("Edit employee info");
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
                optionSelected = MenuInteraction([
                    "0. Return",
                    "1. Edit restaurant working hours"
                ]);

                switch (optionSelected)
                {
                    case 0:
                        BreadCrumb.Remove("Restaurant management");
                        continue;
                    case 1:
                        BreadCrumb.Add("Working hours");
                        Restaurant.SetWorkingHours();
                        BreadCrumb.Remove("Working hours");
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
                optionSelected = MenuInteraction([
                    "0. Return",
                    "1. List all items",
                    "2. Add an item",
                    "3. Remove an item",
                    "4. Edit item info"
                ]);

                switch (optionSelected)
                {
                    case 0:
                        BreadCrumb.Remove("Item management");
                        return;
                    case 1:
                        Item.ListAllItems(Items);
                        ReturnToMainMenu();
                        continue;
                    case 2:
                        Item.AddItem(Items);
                        continue;
                    case 3:
                        BreadCrumb.Add("Remove an item");
                        Item.RemoveItem(Items);
                        BreadCrumb.Remove("Remove an item");
                        continue;
                    case 4:
                        BreadCrumb.Add("Edit item info");
                        Item.EditItemInfo(Items);
                        BreadCrumb.Remove("Edit item info");
                        continue;
                }
            }
            while (optionSelected != 0);
        }

        public static CancellationTokenSource GenerateCTS()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, args) =>
            {
                //Console.Write("Cancelling...");
                //HelperMethods.ProceedIn(3);
                args.Cancel = true;
                cts.Cancel();
            };
            return cts;
        }

        public static void DispoteCTS(CancellationTokenSource cts)
        {
            Console.Write("\nCancelled... ");
            ProceedIn(3);
            cts.Dispose();
        }
    }
}
