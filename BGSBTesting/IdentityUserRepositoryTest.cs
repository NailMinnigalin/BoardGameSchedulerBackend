using BoardGameSchedulerBackend.BusinessLayer;
using BoardGameSchedulerBackend.DataLayer;
using BoardGameSchedulerBackend.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SignInResult = BoardGameSchedulerBackend.BusinessLayer.SignInResult;

namespace BGSBTesting
{
	[TestClass]
	public class IdentityUserRepositoryTest
	{
		private Mock<UserManager<IdentityUser>> _userManagerMock = default!;
		private Mock<SignInManager<IdentityUser>> _signInManagerMock = default!;
		private IdentityUserRepository _identityUserRepository = default!;

		[TestInitialize]
		public void TestInitialize()
		{
			_userManagerMock = CreateUserManagerMock();
			_signInManagerMock = CreateSignInManager(_userManagerMock);
			_identityUserRepository = new IdentityUserRepository(_userManagerMock.Object, _signInManagerMock.Object);
		}

		[TestMethod]
		public async Task CreateAsyncSuccessfulUserCreation()
		{
			_userManagerMock
				.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Success);

		   await _identityUserRepository.CreateAsync("ValidUserName", "validEmail@example.com", "ValidPassword");
		}

		[TestMethod]
		public async Task CreateAsyncReturnsUserCreationResultWithAppropriateErrorWhenUserCreationErrorHappened()
		{
			IdentityErrorDescriber _identityErrorDescriber = new IdentityErrorDescriber();
			const string email = "email";
			const string userName = "userName";
			List<(IdentityError identityError, UserCreationResult.ErrorCode exprectedErrorCode)> IdentityErrorAndExpectedErrorCodePairs = [
				(_identityErrorDescriber.DuplicateEmail(email), UserCreationResult.ErrorCode.DuplicateEmail),
				(_identityErrorDescriber.InvalidEmail(email), UserCreationResult.ErrorCode.InvalidEmail),
				(_identityErrorDescriber.PasswordRequiresUniqueChars(1), UserCreationResult.ErrorCode.PasswordRequiresUniqueChars),
				(_identityErrorDescriber.DuplicateUserName(userName), UserCreationResult.ErrorCode.DuplicateUserName),
				(_identityErrorDescriber.InvalidUserName(userName), UserCreationResult.ErrorCode.InvalidUserName),
				(_identityErrorDescriber.PasswordRequiresDigit(), UserCreationResult.ErrorCode.PasswordRequiresDigit),
				(_identityErrorDescriber.PasswordRequiresLower(), UserCreationResult.ErrorCode.PasswordRequiresLower),
				(_identityErrorDescriber.PasswordRequiresNonAlphanumeric(), UserCreationResult.ErrorCode.PasswordRequiresNonAlphanumeric),
				(_identityErrorDescriber.PasswordRequiresUpper(), UserCreationResult.ErrorCode.PasswordRequiresUpper),
				(_identityErrorDescriber.PasswordTooShort(1), UserCreationResult.ErrorCode.PasswordTooShort),
			];

			foreach(var pair in IdentityErrorAndExpectedErrorCodePairs)
			{
				await TestThatIdentityErrorLeadsToCorrespondErrorCode(pair.identityError, pair.exprectedErrorCode);
			}
		}

		[TestMethod]
		public async Task GetByIdAsyncSuccessfulGetExistingUser()
		{
			User existedUserId = new() { Email = "validEmail@example.com", UserName = "ValidUserName", Id = new Guid() };
			_userManagerMock
				.Setup(um => um.FindByIdAsync(existedUserId.Id.ToString()))
				.ReturnsAsync(new IdentityUser { Email = existedUserId.Email, UserName = existedUserId.UserName });

			var foundUser = await _identityUserRepository.GetByIdAsync(existedUserId.Id);

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

			var user = await _identityUserRepository.GetByIdAsync(new Guid());

			Assert.IsNull(user);
		}

		[TestMethod]
		public async Task IdentityUserRepositoryHasSignInMethod()
		{
			SetupSignInManagerPasswordSignInAsyncToReturnSuccess();

			await _identityUserRepository.SignInAsync("userName", "password");
		}

		[TestMethod]
		public async Task SignInReturnsSignInResult()
		{
			SetupSignInManagerPasswordSignInAsyncToReturnSuccess();

			var signInResult = await _identityUserRepository.SignInAsync("userName", "password");

			Assert.IsInstanceOfType<SignInResult>(signInResult);
		}

		[TestMethod]
		public async Task SignInReturnsSignInResultWithSuccesfulFlagTrueWhenSignInWasSuccesful()
		{
			SetupSignInManagerPasswordSignInAsyncToReturnSuccess();

			var signInResult = await _identityUserRepository.SignInAsync("userName", "password");

			Assert.IsTrue(signInResult.IsSuccesful);
		}

		[TestMethod]
		public async Task SignInReturnsSignInResultWithSuccesfulFlagFalseWhenSignInWasFailed()
		{
			_signInManagerMock
				.Setup(sm => sm.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
				.ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

			var signInResult = await _identityUserRepository.SignInAsync("userName", "password");

			Assert.IsFalse(signInResult.IsSuccesful);
		}

		private async Task TestThatIdentityErrorLeadsToCorrespondErrorCode(IdentityError identityError, UserCreationResult.ErrorCode expectedErrorCode)
		{
			_userManagerMock
				.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Failed(identityError));

			UserCreationResult userCreationResult = await _identityUserRepository.CreateAsync("ValidUserName", "validEmail@example.com", "ValidPassword");

			Assert.IsTrue(userCreationResult.Errors.Count == 1, "Not only 1 error");
			Assert.IsTrue(userCreationResult.Errors.First() == expectedErrorCode, $"Expected: {expectedErrorCode} was: {userCreationResult.Errors.First()}");
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
				loggerMock.Object
			);
		}

		private static Mock<SignInManager<IdentityUser>> CreateSignInManager(Mock<UserManager<IdentityUser>> userManagerMock)
		{
			var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
			var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
			var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
			var loggerMock = new Mock<ILogger<SignInManager<IdentityUser>>>();
			var authenticationSchemeProviderMock = new Mock<IAuthenticationSchemeProvider>();
			var userConfirmationMock = new Mock<IUserConfirmation<IdentityUser>>();

			return new Mock<SignInManager<IdentityUser>>(
				userManagerMock.Object,
				httpContextAccessorMock.Object,
				userClaimsPrincipalFactoryMock.Object,
				identityOptionsMock.Object,
				loggerMock.Object,
				authenticationSchemeProviderMock.Object,
				userConfirmationMock.Object
			);
		}

		private void SetupSignInManagerPasswordSignInAsyncToReturnSuccess()
		{
			_signInManagerMock
							.Setup(sm => sm.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
							.ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
		}
	}
}
