// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using OpenHAB.Core.Rest.Addons.Item.Config;
using OpenHAB.Core.Rest.Addons.Item.Install;
using OpenHAB.Core.Rest.Addons.Item.Uninstall;
using OpenHAB.Core.Rest.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace OpenHAB.Core.Rest.Addons.Item {
    /// <summary>
    /// Builds and executes requests for operations under \addons\{addonId}
    /// </summary>
    public class WithAddonItemRequestBuilder : BaseRequestBuilder {
        /// <summary>The config property</summary>
        public ConfigRequestBuilder Config { get =>
            new ConfigRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The install property</summary>
        public InstallRequestBuilder Install { get =>
            new InstallRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The uninstall property</summary>
        public UninstallRequestBuilder Uninstall { get =>
            new UninstallRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new <see cref="WithAddonItemRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WithAddonItemRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/addons/{addonId}{?serviceId*}", pathParameters) {
        }
        /// <summary>
        /// Instantiates a new <see cref="WithAddonItemRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WithAddonItemRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/addons/{addonId}{?serviceId*}", rawUrl) {
        }
        /// <summary>
        /// Get add-on with given ID.
        /// </summary>
        /// <returns>A <see cref="Addon"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<Addon?> GetAsync(Action<RequestConfiguration<WithAddonItemRequestBuilderGetQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default) {
#nullable restore
#else
        public async Task<Addon> GetAsync(Action<RequestConfiguration<WithAddonItemRequestBuilderGetQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default) {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            return await RequestAdapter.SendAsync<Addon>(requestInfo, Addon.CreateFromDiscriminatorValue, default, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Get add-on with given ID.
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<WithAddonItemRequestBuilderGetQueryParameters>>? requestConfiguration = default) {
#nullable restore
#else
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<WithAddonItemRequestBuilderGetQueryParameters>> requestConfiguration = default) {
#endif
            var requestInfo = new RequestInformation(Method.GET, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/json");
            return requestInfo;
        }
        /// <summary>
        /// Returns a request builder with the provided arbitrary URL. Using this method means any other path or query parameters are ignored.
        /// </summary>
        /// <returns>A <see cref="WithAddonItemRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public WithAddonItemRequestBuilder WithUrl(string rawUrl) {
            return new WithAddonItemRequestBuilder(rawUrl, RequestAdapter);
        }
        /// <summary>
        /// Get add-on with given ID.
        /// </summary>
        public class WithAddonItemRequestBuilderGetQueryParameters {
            /// <summary>service ID</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("serviceId")]
            public string? ServiceId { get; set; }
#nullable restore
#else
            [QueryParameter("serviceId")]
            public string ServiceId { get; set; }
#endif
        }
        /// <summary>
        /// Configuration for the request such as headers, query parameters, and middleware options.
        /// </summary>
        [Obsolete("This class is deprecated. Please use the generic RequestConfiguration class generated by the generator.")]
        public class WithAddonItemRequestBuilderGetRequestConfiguration : RequestConfiguration<WithAddonItemRequestBuilderGetQueryParameters> {
        }
    }
}
