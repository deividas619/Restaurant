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
        public static List<string> breadCrumb = new List<string>();
        public static string CurrentUser = null!;
        private static Timer autoLogout;
        private static bool keyPressed = false;
        public static void InitMainMenu()
        {
            breadCrumb.Add("Main menu");

            var testEmployees = new List<Employee>
            {
                new( 1, "Ned", "Flanders", "nedfla", "1234", true, true),
                new( 2, "Bob", "Bobinski", "bobbob", "1234", true, true),
                new( 3, "Tod", "Flanders", "todfla", "1234", false, true),
                new( 4, "Rod", "Flanders", "rodfla", "1234", true, false),
                new( 5, "Dennis", "Rodman", "denrod", "1234", false, false)
            };
            
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
                        Login(ref CurrentUser, testEmployees);
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
                        continue;
                    case 5:
                        return;
                    case 6:
                        return;
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

            
            if (breadCrumb.Count > 1)
            {
                Console.WriteLine("");
                breadCrumb.ForEach(b => Console.Write(b + " => "));
                //breadCrumb.ForEach(b => Console.Write(b.Split().ToString() + " => "));
                Console.WriteLine("\n");
            }
        }
        /*private static void PrintMenu()
        {
            Console.WriteLine("Select action:");
            List<string> menu = new List<string>
            {
                "    0. Login",
                "    1. See table availability",
                "    2. Place an order",
                "    3. Close an order",
                "    4. Make table reservation",
                "    5. Exit"
            };
            menu.ForEach(x => Console.WriteLine(x));
        }*/
        /*private static void MenuInteraction(ref string CurrentUser, List<Employee> testEmployees)
        {
            int selection = 0;
            SelectedMenuNumberAction(ref CurrentUser, testEmployees);
            do
            {
                Console.Clear();
                PrintWelcomeScreen();
                selection = SelectMenuNumber();
                if (selection != 5)
                {
                    SelectedMenuNumberAction(selection, ref CurrentUser, testEmployees);
                }
                else
                {
                    Console.WriteLine("\nExiting the application... Goodbye!");
                    break;
                }
            }
            while (selection != 5);
        }*/
        /*private static int SelectMenuNumber()
        {
            int selection = -1;
            do
            {
                if (int.TryParse(Console.ReadLine(), out selection) && selection >= 0 && selection <= 5)
                {
                    return selection;
                }
                else
                {
                    PrintError("Incorrect selection!");
                    Console.Write("Select action number: ");
                }
            }
            while (true);
        }*/
        public static int MenuInteraction(List<string> menuOptions)
        {
            var coordinateBuffer = 0;
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
        private static void ReturnToMainMenu()
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

        private static void Login(ref string CurrentUser, List<Employee> testEmployees)
        {
            ConsoleKeyInfo cki;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelHandler);
            string username = null;
            while (true)
            {
                Console.Write("\nUsername: ");
                cki = Console.ReadKey(true);
                username = Console.ReadLine();
                if (string.IsNullOrEmpty(username) || !Regex.IsMatch(username, @"^[a-zA-Z]"))
                {
                    PrintError("Incorrect format!");
                }
                else
                {
                    if (testEmployees.FirstOrDefault(e => e.Username == username) != null)
                    {
                        var loggedInEmployee = testEmployees.FirstOrDefault(e => e.Username == username);
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
                    PrintError("User not found!");
                }
            }

            var counter = 1;
            do
            {
                Console.Write("Password: ");
                if (GetUserEncryptedPassword() == testEmployees.FirstOrDefault(e => e.Username == username).Password)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nLogin successful!");
                    Console.ResetColor();
                    //ProceedIn(3);
                    CurrentUser = username;
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

                        //StartAutoLogoutTimer();

                        optionSelected = MenuInteraction(menuOptions);

                        switch (optionSelected)
                        {
                            case 0:
                                Console.Write("\nLogging out... ");
                                CurrentUser = null;
                                //StopAutoLogoutTimer();
                                breadCrumb.Remove("Manager menu");
                                ProceedIn(3);
                                return;
                            case 1:
                                breadCrumb.Add("Employee management");
                                EmployeeManagement(testEmployees);
                                continue;
                            case 2:
                                breadCrumb.Add("Item management");
                                //ItemManagement();
                                return;
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
                        InitMainMenu();
                        break;
                    }
                    counter++;
                }
            }
            while (counter < 4);
        }

        private static string GetUserEncryptedPassword()
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

        private static void CancelHandler(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
        }
        /*private static void StartAutoLogoutTimer()
        {
            autoLogout = new Timer(_ =>
            {
                if (!keyPressed)
                {
                    Console.SetCursorPosition(0, Console.WindowHeight - 1);
                    Console.WriteLine("Logging out due to inactivity...");
                    ProceedIn(3);
                    StopAutoLogoutTimer();
                }
            }, null, 5000, Timeout.Infinite);
        }*/

        /*private static void StopAutoLogoutTimer()
        {
            if (autoLogout != null)
            {
                autoLogout.Dispose();
                autoLogout = null;
            }
            Console.Clear();
            CurrentUser = null;
            PrintWelcomeScreen();
        }*/
        private static void EmployeeManagement(List<Employee> testEmployees)
        {
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
                        Employee.ListAllEmployees(testEmployees);
                        ReturnToMainMenu();
                        continue;
                    case 2:
                        Employee.AddEmployee(testEmployees);
                        continue;
                    case 3:
                        breadCrumb.Add("Remove an employee");
                        Employee.RemoveEmployee(testEmployees);
                        breadCrumb.Remove("Remove an employee");
                        continue;
                    case 4:
                        breadCrumb.Add("Edit employee info");
                        //Employee.EditEmployeeInfo();
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
    }
}
