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
		public async Task RegisterReturnsUserCreationResultWithCorrespodingErrorCodeWhenErrorOccurs()
		{
			var exptectedError = UserCreationResult.ErrorCode.InvalidEmail;
			_iUserService
				.Setup(us => us.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new UserCreationResult([exptectedError]));

			var actionResult = await registrationController.Register(new RegistrationData { UserName = "testUserName", Email = "testEmail@example.com", Password = "testPassword" });
			var createdResult = actionResult as BadRequestObjectResult;
			var userCreationResult = createdResult?.Value as UserCreationResult;

			Assert.IsNotNull(userCreationResult, $"{nameof(userCreationResult)} is null");
			Assert.IsTrue(userCreationResult.Errors.Count == 1, "Expected one error");
			Assert.IsTrue(userCreationResult.Errors[0] == UserCreationResult.ErrorCode.InvalidEmail, $"Expected {exptectedError} but got {userCreationResult.Errors[0]}");
		}
	}
}
