using DevoteWebsite.Data;
using DevoteWebsite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DevoteWebsite.Controllers
{
    // pk_test_51R0xj8QkznWYVUdaHlNWJDu1BwASKwRU4JkkcGLYB6j7nuNiMbgtJJYpj6kZrJZF6JEicxbg6vKSfuE4JaoyMJC100X7p2nJlQ
    // sk_test_51R0xj8QkznWYVUdaCYG86XBCPKMOOk8GvpQeu4IqOXAoiDMSQ7DCBvSKq4taFvUEKeXMS04LzKrmcHmxRvwlKxci00LBK4X7oc
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StoreController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {

            HasCartSessionOrCreate();
            
            var storeItems = await _db.StoreItems
                .Include(item => item.StoreItemSaleInfo)
                .ToListAsync();

            HttpContext.Session.SetString("cart_total", "33.3");

            var storeItemsViewModel = storeItems.Select(item => new StoreItemViewModel
            {
                Name = item.Name,
                Price = item.Price,
                Uid = item.Uid,
                Description = item.Description,
                SalePercentage = item.StoreItemSaleInfo != null ? item.StoreItemSaleInfo.PercentageOff : 0,
                DiscountedPrice = item.StoreItemSaleInfo != null ? GetDiscountedPrice(item.Price, item.StoreItemSaleInfo.PercentageOff) : -1
            });

            return View(storeItemsViewModel);
        }

        private static decimal GetDiscountedPrice(decimal price, int percentageOff)
        {
            return Math.Round((decimal)(price * (1 - (decimal)percentageOff / 100)), 2);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart([FromBody] StoreCartItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cartDBModel = await _db.StoreItems.Include(item => item.StoreItemSaleInfo).SingleOrDefaultAsync(m => m.Uid.ToString() == model.Uid);
            if (cartDBModel == null || cartDBModel.Name == null)
            {
                return NotFound("Item not found");
            }

            model.Price = cartDBModel.StoreItemSaleInfo != null ? GetDiscountedPrice(cartDBModel.Price, cartDBModel.StoreItemSaleInfo.PercentageOff) : cartDBModel.Price;
            model.Name = cartDBModel.Name;

            bool newSession = HasCartSessionOrCreate(); // @Todo Send New Session Alert

            var cartString = HttpContext.Session.GetString("cart");
            if(cartString == null)
            {
                return NotFound("Session Not Found");
            }

            var cartItems = JsonConvert.DeserializeObject<Dictionary<string, StoreCartItemViewModel>>(cartString) ?? new Dictionary<string, StoreCartItemViewModel>();
            if(cartItems.ContainsKey(model.Uid))
            {
                cartItems[model.Uid].Quantity += model.Quantity;
            } else
            {
                cartItems.Add(model.Uid, model);
            }

            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cartItems));
         
            return PartialView("_cart", cartItems.Values.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(string uid)
        {
            bool newSession = HasCartSessionOrCreate(); // @Todo Send New Session Alert

            var cartString = HttpContext.Session.GetString("cart");
            if (cartString == null)
            {
                return NotFound("Session Not Found");
            }

            var cartItems = JsonConvert.DeserializeObject<Dictionary<string, StoreCartItemViewModel>>(cartString) ?? new Dictionary<string, StoreCartItemViewModel>();
            if (cartItems.ContainsKey(uid))
            {
                bool isSuccessful = cartItems.Remove(uid);
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cartItems));
            }
    
            return PartialView("_cart", cartItems.Values.ToList());
        }

        public bool HasCartSessionOrCreate()
        {
            bool hasCartSession = HttpContext.Session.GetString("cart") != null;
            if(!hasCartSession)
            {
                HttpContext.Session.SetString("cart", "");
                HttpContext.Session.SetString("cart_total", "");
                return false;
            }

            return true;
        }

        private void setCartTotalSessionValue()
        {

        }
    }
}
