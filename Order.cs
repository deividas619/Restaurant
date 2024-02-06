using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
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
    public class Order
    {
        public int OrderID { get; private set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Table Table { get; set; }
        private int AverageTimePerOrder { get; set; } = 1;
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
                        Console.Write($"\nSelected: {itemsList[optionSelected]}. How many: ");
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
            }
        }
        public void AddItem(Item item, int amount)
        {
            Items.Add(new OrderItem(item, amount));
        }
        public async Task MealPreparationAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(60));

            foreach (var item in Items)
            {
                await Task.Delay(TimeSpan.FromMinutes(AverageTimePerOrder));
            }

            OrderStatus = "Completed";
        }
        public static void ListOngoingOrders()
        {
            foreach (var ongoingOrder in Program.database.Order.Where(o => o.OrderStatus == "Ongoing"))
            {
                Console.WriteLine($"\nOrder: {ongoingOrder.OrderID}. Table: {ongoingOrder.Table.TableNumber}");
                Console.WriteLine($"Estimated Finish Time: {ongoingOrder.EstimatedFinishTime.AddMinutes(1).ToString("HH:mm")}");

                int counter = 1;
                foreach (var orderItem in ongoingOrder.Items)
                {
                    Console.WriteLine($"  Item {counter++}: ({orderItem.Amount}) {orderItem.Item.Name}");
                }
            }
            HelperMethods.ReturnToMainMenu();
        }
        public static void CloseTable(List<Table> tables)
        {
            Table.CheckTableAvailability();
            Console.Write("Which table placed an order (1-8): ");
            int.TryParse(Console.ReadLine(), out int tableNumber);
            var table = tables.FirstOrDefault(t => t.TableNumber == tableNumber);

            if (table != null)
            {
                if (!table.IsFree)
                {
                    List<Order> ordersForTable = Order.CombineOrdersForTable(table.TableNumber);
                    decimal totalAmountForTable = 0;
                    foreach (var order in ordersForTable)
                    {
                        totalAmountForTable += order.CalculateTotalAmount();
                    }
                    Console.WriteLine($"\nTotal amount for Table {table.TableNumber}: {totalAmountForTable}");
                }
                else
                {
                    HelperMethods.PrintError($"Table {table.TableNumber} doesn't have active orders!");
                }
            }
            else
            {
                HelperMethods.PrintError($"Table {tableNumber} not found!");
            }
            table.IsFree = true;
            GenerateBill();
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
        public static void GenerateBill()
        {

        }
    }
}
