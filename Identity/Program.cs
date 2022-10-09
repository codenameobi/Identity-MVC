using Identity.IdentityPolicy;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Get connection string from environment variables
var conString = builder.Configuration.GetConnectionString("DefaultConnection");


// Add services to the container.
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(conString));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(opts =>
{
    opts.User.RequireUniqueEmail = true;
    opts.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz";
    opts.Password.RequiredLength = 8;
    opts.Password.RequireLowercase = true;
});

builder.Services.AddControllersWithViews();

// Add Identity Custom Password, Username and Email Policies.
builder.Services.AddTransient<IPasswordValidator<AppUser>, CustomPasswordPolicy>();
builder.Services.AddTransient<IUserValidator<AppUser>, CustomUsernameEmailPolicy>();

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

