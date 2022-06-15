using Bots.CopyFieldBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

			// request to test locally because Yandex passes the request in the 'body' tag of the json file
			var request = new Request
			{
				Body = $"{{ \"task_id\": \"123456789\", \"user_id\": \"123456\", \"access_token\": \"{token}\", \"bot_settings\": \"{RemoveNonPrintableChars(settings)}\" }}"
			};
			// request to test the bot deployed in Yandex
			//var request = $"{{ \"task_id\": \"123456789\", \"user_id\": \"123456\", \"access_token\": \"{token}\", \"bot_settings\": \"{RemoveNonPrintableChars(settings)}\" }}";

			var serializeOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = true
			};

			// call the bot locally
			function.FunctionHandler(JsonSerializer.Serialize(request, serializeOptions));

			// call the bot deployed in Yandex
			//var client = new HttpClient();
			//var content = new StringContent(JsonSerializer.Serialize(request, serializeOptions));
			//await client.PostAsync("https://functions.yandexcloud.net/your_identificator", content);

			Console.WriteLine("Done.");
		}

		private static async Task<string> GetToken(PyrusApiClient.PyrusClient client)
		{
			var response = await client.Auth(
				"bot_login",
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
