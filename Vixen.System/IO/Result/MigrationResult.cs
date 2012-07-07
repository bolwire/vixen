﻿namespace Vixen.IO.Result {
	class MigrationResult : IResult {
		public MigrationResult(bool result, string message, int fromVersion, int toVersion) {
			Success = result;
			Message = "Migration from version " + fromVersion + " to " + toVersion + ". " + message;
		}

		public bool Success { get; private set; }

		public string Message { get; private set; }
	}
}
