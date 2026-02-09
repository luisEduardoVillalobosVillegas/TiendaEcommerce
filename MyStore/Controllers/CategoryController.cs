using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyStore.Models;

//using MyStore.Context;
//using MyStore.Entities;
//using MyStore.Models;
using MyStore.Services;
//using System.Threading.Tasks;

namespace MyStore.Controllers
{
    //public class CategoryController(AppDbContext _dbContext) : Controller
    [Authorize(Roles = "Admin")]
    public class CategoryController(CategoryService _categoryService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            //var categories = _dbContext.Category.Select(item =>
            //new CategoryVM
            //{
              //  CategoryId = item.CategoryId,
              //  Name = item.Name,
            //}            
            //).ToList();
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            ViewBag.Message = TempData["Message"];

            if (id == 0)
                return View(new CategoryVM());

            var vm = await _categoryService.GetByIdAsync(id);

            if (vm == null)
                return NotFound();

            return View(vm);
        }



        [HttpPost]
        public async Task<IActionResult> AddEdit(CategoryVM entityVM)
        {
            ViewBag.message = null;
            if (!ModelState.IsValid) return View(entityVM);

            if (entityVM.CategoryId == 0)
            {
                await _categoryService.AddAsync(entityVM);
                ModelState.Clear();
                entityVM = new CategoryVM();
                //ViewBag permite enviar un mensaje a la vista
                ViewBag.message = "Created category";
            }
            else
            {
                await _categoryService.EditAsync(entityVM);
                ModelState.Clear();
                entityVM = new CategoryVM();
                ViewBag.message = "Edited category";
            }

            return View(entityVM);
            //return RedirectToAction(nameof(AddEdit), new { id = entityVM.CategoryId });
        }


        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            return RedirectToAction("Index");
        }



    }

}
