using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using MyStore.Entities;
using MyStore.Models;
using MyStore.Repositories;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace MyStore.Services
{
    public class ProductService(
        GenericRepository<Category> _categoryRepository,
        GenericRepository<Product> _productRepository,
        //recurso para enviar la imagen dentro de la carpeta del proyecto
        IWebHostEnvironment _webHostEnviroment
        )
    {


        public async Task<IEnumerable<ProductVM>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync(
            //devuelve productos con su categorias por eso de modifico en clase GenericRepository
            includes: new Expression<Func<Product, object>>[] { x => x.Category! });

            //var products = await _productRepository.GetAllAsync();

            var productsVM = products.Select(item =>
            new ProductVM
            {
                ProductId = item.ProductId,
                Category = new CategoryVM
                {
                    //! quiere decir que efectivamente recibiremos un valor
                    CategoryId = item.Category!.CategoryId,
                    Name = item.Category!.Name,
                },
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Stock = item.Stock,
                ImageName = item.ImageName,
            }).ToList();

            return productsVM;
        }


        //este es para devolver un producto x id
        public async Task<ProductVM> GetByIdAsync(int id)
        {
            //primero obtenemos el objerto de la base de datos
            var product = await _productRepository.GetByIdAsync(id);
            var categories = await _categoryRepository.GetAllAsync();


            var productVM = new ProductVM();
            if (product != null)
            {
                productVM = new ProductVM
                {
                    ProductId = product.ProductId,
                    Category = new CategoryVM
                    {
                        //! quiere decir que efectivamente recibiremos un valor
                        CategoryId = product.Category!.CategoryId,
                        Name = product.Category!.Name,
                    },
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    ImageName = product.ImageName,
                };
            }

            productVM.Categories = categories.Select(item => new SelectListItem
            {
                Value = item.CategoryId.ToString(),
                Text = item.Name,
            }).ToList();


            return productVM;
        }



        public async Task AddAsync(ProductVM viewModel)
        {

            if (viewModel.ImageFile != null)
            {
                string uploadFolder = Path.Combine(_webHostEnviroment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.ImageFile.FileName);
                //vamos a concatenar nuestra carpeta de imagenes
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                //para guardar la imagen
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    await viewModel.ImageFile.CopyToAsync(fileStream);

                viewModel.ImageName = uniqueFileName;
            }

            var entity = new Product
            {
                CategoryId = viewModel.Category.CategoryId,
                Name = viewModel.Name,
                Description = viewModel.Description,
                Price = viewModel.Price,
                Stock = viewModel.Stock,
                ImageName = viewModel.ImageName
            };

            await _productRepository.AddAsync(entity);
        }

        public async Task EditAsync(ProductVM viewModel)
        {
            var product = await _productRepository.GetByIdAsync(viewModel.ProductId);

            if (viewModel.ImageFile != null)
            {
                string uploadFolder = Path.Combine(_webHostEnviroment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.ImageFile.FileName);
                //vamos a concatenar nuestra carpeta de imagenes
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                //para guardar la imagen
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    await viewModel.ImageFile.CopyToAsync(fileStream);

                //si la imagen previa no es nula borramos la imagen previa
                if (!product.ImageName.IsNullOrEmpty())
                {
                    var previousImage = product.ImageName;
                    string deleteFilePath = Path.Combine(uploadFolder, previousImage);

                    if (File.Exists(deleteFilePath)) System.IO.File.Delete(deleteFilePath);

                    //actualizamos el nombre de la imagen
                    viewModel.ImageName = uniqueFileName;



                }


            }
            //si no tiene la imagen
            else
            {
                viewModel.ImageName = product.ImageName;

            }
            //si no tiene imagen asignamos los valores del viewModel
            product.CategoryId = viewModel.Category.CategoryId;
            product.Name = viewModel.Name;
            product.Description = viewModel.Description;
            product.Price = viewModel.Price;
            product.Stock = viewModel.Stock;
            product.ImageName = viewModel.ImageName;

            //aca editamos el producto
            await _productRepository.EditAsync(product);

        }

        public async Task DeleteAsync(int id)
        {
            //ojo que en el video podria dejar como tarea que movamos este codigo para la eliminacion de la imagen en
            //el metodo anterior 
            var product = await _productRepository.GetByIdAsync(id);
            await _productRepository.DeleteAsync(product);
        }


        public async Task<IEnumerable<ProductVM>> GetCatalogAsync(int categoryId = 0, string search = "")
        {
            var conditions = new List<Expression<Func<Product, bool>>>
            {
                x => x.Stock > 0

            };
            //estas dos lineas siguientes son para la busqueda por categoria y por el string de la lupa en el home
            if (categoryId != 0) conditions.Add(x => x.CategoryId == categoryId);
            if (!string.IsNullOrEmpty(search)) conditions.Add(x => x.Name.Contains(search));

            //aca esta mal por que esta trayendo productos cuando no tiene la categoria productos
            var products = await _productRepository.GetAllAsync(conditions: conditions.ToArray());

            //var products = await _productRepository.GetAllAsync();

            var productsVM = products.Select(item =>
            new ProductVM
            {
                ProductId = item.ProductId,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Stock = item.Stock,
                ImageName = item.ImageName,
            }).ToList();

            return productsVM;
        }

    }
}
