using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Adressboken.Data;
using Microsoft.AspNetCore.Identity.UI.Services;

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


            // Lägg till autentisering
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Cognito:ClientId"] ?? throw new InvalidOperationException("Cognito ClientId is not set.");
                options.ResponseType = builder.Configuration["Authentication:Cognito:ResponseType"] ?? throw new InvalidOperationException("Cognito ResponseType is not set.");
                options.MetadataAddress = builder.Configuration["Authentication:Cognito:MetadataAddress"] ?? throw new InvalidOperationException("Cognito MetadataAddress is not set.");
                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProviderForSignOut = context =>
                    {
                        context.ProtocolMessage.Scope = builder.Configuration["Authentication:Cognito:Scope"] ?? throw new InvalidOperationException("Cognito Scope is not set.");
                        context.ProtocolMessage.ResponseType = builder.Configuration["Authentication:Cognito:ResponseType"] ?? throw new InvalidOperationException("Cognito ResponseType is not set."); ;
                        // context.ProtocolMessage.IssuerAddress = CognitoHelpers.GetCognitoLogoutUrl(builder.Configuration, context.HttpContext);
                        // Create Cognito logout URL
                        var cognitoDomain = builder.Configuration["Authentication:Cognito:CognitoDomain"] ?? throw new InvalidOperationException("Cognito CognitoDomain is not set.");
                        var clientId = builder.Configuration["Authentication:Cognito:ClientId"] ?? throw new InvalidOperationException("Cognito ClientId is not set.");
                        var appSignOutUrl = builder.Configuration["Authentication:Cognito:AppSignOutUrl"] ?? throw new InvalidOperationException("Cognito AppSignOutUrl is not set.");
                        var logoutUrl = $"{context.Request.Scheme}://{context.Request.Host}{appSignOutUrl}";
                        var cognitoLogoutUrl = $"{cognitoDomain}/logout?client_id={clientId}&logout_uri={logoutUrl}";
                        context.ProtocolMessage.IssuerAddress = cognitoLogoutUrl;
                        // Close authentication sessions
                        context.Properties.Items.Remove(CookieAuthenticationDefaults.AuthenticationScheme);
                        context.Properties.Items.Remove(OpenIdConnectDefaults.AuthenticationScheme);
                        return Task.CompletedTask;
                    }
                };
            });


            // L�gg till MongoDB-anslutningen
            var connectionString = "mongodb+srv://martinsandung:IDjcjDU7aeePGhEX@cluster1.chdrb4f.mongodb.net/?retryWrites=true&w=majority";
            var databaseName = "Person";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            builder.Services.AddSingleton(database);
            
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
           {
               endpoints.MapControllerRoute(
                   name: "search",
                   pattern: "Kund/Search",
                   defaults: new { controller = "Kund", action = "Search" });

               endpoints.MapControllerRoute(
                   name: "searchResults",
                   pattern: "Kund/SearchResults",
                   defaults: new { controller = "Kund", action = "SearchResults" });

               endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
           });


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}