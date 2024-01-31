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

        public static void CheckTableAvailability(Table[] tables)
        {
            /*Console.WriteLine(@"
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
|---------------------------------|");*/
            Console.ResetColor();
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
            int counterForShiftingColumn = 5;
            int counterForShiftingRow = 7;
            for (int i = 0; i < tables.Length; i++) // Cycling through columns
            {
                for (int j = 0; j < tables.Length; j++) // Cycling through rows
                {
                    var StartCursorPositionColumn = (i % 4) * 15 + counterForShiftingColumn;
                    var StartCursorPositionRow = (i / 4) * 8 + counterForShiftingRow;
                    Console.SetCursorPosition(StartCursorPositionColumn, StartCursorPositionRow);
                    Console.ForegroundColor = tables[i].IsFree ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.Write($"{tables[i].TableNumber}");
                    Console.ResetColor();
                    counterForShiftingColumn -= 3;
                }
            }

            Console.ResetColor();
        }
    }
}
