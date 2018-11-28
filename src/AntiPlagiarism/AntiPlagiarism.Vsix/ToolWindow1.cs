using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace AntiPlagiarism.Vsix
{
	/// <summary>
	/// This class implements the tool window exposed by this package and hosts a user control.
	/// </summary>
	/// <remarks>
	/// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
	/// usually implemented by the package implementer.
	/// <para>
	/// This class derives from the ToolWindowPane class provided from the MPF in order to use its
	/// implementation of the IVsUIElementPane interface.
	/// </para>
	/// </remarks>
	[Guid("91e870de-2be3-42ac-83cd-800ed052c7ba")]
	public class ToolWindow1 : ToolWindowPane
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ToolWindow1"/> class.
		/// </summary>
		public ToolWindow1() : base(null)
		{
			this.Caption = "ToolWindow1";

			// This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
			// we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
			// the object returned by the Content property.
			this.Content = new ToolWindow1Control();
		}
	}
}
