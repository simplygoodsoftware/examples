using System.Text.Json.Serialization;

namespace Bots.CopyFieldBot
{
	/// <summary>
	/// Http request from Pyrus to invoke bot execution.
	/// </summary>
	internal class BotRequest
	{
		/// <summary>
		/// Bearer token sent from Pyrus for subsequent bot requests.
		/// </summary>
		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; }

		/// <summary>
		/// Id of the task from which the bot was invoked.
		/// </summary>
		[JsonPropertyName("task_id")]
		public int TaskId { get; set; }

		/// <summary>
		/// Id of the bot in Pyrus.
		/// </summary>
		[JsonPropertyName("user_id")]
		public int UserId { get; set; }

		/// <summary>
		/// Content of the 'Settings' field from the bot profile.
		/// </summary>
		[JsonPropertyName("bot_settings")]
		public string BotSettings { get; set; }
	}
}
