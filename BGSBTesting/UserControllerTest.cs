using BoardGameSchedulerBackend.BusinessLayer;
using BoardGameSchedulerBackend.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SignInResult = BoardGameSchedulerBackend.BusinessLayer.SignInResult;

namespace BGSBTesting
{
	[TestClass]
	public class UserControllerTest
	{
		private Mock<IUserService> _iUserService = default!;
		private UserController userController = default!;

		[TestInitialize]
		public void TestInitialize()
		{
			_iUserService = new Mock<IUserService>();
			userController = new(_iUserService.Object);
		}

		[TestMethod]
		public async Task UserControllerControllerHasRegisterEndpointThatTakesRegistrationDataToRegisterUser()
		{
			SetupIUserServiceRegisterUserAsyncToSuccesfulResult();

			await userController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });
		}

		[TestMethod]
		public async Task RegisterReturnsCreatedResultOnSuccesfulUserCreation()
		{
			SetupIUserServiceRegisterUserAsyncToSuccesfulResult();

			var actionResult = await userController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });

			Assert.IsInstanceOfType<CreatedResult>(actionResult);
		}

		[TestMethod]
		public async Task ReigsterReturnsCreatedResultWithUserCreationResultValueOnSuccesfulUserCreation()
		{
			SetupIUserServiceRegisterUserAsyncToSuccesfulResult();

			var actionResult = await userController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });
			var createdResult = (actionResult as CreatedResult)!;

			Assert.IsInstanceOfType<UserCreationResult>(createdResult.Value);
		}

		[TestMethod]
		public async Task ReigsterReturnsCreatedResultWithUserCreationResultValueWithSucessStatusOnSuccesfulUserCreation()
		{
			SetupIUserServiceRegisterUserAsyncToSuccesfulResult();

			var actionResult = await userController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });
			var createdResult = (actionResult as CreatedResult)!;
			var userCreationResult = (createdResult.Value as UserCreationResult)!;

			Assert.IsTrue(userCreationResult.IsSuccessful);
		}

		[TestMethod]
		public async Task RegisterReturnsBadRequstResultWhenErrorOccurs()
		{
			SetupIUserServiceRegisterUserAsyncToReturnError(UserCreationResult.ErrorCode.InvalidEmail);

			var actionResult = await userController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });

			Assert.IsInstanceOfType<BadRequestObjectResult>(actionResult);
		}

		[TestMethod]
		public async Task RegisterReturnsBadRequstResultWithUserCreationResultValueWhenErrorOccurs()
		{
			SetupIUserServiceRegisterUserAsyncToReturnError(UserCreationResult.ErrorCode.InvalidEmail);

			var actionResult = await userController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });
			var badRequestObjectResult = actionResult as BadRequestObjectResult;

			Assert.IsNotNull(badRequestObjectResult);
		}

		[TestMethod]
		public async Task RegisterReturnsBadRequstResultWithUserCreationResultValueWithErrorWhenErrorOccurs()
		{
			SetupIUserServiceRegisterUserAsyncToReturnError(UserCreationResult.ErrorCode.InvalidEmail);

			var actionResult = await userController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });
			var badRequestObjectResult = (actionResult as BadRequestObjectResult)!;
			var userCreationResult = (badRequestObjectResult.Value as UserCreationResult)!;

			Assert.IsTrue(userCreationResult.Errors.Count == 1);
		}

		[TestMethod]
		public async Task RegisterReturnsBadRequstResultWithUserCreationResultValueWithRightErrorWhenErrorOccurs()
		{
			var expectedError = UserCreationResult.ErrorCode.InvalidEmail;
			SetupIUserServiceRegisterUserAsyncToReturnError(expectedError);

			var actionResult = await userController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });
			var badRequestObjectResult = (actionResult as BadRequestObjectResult)!;
			var userCreationResult = (badRequestObjectResult.Value as UserCreationResult)!;

			Assert.IsTrue(userCreationResult.Errors[0] == expectedError);
		}

		[TestMethod]
		public async Task UserControllerHasSignInEndpoint()
		{
			SetupIUserServiceSignInToReturnSuccess(true);

			await userController.SignIn(new SignInData { UserName = "userName", Password = "password" });
		}

		[TestMethod]
		public async Task SignInReturnOkResultWhenSignInWasSuccesful()
		{
			SetupIUserServiceSignInToReturnSuccess(true);

			var result = await userController.SignIn(new SignInData { UserName = "userName", Password = "password" });

			Assert.IsInstanceOfType<OkResult>(result);
		}

		[TestMethod]
		public async Task SignInReturnBadRequestResultWhenSignInWasFail()
		{
			SetupIUserServiceSignInToReturnSuccess(false);

			var result = await userController.SignIn(new SignInData { UserName = "userName", Password = "password" });

			Assert.IsInstanceOfType<BadRequestResult>(result);
		}

		private void SetupIUserServiceSignInToReturnSuccess(bool success)
		{
			_iUserService
				.Setup(us => us.SignInAsync(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new SignInResult { IsSuccesful = success });
		}

		private void SetupIUserServiceRegisterUserAsyncToReturnError(UserCreationResult.ErrorCode error)
		{
			_iUserService
				.Setup(us => us.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new UserCreationResult([error]));
		}

		private void SetupIUserServiceRegisterUserAsyncToSuccesfulResult()
		{
			_iUserService
				.Setup(us => us.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new UserCreationResult());
		}
	}
}
