using System.Text.Encodings.Web;
using System.Text.Unicode;
using ext_trans_dbctx;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/*
https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/providers?tabs=dotnet-core-cli
dotnet ef migrations add InitSql --context AlphaDbContext --output-dir Migrations\AlphaSql -- --dbType sql
dotnet ef migrations add InitSql --context BetaDbContext --output-dir Migrations\BetaSql -- --dbType sql
dotnet ef migrations add InitSqlite --context AlphaDbContext --output-dir Migrations\AlphaSqlite -- --dbType sqlite
dotnet ef migrations add InitSqlite --context BetaDbContext --output-dir Migrations\BetaSqlite -- --dbType sqlite
*/

if (builder.Configuration.GetValue<string>("dbType", "sql") == "sqlite")
{
    var cs = "data source=" + Path.Combine(builder.Environment.ContentRootPath,
        "demo.sqlite");
    builder.Services.AddDbContext<AlphaDbContext>(options =>
    {
        options.UseSqlite(cs);
    });
    builder.Services.AddDbContext<BetaDbContext>(options =>
    {
        options.UseSqlite(cs);
    });
}
else
{
    var cs = @"Data Source=(localdb)\MSSQLLOcalDB;Initial Catalog=TransTest;Integrated Security=True;";
    builder.Services.AddDbContext<AlphaDbContext>(options =>
    {
        options.UseSqlServer(cs);
    });
    builder.Services.AddDbContext<BetaDbContext>(options =>
    {
        options.UseSqlServer(cs);
    });
}
builder.Services.AddScoped<TransTester>();

var app = builder.Build();
app.MapGet("/", () =>
    Results.Content(
        @"<a href=transcope target=res>TransactionScope</a> 
        <a href=sharetrans target=res>Shared Transaction</a> <br />
        <iframe name=res style='width: 640px; height: 150px'></iframe>",
        "text/html"));
app.MapGet("/transcope", (TransTester tester) => tester.TestTranScope());
app.MapGet("/sharetrans", (TransTester tester) => tester.TestSharedTrans());
app.Run();
