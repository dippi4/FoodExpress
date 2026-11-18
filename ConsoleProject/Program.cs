using ConsoleProject.Menu;
using ConsoleProject.Repositories;
using ConsoleProject.Services;
using Microsoft.Extensions.Configuration;

namespace ConsoleProject
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
			.Build();

			var userRepository = new UserRepository();
			var foodRepository = new FoodRepository();
			var restaurantRepository = new RestaurantRepository();
			var orderRepository = new OrderRepository();
			var emailService = new EmailService(configuration);

			var userService = new UserService(userRepository, emailService);
			var foodService = new FoodService(foodRepository);
			var restaurantService = new RestaurantService(restaurantRepository);
			var orderService = new OrderService(orderRepository);

			var clientMenu = new ClientMenu(foodService, restaurantService, orderService);
			var adminMenu = new AdminMenu(foodService, restaurantService, orderService);
			var userMenu = new UserMenu(userService, clientMenu, adminMenu);

			userMenu.ShowMainMenu();
		}
	}
}
