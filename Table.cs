using Restaurant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    internal class Table
    {
        public int TableNumber;
        public int Seats;
        public bool IsFree = true;
        public static List<Table> Tables { get; set; } = new List<Table>
        {
            new Table { TableNumber = 1, Seats = 4, IsFree = true },
            new Table { TableNumber = 2, Seats = 4, IsFree = true },
            new Table { TableNumber = 3, Seats = 4, IsFree = true },
            new Table { TableNumber = 4, Seats = 4, IsFree = true },
            new Table { TableNumber = 5, Seats = 4, IsFree = true },
            new Table { TableNumber = 6, Seats = 10, IsFree = true },
            new Table { TableNumber = 7, Seats = 10, IsFree = true },
            new Table { TableNumber = 8, Seats = 10, IsFree = true }
        };

        public static void CheckTableAvailability()
        {
             Console.WriteLine(@"
|---------------------------------|
|  _____       _____       _____  |
| |     |     |     |     |     | |
| |  1  |     |  2  |     |  3  | |
| |_____|     |_____|     |_____| |
|                                 |
|                                 |
|  _____                   _____  |
| |     |                 |     | |
| |  4  |                 |  5  | |
| |_____|                 |_____| |
|                                 |
|_______                          |
|       |                        -----
|  BAR  |                        <= ENTRANCE
|  HERE |                        -----
|_______|                         |
|                                 |
|  _____       _____       _____  |
| |     |     |     |     |     | |
| |     |     |     |     |     | |
| |     |     |     |     |     | |
| |  6  |     |  7  |     |  8  | |
| |     |     |     |     |     | |
| |     |     |     |     |     | |
| |_____|     |_____|     |_____| |
|                                 |
|---------------------------------|");

            var initialCursorPosition = Console.GetCursorPosition();
            Dictionary<int, List<int>> tableNumberPositions = new Dictionary<int, List<int>>
            {
                { 1, [5, 14] },
                { 2, [17, 14] },
                { 3, [29, 14] },
                { 4, [5, 20] },
                { 5, [29, 20] },
                { 6, [5, 33] },
                { 7, [17, 33] },
                { 8, [29, 33] }
            };

            foreach (var table in Tables)
            {
                tableNumberPositions.TryGetValue(table.TableNumber, out var position);
                Console.SetCursorPosition(position[0], position[1]);
                Console.ForegroundColor = table.IsFree ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write($"{table.TableNumber}");
                Console.ResetColor();
            }

            Console.SetCursorPosition(initialCursorPosition.Left, initialCursorPosition.Top);
        }

        public static void MakeTableReservation()
        {
            CheckTableAvailability();
            do
            {
                Console.Write("\nWhich table to reserve (1-8): ");
                var input = Console.ReadLine();
                //if (int.TryParse(Console.ReadLine(), out int tableNumber))
                if (int.TryParse(input, out int tableNumber))
                {
                    var table = Tables.FirstOrDefault(t => t.TableNumber == tableNumber);
                    if (table != null && table.IsFree)
                    {
                        table.IsFree = false;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Table {tableNumber} reserved successfully!");
                        Console.ResetColor();
                        HelperMethods.ProceedIn(3);
                        break;
                    }
                    else if (table != null && !table.IsFree)
                    {
                        HelperMethods.PrintError($"Table {tableNumber} is already reserved!");
                    }
                    else
                    {
                        HelperMethods.PrintError($"Table {tableNumber} not found!");
                    }
                }
                else if (char.TryParse(input, out char q)) // cancel operation if q is entered
                {

                }
                else
                {
                    HelperMethods.PrintError("Incorrect selection!");
                }
            }
            while (true);
        }
    }
}
