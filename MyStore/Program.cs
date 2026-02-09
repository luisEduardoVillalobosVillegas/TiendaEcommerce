using Microsoft.EntityFrameworkCore;
using MyStore.Context;
using MyStore.Repositories;
using MyStore.Services;
using System.Data.SqlTypes;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlString"));

});

//se agrega al agregar la carpeta services y repositories en la primera linea de las siguientes dos <> es que espera cualquier entidad
builder.Services.AddScoped(typeof(GenericRepository<>));
//agrego el repositorio de Order
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();
//agrego el servicio de Order
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<UserService>();
//se agrega sesiones
builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(10); });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.AccessDeniedPath = "/Home/Error";
    }
    );
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
//para la carga de imagenes en wwwroot
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
