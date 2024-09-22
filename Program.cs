using Microsoft.EntityFrameworkCore;
using WebAspDBeaverStudy.Data;
using WebAspDBeaverStudy.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("MyConnectionDB")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}");

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
    context.Database.Migrate();

    if (!context.Categories.Any())
    {
        var sausage = new CategoryEntity
        {
            Name = "Sausage",
            Image = "https://rivnepost.rv.ua/img/650/korisnoi-kovbasi-ne-buvae-hastroenterolohi-nazvali_20240612_4163.jpg",
            Description = "��� ����� ����������� �� ������� ������� �� ����������. " +
            "������� ��������, �� �� ��������, ���� ����� ������� �� ����� 50 ����� �� ����."
        };

        var cheese = new CategoryEntity
        {
            Name = "Cheese",
            Image = "https://www.vsesmak.com.ua/sites/default/files/styles/large/public/field/image/syrnaya_gora_5330_1900_21.jpg?itok=RPUrRskl",
            Description = "C�� � ���� � ���������� ������ �� ������ ����. " +
            "���� �� � ������, � �������, � ��������. �� ����� �������, �� �����, " +
            "�� ��������� �� ��������� ������������ ������� ��� � ��������."
        };

        var bread = new CategoryEntity
        {
            Name = "Bread",
            Image = "https://ukr.media/static/ba/aimg/3/7/7/377126_1.jpg",
            Description = "� ������� ����� ���������� ����������� ������� ����� ����, " +
            "�� ����� �� �������� ������ ����� ���� � ���������, �������������� ���."
        };
        context.Categories.Add(sausage);
context.Categories.Add(cheese);
        context.Categories.Add(bread);
        context.SaveChanges();
    }
}

app.Run();
