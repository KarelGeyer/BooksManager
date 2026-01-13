using BookManagement.Common.Middleware;
using BookManagement.DbService;
using BookManagement.DbService.Interfaces;
using BookManagement.Services;
using BookManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowBlazor",
        policyBuilder =>
        {
            policyBuilder
                .WithOrigins("https://localhost:7112")
                .AllowCredentials()
                .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                .WithHeaders("Content-Type", "Authorization", "X-Requested-With")
                .WithExposedHeaders("X-Total-Count");
        }
    );
});

builder.Services.AddScoped(typeof(IDbService<>), typeof(DbService<>));

builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();

app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowBlazor");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
