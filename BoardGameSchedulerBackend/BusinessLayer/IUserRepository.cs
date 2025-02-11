﻿using BoardGameSchedulerBackend.DataLayer;

namespace BoardGameSchedulerBackend.BusinessLayer
{
	public class UserCreationResult
	{
		public enum ErrorCode
		{
			DuplicateEmail
		}

		public List<ErrorCode> Errors { get; init; }

		public UserCreationResult()
		{
			Errors = new List<ErrorCode>();
		}

		public UserCreationResult(List<ErrorCode> errors)
		{
			Errors = errors;
		}
	}

	public interface IUserRepository
	{
		Task<User?> GetByIdAsync(Guid id);
		Task<UserCreationResult> CreateAsync(string userName, string userEmail, string password);
	}
}
