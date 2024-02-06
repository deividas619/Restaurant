using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    internal class Item
    {
        public string Name;
        public decimal Price;
        public string Description;
        public bool IsFood;
        public bool IsDrink;
        public bool IsAvailable;

        public Item(string name, decimal price, string description, bool isFood, bool isDrink, bool isAvailable)
        {
            Name = name;
            Price = price;
            Description = description;
            IsFood = isFood;
            IsDrink = isDrink;
            IsAvailable = isAvailable;
        }

        public static void ListAllItems(List<Item> Items)
        {
            Console.WriteLine("");
            foreach (var item in Items)
            {
                Console.WriteLine($"{item.Name} ({item.Price}€) || Food: {item.IsFood} || Drink: {item.IsDrink} || Available: {item.IsAvailable}");
            }
        }

        public static void AddItem(List<Item> Items)
        {
            do
            {
                Console.WriteLine("\nProvide new item information:");
                Console.Write("Name: ");
                var newItemName = Console.ReadLine();
                if (HelperMethods._cancellation.Token.IsCancellationRequested)
                {
                    Console.Write("\nCancelled... ");
                    HelperMethods.ProceedIn(3);
                    HelperMethods._cancellation.Cancel();
                    return;
                }

                Console.Write("Price: ");
                decimal.TryParse(Console.ReadLine(), out var newItemPrice);
                if (HelperMethods._cancellation.Token.IsCancellationRequested)
                {
                    Console.Write("\nCancelled... ");
                    HelperMethods.ProceedIn(3);
                    HelperMethods._cancellation.Cancel();
                    return;
                }

                Console.Write("Drink? (y/N): ");
                var newItemIsDrink = Console.ReadLine();
                if (HelperMethods._cancellation.Token.IsCancellationRequested)
                {
                    Console.Write("\nCancelled... ");
                    HelperMethods.ProceedIn(3);
                    HelperMethods._cancellation.Cancel();
                    return;
                }

                Console.Write("Food? (y/N): ");
                var newItemIsFood = Console.ReadLine();
                if (HelperMethods._cancellation.Token.IsCancellationRequested)
                {
                    Console.Write("\nCancelled... ");
                    HelperMethods.ProceedIn(3);
                    HelperMethods._cancellation.Cancel();
                    return;
                }

                Console.Write("Description: ");
                var newItemDescription = Console.ReadLine();
                if (HelperMethods._cancellation.Token.IsCancellationRequested)
                {
                    Console.Write("\nCancelled... ");
                    HelperMethods.ProceedIn(3);
                    HelperMethods._cancellation.Cancel();
                    return;
                }

                bool newItemDrink = false;
                bool newItemFood = false;
                if (!string.IsNullOrEmpty(newItemName) && newItemPrice != null)
                {
                    if (newItemIsDrink is "y" or "Y" ? newItemDrink = true : newItemDrink = false);
                    if (newItemIsFood is "y" or "Y" ? newItemFood = true : newItemFood = false);
                    if (!newItemDrink && !newItemFood)
                    {
                        HelperMethods.PrintError("Neither food nor drink!");
                    }
                    else if (newItemDrink && newItemFood)
                    {
                        HelperMethods.PrintError("Both food and drink!");
                    }
                    else
                    {
                        Items.Add(new Item(newItemName, newItemPrice, newItemDescription, newItemFood, newItemDrink, true));
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("\nSuccessfully added ");
                        Console.ResetColor();
                        Console.Write($"{newItemName} ({newItemPrice}€)! ");
                        HelperMethods.ProceedIn(3);
                        break;
                    }
                }
                else
                {
                    HelperMethods.PrintError("Incorrect input!");
                }
            }
            while (true);
        }

        public static void RemoveItem(List<Item> Items)
        {
            List<string> itemsToRemove = Items.Select(i => $"{i.Name}").ToList();
            var optionSelected = HelperMethods.MenuInteraction(itemsToRemove);
            var itemToRemove = Items.FirstOrDefault(i => i == Items[optionSelected]);
            while (true)
            {
                Console.Write($"Remove {itemToRemove.Name}? (y/N): ");
                var input = Console.ReadLine();
                if (input is "y" or "Y")
                {
                    itemToRemove.IsAvailable = false;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\nSuccessfully removed ");
                    Console.ResetColor();
                    Console.Write($"{itemToRemove.Name} ");
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

        public static void EditItemInfo(List<Item> Items)
        {

        }
    }
}
