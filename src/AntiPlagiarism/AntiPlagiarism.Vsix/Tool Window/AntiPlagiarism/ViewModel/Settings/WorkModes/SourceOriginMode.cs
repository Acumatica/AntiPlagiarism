using System;



namespace AntiPlagiarism.Vsix.ToolWindows
{
	/// <summary>
	/// Values that represent different source origin modes.
	/// </summary>
	public enum SourceOriginMode
	{
		/// <summary>
		/// Use current solution as a source.
		/// </summary>
		CurrentSolution,

		/// <summary>
		/// Use selected project as a source.
		/// </summary>
		SelectedProject,

		/// <summary>
		/// Use selected folder as a source.
		/// </summary>
		SelectedFolder
	}
}
