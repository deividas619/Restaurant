using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    public class Bill
    {
        public int TableNumber { get; set; }
        public List<Item> Items { get; set; }
        public decimal TotalAmountForTable { get; set; }

        //public Bill(int tableNumber, List<OrderItem> items, decimal totalAmountForTable)
        public Bill(int tableNumber, decimal totalAmountForTable)
        {
            TableNumber = tableNumber;
            TotalAmountForTable = totalAmountForTable;
        }
    }
}
