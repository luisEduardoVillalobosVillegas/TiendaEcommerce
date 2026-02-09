using MyStore.Context;
using MyStore.Entities;
using Microsoft.EntityFrameworkCore;


namespace MyStore.Repositories
{
    //herencia con un metodo sobreescrito
    public class OrderRepository : GenericRepository<Order>
    {
        private readonly AppDbContext _appDbContext;

        public OrderRepository(AppDbContext dbContext) : base(dbContext)
        {
            _appDbContext = dbContext;
        }

        //sobre escritura de metodo de genericRepository y manejo de transacciones
        public override async Task AddAsync(Order order)
        {
            using var transaction = await _appDbContext.Database.BeginTransactionAsync();

            try
            {
                //1 reducir el stock de los productos
                foreach (var detail in order.OrderItems)
                {
                    var product = await _appDbContext.Product.FindAsync(detail.ProductId);
                    product.Stock -= detail.Quantity;
                }

                //2 registro de orden
                await _appDbContext.Order.AddAsync(order);
                //3 cambios realizados para la bd los guardes
                await _appDbContext.SaveChangesAsync();
                //4 finaliza la transaccion
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                //si alguna de las 3 entidades que interactuan en la transacion como orden-ordenDetalle y producto esta mal
                //vualve la transaccion atras
                await transaction.RollbackAsync();
                throw;
            }



        }

        public async Task<IEnumerable<Order>> GetAllWithDetailAsync(int userId)
        {
            //estas lineas serian como el siguiente query: 
            //select * from Product p
            //inner join Category c on c.CategoryId = p.CategoryId
            //where c.CategoryId = 1 o 2 o 3 o ...
            var orders = await _appDbContext.Order.Where(x => x.UserId == userId).Include(x => x.OrderItems)
                .ThenInclude(x => x.Product).ToListAsync();
            //ojo aca que el el video iba a _dbContext y no a _appDbContext como lo hice

            return orders;
        }
    }
}
