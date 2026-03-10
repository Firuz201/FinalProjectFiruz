using FinalProjectFiruz.Abstraction;
using FinalProjectFiruz.Contexts;
using FinalProjectFiruz.Models;
using FinalProjectFiruz.ViewModels.CategoryViewModels;
using FinalProjectFiruz.ViewModels.HomeViewModels;
using FinalProjectFiruz.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FinalProjectFiruz.Controllers;

public class HomeController(AppDbContext _context) : Controller
{
    public async Task<IActionResult> Index(int? categoryId)
    {
        var query = _context.Products.AsQueryable();

        if (categoryId != null)
            query = query.Where(p => p.CategoryId == categoryId);

        var products = await query.Select(product => new ProductGetVM()
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

        var categories = await _context.Categories.Select(category => new CategoryGetVM()
        {
            Id = category.Id,
            Name = category.Name
        }).ToListAsync();

        HomeVM vm = new HomeVM
        {
            Products = products,
            Categories = categories
        };

        return View(vm);
    }

    //public async Task<IActionResult> SendEmail()
    //{
    //    await _service.SendEmailAsync("firuzvh-mpa201@code.edu.az", "Email service", "Service is done");
    //    return Ok("Ok");
    //}
}
