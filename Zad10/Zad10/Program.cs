using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Zad10.Models;
using Zad10.Repositories;
using Zad10.Services;

public class Program
{
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        
        //Registering services
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();

        builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
        builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
        builder.Services.AddDbContext<PrescriptionContext>(opt =>
        {
            string? connString = builder.Configuration.GetConnectionString("DefaultConnection");
            opt.UseSqlServer(connString);
        });
        
        var app = builder.Build();

        //Configuring the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();
        /*using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<PrescriptionContext>();
                context.Database.Migrate();
                DbInitializer.Initialize(context).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
                
        }*/

        app.Run();
    }
}