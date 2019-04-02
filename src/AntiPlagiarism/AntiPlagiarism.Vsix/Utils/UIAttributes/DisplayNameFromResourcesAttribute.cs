using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;
using System;
using System.ComponentModel;


namespace AntiPlagiarism.Vsix
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	internal sealed class DisplayNameFromResourcesAttribute : DisplayNameAttribute
	{		
		public string ResourceKey { get; }

		public override string DisplayName => VSIXResource.ResourceManager.GetStringResourceSafe(ResourceKey);

		public DisplayNameFromResourcesAttribute(string resourceKey)
		{
			resourceKey.ThrowOnNullOrWhiteSpace(nameof(resourceKey));
			ResourceKey = resourceKey;
		}	
	}
}
