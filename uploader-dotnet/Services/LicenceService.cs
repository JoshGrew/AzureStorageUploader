using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Azure.Uploader.Services
{
	// Dummy class to replace custom licence validation
	public class LicenceService : ILicenceService
	{
		// Originally the licence validation process was supplied as a private 
		// NuGet package. As this reference has been removed, this dummy method has 
		// been supplied.
		public bool ValidateLicence(string licence)
		{
			return !String.IsNullOrEmpty(licence);
		}
	}
}
