

namespace AntiPlagiarism.Vsix.Logger
{
	/// <summary>
	/// An AntiPlagiarism logger log modes.
	/// </summary>
	internal enum LogMode : short
	{
		/// <summary>
		/// Log nothing.
		/// </summary>
		None,

		/// <summary>
		/// Log informational entry.
		/// </summary>
		Information,

		/// <summary>
		/// Log warning.
		/// </summary>
		Warning,

		/// <summary>
		/// Log error.
		/// </summary>
		Error
	}
}
