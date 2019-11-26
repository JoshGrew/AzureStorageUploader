using Azure.Uploader.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azure.Uploader.Services
{
	/// <summary>
	/// Interface for Upload Service
	/// </summary>
	public interface IUploadService
	{
		/// <summary>
		/// Validates the form data, sends the information to the relevant sources
		/// and returns a result of the messages.
		/// </summary>
		/// <param name="uploadForm">The data from the form captured in the model</param>
		Task<List<string>> ProcessRequest(Upload uploadForm);
	}
}