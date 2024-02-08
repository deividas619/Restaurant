using Restaurant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    public class Table
    {
        public int TableNumber;
        //public int Seats;
        public bool IsFree = true;

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
            Dictionary<int, List<int>> tableNumberPositions = new()
            {
                { 1, [5, 18] },
                { 2, [17, 18] },
                { 3, [29, 18] },
                { 4, [5, 24] },
                { 5, [29, 24] },
                { 6, [5, 37] },
                { 7, [17, 37] },
                { 8, [29, 37] }
            };

            foreach (var table in Program.database.Tables)
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
                if (HelperMethods._cancellation.Token.IsCancellationRequested)
                {
                    Console.Write("\nCancelled... ");
                    HelperMethods.ProceedIn(3);
                    HelperMethods._cancellation.Cancel();
                    return;
                }
                if (int.TryParse(input, out var tableNumber))
                {
                    var table = Program.database.Tables.FirstOrDefault(t => t.TableNumber == tableNumber);
                    if (table is { IsFree: true })
                    {
                        table.IsFree = false;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Table {tableNumber} reserved successfully!");
                        Console.ResetColor();
                        Program.UpdateDatabase(Program.database);
                        //HelperMethods.ProceedIn(3);
                        HelperMethods.ReturnToMainMenu();
                        break;
                    }
                    else if (table is { IsFree: false })
                    {
                        HelperMethods.PrintError($"Table {tableNumber} is already reserved!");
                    }
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
