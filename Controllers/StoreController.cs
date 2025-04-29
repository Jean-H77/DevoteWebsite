using DevoteWebsite.Data;
using DevoteWebsite.Services.Stripe;
using DevoteWebsite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DevoteWebsite.Controllers
{

    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly StripeService _stripeService;

        public StoreController(ApplicationDbContext db, StripeService stripeService)
        {
            _db = db;
            _stripeService = stripeService;
        }

        public async Task<IActionResult> Index()
        {

            HasCartSessionOrCreate();
            
            var storeItems = await _db.StoreItems
                .Include(item => item.StoreItemSaleInfo)
                .ToListAsync();

            var storeItemsViewModel = storeItems.Select(item => new StoreItemViewModel
            {
                Name = item.Name,
                Price = item.Price,
                Uid = item.Uid,
                Description = item.Description,
                Thumbnail = item.ThumbnailUrl,
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
        //@todo I need to refactor this
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

        private bool HasCartSessionOrCreate()
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

        private async Task<StoreCartItemViewModel> ValidateAndPrepareCartItem(StoreCartItemViewModel model)
        {
            var dbItem = await _db.StoreItems
                .Include(item => item.StoreItemSaleInfo)
                .SingleOrDefaultAsync(m => m.Uid.ToString() == model.Uid);

            if (dbItem == null || dbItem.Name == null)
            {
                return null;
            }

            return new StoreCartItemViewModel
            {
                Uid = model.Uid,
                Name = dbItem.Name,
                Price = dbItem.StoreItemSaleInfo != null
                    ? GetDiscountedPrice(dbItem.Price, dbItem.StoreItemSaleInfo.PercentageOff)
                    : dbItem.Price,
                Quantity = model.Quantity
            };
        }


        [HttpGet]
        public async Task<IActionResult> HandleCheckout()
        {
            var cartString = HttpContext.Session.GetString("cart");
            var cartItems = JsonConvert.DeserializeObject<Dictionary<string, StoreCartItemViewModel>>(cartString) ?? new Dictionary<string, StoreCartItemViewModel>();

            var stripeItems = new List<StripeItem>();
            foreach (var c in cartItems)
            {
                var validCartItemOrNull = await ValidateAndPrepareCartItem(c.Value);

                if (validCartItemOrNull == null)
                {
                    continue;
                }

                var stripeItem = new StripeItem()
                {
                    Name = validCartItemOrNull.Name,
                    Quantity = 3,
                    TotalPrice = (decimal)validCartItemOrNull.Price
                };

                stripeItems.Add(stripeItem);
            }


            var checkoutUrl = _stripeService.GetCheckoutUrl(stripeItems);

            return Redirect(checkoutUrl);
        }
    }
}
