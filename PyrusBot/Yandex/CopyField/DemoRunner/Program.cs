using Bots.CopyFieldBot;
using System;
using System.Linq;
using System.Text.Json;
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

			var request = new BotRequest
			{
				Body = $"{{ \"task_id\": \"123456789\", \"user_id\": \"123456\", \"access_token\": \"{token}\", \"bot_settings\": \"{RemoveNonPrintableChars(settings)}\" }}"
			};

			function.FunctionHandler(JsonSerializer.Serialize(request));

			Console.WriteLine("Done.");
		}

		private static async Task<string> GetToken(PyrusApiClient.PyrusClient client)
		{
			var response = await client.Auth(
				"bot_logini",
				"security_key"
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
