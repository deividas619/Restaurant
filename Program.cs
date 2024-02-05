using System.Text.Json.Serialization;
using System.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Restaurant
{
    internal class Program
    {
        public static List<Data> database;

        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, args) =>
            {
                args.Cancel = true;
                cts.Cancel();
            };
            
            HelperMethods.InitMainMenu(cts);
        }

        public static void WriteJSON(List<Data> database)
        {
            string jsonOutput = JsonConvert.SerializeObject(database, Formatting.Indented);
            File.WriteAllText("database.json", jsonOutput);
        }
    }
}

/*
 * TO-DO
 * all data to json and read it globally
 * cancel operation if ctrl + c sometimes is cancelling only from 2nd input
 * fix auto logout timer when manager is logged in
*/