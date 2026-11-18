using ConsoleProject.Exceptions;
using ConsoleProject.Models;
using ConsoleProject.Repositories;

namespace ConsoleProject.Services
{
	internal class UserService
	{
		private readonly UserRepository _userRepository;
		private readonly EmailService _emailService;

		public UserService(UserRepository userRepository, EmailService emailService)
		{
			_userRepository = userRepository;
			_emailService = emailService;
		}

		public void RegisterUser(User user)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));
			if (string.IsNullOrWhiteSpace(user.Name))
				throw new ArgumentException("Name cannot be empty.");
			if (string.IsNullOrWhiteSpace(user.Password))
				throw new PasswordNullexception();

			List<User> users = _userRepository.GetAllUsers();
			if (users.Any(oldUser => oldUser.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
				throw new UsedEmailException();

			user.Id = GetNextId(users);
			user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
			user.VerificationCode = Random.Shared.Next(1000, 10000).ToString();
			user.IsVerified = false;
			user.IsLogged = false;
			user.IsAdmin = false;

			_userRepository.SaveUser(user);

			try
			{
				_emailService.SendEmail(
					user.Email,
					"FoodExpress verification code",
					$"Your verification code is: {user.VerificationCode}");
			}
			catch (Exception error)
			{
				Console.WriteLine("Email could not be sent: " + error.Message);
				Console.WriteLine("Your verification code is: " + user.VerificationCode);
			}
		}

		public void VerifyUser(string email, string code)
		{
			CheckEmail(email);
			if (string.IsNullOrWhiteSpace(code))
				throw new VerificationCodeNullException();

			_userRepository.VerifyUser(email.Trim(), code.Trim());
		}

		public User LoginUser(string email, string password)
		{
			CheckEmail(email);
			if (string.IsNullOrWhiteSpace(password))
				throw new PasswordNullexception();

			User? user = _userRepository.GetUserByEmail(email.Trim());
			if (user == null)
				throw new UserNotFoundException();
			if (!user.IsVerified)
				throw new InvalidOperationException("Verify your email before logging in.");
			if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
				throw new PasswordNotMatchException();

			List<User> users = _userRepository.GetAllUsers();
			foreach (User item in users)
				item.IsLogged = item.Id == user.Id;
			_userRepository.SaveAllUsers(users);

			user.IsLogged = true;
			return user;
		}

		public void Logout(User user)
		{
			List<User> users = _userRepository.GetAllUsers();

			foreach (User item in users)
			{
				if (item.Id == user.Id)
					item.IsLogged = false;
			}

			user.IsLogged = false;
			_userRepository.SaveAllUsers(users);
		}

		private static int GetNextId(List<User> users)
		{
			return users.Select(user => user.Id).DefaultIfEmpty(0).Max() + 1;
		}

		private static void CheckEmail(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
				throw new EmailNullexception();
			if (!email.Contains("@"))
				throw new InvalidEmailException();
		}
	}
}
