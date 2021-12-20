using System;
using Microsoft.EntityFrameworkCore;
using eBrokerDB.Models;
using System.Collections.Generic;

namespace eBrokerDB { 
    public class EBrokerDBContext : DbContext
    {
        public DbSet<Equity> Equities { get; set; }
        public DbSet<Trader> Traders { get; set; }

        public EBrokerDBContext(DbContextOptions options): base(options)
        {
            
        }
    }
}
