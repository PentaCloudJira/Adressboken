using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Adressboken.Data;
using Adressboken.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.HttpOverrides;

using System.Security.Claims;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Compact;
using Utilities.Logging;

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
            builder.Services.AddHttpClient<RegCheckApiService>(client =>
            {
                client.BaseAddress = new Uri("https://www.regcheck.org.uk/api/reg.asmx/");
            });
            builder.Services.AddScoped<RegCheckApiService>();

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
                    },
                    OnTokenValidated = context =>
                    {
                        var claims = context.Principal.Claims
                        .Append(new Claim(ClaimTypes.Name, context.Principal.FindFirst("cognito:username").Value));
                        var claimsIdentity = new ClaimsIdentity(claims, context.Scheme.Name, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                        context.Principal = new ClaimsPrincipal(claimsIdentity);
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<UserNameEnricher>();

            var serviceProvider = builder.Services.BuildServiceProvider();
            var userNameEnricher = serviceProvider.GetService<UserNameEnricher>();
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                // .Enrich.FromLogContext() // Enrich log messages with additional context (e.g., request information).
                .Enrich.With(userNameEnricher) // Enrich log messages with the current user name.
                .WriteTo.Console(new RenderedCompactJsonFormatter()) // Output logs in JSON format.
                .CreateLogger();

            // Override the default logger configuration with Serilog.
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(Log.Logger);
            // MongoDB-anslutningen
            var connectionString = builder.Configuration["ConnectionString:DefaultConnection"];
            var databaseName = "Person";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            builder.Services.AddSingleton(database);

            builder.Services.AddSingleton<IEmailSender, EmailSender>();

            var app = builder.Build();

            var port = Environment.GetEnvironmentVariable("PORT") ?? "";

            if (!string.IsNullOrEmpty(port))
            {
                app.Urls.Add($"http://*:{port}");
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseForwardedHeaders();

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