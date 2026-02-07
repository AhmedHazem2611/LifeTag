using LifeTag.Models;
using LifeTag.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<LifeTagContext>(options =>
{
    options.UseSqlServer("Server=DESKTOP-513950Q\\SQLEXPRESS;Database=LifeTagDB;Trusted_Connection=True;TrustServerCertificate=True;");
});
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<MedicalProfileService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
