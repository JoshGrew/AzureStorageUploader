using Azure.License.Services;
using Azure.Uploader.Models;
using Azure.Uploader.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Azure.Uploader.Services
{
	/// <summary>
	/// Service available to upload the data sent over through the upload form
	/// to Azure Storage and alert the team it's available.
	/// </summary>
	public class UploadService : IUploadService
	{
		private readonly UploadServiceSubOptions _options;
		/// <summary>
		/// Constructor for the Upload Service
		/// </summary>
		/// /// <param name="config"> Pull the values required from AppSettings
		/// e.g Azure connection string, SMTP settings </param>
		public UploadService(IOptionsMonitor<UploadServiceSubOptions> options) => _options = options.CurrentValue;
		
		/// <summary>
		/// Validates the form data, sends the information to the relevant sources
		/// and returns a result of the messages.
		/// </summary>
		/// <param name="uploadForm">The data from the form captured in the model</param>
		public async Task<List<string>> ProcessRequest(Upload uploadForm)
		{
			// Collate any responses to display on the form
			List<string> responses = new List<string>();

			// Validate the licence key supplied and return a list of valid licence ids for use 
			// with the upload to the storage account
			var licenseId = LicenceHelper.GetValidLicences(uploadForm.LicenceKey)
						   .FirstOrDefault()
						   .LicenceId;

			CloudStorageAccount storageAccount;
			// Create a blob name for use with the date and time of the request
			string _blobName = String.Join("-", "UAs"
						, DateTime.UtcNow.ToString("u")) + ".bin";
			// Check whether the connection string can be parsed.
			if (CloudStorageAccount.TryParse(_options.AzureConnectionString, out storageAccount))
			{
				// If the connection string is valid, proceed with operations against Blob storage here.
				CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
				// Start the upload process to Blob Storage
				try
				{
					// Check if the container exists for the license key
					CloudBlobContainer blobContainer = blobClient.GetContainerReference("####" + licenseId);
					// If it does not exist, create it
					await blobContainer.CreateIfNotExistsAsync();
					// Check if there is a blob reference with this name, create if not
					CloudBlockBlob blobReference = blobContainer.GetBlockBlobReference(_blobName);
					// Upload file to blob as stream
					using (var stream = uploadForm.FormFile.OpenReadStream())
					{
						await blobReference.UploadFromStreamAsync(stream);
					}

					// Send an email alerting the team
					if (SendMail(uploadForm.Name, uploadForm.Email, uploadForm.Notes, blobReference))
					{
						responses.Add("Successfully submitted User-Agents. A member of our team will be in touch shortly.");
					}
					else
					{
						responses.Add("Failed to contact, please " +
							"<a href=\"####\">get in touch</a>.");
					}
				}
				catch(Exception ex)
				{
					responses.Clear();
					responses.Add( "Failed to upload the file, please " +
						"<a href=\"####">get in touch</a>.");
				}
			}
			return responses;
		}


		/// <summary>
		/// Sends an Email alerting of the upload request
		/// </summary>
		/// <param name="name">Users name supplied on form</param>
		/// <param name="email">Users email supplied on form</param>
		/// <param name="notes">Users notes supplied on form</param>
		/// <param name="licenceId">LicenceId retrieved from users license key</param>
		private bool SendMail(string name, string email, string notes, CloudBlockBlob blobReference)
		{
			// Default draft message
			var defaultMessage = $"<p>{name} ({email}) has uploaded a list of User-Agents</p>";
			// Append notes if available
			if (String.IsNullOrEmpty(notes) == false)
			{
				defaultMessage += $"<p>{notes}</p>";
			}
			// Generate an access token URL for the message
			var sharedAccessToken = GetBlobSasUri(blobReference, null);
			// Create a hyper-link for the Email
			var hyperlinkToken = $"<p><a href=\"{sharedAccessToken}\">Download Here</a></p>";
			// Concatenate the body using all messages
			var message = defaultMessage + hyperlinkToken;
			// Send the email
			try
			{
				SmtpClient client = new SmtpClient
				{
					Host = _options.Server,
					Port = _options.Port,
					Credentials = new System.Net.NetworkCredential(_options.UserName, _options.Password),
					DeliveryMethod = SmtpDeliveryMethod.Network,
					EnableSsl = true
				};
				MailMessage newMessage = new MailMessage
				{
					From = new MailAddress(_options.UserName, "Alerts"),
					Subject = "#### - UA review",
					Body = message,
					IsBodyHtml = true

				};
				newMessage.To.Add(new MailAddress(
					_options.SupportEmail));

				client.Send(newMessage);
				newMessage.Dispose();
			}
			catch
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Generates a Secure access token URL for the uploaded UA file
		/// </summary>
		/// <param name="blobReference">The Blob reference used to upload the file </param>
		/// <param name="policyName">The access policy set up on Blob Storage</param>
		private static string GetBlobSasUri(CloudBlockBlob blobReference, string policyName = null)
		{
			string sasBlobToken;

			if (policyName == null)
			{
				// Create a new access policy and define its constraints.
				SharedAccessBlobPolicy adHocSAS = new SharedAccessBlobPolicy()
				{
					// When the start time for the SAS is omitted, the start time is assumed to be 
					// the time when the storage service receives the request and is effective immediately
					SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
					Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create
				};
				// Generate the shared access signature on the blob, setting the constraints directly on the signature.
				sasBlobToken = blobReference.GetSharedAccessSignature(adHocSAS);
			}
			else
			{
				// Generate the shared access signature on the blob if there a already a shared access policy
				sasBlobToken = blobReference.GetSharedAccessSignature(null, policyName);
			}
			// Return the URI string for the container, including the SAS token.
			return blobReference.Uri + sasBlobToken;
		}

	}
}
