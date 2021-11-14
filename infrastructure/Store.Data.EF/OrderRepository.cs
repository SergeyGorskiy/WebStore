using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Store.Data.EF
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextFactory _dbContextFactory;

        public OrderRepository(DbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<Order> CreateAsync()
        {
            var dbContext = _dbContextFactory.Create(typeof(OrderRepository));

            var dto = Order.DtoFactory.Create();
            dbContext.Orders.Add(dto);
            await dbContext.SaveChangesAsync();

            return Order.Mapper.Map(dto);
        }

        public async Task<Order> GetByIdAsync(int orderId)
        {
            var dbContext = _dbContextFactory.Create(typeof(OrderRepository));

            var dto = await dbContext.Orders.Include(order => order.Items)
                                            .SingleAsync(order => order.Id == orderId);

            return Order.Mapper.Map(dto);
        }

        public async Task UpdateAsync(Order order)
        {
            var dbContext = _dbContextFactory.Create(typeof(OrderRepository));

            await dbContext.SaveChangesAsync();
        }
    }
}