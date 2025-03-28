﻿using System.Net.Http.Json;

namespace BGSIntegrationTesting
{
	[TestClass]
	public class UserControllerTest
	{
		BGSWebApplicationFactory<Program> _factory = default!;
		private HttpClient _client = default!;

		[TestInitialize]
		public void TestInitialize()
		{
			_factory = new BGSWebApplicationFactory<Program>();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			_client.Dispose();
			_factory.Dispose();
		}

		[TestMethod]
		public async Task ApiHasRegisterEndPointThatRequersRegistrationData()
		{
			_client = _factory.CreateClient();
			var jsonContent = JsonContent.Create(new RegistrationData { Email = "testEmail@email.com", Password = "testPassword1!", UserName = "TestName" });

			var response = await _client.PostAsync("/register", jsonContent);

			Assert.IsFalse(response.StatusCode == System.Net.HttpStatusCode.NotFound);
		}

		[TestMethod]
		public async Task RegisterEndPointReturn400ForBadData()
		{
			_client = _factory.CreateClient();
			var jsonContent = JsonContent.Create(new RegistrationData { Email = "testEmail@email.com", Password = "testPassword", UserName = "TestName" });

			var response = await _client.PostAsync("/register", jsonContent);

			Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
		}

		[TestMethod]
		public async Task ApiHasSignInEndPointThatRequersRegistrationData()
		{
			_client = _factory.CreateClient();
			var jsonContent = JsonContent.Create(new SignInData { UserName = "TestName", Password = "testPassword" });

			var response = await _client.PostAsync("/signin", jsonContent);

			Assert.IsFalse(response.StatusCode == System.Net.HttpStatusCode.NotFound);
		}

		[TestMethod]
		public async Task SignInEndPointAllowSignInToExistingUser()
		{
			_client = _factory.CreateClient();
			var email = "testEmail@email.com"; 
			var password = "testPassword1!";
			var userName = "TestName";
			await RegisterUser(email, password, userName);
			var signInJsonContent = JsonContent.Create(new SignInData { UserName = userName, Password = password });

			var response = await _client.PostAsync("/signin", signInJsonContent);

			Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
		}

		[TestMethod]
		public async Task SignInEndPointReturnBadRequestForNonExistingUser()
		{
			_client = _factory.CreateClient();
			var email = "testEmail@email.com";
			var password = "testPassword1!";
			var userName = "TestName";
			var signInJsonContent = JsonContent.Create(new SignInData { UserName = userName, Password = password });

			var response = await _client.PostAsync("/signin", signInJsonContent);

			Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
		}

		[TestMethod]
		public async Task ApiHasTestAuthEndPoint()
		{
			_client = _factory.CreateClient();

			var response = await _client.GetAsync("/testauth");

			Assert.IsFalse(response.StatusCode == System.Net.HttpStatusCode.NotFound, $"response.StatusCode was {response.StatusCode}");
		}

		[TestMethod]
		public async Task TestAuthEndPointReturnUnauthorizedWhenUserDidNotSignin()
		{
			_client = _factory.CreateClient();

			var response = await _client.GetAsync("/testauth");

			Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Unauthorized, $"response.StatusCode was {response.StatusCode}");
		}

		[TestMethod]
		public async Task TestAuthEndPointReturnOkWhenUserSignin()
		{
			_client = _factory.CreateClient();
			var email = "testEmail@email.com";
			var password = "testPassword1!";
			var userName = "TestName";
			await RegisterUser(email, password, userName);
			await SignIn(email, password, userName);

			var response = await _client.GetAsync("/testauth");

			Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK, $"response.StatusCode was {response.StatusCode}");
		}

		[TestMethod]
		public async Task ApiHasSignOutEndpoint()
		{
			_client = _factory.CreateClient();

			var response = await _client.GetAsync("/signout");

			Assert.IsFalse(response.StatusCode == System.Net.HttpStatusCode.NotFound, $"response.StatusCode was {response.StatusCode}");
		}

		[TestMethod]
		public async Task SignOutEndPointAllowsToSignOut()
		{
			_client = _factory.CreateClient();
			var email = "testEmail@email.com";
			var password = "testPassword1!";
			var userName = "TestName";
			await RegisterUser(email, password, userName);
			await SignIn(email, password, userName);

			await _client.GetAsync("/signout");
			
			var response = await _client.GetAsync("/testauth");
			Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Unauthorized, $"response.StatusCode was {response.StatusCode}");
		}

		private async Task SignIn(string email, string password, string userName)
		{
			var registerJsonContent = JsonContent.Create(new RegistrationData { Email = email, Password = password, UserName = userName });
			await _client.PostAsync("/signin", registerJsonContent);
		}

		private async Task RegisterUser(string email, string password, string userName)
		{
			var registerJsonContent = JsonContent.Create( new RegistrationData { Email = email, Password = password, UserName = userName });
			await _client.PostAsync("/register", registerJsonContent);
		}
	}

	public class RegistrationData()
	{
		public required string UserName { get; set; }
		public required string Email { get; set; }
		public required string Password { get; set; }
	}

	public class SignInData()
	{
		public required string UserName { get; set; }
		public required string Password { get; set; }
	}
}
