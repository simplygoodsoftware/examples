namespace Bots.CopyFieldBot
{
	/// <summary>
	/// Bot parameters defined in Settings field in bot profile page in Pyrus.
	/// </summary>
	public class BotSettings
	{
		/// <summary>
		/// Code of the field to copy from.
		/// </summary>
		public string SourceFieldCode { get; set; }
		/// <summary>
		/// Code of the field to copy to.
		/// </summary>
		public string TargetFieldCode { get; set; }
	}
}
