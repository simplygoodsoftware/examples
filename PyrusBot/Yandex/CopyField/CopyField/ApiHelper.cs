using PyrusApiClient;

namespace Bots.CopyFieldBot
{
	/// <summary>
	/// Helper class for API calls.
	/// </summary>
	public static class ApiHelper
	{
		/// <summary>
		/// Verifies the success of API call.
		/// The response must not be <see langword="null"/> and does not contain any error.
		/// </summary>
		/// <typeparam name="T">Response type.</typeparam>
		/// <param name="response">Response from Pyrus API to verify.</param>
		/// <returns>Passes the incoming response on success, otherwise throws an exception.</returns>
		public static T EnsureSuccess<T>(T response) where T : ResponseBase
		{
			if (response == null)
				throw new System.Exception($"Pyrus API response '{typeof(T)}' is null.");

			if (response.Error != null)
				throw new System.Exception($"Pyrus API response '{typeof(T)}' contains error: \"{response.Error}\".");

			return response;
		}
	}
}
