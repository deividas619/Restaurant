using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Restaurant.Services;

namespace Restaurant.Repositories
{
    public interface ISendEmail
    {
        static void SendEmail()
        {
        }
    }
    public class OrderItem
    {
        public Item Item { get; set; }
        public int Amount { get; set; }

        public OrderItem(Item item, int amount)
        {
            Item = item;
            Amount = amount;
        }
    }
    public class Order : ISendEmail
    {
        public int OrderID { get; private set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Table Table { get; set; }
        private int AverageTimePerOrder { get; set; } = 10;
        private string OrderStatus { get; set; } = "Ongoing";
        public DateTime EstimatedFinishTime { get; private set; }
        private static int _nextOrderId = 1;

        public Order(Table table)
        {
            OrderID = _nextOrderId++;
            Table = table;
            EstimatedFinishTime = DateTime.Now.AddMinutes(AverageTimePerOrder);
        }
        public static void PlaceOrder(List<Table> tables, List<Item> items)
        {
            Table.CheckTableAvailability();
            Console.Write("Which table placed an order (1-8): ");
            int.TryParse(Console.ReadLine(), out int tableNumber);
            var table = tables.FirstOrDefault(t => t.TableNumber == tableNumber);

            if (table != null)
            {
                Console.WriteLine("\nSelect items ordered:");
                List<string> itemsList = items.Select(i => i.Name).ToList();
                var order = new Order(table);

                do
                {
                    var optionSelected = HelperMethods.MenuInteraction(itemsList);
                    var amount = 0;
                    do
                    {
                        Console.Write($"\nSelected: {itemsList[optionSelected]}. Amount: ");
                        int.TryParse(Console.ReadLine(), out amount);
                        if (amount < 1 || amount > 99)
                        {
                            HelperMethods.PrintError("Incorrect input!");
                        }
                        else
                        {
                            break;
                        }
                    }
                    while (true);

                    order.AddItem(items[optionSelected], amount);

                    Console.Write("\nDo you want to add more items? (y/N): ");
                    if (Console.ReadLine().ToUpper() != "Y")
                    {
                        break;
                    }
                }
                while (true);

                Program.database.Order.Add(order);
                Program.UpdateDatabase(Program.database);
                table.IsFree = false;
                _ = order.MealPreparationAsync();
                return;
            }
            else
            {
                HelperMethods.PrintError($"Table {tableNumber} not found!");
                //HelperMethods.ProceedIn(3);
                HelperMethods.ReturnToMainMenu();
            }
        }
        public void AddItem(Item item, int amount)
        {
            Items.Add(new OrderItem(item, amount));
        }
        public async Task MealPreparationAsync()
        {
            int orderTime = 1;
            if (Items.Any(i => i.Item.IsFood == true))
            {
                orderTime = AverageTimePerOrder;
            }
            EstimatedFinishTime = DateTime.Now.AddMinutes(orderTime);
            await Task.Delay(TimeSpan.FromSeconds(60));

            foreach (var item in Items)
            {
                await Task.Delay(TimeSpan.FromMinutes(orderTime));
            }

            OrderStatus = "Completed";
        }
        public static void ListOngoingOrders()
        {
            foreach (var ongoingOrder in Program.database.Order.Where(o => o.OrderStatus == "Ongoing"))
            {
                Console.WriteLine($"\nOrder: {ongoingOrder.OrderID}. Table: {ongoingOrder.Table.TableNumber}");
                Console.WriteLine($"   Estimated Finish Time: {ongoingOrder.EstimatedFinishTime.AddMinutes(1):HH:mm}");

                int counter = 1;
                foreach (var orderItem in ongoingOrder.Items)
                {
                    Console.WriteLine($"     Item {counter++}: ({orderItem.Amount}) {orderItem.Item.Name}");
                }
            }
            HelperMethods.ReturnToMainMenu();
        }
        public static void CloseTable(List<Table> tables, List<string> BreadCrumb)
        {
            Table.CheckTableAvailability();
            Console.Write("Which table placed an order (1-8): ");
            int.TryParse(Console.ReadLine(), out int tableNumber);
            var table = tables.FirstOrDefault(t => t.TableNumber == tableNumber);
            decimal totalAmountForTable = 0;
            List<Order> ordersForTable = CombineOrdersForTable(table.TableNumber);

            if (table != null)
            {
                if (!table.IsFree)
                {
                    foreach (var order in ordersForTable)
                    {
                        totalAmountForTable += order.CalculateTotalAmount();
                    }
                    Console.WriteLine($"\nTotal amount for Table {table.TableNumber}: {totalAmountForTable}€");
                }
                else
                {
                    HelperMethods.PrintError($"Table {table.TableNumber} doesn't have active orders!");
                    HelperMethods.ReturnToMainMenu();
                    return;
                }
            }
            else
            {
                HelperMethods.PrintError($"Table {tableNumber} not found!");
                HelperMethods.ReturnToMainMenu();
            }

            List<OrderItem> items = Program.database.Order.Where(o => o.Table.TableNumber == tableNumber).SelectMany(i => i.Items).ToList();
            BreadCrumb.Add("Generate bill");
            GenerateBill(tableNumber, items, totalAmountForTable);
            BreadCrumb.Remove("Generate bill");
            table.IsFree = true;
        }
        public decimal CalculateTotalAmount()
        {
            return Items.Sum(orderItem => orderItem.Item.Price * orderItem.Amount);
        }
        public static List<Order> CombineOrdersForTable(int tableNumber)
        {
            return Program.database.Order.Where(order => order.Table.TableNumber == tableNumber).ToList();
        }
        public static void GenerateBill(int tableNumber, List<OrderItem> items, decimal totalAmountForTable)
        {
            Bill bill = new Bill(tableNumber, items, totalAmountForTable);
            int totalAmountLength = bill.TotalAmountForTable.ToString().Length;
            string restaurantBill = $@"
|-------------------------------------|
|############### JAMMY ###############
|
|  Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
|
|  Table: {bill.TableNumber}
|  Items:
{string.Join(Environment.NewLine, bill.Items.Select(item => $"|  - ({item.Amount}) {Truncate(item.Item.Name, 15)}"))}
|
|
|  Total amount: {bill.TotalAmountForTable + "€"}{new string(' ', 25 - totalAmountLength)}
|            ¯\_(ツ)_/¯
|-------------------------------------|
";

            string customerBill = $@"
|-------------------------------------|
|############### JAMMY ###############
|
|  Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
|
|  Table: {bill.TableNumber}
|  Items:
{string.Join(Environment.NewLine, bill.Items.Select(item => $"|  - ({item.Amount}) {Truncate(item.Item.Name, 15)}"))}
|
|
|  Total amount: {bill.TotalAmountForTable + "€"}{new string(' ', 25 - totalAmountLength)}
|-------------------------------------|
";
            Console.WriteLine("\n######## // RESTAURANT BILL \\\\ ########");
            Console.WriteLine(restaurantBill);
            Console.WriteLine("#######################################");
            do
            {
                Console.Write("\nPrint customer bill? (y/N): ");
                var input = Console.ReadLine();
                if (input.ToUpper() == "Y")
                {
                    Console.WriteLine("\n######### // CUSTOMER BILL \\\\ #########");
                    Console.WriteLine(customerBill);
                    Console.WriteLine("#######################################");
                    break;
                }
                else if (input.ToUpper() == "N")
                {
                    break;
                }
                else
                {
                    HelperMethods.PrintError("Incorrect input!");
                }
            }
            while (true);


            Console.WriteLine("\nPrinting bill for restaurant...");
            Console.WriteLine("Saving restaurant bill to Bills.txt");
            File.AppendAllText("Bills.txt", restaurantBill);

            do
            {
                Console.Write("\nSend customer bill to their email? (y/N): ");
                var input = Console.ReadLine();
                if (input.ToUpper() == "Y")
                {
                    Console.Write("Email: ");
                    var emailAddress = Console.ReadLine();
                    Console.Write($"\nSend customer bill to {emailAddress}? (y/N): ");
                    input = Console.ReadLine();
                    if (input.ToUpper() == "Y")
                    {
                        SendEmail(customerBill, emailAddress);
                        return;
                    }
                    else if (input.ToUpper() == "N")
                    {
                        break;
                    }
                    else
                    {
                        HelperMethods.PrintError("Incorrect input!");
                        continue;
                    }
                }
                else if (input.ToUpper() == "N")
                {
                    break;
                }
                else
                {
                    HelperMethods.PrintError("Incorrect input!");
                }
            }
            while (true);
        }
        public static void SendEmail(string customerBill, string emailAddress)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Jammy mailing system", "dscep93@gmail.com"));
            message.To.Add(new MailboxAddress("", emailAddress));
            message.Subject = "Jammy bill";
            message.Body = new TextPart("plain") { Text = customerBill };
            Console.WriteLine($"\nSending the bill to {emailAddress}");
            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    client.Authenticate("dscep93@gmail.com", Program.pw);
                    client.Send(message);
                    client.Disconnect(true);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Mail sent successfully!");
                    Console.ResetColor();
                    HelperMethods.ReturnToMainMenu();
                    return;
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nMail sending failed!");
                Console.ResetColor();
                Console.WriteLine(e.Message);
                HelperMethods.ReturnToMainMenu();
            }
        }
        private static string Truncate(string value, int maxLength)
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
        }
    }
}
