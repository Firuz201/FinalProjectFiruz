namespace FinalProjectFiruz.ViewModels.ProductViewModels;

public class ProductGetVM
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImagePath { get; set; } = string.Empty;

    public string SecondImagePath { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public double Rating {  get; set; }

}
