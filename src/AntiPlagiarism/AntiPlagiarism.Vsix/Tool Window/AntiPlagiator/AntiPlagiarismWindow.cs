using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;


namespace AntiPlagiarism.Vsix.ToolWindows
{
	/// <summary>
	/// The tool window for the <see cref="AntiPlagiarismWindow"/>.
	/// </summary>
	[Guid(AntiPlagiarismWindowGuidString)]
	public class AntiPlagiarismWindow : ToolWindowPane
	{
		public const string AntiPlagiarismWindowGuidString = "A78F6E50-0A04-4A19-B858-F1BB83AAD8A7";

		private readonly AntiPlagiatorWindowControl _control;

		/// <summary>
		/// Initializes a new instance of the <see cref="AntiPlagiarismWindow"/> class.
		/// </summary>
		public AntiPlagiarismWindow() : base(null)
		{
			this.Caption = VSIXResource.AntiPlagiarismWindowTitle;

			// This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
			// we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
			// the object returned by the Content property.
			_control = new AntiPlagiatorWindowControl();
			this.Content = _control;
		}

		protected override void OnClose()
		{
			if (_control?.DataContext is AntiPlagiatorWindowViewModel antiPlagiatorVM)
			{
				antiPlagiatorVM.Dispose();
			}

			base.OnClose();
		}
	}
}
