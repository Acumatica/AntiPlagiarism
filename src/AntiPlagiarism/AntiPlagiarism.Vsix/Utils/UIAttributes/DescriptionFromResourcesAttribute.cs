using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;
using System;
using System.ComponentModel;


namespace AntiPlagiarism.Vsix
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	internal sealed class DescriptionFromResourcesAttribute : DescriptionAttribute
	{		
		public string ResourceKey { get; }

		public override string Description => VSIXResource.ResourceManager.GetStringResourceSafe(ResourceKey);

		public DescriptionFromResourcesAttribute(string resourceKey)
		{
			resourceKey.ThrowOnNullOrWhiteSpace(nameof(resourceKey));
			ResourceKey = resourceKey;
		}	
	}
}
