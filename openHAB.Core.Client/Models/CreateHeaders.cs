// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace openHAB.Core.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Defines headers for create operation.
    /// </summary>
    public partial class CreateHeaders
    {
        /// <summary>
        /// Initializes a new instance of the CreateHeaders class.
        /// </summary>
        public CreateHeaders()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CreateHeaders class.
        /// </summary>
        /// <param name="location">Newly created Rule</param>
        public CreateHeaders(string location = default(string))
        {
            Location = location;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets newly created Rule
        /// </summary>
        [JsonProperty(PropertyName = "Location")]
        public string Location { get; set; }

    }
}
