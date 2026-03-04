using System.ComponentModel.DataAnnotations;

namespace FinalProjectFiruz.ViewModels.ProductViewModels;

public class ProductCreateVM
{
    [Required, MaxLength(256), MinLength(3)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(1024), MinLength(3)]
    public string Description { get; set; } = string.Empty;

    [Required, Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required, Range(0,5)]
    public double Rating { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public IFormFile Image { get; set; } = null!;

    [Required]
    public IFormFile SecondImage { get; set; } = null!;

}
