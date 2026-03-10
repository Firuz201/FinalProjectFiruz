using FinalProjectFiruz.Contexts;
using FinalProjectFiruz.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FinalProjectFiruz.Controllers;

public class ProductPageController : Controller
{
    private readonly AppDbContext _context;

    public ProductPageController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Detail(int id)
    {
        var product = _context.Products
            .Where(x => x.Id == id)
            .Select(x => new ProductGetVM
            {
                Id = x.Id,
                Title = x.Title,
                Price = x.Price,
                Rating = x.Rating,
                ImagePath = x.ImagePath,
                Description = x.Description
            })
            .FirstOrDefault();

        if (product == null)
            return NotFound();

        return View(product);
    }

    public IActionResult Index()
    {
        return View();
    }
}
