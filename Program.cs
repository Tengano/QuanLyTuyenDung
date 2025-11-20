using Microsoft.EntityFrameworkCore;
using TrangChu.Models;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connection))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connection));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<DataContext>();
        
        if (await context.Database.CanConnectAsync())
        {
            logger.LogInformation("Database connected. Applying migrations...");
            try
            {
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully.");
            }
            catch (Exception migEx)
            {
                logger.LogWarning(migEx, "Migration warning: {Message}", migEx.Message);
            }
        }
        else
        {
            logger.LogWarning("Cannot connect to database. Please check your connection string and SQL Server.");
            logger.LogInformation("Connection string: {Connection}", connection?.Replace("Password=.*;", "Password=***;"));
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while setting up the database.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
