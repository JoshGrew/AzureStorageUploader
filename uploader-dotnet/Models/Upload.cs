using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Azure.Uploader.Attributes;

namespace Azure.Uploader.Models
{
    /// <summary>
    /// Model data set for use with the UploadController form
    /// </summary>
    public class Upload
    {
		/// <summary>
		/// Additional Notes the User wishes to send
		/// </summary>
		public string Notes { get; set; }

		/// <summary>
		/// The Name of the user. Required but not validated
		/// </summary>
		[Required]
        public string Name { get; set; }

		/// <summary>
		/// Licence key associated with the User's product. Validated
		/// through the Valid Licence attribute which checks if the
		/// licence key exists and has the correct rights to upload
		/// </summary>
		[DisplayName("Licence Key")]
        [ValidLicense]
        [Required]
        public string LicenceKey { get; set; }

		/// <summary>
		/// Email address of the user. Required and validated
		/// </summary>
		[DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Required]
        public string Email { get; set; }

		/// <summary>
		/// File of User-Agents they wish to review. Checks that
		/// the file supplied is of type CSV or XLSX
		/// </summary>
		[DisplayName("File")]
        [FileType]
        [Required]
        public IFormFile FormFile { get; set; }
    }
}
