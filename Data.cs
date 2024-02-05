using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    internal class Data
    {
        public Restaurant Restaurant;
        public Employee Employees;
        public Items Items;
        public Table Tables;
            public Order Order;
            public BillCustomer BillCustomer;
            public BillRestaurant BillRestaurant;
    }
}

/*
https://csharp2json.azurewebsites.net/

internal class Employee
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
}

internal class Dummy
{
  public List<Employee> testEmployees { get; set; }
  public Dummy()
  {
      this.testEmployees = new List<Employee>
      {
          new Employee( 1, "Ned", "Flanders", "nedfla", "1234", true, true),
          new Employee( 2, "Bob", "Bobinski", "bobbob", "1234", true, true),
          new Employee( 3, "Tod", "Flanders", "todfla", "1234", false, true),
          new Employee( 4, "Rod", "Flanders", "rodfla", "1234", true, false),
          new Employee( 5, "Dennis", "Rodman", "denrod", "1234", false, false)
      };
  }
}
*/