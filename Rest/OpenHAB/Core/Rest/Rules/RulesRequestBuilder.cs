// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using OpenHAB.Core.Rest.Models;
using OpenHAB.Core.Rest.Rules.Item;
using OpenHAB.Core.Rest.Rules.Schedule;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace OpenHAB.Core.Rest.Rules {
    /// <summary>
    /// Builds and executes requests for operations under \rules
    /// </summary>
    public class RulesRequestBuilder : BaseRequestBuilder {
        /// <summary>The schedule property</summary>
        public ScheduleRequestBuilder Schedule { get =>
            new ScheduleRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>Gets an item from the openHAB.Core.Rest.rules.item collection</summary>
        /// <param name="position">ruleUID</param>
        /// <returns>A <see cref="WithRuleUItemRequestBuilder"/></returns>
        public WithRuleUItemRequestBuilder this[string position] { get {
            var urlTplParams = new Dictionary<string, object>(PathParameters);
            urlTplParams.Add("ruleUID", position);
            return new WithRuleUItemRequestBuilder(urlTplParams, RequestAdapter);
        } }
        /// <summary>
        /// Instantiates a new <see cref="RulesRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public RulesRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/rules{?prefix*,staticDataOnly*,summary*,tags*}", pathParameters) {
        }
        /// <summary>
        /// Instantiates a new <see cref="RulesRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public RulesRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/rules{?prefix*,staticDataOnly*,summary*,tags*}", rawUrl) {
        }
        /// <summary>
        /// Get available rules, optionally filtered by tags and/or prefix.
        /// </summary>
        /// <returns>A List&lt;EnrichedRuleDTO&gt;</returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<List<EnrichedRuleDTO>?> GetAsync(Action<RequestConfiguration<RulesRequestBuilderGetQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default) {
#nullable restore
#else
        public async Task<List<EnrichedRuleDTO>> GetAsync(Action<RequestConfiguration<RulesRequestBuilderGetQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default) {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            var collectionResult = await RequestAdapter.SendCollectionAsync<EnrichedRuleDTO>(requestInfo, EnrichedRuleDTO.CreateFromDiscriminatorValue, default, cancellationToken).ConfigureAwait(false);
            return collectionResult?.ToList();
        }
        /// <summary>
        /// Creates a rule.
        /// </summary>
        /// <param name="body">The request body</param>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task PostAsync(RuleDTO body, Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default) {
#nullable restore
#else
        public async Task PostAsync(RuleDTO body, Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default) {
#endif
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = ToPostRequestInformation(body, requestConfiguration);
            await RequestAdapter.SendNoContentAsync(requestInfo, default, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Get available rules, optionally filtered by tags and/or prefix.
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<RulesRequestBuilderGetQueryParameters>>? requestConfiguration = default) {
#nullable restore
#else
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<RulesRequestBuilderGetQueryParameters>> requestConfiguration = default) {
#endif
            var requestInfo = new RequestInformation(Method.GET, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/json");
            return requestInfo;
        }
        /// <summary>
        /// Creates a rule.
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="body">The request body</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToPostRequestInformation(RuleDTO body, Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default) {
#nullable restore
#else
        public RequestInformation ToPostRequestInformation(RuleDTO body, Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default) {
#endif
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = new RequestInformation(Method.POST, "{+baseurl}/rules", PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.SetContentFromParsable(RequestAdapter, "application/json", body);
            return requestInfo;
        }
        /// <summary>
        /// Returns a request builder with the provided arbitrary URL. Using this method means any other path or query parameters are ignored.
        /// </summary>
        /// <returns>A <see cref="RulesRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public RulesRequestBuilder WithUrl(string rawUrl) {
            return new RulesRequestBuilder(rawUrl, RequestAdapter);
        }
        /// <summary>
        /// Get available rules, optionally filtered by tags and/or prefix.
        /// </summary>
        public class RulesRequestBuilderGetQueryParameters {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("prefix")]
            public string? Prefix { get; set; }
#nullable restore
#else
            [QueryParameter("prefix")]
            public string Prefix { get; set; }
#endif
            /// <summary>provides a cacheable list of values not expected to change regularly and honors the If-Modified-Since header, all other parameters are ignored</summary>
            [QueryParameter("staticDataOnly")]
            public bool? StaticDataOnly { get; set; }
            /// <summary>summary fields only</summary>
            [QueryParameter("summary")]
            public bool? Summary { get; set; }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("tags")]
            public string[]? Tags { get; set; }
#nullable restore
#else
            [QueryParameter("tags")]
            public string[] Tags { get; set; }
#endif
        }
        /// <summary>
        /// Configuration for the request such as headers, query parameters, and middleware options.
        /// </summary>
        [Obsolete("This class is deprecated. Please use the generic RequestConfiguration class generated by the generator.")]
        public class RulesRequestBuilderGetRequestConfiguration : RequestConfiguration<RulesRequestBuilderGetQueryParameters> {
        }
        /// <summary>
        /// Configuration for the request such as headers, query parameters, and middleware options.
        /// </summary>
        [Obsolete("This class is deprecated. Please use the generic RequestConfiguration class generated by the generator.")]
        public class RulesRequestBuilderPostRequestConfiguration : RequestConfiguration<DefaultQueryParameters> {
        }
    }
}
