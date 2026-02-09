using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyStore.Models;
using MyStore.Services;
using MyStore.Utilities;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyStore.Controllers
{
    [Authorize]
    public class HomeController(
        CategoryService _categoryService,
        ProductService _productService,
        OrderService _orderService
        ) : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
          //  _logger = logger;
        //}


        //se modela para la busqueda del home
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            var products = await _productService.GetCatalogAsync();
            var catalog = new CatalogVM { Categories = categories, Products = products };


            return View(catalog);
        }

        public async Task<IActionResult> FilterByCategory(int id, string name)
        {
            var categories = await _categoryService.GetAllAsync();
            //llamamos las categorias en base al id
            var products = await _productService.GetCatalogAsync(categoryId:id);
            var catalog = new CatalogVM { Categories = categories, Products = products, filterBy=name};


            return View("Index",catalog);
        }

        [HttpPost]
        public async Task<IActionResult> FilterBySearch(string value)
        {
            var categories = await _categoryService.GetAllAsync();
            //llamamos las categorias en base al id
            var products = await _productService.GetCatalogAsync(search: value);
            var catalog = new CatalogVM { Categories = categories, Products = products, filterBy = $"Resultado para: {value}" };


            return View("Index", catalog);
        }


        public async Task<IActionResult> ProductDetail(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddItemToCart(int productId, int quantity)
        {

            var product = await _productService.GetByIdAsync(productId);

            //dentro del las sesiones solo podemos guardar int y strings el VM vamos a tener que convertir en string 
            //que devuelva lo que contiene la ssesion sino devuelve una lista de CartItemVM
            var cart = HttpContext.Session.Get<List<CartItemVM>>("Cart") ?? new List<CartItemVM>();

            //quiero que encuentres el producto y si no encuantra agrega a carrito un nueco CartItemVM
            if (cart.Find(x => x.ProductId == productId) == null)
            {
                cart.Add(new CartItemVM
                {
                    ProductId = productId,
                    ImageName = product.ImageName,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity
                });
            }
            //si ya existe el  producto en el carrito se actualiza
            else 
            {
                var updateProduct = cart.Find(x => x.ProductId == productId);
                updateProduct!.Quantity += quantity;
            
            }

            HttpContext.Session.Set("Cart", cart);
            ViewBag.message = "Producto agregado al carrito";
            return View("ProductDetail", product);
        }


        public IActionResult ViewCart()
        {
            var cart = HttpContext.Session.Get<List<CartItemVM>>("Cart") ?? new List<CartItemVM>();
            return View(cart);
        }

        public IActionResult RemoveItemToCart(int productId)
        {
            var cart = HttpContext.Session.Get<List<CartItemVM>>("Cart");

            var product = cart.Find(x => x.ProductId == productId);
            cart.Remove(product!);
            HttpContext.Session.Set("Cart", cart);
            return View("ViewCart",cart);
        }

        [HttpPost]
        public async Task<IActionResult> PayNow()
        {
            
            var cart = HttpContext.Session.Get<List<CartItemVM>>("Cart");           
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            await _orderService.AddAsync(cart, int.Parse(userId));
            //termimnamos la venta
            HttpContext.Session.Remove("Cart");

            return View("SaleCompleted");

        }






        public IActionResult SaleCompleted()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
