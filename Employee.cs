using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    internal class Employee
    {
        public int Id { get; set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public bool IsEmployed { get; private set; } = true;
        public bool IsManager { get; private set; } = false;

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

        public static void ListAllEmployees(List<Employee> testEmployees)
        {
            Console.WriteLine("");
            foreach (var employee in testEmployees)
            {
                Console.WriteLine($"{employee.Id}. {employee.Name} {employee.Surname} ({employee.Username})");
                Console.Write($"   Works: ");
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
                Console.Write($"   Manages: ");
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
        public static void AddEmployee(List<Employee> testEmployees)
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

            if (string.IsNullOrEmpty(newEmployeeName) && string.IsNullOrEmpty(newEmployeeSurname))
            {
                var newEmployeeUsername = $"{newEmployeeName.Substring(0, 3)}{newEmployeeSurname.Substring(0, 3)}";
                var newEmployeePassword = "1234";
                testEmployees.Add(new Employee(testEmployees.Count + 1, newEmployeeName, newEmployeeSurname, newEmployeeUsername, newEmployeePassword, true, false));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\nSuccessfully added ");
                Console.ResetColor();
                Console.Write($"{newEmployeeName} {newEmployeeSurname} ({newEmployeeUsername})! ");
                HelperMethods.ProceedIn(3);
            }
        }
        public static void RemoveEmployee(List<Employee> testEmployees)
        {
            List<string> employeesToRemove = testEmployees.Where(e => e.IsEmployed == true).Where(e => e.IsManager == false).Select(e => $"{e.Name} {e.Surname}").ToList();
            var optionSelected = HelperMethods.MenuInteraction(employeesToRemove);
            var employeeToRemove = testEmployees.FirstOrDefault(e => $"{e.Name} {e.Surname}" == employeesToRemove[optionSelected]);
            while (true)
            {
                Console.Write($"Remove {employeeToRemove.Name} {employeeToRemove.Surname} ({employeeToRemove.Username})? (y/N): ");
                var input = Console.ReadLine();
                if (input is "y" or "Y")
                {
                    employeeToRemove.IsEmployed = false;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\nSuccessfully removed ");
                    Console.ResetColor();
                    Console.Write($"{employeeToRemove.Name} {employeeToRemove.Surname} ({employeeToRemove.Username})! ");
                    HelperMethods.ProceedIn(3);
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
        public static void EditEmployeeInfo(List<Employee> testEmployees)
        {
            List<string> employeesToEdit = testEmployees.Select(e => $"{e.Name} {e.Surname}").ToList();
            var optionSelected = HelperMethods.MenuInteraction(employeesToEdit);
            var employeeToEdit = testEmployees.FirstOrDefault(e => $"{e.Name} {e.Surname}" == employeesToEdit[optionSelected]);
            while (true)
            {
                Console.Write($"\nEdit {employeeToEdit.Name} {employeeToEdit.Surname} ({employeeToEdit.Username})? (y/N): ");
                var input = Console.ReadLine();
                if (input is "y" or "Y")
                {
                    List<string> menuOptions =
                    [
                        "0. Return",
                        "1. Name",
                        "2. Surname",
                        "3. Username",
                        "4. Password",
                        "5. Employment status",
                        "6. Management status"
                    ];
                    optionSelected = HelperMethods.MenuInteraction(menuOptions);
                    switch (optionSelected)
                    {
                        case 0:
                            return;
                        case 1:
                            Console.Write("\nNew name: ");
                            string newName = Console.ReadLine();
                            employeeToEdit.Name = newName;
                            continue;
                        case 2:
                            Console.Write("\nNew surname: ");
                            string newSurname = Console.ReadLine();
                            employeeToEdit.Surname = newSurname;
                            continue;
                        case 3:
                            Console.Write("\nNew username: ");
                            string newUsername = Console.ReadLine();
                            employeeToEdit.Username = newUsername;
                            continue;
                        case 4:
                            Console.Write("\nNew password: ");
                            string newPassword = HelperMethods.GetUserEncryptedPassword();
                            employeeToEdit.Password = newPassword;
                            continue;
                        case 5:
                            Console.Write($"\nCurrently employed: {employeeToEdit.IsEmployed}. Switch? (y/N): ");
                            input = Console.ReadLine();
                            if (input is "y" or "Y")
                            {
                                if (employeeToEdit.IsEmployed == false ? employeeToEdit.IsEmployed = true : employeeToEdit.IsEmployed = false);
                            }
                            else if (input is "n" or "N")
                            {
                                continue;
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
                                if (testEmployees.Count(e => e.IsManager) > 1)
                                {
                                    if (employeeToEdit.IsManager == false ? employeeToEdit.IsManager = true : employeeToEdit.IsManager = false);
                                }
                                else
                                {
                                    HelperMethods.PrintError("Restaurant cannot be without a manager!");
                                }
                            }
                            else if (input is "n" or "N")
                            {
                                continue;
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
