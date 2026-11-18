using System.ComponentModel.DataAnnotations;

namespace wt_lab6_6minimalapi_ostashevskaya.Models;

/// <summary>
/// Товар на складе — основная сущность предметной области «Склад и логистика».
/// Модель перенесена из лабораторных работ WT-4 (MVC) и WT-5 (Razor Pages).
/// </summary>
public class Product
{
    /// <summary>Идентификатор товара. При создании через POST генерируется сервером.</summary>
    public int Id { get; set; }

    /// <summary>Наименование товара.</summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Артикул товара.</summary>
    [Required]
    [StringLength(20)]
    public string Sku { get; set; } = string.Empty;

    /// <summary>Категория товара.</summary>
    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    /// <summary>Поставщик.</summary>
    [Required]
    [StringLength(100)]
    public string Supplier { get; set; } = string.Empty;

    /// <summary>Количество на складе, шт.</summary>
    [Range(0, 100_000)]
    public int Quantity { get; set; }

    /// <summary>Цена за единицу, BYN.</summary>
    [Range(0.01, 1_000_000)]
    public decimal Price { get; set; }

    /// <summary>Описание товара.</summary>
    [StringLength(300)]
    public string? Description { get; set; }

    /// <summary>Доступен к продаже.</summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>Дата поступления на склад.</summary>
    public DateTime ReceivedAt { get; set; } = DateTime.Today;

    /// <summary>Стоимость партии: цена × количество.</summary>
    public decimal TotalValue => Price * Quantity;
}
