using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBrokerDB.Models
{
    public class Trader
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public double Funds { get; set; }
        public String Holdings { get; set; } // Holdings are stored in a string with {equityid, quantity} pair 1,5;2,10
    }
}
