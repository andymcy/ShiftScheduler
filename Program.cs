using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using ShiftScheduler.Models;              // ← for ShiftSchedulerContext

var builder = WebApplication.CreateBuilder(args);

// 1) register the scaffolded context
builder.Services.AddDbContext<ShiftSchedulerContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    )
);

// 2) add MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 3) middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// 4) default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Schedule}/{action=Index}/{id?}"
);

app.Run();
