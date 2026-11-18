using wt_lab6_6minimalapi_ostashevskaya.Models;

namespace wt_lab6_6minimalapi_ostashevskaya.Data;

/// <summary>
/// Хранилище товаров в памяти приложения.
/// Одна коллекция используется всеми endpoint-ами Minimal API:
/// список, поиск, создание, изменение и удаление работают с одними и теми же данными.
/// База данных в WT-6 не используется, данные теряются при перезапуске приложения.
/// </summary>
public static class ProductStore
{
    private static readonly object SyncRoot = new();
    private static int _nextId = 7;

    public static List<Product> Products { get; } = new()
    {
        new Product
        {
            Id = 1,
            Name = "Ноутбук Lenovo ThinkPad E14",
            Sku = "SKU-1001",
            Category = "Компьютеры",
            Supplier = "ООО «ТехноСнаб»",
            Quantity = 12,
            Price = 2_899m,
            Description = "Корпоративный ноутбук для офисных задач: 16 ГБ ОЗУ, SSD 512 ГБ, экран 14″.",
            IsAvailable = true,
            ReceivedAt = DateTime.Today.AddDays(-3)
        },
        new Product
        {
            Id = 2,
            Name = "Беспроводная мышь Logitech M185",
            Sku = "SKU-1002",
            Category = "Аксессуары",
            Supplier = "ООО «ТехноСнаб»",
            Quantity = 48,
            Price = 79m,
            Description = "Компактная беспроводная мышь с USB-приёмником и временем работы до 12 месяцев.",
            IsAvailable = true,
            ReceivedAt = DateTime.Today.AddDays(-5)
        },
        new Product
        {
            Id = 3,
            Name = "Офисное кресло ErgoLine Comfort",
            Sku = "SKU-2001",
            Category = "Мебель",
            Supplier = "Мебельная фабрика «Комфорт»",
            Quantity = 7,
            Price = 529m,
            Description = "Эргономичное кресло с регулировкой высоты, поясничной поддержкой и сетчатой спинкой.",
            IsAvailable = true,
            ReceivedAt = DateTime.Today.AddDays(-10)
        },
        new Product
        {
            Id = 4,
            Name = "Монитор Dell UltraSharp 27″",
            Sku = "SKU-1003",
            Category = "Компьютеры",
            Supplier = "ООО «ТехноСнаб»",
            Quantity = 0,
            Price = 1_099m,
            Description = "27-дюймовый IPS-монитор с разрешением 2560×1440 и поддержкой USB-C.",
            IsAvailable = false,
            ReceivedAt = DateTime.Today.AddDays(-14)
        },
        new Product
        {
            Id = 5,
            Name = "Бумага офисная А4, 500 листов",
            Sku = "SKU-3001",
            Category = "Расходники",
            Supplier = "ООО «Офис-Маркет»",
            Quantity = 120,
            Price = 12.90m,
            Description = "Белая офисная бумага плотностью 80 г/м². Упаковка 500 листов.",
            IsAvailable = true,
            ReceivedAt = DateTime.Today.AddDays(-1)
        },
        new Product
        {
            Id = 6,
            Name = "Стеллаж складской металлический",
            Sku = "SKU-2002",
            Category = "Мебель",
            Supplier = "Мебельная фабрика «Комфорт»",
            Quantity = 15,
            Price = 249m,
            Description = "Разборный металлический стеллаж для складских помещений, нагрузка до 150 кг на полку.",
            IsAvailable = true,
            ReceivedAt = DateTime.Today.AddDays(-20)
        }
    };

    /// <summary>Найти товар по идентификатору.</summary>
    public static Product? Find(int id) => Products.FirstOrDefault(product => product.Id == id);

    /// <summary>Добавить товар и присвоить ему следующий идентификатор.</summary>
    public static Product Add(Product product)
    {
        lock (SyncRoot)
        {
            product.Id = _nextId++;
            Products.Add(product);
            return product;
        }
    }

    /// <summary>Сохранить изменения существующего товара.</summary>
    public static bool Update(Product product)
    {
        lock (SyncRoot)
        {
            var existing = Find(product.Id);
            if (existing is null)
            {
                return false;
            }

            existing.Name = product.Name;
            existing.Sku = product.Sku;
            existing.Category = product.Category;
            existing.Supplier = product.Supplier;
            existing.Quantity = product.Quantity;
            existing.Price = product.Price;
            existing.Description = product.Description;
            existing.IsAvailable = product.IsAvailable;
            existing.ReceivedAt = product.ReceivedAt;
            return true;
        }
    }

    /// <summary>Удалить товар по идентификатору.</summary>
    public static bool Remove(int id)
    {
        lock (SyncRoot)
        {
            var existing = Find(id);
            if (existing is null)
            {
                return false;
            }

            Products.Remove(existing);
            return true;
        }
    }
}
