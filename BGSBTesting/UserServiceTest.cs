using BoardGameSchedulerBackend.BusinessLayer;
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
	}
}
