using BoardGameSchedulerBackend.BusinessLayer;
using BoardGameSchedulerBackend.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BGSBTesting
{
	[TestClass]
	public class RegistrationControllerTest
	{
		private Mock<IUserService> _iUserService = default!;
		private RegistrationController registrationController = default!;

		[TestInitialize]
		public void TestInitialize()
		{
			_iUserService = new Mock<IUserService>();
			registrationController = new(_iUserService.Object);
		}

		[TestMethod]
		public async Task RegistrationControllerHasRegisterEndpointThatTakesRegistrationDataToRegisterUser()
		{
			_iUserService
				.Setup(us => us.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new UserCreationResult());

			await registrationController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });
		}

		[TestMethod]
		public async Task RegisterReturnsCreatedResultOnSuccesfulUserCreation()
		{
			_iUserService
				.Setup(us => us.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new UserCreationResult());

			var actionResult = await registrationController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });

			Assert.IsInstanceOfType<CreatedResult>(actionResult);
		}

		[TestMethod]
		public async Task ReigsterReturnsCreatedResultWithUserCreationResultValueOnSuccesfulUserCreation()
		{
			_iUserService
				.Setup(us => us.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new UserCreationResult());

			var actionResult = await registrationController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });
			var createdResult = (actionResult as CreatedResult)!;

			Assert.IsInstanceOfType<UserCreationResult>(createdResult.Value);
		}

		[TestMethod]
		public async Task ReigsterReturnsCreatedResultWithUserCreationResultValueWithSucessStatusOnSuccesfulUserCreation()
		{
			_iUserService
				.Setup(us => us.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new UserCreationResult());

			var actionResult = await registrationController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });
			var createdResult = (actionResult as CreatedResult)!;
			var userCreationResult = (createdResult.Value as UserCreationResult)!;

			Assert.IsTrue(userCreationResult.IsSuccessful);
		}


		[TestMethod]
		public async Task RegisterReturnsBadRequstResultWhenErrorOccurs()
		{
			var error = UserCreationResult.ErrorCode.InvalidEmail;
			_iUserService
				.Setup(us => us.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new UserCreationResult([error]));

			var actionResult = await registrationController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });

			Assert.IsInstanceOfType<BadRequestObjectResult>(actionResult);
		}

		[TestMethod]
		public async Task RegisterReturnsBadRequstResultWithUserCreationResultValueWhenErrorOccurs()
		{
			var error = UserCreationResult.ErrorCode.InvalidEmail;
			_iUserService
				.Setup(us => us.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new UserCreationResult([error]));

			var actionResult = await registrationController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });
			var badRequestObjectResult = actionResult as BadRequestObjectResult;

			Assert.IsNotNull(badRequestObjectResult);
		}

		[TestMethod]
		public async Task RegisterReturnsBadRequstResultWithUserCreationResultValueWithErrorWhenErrorOccurs()
		{
			var error = UserCreationResult.ErrorCode.InvalidEmail;
			_iUserService
				.Setup(us => us.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new UserCreationResult([error]));

			var actionResult = await registrationController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });
			var badRequestObjectResult = (actionResult as BadRequestObjectResult)!;
			var userCreationResult = (badRequestObjectResult.Value as UserCreationResult)!;

			Assert.IsTrue(userCreationResult.Errors.Count == 1);
		}

		[TestMethod]
		public async Task RegisterReturnsBadRequstResultWithUserCreationResultValueWithRightErrorWhenErrorOccurs()
		{
			var expectedError = UserCreationResult.ErrorCode.InvalidEmail;
			_iUserService
				.Setup(us => us.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new UserCreationResult([expectedError]));

			var actionResult = await registrationController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });
			var badRequestObjectResult = (actionResult as BadRequestObjectResult)!;
			var userCreationResult = (badRequestObjectResult.Value as UserCreationResult)!;

			Assert.IsTrue(userCreationResult.Errors[0] == expectedError);
		}
	}
}
