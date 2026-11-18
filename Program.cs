using Microsoft.OpenApi.Models;
using wt_lab6_6minimalapi_ostashevskaya.Data;
using wt_lab6_6minimalapi_ostashevskaya.Models;

var builder = WebApplication.CreateBuilder(args);

// Описание API (OpenAPI): документ доступен по адресу /swagger/v1/swagger.json,
// графический интерфейс Swagger UI — по адресу /swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Складской учёт — Minimal API",
        Version = "v1",
        Description = "REST API для управления товарами на складе. Лабораторная работа WT-6."
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Складской учёт API v1");
});

// GET / — проверка работоспособности приложения.
app.MapGet("/", () => "Minimal API работает!");

// GET /api/info — сведения о приложении в формате JSON.
app.MapGet("/api/info", () => new
{
    Application = "Складской учёт",
    Technology = "ASP.NET Core Minimal API",
    Version = "1.0",
    Domain = "Склад и логистика",
    Documentation = "/swagger"
})
.WithTags("Info")
.WithSummary("Информация о приложении");

// GET /api/products?search=...&category=... — список товаров с фильтрацией.
app.MapGet("/api/products", (string? search, string? category) =>
{
    IEnumerable<Product> products = ProductStore.Products;

    if (!string.IsNullOrWhiteSpace(search))
    {
        products = products.Where(product =>
            product.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
            || product.Sku.Contains(search, StringComparison.OrdinalIgnoreCase));
    }

    if (!string.IsNullOrWhiteSpace(category))
    {
        products = products.Where(product =>
            product.Category.Contains(category, StringComparison.OrdinalIgnoreCase));
    }

    return Results.Ok(products.ToList());
})
.WithTags("Products")
.WithSummary("Получить список товаров")
.WithDescription("Query-параметры search и category необязательны.");

// GET /api/products/{id} — товар по идентификатору.
app.MapGet("/api/products/{id:int}", (int id) =>
{
    var product = ProductStore.Find(id);
    return product is null ? Results.NotFound() : Results.Ok(product);
})
.WithTags("Products")
.WithSummary("Получить товар по идентификатору")
.WithName("GetProduct");

// POST /api/products — создание товара, идентификатор генерируется автоматически.
app.MapPost("/api/products", (Product product) =>
{
    var error = Validate(product);
    if (error is not null)
    {
        return Results.BadRequest(new { error });
    }

    ProductStore.Add(product);
    return Results.Created($"/api/products/{product.Id}", product);
})
.WithTags("Products")
.WithSummary("Создать товар")
.WithDescription("Идентификатор генерируется сервером, в теле запроса его передавать не нужно.");

// PUT /api/products/{id} — изменение товара.
app.MapPut("/api/products/{id:int}", (int id, Product updatedProduct) =>
{
    var product = ProductStore.Find(id);
    if (product is null)
    {
        return Results.NotFound();
    }

    var error = Validate(updatedProduct);
    if (error is not null)
    {
        return Results.BadRequest(new { error });
    }

    updatedProduct.Id = id;
    ProductStore.Update(updatedProduct);
    return Results.Ok(updatedProduct);
})
.WithTags("Products")
.WithSummary("Изменить товар");

// DELETE /api/products/{id} — удаление товара.
app.MapDelete("/api/products/{id:int}", (int id) =>
{
    var removed = ProductStore.Remove(id);
    return removed ? Results.NoContent() : Results.NotFound();
})
.WithTags("Products")
.WithSummary("Удалить товар");

// Render передаёт порт в переменной окружения PORT.
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");

// Проверка обязательных полей модели: при ошибке возвращается 400 Bad Request.
static string? Validate(Product product)
{
    if (string.IsNullOrWhiteSpace(product.Name))
    {
        return "Не указано наименование товара (Name).";
    }

    if (string.IsNullOrWhiteSpace(product.Sku))
    {
        return "Не указан артикул (Sku).";
    }

    if (string.IsNullOrWhiteSpace(product.Category))
    {
        return "Не указана категория (Category).";
    }

    if (product.Price <= 0)
    {
        return "Цена (Price) должна быть больше нуля.";
    }

    if (product.Quantity < 0)
    {
        return "Количество (Quantity) не может быть отрицательным.";
    }

    return null;
}
