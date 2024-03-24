// <auto-generated/>
using Microsoft.Kiota.Abstractions;
using OpenHAB.Core.Rest.Voice.Dialog.Start;
using OpenHAB.Core.Rest.Voice.Dialog.Stop;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
namespace OpenHAB.Core.Rest.Voice.Dialog {
    /// <summary>
    /// Builds and executes requests for operations under \voice\dialog
    /// </summary>
    public class DialogRequestBuilder : BaseRequestBuilder {
        /// <summary>The start property</summary>
        public StartRequestBuilder Start { get =>
            new StartRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The stop property</summary>
        public StopRequestBuilder Stop { get =>
            new StopRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new <see cref="DialogRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public DialogRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/voice/dialog", pathParameters) {
        }
        /// <summary>
        /// Instantiates a new <see cref="DialogRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public DialogRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/voice/dialog", rawUrl) {
        }
    }
}
