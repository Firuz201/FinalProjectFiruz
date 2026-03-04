using FinalProjectFiruz.Contexts;
using FinalProjectFiruz.Helpers;
using FinalProjectFiruz.Models;
using FinalProjectFiruz.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FinalProjectFiruz.Areas.Admin.Controllers;
[Area("Admin")]
public class ProductController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly string folderPath;

    public ProductController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
        folderPath = Path.Combine(_environment.WebRootPath, "assets", "images");
    }

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
            Rating = product.Rating,
            
        }).ToListAsync();
        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        await SendCategoriesWithViewBag();

        return View();
    }

    

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateVM vm)
    {
        await SendCategoriesWithViewBag();

        if (!ModelState.IsValid)
            return View(vm);

        var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == vm.CategoryId);

        if (!isExistCategory)
        {
            ModelState.AddModelError("CategoryId", "This category is not found");
            return View(vm);
        }


        //FIRST IMAGE CHECK
        if (vm.Image.Length > 2 * 1024 * 1024)
        {
            ModelState.AddModelError("Image", "The image size must not exceed 2 MB");
            return View(vm);
        }

        if (!vm.Image.ContentType.Contains("image"))
        {
            ModelState.AddModelError("Image", "You can only upload file with image type");
            return View(vm);
        }


        //SECOND IMAGE
        if (vm.SecondImage.Length > 2 * 1024 * 1024)
        {
            ModelState.AddModelError("SecondImage", "The image size must not exceed 2 MB");
            return View(vm);
        }

        if (!vm.SecondImage.ContentType.Contains("image"))
        {
            ModelState.AddModelError("SecondImage", "You can only upload file with image type");
            return View(vm);
        }

        string secondUniqueFileName = Guid.NewGuid().ToString() + vm.SecondImage.FileName;




        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images");



        //ADDING FIRST IMAGE
        string uniqueFileName = await vm.Image.FileUploadAsync(folderPath);


        //ADDING SECOND IMAGE
        string secondpath = Path.Combine(folderPath, secondUniqueFileName);

        using FileStream secondstream = new(secondpath, FileMode.Create);

        await vm.SecondImage.CopyToAsync(secondstream);


        Product product = new()
        {
            Title = vm.Title,
            Description = vm.Description,
            Rating = vm.Rating,
            Price = vm.Price,
            CategoryId = vm.CategoryId,
            ImagePath = uniqueFileName,
            SecondImagePath = secondUniqueFileName
        };
        
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id); 

        if (product is null)
            return NotFound();

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        string deletedFilePath = Path.Combine(folderPath, product.ImagePath);

        //if (System.IO.File.Exists(deletedFilePath))
        //    System.IO.File.Delete(deletedFilePath);

        FileHelper.FileDelete(deletedFilePath);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
            return NotFound();

        ProductUpdateVM vm = new()
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Rating = product.Rating,
            Price = product.Price,
            CategoryId = product.CategoryId,
        };

        await SendCategoriesWithViewBag();

        return View(vm);
    }


    [HttpPost]
    public async Task<IActionResult> Update(ProductUpdateVM vm)
    {
        await SendCategoriesWithViewBag();

        if (!ModelState.IsValid)
            return View(vm);

        var existProduct = await _context.Products.FindAsync(vm.Id);

        if (existProduct is null)
            return BadRequest();

        var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == vm.CategoryId);

        if (!isExistCategory)
        {
            ModelState.AddModelError("CategoryId", "This category is not found");
            return View(vm);
        }

        if (!vm.Image?.CheckSize(2) ?? false)
        {
            ModelState.AddModelError("Image", "The image size must not exceed 2 MB");
            return View(vm);
        }

        if (!vm.Image?.CheckType("image") ?? false)
        {
            ModelState.AddModelError("Image", "You can only upload file with image type");
            return View(vm);
        }

        existProduct.Title = vm.Title;
        existProduct.Description = vm.Description;
        existProduct.Price = vm.Price;
        existProduct.Rating = vm.Rating;
        existProduct.CategoryId = vm.CategoryId;

        if (vm.Image is { })
        {
            string newImagePath = await vm.Image.FileUploadAsync(folderPath);

            string deletedImagePath = Path.Combine(folderPath, existProduct.ImagePath);

            FileHelper.FileDelete(deletedImagePath);

            existProduct.ImagePath = newImagePath;
        }

        _context.Products.Update(existProduct);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    private async Task SendCategoriesWithViewBag()
    {
        var categories = await _context.Categories.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() }).ToListAsync();

        ViewBag.Categories = categories;
    }

}
