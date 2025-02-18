﻿using System.Net.Http.Json;

namespace BGSIntegrationTesting
{
	[TestClass]
	public class UserControllerTest
	{
		BGSWebApplicationFactory<Program> _factory;
		private HttpClient _client;

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
			var registrationData = new RegistrationData { Email = "testEmail@email.com", Password = "testPassword1!", UserName = "TestName" };
			var jsonContent = JsonContent.Create(registrationData);

			var response = await _client.PostAsync("/register", jsonContent);

			response.EnsureSuccessStatusCode();
		}
	}

	public class RegistrationData()
	{
		public required string UserName { get; set; }
		public required string Email { get; set; }
		public required string Password { get; set; }
	}
}
