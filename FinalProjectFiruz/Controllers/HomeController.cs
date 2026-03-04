using System.Diagnostics;
using FinalProjectFiruz.Contexts;
using FinalProjectFiruz.Models;
using FinalProjectFiruz.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectFiruz.Controllers
{
    public class HomeController(AppDbContext _context) : Controller
    {
        public async Task <IActionResult> Index()
        {
            var products = await _context.Products.Select(product => new ProductGetVM()
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                CategoryName = product.Category.Name,
                ImagePath = product.ImagePath,
                SecondImagePath = product.SecondImagePath,
                Price = product.Price,
                Rating = product.Rating
            }).ToListAsync();
            return View(products);
        }
    }
}
