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

		private readonly AntiPlagiarismWindowControl _control;

		/// <summary>
		/// Initializes a new instance of the <see cref="AntiPlagiarismWindow"/> class.
		/// </summary>
		public AntiPlagiarismWindow() : base(null)
		{
			// This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
			// we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
			// the object returned by the Content property.
			_control = new AntiPlagiarismWindowControl();
			this.Content = _control;
		}

		/// <summary>
		/// This is called after our control has been created and sited. 
		/// This is a good place to initialize the control with data gathered from Visual Studio services.
		/// </summary>
		public override void OnToolWindowCreated()
		{
			base.OnToolWindowCreated();

			// Set the text that will appear in the title bar of the tool window. Note that because we need access to the package for localization,
			// we have to wait to do this here. If we used a constant string, we could do this in the constructor.
			this.Caption = VSIXResource.AntiPlagiarismWindowTitle;
		}

		protected override void OnClose()
		{
			if (_control?.DataContext is AntiPlagiarismWindowViewModel antiPlagiarismVM)
			{
				antiPlagiarismVM.Dispose();
			}

			base.OnClose();
		}
	}
}
