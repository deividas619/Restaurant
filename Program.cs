namespace Restaurant
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var testTables = GenerateTestData();
            PrintWelcomeScreen(testTables);
        }

        public static Table[] GenerateTestData()
        {
            Table[] tables = new Table[]
            {
                new Table { TableNumber = 1, Seats = 4, IsFree = true },
                new Table { TableNumber = 2, Seats = 2, IsFree = false },
                new Table { TableNumber = 3, Seats = 6, IsFree = true },
            };

            return tables;
        }
        public static void PrintWelcomeScreen(Table[] testTables)
        {
            Console.WriteLine("####################");
            Console.WriteLine($"# Welcome to JAMMY #");
            Console.WriteLine("####################");
            //ProceedIn(3);
            Table.CheckTableAvailability(testTables);
        }

        public static void ProceedIn(int from)
        {
            Console.Write("Proceeding in: ");
            for (int i = from; i >= 0; i--)
            {
                Console.Write(i);
                Thread.Sleep(1000);
                Console.Write("\b \b");
                if (i == 0) Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
            }
        }
    }
}
