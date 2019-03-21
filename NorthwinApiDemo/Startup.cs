using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NorthwindApiDemo.EFModels;
using NorthwindApiDemo.Models;
using NorthwindApiDemo.Services;

namespace NorthwindApiDemo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<NorthwindContext>(options =>
            {
                options.UseSqlServer("Server=.;Database=Northwind;Trusted_Connection=True;");
            });

            services.AddScoped<ICustomerRepository, CustomerRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();

            AutoMapper.Mapper.Initialize(config =>
            {
                config.CreateMap<Customers, CustomerWithoutOrders>();//Mapeo de Customers a CustomerWithoutOrders
                config.CreateMap<Customers, CustomerDTO>();
                config.CreateMap<Orders, OrdersDTO>();
                config.CreateMap<OrdersForCreationDTO, Orders>();
                config.CreateMap<OrdersForUpdateDTO, Orders>();
            });

            app.UseMvc();

            //Esta forma de route no se recomienda
            //app.UseMvc(config =>
            //{
            //config.MapRoute(
            //    name: "Default",
            //    template: "{controller}/{action}/{id?}",
            //        defaults: new
            //        {
            //            controller = "Home",
            //            action = "Index"
            //        });
            //});

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
