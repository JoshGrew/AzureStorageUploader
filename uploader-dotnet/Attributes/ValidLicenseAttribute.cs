using Azure.Uploader.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Azure.Uploader.Attributes
{
	/// <summary>
	/// Attribute class that is used to validate licence keys
	/// during model binding
	/// </summary>
	public class ValidLicenseAttribute : AttributeBase, IClientModelValidator
    {
		/// <summary>
		/// Override method validating the license key
		/// </summary>
		/// <param name="value">The object being validated in the model</param>
		/// <param name="validationContext">The context in which the validation is performed </param>
		protected override ValidationResult IsValid(object value,
                    ValidationContext validationContext)
        {
			// Use validationContext to retrieve the licence service
			var service = (ILicenceService)validationContext
						.GetService(typeof(ILicenceService));
			
            string license = value as string;
            if (String.IsNullOrWhiteSpace(license) == false)
            {
				// Get a list of all valid licenses
				var validLicence = service.ValidateLicence(license);
                // Ensure the license key is valid before uploading to blob storage
                if (validLicence)
					return ValidationResult.Success;
            }
            return new ValidationResult(GetErrorMessage());
        }

        /// <summary>
        /// Add necessary attributes to file validation context
        /// </summary>
        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-licencekey", GetErrorMessage());
        }

        /// <summary>
        /// Return a formatted error message
        /// </summary>
        protected string GetErrorMessage()
        {
            return "Invalid licence key supplied.";
        }
    }
}
