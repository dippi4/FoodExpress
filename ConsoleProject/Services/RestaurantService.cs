using ConsoleProject.Models;
using ConsoleProject.Repositories;

namespace ConsoleProject.Services
{
    internal class RestaurantService
    {
        private readonly RestaurantRepository _restaurantRepository;

        public RestaurantService(RestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        public List<Restaurant> GetAllRestaurants() => _restaurantRepository.GetAllRestaurants();

        public Restaurant? GetRestaurantById(int id) => _restaurantRepository.GetRestaurantById(id);

        public void AddRestaurant(string name, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Restaurant name cannot be empty.");
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Restaurant address cannot be empty.");

            List<Restaurant> restaurants = GetAllRestaurants();
            _restaurantRepository.SaveRestaurant(new Restaurant
            {
                Id = restaurants.Select(restaurant => restaurant.Id).DefaultIfEmpty(0).Max() + 1,
                Name = name.Trim(),
                Address = address.Trim()
            });
        }
    }
}
