using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Azure.Uploader.Attributes
{
    /// <summary>
    /// Custom validation method to check the file extensions
    /// are of type csv, xlsx
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FileTypeAttribute : AttributeBase, IClientModelValidator
    {
        // Supported file types
        private readonly string[] extensions = { "csv", "xlsx" };

        /// <summary>
        /// Override method validating the file extension
        /// </summary>
        /// <param name="value">The object being validated in the model</param>
        /// // <param name="validationContext">The context in which the validation is performed </param>
        protected override ValidationResult IsValid(object value,
                    ValidationContext validationContext)
        {
            if (value is IFormFile formFile)
            {
                var file = formFile.FileName;
                var fileExtension = file.Substring(file.LastIndexOf('.') + 1);

                if (extensions.Contains(fileExtension))
                {
                    return ValidationResult.Success;
                }
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
            MergeAttribute(context.Attributes, "data-val-filetype", GetErrorMessage());
        }

        /// <summary>
        /// Return a formatted error message
        /// </summary>
        protected string GetErrorMessage()
        {
            return "File must of type csv or xlsx.";
        }

    }
}
