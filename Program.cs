using cugoj_ng_server.Utilities;
using StackExchange.Redis;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
var redisConnstr = configuration.GetConnectionString("redis");
var mysqlConnstr = configuration.GetConnectionString("mysql");
var sessionTimeout = configuration.GetValue<int>("Config:SessionTimeout");

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

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

Inject.SetPropertiesByType<cugoj_ng_server.Models.DbConn>(new()
{
    { typeof(IConnectionMultiplexer), ConnectionMultiplexer.Connect(redisConnstr) },
    { typeof(Func<IDbConnection>), new Func<IDbConnection>(() => new MySql.Data.MySqlClient.MySqlConnection(mysqlConnstr)) },
});

services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.IncludeFields = true;
});

// Configure the HTTP request pipeline.
var app = builder.Build();

if (env.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseRouting();

app.UseSession();

app.MapControllers();

app.Run();
