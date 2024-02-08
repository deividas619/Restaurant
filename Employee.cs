using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public bool IsEmployed { get; private set; }
        public bool IsManager { get; private set; }

        public Employee(int id, string name, string surname, string username, string password, bool isEmployed, bool isManager)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Username = username;
            Password = password;
            IsEmployed = isEmployed;
            IsManager = isManager;
        }
        public static void ListAllEmployees(List<Employee> Employees)
        {
            Console.WriteLine("");
            foreach (var employee in Employees)
            {
                Console.WriteLine($"{employee.Id}. {employee.Name} {employee.Surname} ({employee.Username})");
                Console.Write("   Works: ");
                if (employee.IsEmployed)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("yes\n");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("no\n");
                    Console.ResetColor();
                }
                Console.Write("   Manages: ");
                if (employee.IsManager)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("yes\n");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("no\n");
                    Console.ResetColor();
                }
            }
        }
        public static void AddEmployee(List<Employee> Employees)
        {
            Console.WriteLine("\nProvide new employee information:");
            Console.Write("Name: ");
            var newEmployeeName = Console.ReadLine();
            if (HelperMethods._cancellation.Token.IsCancellationRequested)
            {
                Console.Write("\nCancelled... ");
                HelperMethods.ProceedIn(3);
                HelperMethods._cancellation.Cancel();
                return;
            }

            Console.Write("Surname: ");
            var newEmployeeSurname = Console.ReadLine();
            if (HelperMethods._cancellation.Token.IsCancellationRequested)
            {
                Console.Write("\nCancelled... ");
                HelperMethods.ProceedIn(3);
                HelperMethods._cancellation.Cancel();
                return;
            }

            if (!string.IsNullOrEmpty(newEmployeeName) && !string.IsNullOrEmpty(newEmployeeSurname))
            {
                string newEmployeeUsername = $"{newEmployeeName.Substring(0, Math.Min(3, newEmployeeName.Length))}{newEmployeeSurname.Substring(0, Math.Min(3, newEmployeeSurname.Length))}";
                var newEmployeePassword = "1234";
                Employees.Add(new Employee(Employees.Count + 1, newEmployeeName, newEmployeeSurname, newEmployeeUsername, newEmployeePassword, true, false));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\nSuccessfully added ");
                Console.ResetColor();
                Console.Write($"{newEmployeeName} {newEmployeeSurname} ({newEmployeeUsername})! ");
                Program.UpdateDatabase(Program.database);
                HelperMethods.ReturnToMainMenu();
            }
            else
            {
                HelperMethods.PrintError("Some fields were left blank!");
                HelperMethods.ReturnToMainMenu();
            }
        }
        public static void RemoveEmployee(List<Employee> Employees)
        {
            List<string> employeesToRemove = Employees.Where(e => e is { IsEmployed: true, IsManager: false }).Select(e => $"{e.Name} {e.Surname}").ToList();
            if (employeesToRemove.Count > 0)
            {
                var optionSelected = HelperMethods.MenuInteraction(employeesToRemove);
                var employeeToRemove = Employees.FirstOrDefault(e => $"{e.Name} {e.Surname}" == employeesToRemove[optionSelected]);
                while (true)
                {
                    Console.Write($"\nRemove {employeeToRemove.Name} {employeeToRemove.Surname} ({employeeToRemove.Username})? (y/N): ");
                    var input = Console.ReadLine();
                    if (input is "y" or "Y")
                    {
                        employeeToRemove.IsEmployed = false;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("\nSuccessfully removed ");
                        Console.ResetColor();
                        Console.Write($"{employeeToRemove.Name} {employeeToRemove.Surname} ({employeeToRemove.Username})! ");
                        Program.UpdateDatabase(Program.database);
                        HelperMethods.ReturnToMainMenu();
                        break;
                    }
                    else if (input is "n" or "N")
                    {
                        break;
                    }
                    else
                    {
                        HelperMethods.PrintError("Incorrect selection!");
                    }
                }
            }
            else
            {
                HelperMethods.PrintError("\nNo employees found to remove!");
                HelperMethods.ReturnToMainMenu();
            }
        }
        public static void EditEmployeeInfo(List<Employee> Employees)
        {
            List<string> employeesToEdit = Employees.Select(e => $"{e.Name} {e.Surname}").ToList();
            var optionSelected = HelperMethods.MenuInteraction(employeesToEdit);
            var employeeToEdit = Employees.FirstOrDefault(e => $"{e.Name} {e.Surname}" == employeesToEdit[optionSelected]);
            while (true)
            {
                Console.Write($"\nEdit {employeeToEdit.Name} {employeeToEdit.Surname} ({employeeToEdit.Username})? (y/N): ");
                var input = Console.ReadLine();
                if (input is "y" or "Y")
                {
                    optionSelected = HelperMethods.MenuInteraction(
                    [
                        "0. Return",
                        "1. Name",
                        "2. Surname",
                        "3. Username",
                        "4. Password",
                        "5. Employment status",
                        "6. Management status"
                    ]);

                    switch (optionSelected)
                    {
                        case 0:
                            return;
                        case 1:
                            Console.Write("\nNew name: ");
                            string newName = Console.ReadLine();
                            employeeToEdit.Name = newName;
                            Program.UpdateDatabase(Program.database);
                            return;
                        case 2:
                            Console.Write("\nNew surname: ");
                            string newSurname = Console.ReadLine();
                            employeeToEdit.Surname = newSurname;
                            Program.UpdateDatabase(Program.database);
                            return;
                        case 3:
                            Console.Write("\nNew username: ");
                            string newUsername = Console.ReadLine();
                            employeeToEdit.Username = newUsername;
                            Program.UpdateDatabase(Program.database);
                            return;
                        case 4:
                            Console.Write("\nNew password: ");
                            string newPassword = HelperMethods.GetUserEncryptedPassword();
                            employeeToEdit.Password = newPassword;
                            Program.UpdateDatabase(Program.database);
                            return;
                        case 5:
                            Console.Write($"\nCurrently employed: {employeeToEdit.IsEmployed}. Switch? (y/N): ");
                            input = Console.ReadLine();
                            if (input is "y" or "Y")
                            {
                                if (employeeToEdit.IsEmployed == false ? employeeToEdit.IsEmployed = true : employeeToEdit.IsEmployed = false);
                                Program.UpdateDatabase(Program.database);
                            }
                            else if (input is "n" or "N")
                            {
                                return;
                            }
                            else
                            {
                                HelperMethods.PrintError("Incorrect selection!");
                            }
                            break;
                        case 6:
                            Console.Write($"\nCurrently manager: {employeeToEdit.IsManager}. Switch? (y/N): ");
                            input = Console.ReadLine();
                            if (input is "y" or "Y")
                            {
                                if (Employees.Count(e => e.IsManager) > 1)
                                {
                                    if (employeeToEdit.IsManager == false ? employeeToEdit.IsManager = true : employeeToEdit.IsManager = false);
                                    Program.UpdateDatabase(Program.database);
                                }
                                else
                                {
                                    HelperMethods.PrintError("Restaurant cannot be without a manager!");
                                }
                            }
                            else if (input is "n" or "N")
                            {
                                return;
                            }
                            else
                            {
                                HelperMethods.PrintError("Incorrect selection!");
                            }
                            break;
                    }
                }
                else if (input is "n" or "N")
                {
                    break;
                }
                else
                {
                    HelperMethods.PrintError("Incorrect selection!");
                }
                return;
            }
        }
    }
}
