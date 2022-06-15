using System.Text.Json.Serialization;

namespace Bots.CopyFieldBot
{
	/// <summary>
	/// Incoming request to the bot.
	/// </summary>
	public class Request
	{
		[JsonPropertyName("body")]
		public string Body { get; set; }
	}
}

