using System;



namespace AntiPlagiarism.Vsix.ToolWindows
{
	/// <summary>
	/// Values that represent different work modes.
	/// </summary>
	public enum WorkMode
	{
		/// <summary>
		/// Self analysis work mode.
		/// </summary>
		SelfAnalysis,

		/// <summary>
		/// Comparison with selected referenca solution work mode.
		/// </summary>
		ReferenceSolution,

		/// <summary>
		/// Comparison with Acumatica sources provided as folder work mode.
		/// </summary>
		AcumaticaSources
	}
}
