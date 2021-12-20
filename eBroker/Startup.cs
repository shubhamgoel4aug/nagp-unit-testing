using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eBrokerDB;
using eBrokerDB.Models;
using eBrokerDBRepository.Interfaces;
using eBrokerDBRepository.Operations;
using AutoMapper;
using Traders.Mapper;
using Traders.Interfaces;
using Traders.Operations;

namespace eBroker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EBrokerDBContext>(options => options.UseInMemoryDatabase("eBroker"));
            services.AddScoped<EBrokerDBContext>();
            services.AddScoped<ITraderRepository, TraderRepository>();
            services.AddTransient<ITraderOperations, TraderOperations>();
            services.AddSingleton<IWrapper, Wrapper>();
            services.AddAutoMapper(typeof(TraderMapper));
            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var options = new DbContextOptionsBuilder<EBrokerDBContext>().UseInMemoryDatabase("eBroker").Options;
            using (var context = new EBrokerDBContext(options))
            {
                context.Equities.Add(new Equity() { Id = 1, Name = "Nagarro", Price = 3500 });
                context.Equities.Add(new Equity() { Id = 2, Name = "TCS", Price = 3000 });
                context.Equities.Add(new Equity() { Id = 3, Name = "TataSteel", Price = 1200 });

                context.Traders.Add(new Trader()
                {
                    Id = 1,
                    Name = "Champion",
                    Funds = 100000,
                    Holdings = "1,10;2,5"
                });
                context.Traders.Add(new Trader()
                {
                    Id = 2,
                    Name = "Hero",
                    Funds = 200000,
                    Holdings = "2,10;3,5"
                });
                context.Traders.Add(new Trader()
                {
                    Id = 3,
                    Name = "Chris",
                    Funds = 350000,
                    Holdings = "1,5;2,5"
                });

                context.SaveChanges();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
