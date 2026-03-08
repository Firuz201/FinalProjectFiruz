using FinalProjectFiruz.Abstraction;
using FinalProjectFiruz.Contexts;
using FinalProjectFiruz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FinalProjectFiruz.Controllers;
[Authorize]
public class BasketController(AppDbContext _context, IBasketService _basketService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var basketItems = await _basketService.GetBasketItemsAsync();
        return View(basketItems);
    }
    public async Task<IActionResult> AddToBasket(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(x => x.Id == productId);

        if (!isExistProduct)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var isExistUser = await _context.Users.AnyAsync(x => x.Id == userId);

        if (!isExistUser)
            return BadRequest();

        var existBasketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.AppUserId == userId && x.ProductId == productId);

        if(existBasketItem is { })
        {
            existBasketItem.Count++;

            _context.Update(existBasketItem);
        }
        else
        {
            BasketItem item = new()
            {
                ProductId = productId,
                Count = 1,
                AppUserId = userId!
            };

            await _context.BasketItems.AddAsync(item);
        }

            
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> RemoveFromBasket(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(x => x.Id == productId);

        if (!isExistProduct)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var isExistUser = await _context.Users.AnyAsync(x => x.Id == userId);

        if (!isExistUser)
            return BadRequest();

        var existBasketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.AppUserId == userId && x.ProductId == productId);

        if(existBasketItem is null)
        {
            return NotFound();
        }

        _context.BasketItems.Remove(existBasketItem);
        await _context.SaveChangesAsync();

        var returnUrl = Request.Headers["Referer"];
        
        if (!string.IsNullOrEmpty(returnUrl))
            return Redirect(returnUrl!);

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> DecreaseBasketItemCount(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(x => x.Id == productId);

        if (!isExistProduct)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var isExistUser = await _context.Users.AnyAsync(x => x.Id == userId);

        if (!isExistUser)
            return BadRequest();

        var existBasketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.AppUserId == userId && x.ProductId == productId);

        if (existBasketItem is null)
        {
            return NotFound();
        }

        if (existBasketItem.Count > 1)
            existBasketItem.Count--;

        _context.BasketItems.Update(existBasketItem);
        await _context.SaveChangesAsync();

        var returnUrl = Request.Headers["Referer"];

        if (!string.IsNullOrEmpty(returnUrl))
            return Redirect(returnUrl!);

        return RedirectToAction("Index");
    }
}
