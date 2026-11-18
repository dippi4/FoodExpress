using ConsoleProject.Models;
using ConsoleProject.Repositories;

namespace ConsoleProject.Services
{
    internal class OrderService
    {
        private readonly OrderRepository _orderRepository;

        public OrderService(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Order CreateOrder(User user, Food food, Restaurant restaurant, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            List<Order> orders = _orderRepository.GetAllOrders();
            Order order = new Order();
            order.Id = GetNextId(orders);
            order.UserId = user.Id;
            order.FoodId = food.Id;
            order.FoodName = food.Name;
            order.RestaurantId = restaurant.Id;
            order.RestaurantName = restaurant.Name;
            order.Quantity = quantity;

            _orderRepository.SaveOrder(order);
            return order;
        }

        public List<Order> GetOrdersForUser(int userId)
        {
            List<Order> orders = _orderRepository.GetAllOrders();
            return orders.Where(order => order.UserId == userId).ToList();
        }

        public List<Order> GetAllOrders()
        {
            return _orderRepository.GetAllOrders();
        }

        private static int GetNextId(List<Order> orders)
        {
            return orders.Select(order => order.Id).DefaultIfEmpty(0).Max() + 1;
        }
    }
}
