using FinalProjectFiruz.ViewModels.CategoryViewModels;
using FinalProjectFiruz.ViewModels.ProductViewModels;

namespace FinalProjectFiruz.ViewModels.HomeViewModels
{
    public class HomeVM
    {
        public List<ProductGetVM> Products { get; set; }
        public List<CategoryGetVM> Categories { get; set; }
    }
}
