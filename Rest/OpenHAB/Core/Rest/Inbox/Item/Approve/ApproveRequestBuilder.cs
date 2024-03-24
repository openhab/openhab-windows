// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace OpenHAB.Core.Rest.Inbox.Item.Approve {
    /// <summary>
    /// Builds and executes requests for operations under \inbox\{thingUID}\approve
    /// </summary>
    public class ApproveRequestBuilder : BaseRequestBuilder {
        /// <summary>
        /// Instantiates a new <see cref="ApproveRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public ApproveRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/inbox/{thingUID}/approve{?newThingId*}", pathParameters) {
        }
        /// <summary>
        /// Instantiates a new <see cref="ApproveRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public ApproveRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/inbox/{thingUID}/approve{?newThingId*}", rawUrl) {
        }
        /// <summary>
        /// Approves the discovery result by adding the thing to the registry.
        /// </summary>
        /// <returns>A <see cref="Stream"/></returns>
        /// <param name="body">The request body</param>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<Stream?> PostAsync(string body, Action<RequestConfiguration<ApproveRequestBuilderPostQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default) {
#nullable restore
#else
        public async Task<Stream> PostAsync(string body, Action<RequestConfiguration<ApproveRequestBuilderPostQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default) {
#endif
            if(string.IsNullOrEmpty(body)) throw new ArgumentNullException(nameof(body));
            var requestInfo = ToPostRequestInformation(body, requestConfiguration);
            return await RequestAdapter.SendPrimitiveAsync<Stream>(requestInfo, default, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Approves the discovery result by adding the thing to the registry.
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="body">The request body</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToPostRequestInformation(string body, Action<RequestConfiguration<ApproveRequestBuilderPostQueryParameters>>? requestConfiguration = default) {
#nullable restore
#else
        public RequestInformation ToPostRequestInformation(string body, Action<RequestConfiguration<ApproveRequestBuilderPostQueryParameters>> requestConfiguration = default) {
#endif
            if(string.IsNullOrEmpty(body)) throw new ArgumentNullException(nameof(body));
            var requestInfo = new RequestInformation(Method.POST, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.SetContentFromScalar(RequestAdapter, "text/plain", body);
            return requestInfo;
        }
        /// <summary>
        /// Returns a request builder with the provided arbitrary URL. Using this method means any other path or query parameters are ignored.
        /// </summary>
        /// <returns>A <see cref="ApproveRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public ApproveRequestBuilder WithUrl(string rawUrl) {
            return new ApproveRequestBuilder(rawUrl, RequestAdapter);
        }
        /// <summary>
        /// Approves the discovery result by adding the thing to the registry.
        /// </summary>
        public class ApproveRequestBuilderPostQueryParameters {
            /// <summary>new thing ID</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("newThingId")]
            public string? NewThingId { get; set; }
#nullable restore
#else
            [QueryParameter("newThingId")]
            public string NewThingId { get; set; }
#endif
        }
        /// <summary>
        /// Configuration for the request such as headers, query parameters, and middleware options.
        /// </summary>
        [Obsolete("This class is deprecated. Please use the generic RequestConfiguration class generated by the generator.")]
        public class ApproveRequestBuilderPostRequestConfiguration : RequestConfiguration<ApproveRequestBuilderPostQueryParameters> {
        }
    }
}
