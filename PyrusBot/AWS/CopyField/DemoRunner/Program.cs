using Bots.CopyFieldBot;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DemoRunner
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var function = new Bot();
			var token = await GetToken(new PyrusApiClient.PyrusClient());

			var settings = @"
			{
				\""SourceFieldCode\"": \""u_Source\"",
				\""TargetFieldCode\"": \""u_Target\""
			}";

			await function.ParseAndRun(new Amazon.Lambda.APIGatewayEvents.APIGatewayProxyRequest
			{
				// local target task
				Body = $"{{ \"task_id\": 124442848, \"user_id\": 674101, \"access_token\": \"{token}\", \"bot_settings\": \"{RemoveNonPrintableChars(settings)}\" }} ",
			});

			Console.WriteLine("Done.");
		}

		private static async Task<string> GetToken(PyrusApiClient.PyrusClient client)
		{
			var response = await client.Auth(
				"bot@1cde7693-1634-4a13-bfad-3b6c0be14441",
				"~CAFyaF9JB6-G5o7SwMRWbJIgXxjvtDyORFpYb7TR3k65KsK2LbqGnYWdP6OCfKf~E0o1PZexfXYJn9-UUD3WALVeQikOSiH"
			);

			if (response == null || response.ErrorCode != null)
				throw new Exception($"Failed to get token.");

			return response.AccessToken;
		}

		private static string RemoveNonPrintableChars(string text)
		{
			return new string(text.Where(c => !char.IsControl(c)).ToArray());
		}
	}
}
