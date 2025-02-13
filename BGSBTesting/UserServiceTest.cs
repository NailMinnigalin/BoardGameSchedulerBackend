using BoardGameSchedulerBackend.BusinessLayer;
using BoardGameSchedulerBackend.DataLayer;
using Microsoft.OpenApi.Validations;
using Moq;

namespace BGSBTesting
{
	[TestClass]
	public class UserServiceTest
	{
		private Mock<IUserRepository> _iUserRepositoryMock = default!;
		private UserService _userService = default!;

		[TestInitialize]
		public void TestInitialize()
		{
			_iUserRepositoryMock = new Mock<IUserRepository>();
			_userService = new UserService(_iUserRepositoryMock.Object);
		}

		[TestMethod]
		public async Task RegisterUserAsyncValidInputRegistersUserSuccessfully()
		{
			_iUserRepositoryMock	
				.Setup(iur => iur.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new UserCreationResult());

			await _userService.RegisterUserAsync("validUserName", "validUserEmail@example.com", "ValidPassword");
		}

		[TestMethod]
		public async Task RegisterUserAsyncReturnsRegistrationError()
		{
			var error = UserCreationResult.ErrorCode.InvalidEmail;
			_iUserRepositoryMock
				.Setup(iur => iur.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new UserCreationResult() { Errors = [error] });

			var result = await _userService.RegisterUserAsync("validUserName", "validUserEmail@example.com", "ValidPassword");

			Assert.IsTrue(result.Errors.First() == error);
		}

		[TestMethod]
		public async Task RegisterUserAsyncReturnsAllRegistrationErrors()
		{
			var error1 = UserCreationResult.ErrorCode.InvalidEmail;
			var error2 = UserCreationResult.ErrorCode.PasswordTooShort;
			_iUserRepositoryMock
				.Setup(iur => iur.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new UserCreationResult() { Errors = [error1, error2] });

			var result = await _userService.RegisterUserAsync("validUserName", "validUserEmail@example.com", "ValidPassword");

			Assert.IsTrue(result.Errors.Any(e => e == error1) && result.Errors.Any(e => e == error2));
		}

		[TestMethod]
		public async Task GetUserAsyncReturnsCorrectUser()
		{
			var expectedUser = new User { Email = "someEmail", UserName = "someName", Id = Guid.NewGuid() };
			_iUserRepositoryMock
				.Setup(iur => iur.GetByIdAsync(expectedUser.Id))
				.ReturnsAsync(expectedUser);

			var result = await _userService.GetUserAsync(expectedUser.Id);

			Assert.IsNotNull(result, "Received user is null");
			Assert.IsTrue(result.Id == expectedUser.Id, "Expected id != actual id");
			Assert.IsTrue(result.UserName == expectedUser.UserName, $"Expected userName is {expectedUser.UserName}, but actual userName is {expectedUser.UserName}");
			Assert.IsTrue(result.Email == expectedUser.Email, $"Expected email is {expectedUser.Email}, but actual email is {result.Email}");
		}

		[TestMethod]
		public async Task GetUserAsyncReturnsNullWhenUserNotFound()
		{
			var nonExistingId = Guid.NewGuid();
			User? nullUser = null;
			_iUserRepositoryMock
				.Setup(iur => iur.GetByIdAsync(nonExistingId))
				.ReturnsAsync(nullUser);

			var result = await _userService.GetUserAsync(nonExistingId);

			Assert.IsNull(result);
		}

		[TestMethod]
		public async Task UserServiceHasSignInMethod()
		{
			_userService.SignIn("userName", "password");
		}
	}
}
