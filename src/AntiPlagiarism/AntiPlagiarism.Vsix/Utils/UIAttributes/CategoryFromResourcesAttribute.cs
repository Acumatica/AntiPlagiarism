using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;
using System;
using System.ComponentModel;

namespace AntiPlagiarism.Vsix
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	internal sealed class CategoryFromResourcesAttribute : CategoryAttribute
	{		
		public string ResourceKey { get; }

		protected override string GetLocalizedString(string value)
		{
			string resourceString = VSIXResource.ResourceManager.GetStringResourceSafe(ResourceKey);
			return resourceString ?? base.GetLocalizedString(value);
		}

		public CategoryFromResourcesAttribute(string resourceKey, string nonLocalizedCategoryName) : base(nonLocalizedCategoryName) 
		{			
			resourceKey.ThrowOnNullOrWhiteSpace(nameof(resourceKey));
			ResourceKey = resourceKey;
		}	
	}
}
