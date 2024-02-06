using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    internal class DatabaseModel
    {
        public Restaurant Restaurant;
        public List<Employee> Employees;
        public Item Items;
        public Table Tables;
        public Order Order;
        public BillCustomer BillCustomer;
        public BillRestaurant BillRestaurant;

        public DatabaseModel (Restaurant restaurant, List<Employee> employees, Item items, Table tables, Order order, BillCustomer billCustomer, BillRestaurant billRestaurant)
        {
            Restaurant = restaurant;
            Employees = employees;
            Items = items;
            Tables = tables;
            Order = order;
            BillCustomer = billCustomer;
            BillRestaurant = billRestaurant;
        }
    }
}

/*internal class Restaurantt
{
    public static string Name { get; set; }
    private static string defaultHourFrom = "10:00";
    private static string defaultHourTo = "22:00";
    private static List<Employee> Employees;

    public static List<string> WorkingHours =
    [
        "Mon (from {defaultHourFrom} to {defaultHourTo})",
        "Tue (from {defaultHourFrom} to {defaultHourTo})",
        "Wed (from {defaultHourFrom} to {defaultHourTo})",
        "Thu (from {defaultHourFrom} to {defaultHourTo})",
        "Fri (from {defaultHourFrom} to {defaultHourTo})",
        "Sat (from {defaultHourFrom} to {defaultHourTo})",
        "Sun (day off)"
    ];

    public Restaurantt(string name, List<Employee> employees)
    {
        Name = name;
        Employees = employees;
    }
}

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
  public Restaurantt Restaurant { get; set; }
  public List<Employee> Employees { get; set; }
  public Dummy()
  {
      this.Employees = new List<Employee>
      {
          new Employee( 1, "Ned", "Flanders", "nedfla", "1234", true, true),
          new Employee( 2, "Bob", "Bobinski", "bobbob", "1234", true, true),
          new Employee( 3, "Tod", "Flanders", "todfla", "1234", false, true),
          new Employee( 4, "Rod", "Flanders", "rodfla", "1234", true, false),
          new Employee( 5, "Dennis", "Rodman", "denrod", "1234", false, false)
      };
      this.Restaurant = new Restaurantt("Restaurant_1", Employees);
  }
}*/
