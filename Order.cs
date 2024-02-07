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

namespace Restaurant
{
    public interface ISendEmail
    {
        static void SendEmail(string customerBill, string emailAddress)
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
        private int AverageTimePerOrder { get; set; } = 5;
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
                HelperMethods.ProceedIn(3);
                return;
            }
            else
            {
                HelperMethods.PrintError($"Table {tableNumber} not found!");
                HelperMethods.ProceedIn(3);
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
            this.EstimatedFinishTime = DateTime.Now.AddMinutes(orderTime);
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
                Console.WriteLine($"   Estimated Finish Time: {ongoingOrder.EstimatedFinishTime.AddMinutes(1).ToString("HH:mm")}");

                int counter = 1;
                foreach (var orderItem in ongoingOrder.Items)
                {
                    Console.WriteLine($"     Item {counter++}: ({orderItem.Amount}) {orderItem.Item.Name}");
                }
            }
            HelperMethods.ReturnToMainMenu();
        }
        public static void CloseTable(List<Table> tables, List<string> breadCrumb)
        {
            /*Table.CheckTableAvailability();
            Console.Write("Which table placed an order (1-8): ");
            int.TryParse(Console.ReadLine(), out int tableNumber);
            var table = tables.FirstOrDefault(t => t.TableNumber == tableNumber);
            decimal totalAmountForTable = 0;

            if (table != null)
            {
                if (!table.IsFree)
                {
                    List<Order> ordersForTable = Order.CombineOrdersForTable(table.TableNumber);
                    foreach (var order in ordersForTable)
                    {
                        totalAmountForTable += order.CalculateTotalAmount();
                    }
                    Console.WriteLine($"\nTotal amount for Table {table.TableNumber}: {totalAmountForTable}€");
                }
                else
                {
                    HelperMethods.PrintError($"Table {table.TableNumber} doesn't have active orders!");
                }
            }
            else
            {
                HelperMethods.PrintError($"Table {tableNumber} not found!");
                HelperMethods.ReturnToMainMenu();
            }*/

            //List<OrderItem> items = Program.database.Order.SelectMany(i => i.Items).Where(o => o.OrderStatus == "Completed").Where(o => o.Table.TableNumber == tableNumber);
            breadCrumb.Add("Generate bill");
            //GenerateBill(tableNumber, items, totalAmountForTable);
            SendEmail("some text", "dsce@danskebank.lt"); //remove
            breadCrumb.Remove("Generate bill");
            //table.IsFree = true;
            HelperMethods.ReturnToMainMenu();
        }
        public decimal CalculateTotalAmount()
        {
            decimal totalAmount = 0;
            foreach (var orderItem in Items)
            {
                totalAmount += orderItem.Item.Price * orderItem.Amount;
            }
            return totalAmount;
        }
        public static List<Order> CombineOrdersForTable(int tableNumber)
        {
            List<Order> ordersForTable = Program.database.Order.Where(order => order.Table.TableNumber == tableNumber).ToList();
            return ordersForTable;
        }
        public static void GenerateBill(int tableNumber, List<OrderItem> items, decimal totalAmountForTable)
        {
            /*Bill Bill = new Bill(tableNumber, items, totalAmountForTable);
            string restaurantBill = $@"
|---------------------------------|
|  Date: {DateTime.Now}           |
|                                 |
|  Table: {tableNumber}           |
|  Items: {items.ForEach(Console.WriteLine())}|
|                                 |
|                                 |
|                                 |
|                                 |
|  Total amount: {totalAmountForTable}|
|            ¯\_(ツ)_/¯           |
|---------------------------------|

";

            string customerBill = $@"
|---------------------------------|
|  Date: {DateTime.Now}           |
|                                 |
|  Table: {tableNumber}           |
|  Items: {items.ForEach(Console.WriteLine())}|
|                                 |
|                                 |
|                                 |
|                                 |
|  Total amount: {totalAmountForTable}|
|---------------------------------|

";
            Console.WriteLine("##### // RESTAURANT BILL \\ ########## // CUSTOMER BILL \\ #####");
            foreach (var lineInCustomerBill in customerBill)
            {
                foreach (var lineInRestaurantBill in restaurantBill)
                {
                    Console.Write($"{lineInRestaurantBill}   ");
                    break;
                }

                Console.Write($"{lineInCustomerBill}\n");
            }

            Console.WriteLine("########################################################################################################");
            Console.WriteLine("Printing both bills...");
            Console.WriteLine("Saving restaurant bill to Bills.txt");
            File.AppendAllText(@"Bills.txt", restaurantBill);*/

            do
            {
                Console.Write("Send customer bill to their email? (y/N): ");
                var input = Console.ReadLine();
                if (input.ToUpper() == "Y")
                {
                    Console.Write("Email: ");
                    var emailAddress = Console.ReadLine();
                    Console.Write($"Send it to {emailAddress}? (y/N): ");
                    input = Console.ReadLine();
                    if (input.ToUpper() == "Y")
                    {
                        //SendEmail(customerBill, emailAddress);
                        SendEmail("some text", emailAddress);
                    }
                    else if (input.ToUpper() == "N")
                    {
                        HelperMethods.ProceedIn(3);
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
                    HelperMethods.ProceedIn(3);
                    break;
                }
                else
                {
                    HelperMethods.PrintError("Incorrect input!");
                }
            }
            while (true);
            return;
        }
        public static void SendEmail(string customerBill, string emailAddress)
        {
            emailAddress = "dscep93@gmail.com";
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Restaurant mailing system", "dscep93@gmail.com"));
            message.To.Add(new MailboxAddress("", emailAddress));
            message.Subject = "Restaurant bill";
            //message.Body = new TextPart("plain") { Text = customerBill };
            message.Body = new TextPart("plain") { Text = "some text" };
            Console.WriteLine($"\nSending the bill to {emailAddress}");
            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    client.Authenticate("dscep93@gmail.com", "mraabfqiwapjvukm");
                    client.Send(message);
                    client.Disconnect(true);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nMail sent successfully!");
                    Console.ResetColor();
                    return;
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nMail sending failed!");
                Console.ResetColor();
                Console.WriteLine(e.Message);
                throw;
                return;
            }
        }
    }
}
