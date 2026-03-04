using FinalProjectFiruz.Models.Common;

namespace FinalProjectFiruz.Models
{
    public class Product : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;

        public string SecondImagePath { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public double Rating { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

    }

}
