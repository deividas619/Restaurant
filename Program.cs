using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;

namespace Restaurant
{
    internal class Program
    {
        public static string pw = ReturnPW();
        public static RestaurantData database = JsonConvert.DeserializeObject<RestaurantData>(File.ReadAllText("Database.json"));
        public static readonly CancellationTokenSource cts = new CancellationTokenSource();
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CancelKeyPress += (sender, args) =>
            {
                //Console.Write("Cancelling...");
                //HelperMethods.ProceedIn(3);
                args.Cancel = true;
                cts.Cancel();
            };

            HelperMethods.InitMainMenu(cts);
        }

        public static void UpdateDatabase(RestaurantData database)
        {
            string jsonOutput = JsonConvert.SerializeObject(database, Formatting.Indented);
            File.WriteAllText("Database.json", jsonOutput);
        }
        public static string ReturnPW()
        {
            return "mraabfqiwapjvukm";
        }
    }
}

/*
 * TO-DO
 * cancel operation if ctrl + c sometimes is cancelling only from 2nd input
 * cancellation token messes up other inputs like providing username for login
 * fix auto logout timer when manager is logged in
*/