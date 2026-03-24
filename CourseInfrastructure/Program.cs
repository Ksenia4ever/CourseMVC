using CourseDomain.Model;
using CourseInfrastructure;
using CourseInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using static CourseInfrastructure.Services.IDataPortServiceFactory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddDbContext<DbCourseContext>(option => option.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

builder.Services.AddScoped<IDataPortServiceFactory<CourseAccount>, CourseSubscriptionDataPortServiceFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();