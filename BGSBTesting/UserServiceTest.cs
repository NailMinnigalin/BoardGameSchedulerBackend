using BoardGameSchedulerBackend.BusinessLayer;
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
			_iUserRepositoryMock
				.Setup(iur => iur.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new UserCreationResult() { Errors = [UserCreationResult.ErrorCode.InvalidEmail] });

			var result = await _userService.RegisterUserAsync("validUserName", "validUserEmail@example.com", "ValidPassword");

			Assert.IsTrue(result.Errors.First() == UserCreationResult.ErrorCode.InvalidEmail);
		}
	}
}
