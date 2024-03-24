// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using OpenHAB.Core.Rest.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace OpenHAB.Core.Rest.Persistence.Items.Item {
    /// <summary>
    /// Builds and executes requests for operations under \persistence\items\{itemname}
    /// </summary>
    public class WithItemnameItemRequestBuilder : BaseRequestBuilder {
        /// <summary>
        /// Instantiates a new <see cref="WithItemnameItemRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WithItemnameItemRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/persistence/items/{itemname}{?boundary*,endtime*,page*,pagelength*,serviceId*,starttime*}", pathParameters) {
        }
        /// <summary>
        /// Instantiates a new <see cref="WithItemnameItemRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WithItemnameItemRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/persistence/items/{itemname}{?boundary*,endtime*,page*,pagelength*,serviceId*,starttime*}", rawUrl) {
        }
        /// <summary>
        /// Deletes item persistence data from a specific persistence service in a given time range.
        /// </summary>
        /// <returns>A List&lt;string&gt;</returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<List<string>?> DeleteAsync(Action<RequestConfiguration<WithItemnameItemRequestBuilderDeleteQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default) {
#nullable restore
#else
        public async Task<List<string>> DeleteAsync(Action<RequestConfiguration<WithItemnameItemRequestBuilderDeleteQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default) {
#endif
            var requestInfo = ToDeleteRequestInformation(requestConfiguration);
            var collectionResult = await RequestAdapter.SendPrimitiveCollectionAsync<string>(requestInfo, default, cancellationToken).ConfigureAwait(false);
            return collectionResult?.ToList();
        }
        /// <summary>
        /// Gets item persistence data from the persistence service.
        /// </summary>
        /// <returns>A <see cref="ItemHistoryDTO"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<ItemHistoryDTO?> GetAsync(Action<RequestConfiguration<WithItemnameItemRequestBuilderGetQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default) {
#nullable restore
#else
        public async Task<ItemHistoryDTO> GetAsync(Action<RequestConfiguration<WithItemnameItemRequestBuilderGetQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default) {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            return await RequestAdapter.SendAsync<ItemHistoryDTO>(requestInfo, ItemHistoryDTO.CreateFromDiscriminatorValue, default, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Stores item persistence data into the persistence service.
        /// </summary>
        /// <returns>A <see cref="Stream"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<Stream?> PutAsync(Action<RequestConfiguration<WithItemnameItemRequestBuilderPutQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default) {
#nullable restore
#else
        public async Task<Stream> PutAsync(Action<RequestConfiguration<WithItemnameItemRequestBuilderPutQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default) {
#endif
            var requestInfo = ToPutRequestInformation(requestConfiguration);
            return await RequestAdapter.SendPrimitiveAsync<Stream>(requestInfo, default, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Deletes item persistence data from a specific persistence service in a given time range.
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToDeleteRequestInformation(Action<RequestConfiguration<WithItemnameItemRequestBuilderDeleteQueryParameters>>? requestConfiguration = default) {
#nullable restore
#else
        public RequestInformation ToDeleteRequestInformation(Action<RequestConfiguration<WithItemnameItemRequestBuilderDeleteQueryParameters>> requestConfiguration = default) {
#endif
            var requestInfo = new RequestInformation(Method.DELETE, "{+baseurl}/persistence/items/{itemname}?endtime={endtime}&serviceId={serviceId}&starttime={starttime}", PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/json");
            return requestInfo;
        }
        /// <summary>
        /// Gets item persistence data from the persistence service.
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<WithItemnameItemRequestBuilderGetQueryParameters>>? requestConfiguration = default) {
#nullable restore
#else
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<WithItemnameItemRequestBuilderGetQueryParameters>> requestConfiguration = default) {
#endif
            var requestInfo = new RequestInformation(Method.GET, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/json");
            return requestInfo;
        }
        /// <summary>
        /// Stores item persistence data into the persistence service.
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToPutRequestInformation(Action<RequestConfiguration<WithItemnameItemRequestBuilderPutQueryParameters>>? requestConfiguration = default) {
#nullable restore
#else
        public RequestInformation ToPutRequestInformation(Action<RequestConfiguration<WithItemnameItemRequestBuilderPutQueryParameters>> requestConfiguration = default) {
#endif
            var requestInfo = new RequestInformation(Method.PUT, "{+baseurl}/persistence/items/{itemname}?state={state}&time={time}{&serviceId*}", PathParameters);
            requestInfo.Configure(requestConfiguration);
            return requestInfo;
        }
        /// <summary>
        /// Returns a request builder with the provided arbitrary URL. Using this method means any other path or query parameters are ignored.
        /// </summary>
        /// <returns>A <see cref="WithItemnameItemRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public WithItemnameItemRequestBuilder WithUrl(string rawUrl) {
            return new WithItemnameItemRequestBuilder(rawUrl, RequestAdapter);
        }
        /// <summary>
        /// Deletes item persistence data from a specific persistence service in a given time range.
        /// </summary>
        public class WithItemnameItemRequestBuilderDeleteQueryParameters {
            /// <summary>End of the time range to be deleted. [yyyy-MM-dd&apos;T&apos;HH:mm:ss.SSSZ]</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("endtime")]
            public string? Endtime { get; set; }
#nullable restore
#else
            [QueryParameter("endtime")]
            public string Endtime { get; set; }
#endif
            /// <summary>Id of the persistence service.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("serviceId")]
            public string? ServiceId { get; set; }
#nullable restore
#else
            [QueryParameter("serviceId")]
            public string ServiceId { get; set; }
#endif
            /// <summary>Start of the time range to be deleted. [yyyy-MM-dd&apos;T&apos;HH:mm:ss.SSSZ]</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("starttime")]
            public string? Starttime { get; set; }
#nullable restore
#else
            [QueryParameter("starttime")]
            public string Starttime { get; set; }
#endif
        }
        /// <summary>
        /// Configuration for the request such as headers, query parameters, and middleware options.
        /// </summary>
        [Obsolete("This class is deprecated. Please use the generic RequestConfiguration class generated by the generator.")]
        public class WithItemnameItemRequestBuilderDeleteRequestConfiguration : RequestConfiguration<WithItemnameItemRequestBuilderDeleteQueryParameters> {
        }
        /// <summary>
        /// Gets item persistence data from the persistence service.
        /// </summary>
        public class WithItemnameItemRequestBuilderGetQueryParameters {
            /// <summary>Gets one value before and after the requested period.</summary>
            [QueryParameter("boundary")]
            public bool? Boundary { get; set; }
            /// <summary>End time of the data to return. Will default to current time. [yyyy-MM-dd&apos;T&apos;HH:mm:ss.SSSZ]</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("endtime")]
            public string? Endtime { get; set; }
#nullable restore
#else
            [QueryParameter("endtime")]
            public string Endtime { get; set; }
#endif
            /// <summary>Page number of data to return. This parameter will enable paging.</summary>
            [QueryParameter("page")]
            public int? Page { get; set; }
            /// <summary>The length of each page.</summary>
            [QueryParameter("pagelength")]
            public int? Pagelength { get; set; }
            /// <summary>Id of the persistence service. If not provided the default service will be used</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("serviceId")]
            public string? ServiceId { get; set; }
#nullable restore
#else
            [QueryParameter("serviceId")]
            public string ServiceId { get; set; }
#endif
            /// <summary>Start time of the data to return. Will default to 1 day before endtime. [yyyy-MM-dd&apos;T&apos;HH:mm:ss.SSSZ]</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("starttime")]
            public string? Starttime { get; set; }
#nullable restore
#else
            [QueryParameter("starttime")]
            public string Starttime { get; set; }
#endif
        }
        /// <summary>
        /// Configuration for the request such as headers, query parameters, and middleware options.
        /// </summary>
        [Obsolete("This class is deprecated. Please use the generic RequestConfiguration class generated by the generator.")]
        public class WithItemnameItemRequestBuilderGetRequestConfiguration : RequestConfiguration<WithItemnameItemRequestBuilderGetQueryParameters> {
        }
        /// <summary>
        /// Stores item persistence data into the persistence service.
        /// </summary>
        public class WithItemnameItemRequestBuilderPutQueryParameters {
            /// <summary>Id of the persistence service. If not provided the default service will be used</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("serviceId")]
            public string? ServiceId { get; set; }
#nullable restore
#else
            [QueryParameter("serviceId")]
            public string ServiceId { get; set; }
#endif
            /// <summary>The state to store.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("state")]
            public string? State { get; set; }
#nullable restore
#else
            [QueryParameter("state")]
            public string State { get; set; }
#endif
            /// <summary>Time of the data to be stored. Will default to current time. [yyyy-MM-dd&apos;T&apos;HH:mm:ss.SSSZ]</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("time")]
            public string? Time { get; set; }
#nullable restore
#else
            [QueryParameter("time")]
            public string Time { get; set; }
#endif
        }
        /// <summary>
        /// Configuration for the request such as headers, query parameters, and middleware options.
        /// </summary>
        [Obsolete("This class is deprecated. Please use the generic RequestConfiguration class generated by the generator.")]
        public class WithItemnameItemRequestBuilderPutRequestConfiguration : RequestConfiguration<WithItemnameItemRequestBuilderPutQueryParameters> {
        }
    }
}
