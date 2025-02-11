using BoardGameSchedulerBackend.DataLayer;
using BoardGameSchedulerBackend.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace BGSBTesting
{
	[TestClass]
	public class IdentityUserRepositoryTest
	{
		private Mock<UserManager<IdentityUser>> _userManagerMock = default!;
		private IdentityUserRepository _repository = default!;

		[TestInitialize]
		public void TestInitialize()
		{
			_userManagerMock = CreateUserManagerMock();
			_repository = new IdentityUserRepository(_userManagerMock.Object);
		}

		[TestMethod]
		public async Task CreateAsyncSuccessfulUserCreation()
		{
			_userManagerMock
				.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Success);

		   await _repository.CreateAsync("ValidUserName", "validEmail@example.com", "ValidPassword");
		}

		[TestMethod]
		public async Task CreateAsyncFailUserCreation()
		{
			_userManagerMock
				.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Failed());

			await Assert.ThrowsExceptionAsync<IdentityUserRepositoryUserCreationFailedException>(
				async () => 
				await _repository.CreateAsync("ValidUserName", "validEmail@example.com", "ValidPassword")
			);
		}

		[TestMethod]
		public async Task CreateAsyncFailUserCreationIvalidPassword()
		{
			IdentityErrorDescriber _identityErrorDescriber = new IdentityErrorDescriber();
			List<IdentityError> passwordErrors = new List<IdentityError>
			{
				_identityErrorDescriber.PasswordRequiresNonAlphanumeric(),
				_identityErrorDescriber.PasswordRequiresLower(),
				_identityErrorDescriber.PasswordRequiresDigit(),
				_identityErrorDescriber.PasswordRequiresUpper(),
				_identityErrorDescriber.PasswordRequiresUniqueChars(10)
			};

			foreach(var passwordError in passwordErrors)
			{
				await TestForInvalidPassword(passwordError);
			}
		}

		[TestMethod]
		public async Task GetByIdAsyncSuccessfulGetExistingUser()
		{
			User existedUserId = new() { Email = "validEmail@example.com", UserName = "ValidUserName", Id = new Guid() };
			_userManagerMock
				.Setup(um => um.FindByIdAsync(existedUserId.Id.ToString()))
				.ReturnsAsync(new IdentityUser { Email = existedUserId.Email, UserName = existedUserId.UserName });

			var foundUser =  await _repository.GetByIdAsync(existedUserId.Id);
			
			Assert.IsTrue(
				existedUserId.Id == foundUser!.Id && 
				existedUserId.Email == foundUser.Email && 
				existedUserId.UserName == foundUser.UserName
			);
		}

		[TestMethod]
		public async Task GetByIdAsyncGetNullWhenUserNotFound()
		{
			IdentityUser? nullIdentity = null;
			_userManagerMock
				.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
				.ReturnsAsync(nullIdentity);

			var user = await _repository.GetByIdAsync(new Guid());

			Assert.IsNull(user);
		}

		private async Task TestForInvalidPassword(IdentityError identityError)
		{
			_userManagerMock
				.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Failed(identityError));

			await Assert.ThrowsExceptionAsync<IdentityUserRepositoryInvalidPasswordException>(
				async () =>
				await _repository.CreateAsync("ValidUserName", "validEmail@example.com", "ValidPassword")
			);
		}

		private static Mock<UserManager<IdentityUser>> CreateUserManagerMock()
		{
			var storeMock = new Mock<IUserStore<IdentityUser>>();
			var optionsMock = new Mock<IOptions<IdentityOptions>>();
			optionsMock.Setup(o => o.Value).Returns(new IdentityOptions());
			var passwordHasherMock = new Mock<IPasswordHasher<IdentityUser>>();
			var userValidators = new List<IUserValidator<IdentityUser>>();
			var passwordValidators = new List<IPasswordValidator<IdentityUser>>();
			var keyNormalizerMock = new Mock<ILookupNormalizer>();
			var errorsMock = new Mock<IdentityErrorDescriber>();
			var servicesMock = new Mock<IServiceProvider>();
			var loggerMock = new Mock<ILogger<UserManager<IdentityUser>>>();

			return new Mock<UserManager<IdentityUser>>(
				storeMock.Object,
				optionsMock.Object,
				passwordHasherMock.Object,
				userValidators,
				passwordValidators,
				keyNormalizerMock.Object,
				errorsMock.Object,
				servicesMock.Object,
				loggerMock.Object);
		}
	}
}
