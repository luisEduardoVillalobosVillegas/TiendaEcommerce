using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyStore.Models;
using MyStore.Services;

namespace MyStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController(ProductService _productService) : Controller
    {
        public async Task<IActionResult> Index()
        {

            var products = await _productService.GetAllAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            //ACA TUVIMOS QUE MODIFICAR POR QUE NO MOSTRABA LAS CATEGORIAS EN COMBO BOX
            //ViewBag.Message = TempData["Message"];

            //if (id == 0)
            //  return View(new ProductVM());

            //var productVM = await _productService.GetByIdAsync(id);
            var categoryVM = await _productService.GetByIdAsync(id);
            //if (productVM == null)
            //  return NotFound();

            return View(categoryVM);
        }



        [HttpPost]
        public async Task<IActionResult> AddEdit(ProductVM entityVM)
        {
            ViewBag.message = null;
            //esto es para que no valide estas dos propiedades
            ModelState.Remove("Categories");
            ModelState.Remove("Category.Name");
            if (!ModelState.IsValid) return View(entityVM);

            if (entityVM.ProductId == 0)
            {
                await _productService.AddAsync(entityVM);
                ModelState.Clear();
                entityVM = new ProductVM();
                //ViewBag permite enviar un mensaje a la vista
                ViewBag.message = "Producto Creado";
            }
            else
            {
                await _productService.EditAsync(entityVM);
                //ModelState.Clear();
                //entityVM = new ProductVM();
                ViewBag.message = "Producto Editado";
            }

            return View(entityVM);
            //return RedirectToAction(nameof(AddEdit), new { id = entityVM.CategoryId });
        }


        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
