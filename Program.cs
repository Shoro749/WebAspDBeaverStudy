using Bogus;
using Microsoft.EntityFrameworkCore;
using WebAspDBeaverStudy.Data;
using WebAspDBeaverStudy.Data.Entities;
using WebAspDBeaverStudy.Interfaces;
using WebAspDBeaverStudy.Mapper;
using WebAspDBeaverStudy.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("MyConnectionDB")));

builder.Services.AddScoped<IImageWorker, ImageWorker>();

builder.Services.AddAutoMapper(typeof(AppMapperProfile));

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
    var imageWorker = serviceScope.ServiceProvider.GetService<IImageWorker>();
    context.Database.Migrate();

    if (!context.Categories.Any())
    {
        var imageName = imageWorker.Save("https://rivnepost.rv.ua/img/650/korisnoi-kovbasi-ne-buvae-hastroenterolohi-nazvali_20240612_4163.jpg");
        var sausage = new CategoryEntity
        {
            Name = "Sausage",
            Image = imageName,
            Description = "��� ����� ����������� �� ������� �������� �� ����������. " +
            "������� ��������, �� �� ��������, ���� ����� ������� �� ����� 50 ����� �� ����."
        };

        imageName = imageWorker.Save("https://www.vsesmak.com.ua/sites/default/files/styles/large/public/field/image/syrnaya_gora_5330_1900_21.jpg?itok=RPUrRskl");
        var cheese = new CategoryEntity
        {
            Name = "Cheese",
            Image = imageName,
            Description = "C�� � ���� � ���������� ������ �� ������ ����. " +
            "���� �� � ������, � �������, � ��������. �� ����� �������, �� �����, " +
            "�� ��������� �� ��������� ������������ ������� ��� � ��������."
        };

        imageName = imageWorker.Save("https://ukr.media/static/ba/aimg/3/7/7/377126_1.jpg");
        var bread = new CategoryEntity
        {
            Name = "Bread",
            Image = imageName,
            Description = "� ������� ����� ���������� ����������� ������� ����� ����, " +
            "�� ����� �� �������� ������ ����� ���� � ���������, �������������� ���."
        };
        context.Categories.Add(sausage);
        context.Categories.Add(cheese);
        context.Categories.Add(bread);
        context.SaveChanges();
    }

    if (!context.Products.Any())
    {
        var categories = context.Categories.ToList();

        var fakerProduct = new Faker<ProductEntity>("uk")
                    .RuleFor(u => u.Name, (f, u) => f.Commerce.Product())
                    .RuleFor(u => u.Price, (f, u) => decimal.Parse(f.Commerce.Price()))
                    .RuleFor(u => u.Category, (f, u) => f.PickRandom(categories));

        string url = "https://picsum.photos/1200/800?product";

        var products = fakerProduct.GenerateLazy(30);

        Random r = new Random();

        foreach (var product in products)
        {
            context.Add(product);
            context.SaveChanges();
            int imageCount = r.Next(3, 5);
            for (int i = 0; i < imageCount; i++)
            {
                var imageName = imageWorker.Save(url);
                var imageProduct = new ProductImageEntity
                {
                    Product = product,
                    Image = imageName,
                    Priority = i
                };
                context.Add(imageProduct);
                context.SaveChanges();
            }
        }
    }
}

app.Run();
