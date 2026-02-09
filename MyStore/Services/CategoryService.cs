using Microsoft.EntityFrameworkCore;
using MyStore.Entities;
using MyStore.Models;
using MyStore.Repositories;

namespace MyStore.Services
{
    public class CategoryService(GenericRepository<Category> _categoryRepository)
    {
        //metodo para listar categorias
        public async Task<IEnumerable<CategoryVM>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            var categoriesVM = categories.Select(item =>
            new CategoryVM
            {
                CategoryId = item.CategoryId,
                Name = item.Name,
            }
            ).ToList();

            return categoriesVM;

        }

        //category repository no recibe un view model asi que lo convertimos category recibe la entidad no el VM
        public async Task AddAsync(CategoryVM viewModel)
        {
            var entitiy = new Category
            {
                Name = viewModel.Name,
            };
            await _categoryRepository.AddAsync(entitiy);

        }

        public async Task<CategoryVM?> GetByIdAsync(int id)
        {
            var entity = await _categoryRepository.GetByIdAsync(id);

            if (entity == null)
                return null;

            return new CategoryVM
            {
                CategoryId = entity.CategoryId,
                Name = entity.Name
            };
        }


        public async Task EditAsync(CategoryVM viewModel)
        {
            var entity = await _categoryRepository.GetByIdAsync(viewModel.CategoryId)
                ?? throw new Exception("Category not found");

            entity.Name = viewModel.Name;
            entity.CategoryId = viewModel.CategoryId;
            _categoryRepository.EditAsync (entity);
        }

        //category! quiere decir que si  o si va a encontrar
        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            await _categoryRepository.DeleteAsync (category!);
        }

    }
}
