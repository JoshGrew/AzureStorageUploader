using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Azure.Uploader.Attributes
{
	/// <summary>
	/// Abstract class for use with Attribute validation methods
	/// validation
	/// </summary>
	public abstract class AttributeBase : ValidationAttribute
    {
        /// <summary>
        /// Helper method for adding attributes to the client side model
        /// validation
        /// </summary>
        internal bool MergeAttribute(
        IDictionary<string, string> attributes,
        string key,
        string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }
}
