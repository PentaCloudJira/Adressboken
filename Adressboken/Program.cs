using Adressboken.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using MongoDB.Driver;

namespace Adressboken
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var emailSettings = builder.Configuration.GetSection(nameof(EmailSettings)).Get<EmailSettings>();
            
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Lägg till MongoDB-anslutningen
            var connectionString = "mongodb+srv://martinsandung:IDjcjDU7aeePGhEX@cluster1.chdrb4f.mongodb.net/?retryWrites=true&w=majority";
            var databaseName = "Person"; 

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            builder.Services.AddSingleton(database);

            builder.Services.AddSingleton(emailSettings);
            builder.Services.AddSingleton<IEmailSender, EmailSender>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}