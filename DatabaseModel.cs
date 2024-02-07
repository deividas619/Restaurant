using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    public class RestaurantData
    {
        public List<Employee> Employees { get; set; }
        public List<Table> Tables { get; set; }
        public List<Order> Order { get; set; }
        public List<Item> Item { get; set; }
        public List<Bill> Bill { get; set; }
    }
}
