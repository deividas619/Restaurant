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
            Console.WriteLine("Provide new employee information:");
            Console.Write("Name: ");
            var newEmployeeName = Console.ReadLine();
            Console.Write("Surname: ");
            var newEmployeeSurname = Console.ReadLine();
            var newEmployeeUsername = $"{newEmployeeName.Substring(0,3)}{newEmployeeSurname.Substring(0,3)}";
            var newEmployeePassword = "1234";
            testEmployees.Add(new Employee(testEmployees.Count + 1, newEmployeeName, newEmployeeSurname, newEmployeeUsername, newEmployeePassword, true, false));
        }

        public static void RemoveEmployee(List<Employee> testEmployees)
        {
            List<string> employeesToRemove = testEmployees.Where(e => e.IsEmployed == true).Where(e => e.IsManager == false).Select(e => $"{e.Name} {e.Surname}").ToList();
            int optionSelected = HelperMethods.MenuInteraction(employeesToRemove);
            Employee employeeToRemove = testEmployees.FirstOrDefault(e => $"{e.Name} {e.Surname}" == employeesToRemove[optionSelected]);
            while (true)
            {
                Console.Write($"Remove {employeeToRemove.Name} {employeeToRemove.Surname} ({employeeToRemove.Username})? (y/N): ");
                var input = Console.ReadLine();
                if (input == "y" || input == "Y")
                {
                    employeeToRemove.IsEmployed = false;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\nSuccessfully removed ");
                    Console.ResetColor();
                    Console.Write($"{employeeToRemove.Name} {employeeToRemove.Surname} ({employeeToRemove.Username})!");
                    break;
                }
                else if (input == "n" || input == "N")
                {
                    break;
                }
                else
                {
                    HelperMethods.PrintError("Incorrect selection!");
                }
            }
        }
    }
}
