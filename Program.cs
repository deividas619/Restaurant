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
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, args) =>
            {
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
 * object Restaurant doesn't have property for working hours
 * cancel operation if ctrl + c sometimes is cancelling only from 2nd input
 * cancellation token messes up other inputs like providing username for login
 * fix auto logout timer when manager is logged in
*/