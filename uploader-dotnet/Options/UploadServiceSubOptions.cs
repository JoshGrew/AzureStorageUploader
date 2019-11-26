using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Azure.Uploader.Options
{
	/// <summary>
	/// Segmented options available within the Application settings
	/// for use with the Upload service
	/// </summary>
	public class UploadServiceSubOptions
	{
		/// <summary>
		/// Connection string for Azure storage to upload files
		/// </summary>
		public string AzureConnectionString { get; set; }

		/// <summary>
		/// Server used for sending alert email
		/// </summary>
		public string Server { get; set; }

		/// <summary>
		/// Port used for sending alert email
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// Password used for sending alert email
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Username used for sending alert email
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// Recipient of alert email
		/// </summary>
		public string SupportEmail { get; set; }
	}
}
