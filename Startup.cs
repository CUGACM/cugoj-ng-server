using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace cugoj_ng_server
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
            var redisConnstr = Configuration.GetConnectionString("redis");
            var mysqlConnstr = Configuration.GetConnectionString("mysql");
            var sessionTimeout = Configuration.GetValue<int>("Config:SessionTimeout");
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = redisConnstr;
                opt.InstanceName = "CUGOJ$";
            });
            services.AddSession(opt =>
            {
                opt.Cookie.Name = "_SESSION";
                opt.Cookie.IsEssential = true;
                opt.IdleTimeout = TimeSpan.FromMinutes(sessionTimeout);
            });
            Utilities.Inject.SetPropertiesByType(typeof(Models.DbConn), null, new()
            {
                { typeof(IConnectionMultiplexer), ConnectionMultiplexer.Connect(redisConnstr) },
                { typeof(Func<IDbConnection>), new Func<IDbConnection>(() => new MySql.Data.MySqlClient.MySqlConnection(mysqlConnstr)) },
            });
            services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.IncludeFields = true;
            }); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
