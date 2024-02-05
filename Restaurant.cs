using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    internal interface IReturnWorkingHours
    {
        string GetWorkingHours();
    }
    internal class Restaurant : IReturnWorkingHours
    {
        public TimeOnly WorkingHoursFrom = new TimeOnly(10, 00, 00);
        public TimeOnly WorkingHoursTo = new TimeOnly(22, 00, 00);

        public string GetWorkingHours()
        {
            return $"From: {WorkingHoursFrom} to: {WorkingHoursTo}";
        }
    }
}
