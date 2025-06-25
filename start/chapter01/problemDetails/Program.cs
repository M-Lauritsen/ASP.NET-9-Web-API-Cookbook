using ProblemDetailsDemo.Data;
using ProblemDetailsDemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Bogus;
using ProblemDetailsDemo.Services;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var connection = new SqliteConnection("DataSource=:memory:");
connection.Open();

builder.Services.AddControllers();

builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = (context) =>
{
    var httpContext = context.HttpContext;
    context.ProblemDetails.Extensions["traceID"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
    context.ProblemDetails.Extensions["suppport"] = "support@example.com";

    if(context.ProblemDetails.Status == StatusCodes.Status401Unauthorized)
    {
        context.ProblemDetails.Title = "Unauthorized Access";
        context.ProblemDetails.Detail = "You are not authorized to access this resource";
}
    else if (context.ProblemDetails.Status == StatusCodes.
             Status404NotFound)
    {
        context.ProblemDetails.Title = "Resource Not Found";
        context.ProblemDetails.Detail = "The resource you are looking for was not found.";
            }
    else
    {
        context.ProblemDetails.Title = "An unexpected error occurred";
                context.ProblemDetails.Detail = "An unexpected error occurred.Please try again later.";
    }
});

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(connection));
    
builder.Services.AddScoped<IProductsService, ProductReadService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    
    if (!context.Products.Any())
    {
        var productFaker = new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Price, f => f.Finance.Amount(50, 2000))
            .RuleFor(p => p.CategoryId, f => f.Random.Int(1, 5));
        context.Products.AddRange(productFaker.Generate(10000));
        context.SaveChanges();
    }
}

app.MapControllers();
app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.Run();
