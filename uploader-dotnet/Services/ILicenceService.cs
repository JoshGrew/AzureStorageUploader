using System.Collections.Generic;

namespace Azure.Uploader.Services
{
	// Dummy class to replace custom licence validation
	public interface ILicenceService
	{
		// Originally the licence validation process was supplied as a private 
		// NuGet package. As this reference has been removed, this dummy method has 
		// been supplied.
		bool ValidateLicence(string licenceKey);
	}
}