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
            HelperMethods.InitMainMenu();
        }

        public static void UpdateDatabase(RestaurantData database)
        {
            string jsonOutput = JsonConvert.SerializeObject(database, Formatting.Indented);
            File.WriteAllText("Database.json", jsonOutput);
        }
        public static string ReturnPW()
        {
            return "";
        }
    }
}

/*
 * TO-DO
 * cancel operation if ctrl + c sometimes is cancelling only from 2nd interation of the cts check
 * cancellation token messes up other inputs like providing username for login (maybe use dispose() of the cts?)
 * fix auto logout timer when manager is logged in
 * use async for email sending
*/

/*
 * USEFUL LINKS
 * https://quicktype.io/csharp [JSON TO C#]
 * https://json2csharp.com/ [JSON TO C#]
 * https://codebeautify.org/jsonviewer/cbfb9fe5 [JSON TO OBJECT STRUCTURE]
 * https://jsonblob.com/ [JSON TO OBJECT STRUCTURE]
*/