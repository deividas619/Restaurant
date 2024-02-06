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
        //public static JObject databaseObject = JObject.Parse(File.ReadAllText(@"Database.json"));

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, args) =>
            {
                args.Cancel = true;
                cts.Cancel();
            };

            /*Database db = new Database
            {
                Restaurant = new("Restaurant_1"),
                List<Employee> = new
                {
                    (1, "Ned", "Flanders", "nedfla", "1234", true, true),
                    (2, "Bob", "Bobinski", "bobbob", "1234", true, true),
                    (3, "Tod", "Flanders", "todfla", "1234", false, true),
                    (4, "Rod", "Flanders", "rodfla", "1234", true, false),
                    (5, "Dennis", "Rodman", "denrod", "1234", false, false)
                },
                List<Items> items = new(),
            };*/

            ReadJSON();

            HelperMethods.InitMainMenu(cts);
        }

        public static void ReadJSON()
        {
            /*using (StreamReader sr = File.OpenText(@"Database.json"))
            {
                var myObject = JsonConvert.DeserializeObject<List<DatabaseModel>>(sr.ReadToEnd());
            }*/
        }
        public static void WriteJSON(List<DatabaseModel> database)
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