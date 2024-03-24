// <auto-generated/>
using Microsoft.Kiota.Abstractions;
using OpenHAB.Core.Rest.Auth.Apitokens;
using OpenHAB.Core.Rest.Auth.Logout;
using OpenHAB.Core.Rest.Auth.Sessions;
using OpenHAB.Core.Rest.Auth.Token;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
namespace OpenHAB.Core.Rest.Auth {
    /// <summary>
    /// Builds and executes requests for operations under \auth
    /// </summary>
    public class AuthRequestBuilder : BaseRequestBuilder {
        /// <summary>The apitokens property</summary>
        public ApitokensRequestBuilder Apitokens { get =>
            new ApitokensRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The logout property</summary>
        public LogoutRequestBuilder Logout { get =>
            new LogoutRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The sessions property</summary>
        public SessionsRequestBuilder Sessions { get =>
            new SessionsRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The token property</summary>
        public TokenRequestBuilder Token { get =>
            new TokenRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new <see cref="AuthRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public AuthRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/auth", pathParameters) {
        }
        /// <summary>
        /// Instantiates a new <see cref="AuthRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public AuthRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/auth", rawUrl) {
        }
    }
}
