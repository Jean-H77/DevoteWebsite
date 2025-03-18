using System.ComponentModel.DataAnnotations;

namespace DevoteWebsite.ViewModels
{

    public class StoreCartItemViewModel
    {
        public string Uid { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}
