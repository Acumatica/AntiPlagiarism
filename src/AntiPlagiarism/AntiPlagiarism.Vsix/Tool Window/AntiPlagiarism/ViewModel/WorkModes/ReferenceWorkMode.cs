using System;



namespace AntiPlagiarism.Vsix.ToolWindows
{
	/// <summary>
	/// Values that represent different work modes for reference solution.
	/// </summary>
	public enum ReferenceWorkMode
	{
		/// <summary>
		/// Self analysis work mode.
		/// </summary>
		SelfAnalysis,

		/// <summary>
		/// Comparison with selected reference solution work mode.
		/// </summary>
		ReferenceSolution,

		/// <summary>
		/// Comparison with Acumatica sources provided as folder work mode.
		/// </summary>
		AcumaticaSources
	}
}
