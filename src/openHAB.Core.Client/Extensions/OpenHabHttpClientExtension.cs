using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using openHAB.Core.Client.Connection.Models;
using openHAB.Core.Client.Options;

namespace openHAB.Core.Client.Extensions;

/// <summary>
/// Provides extension methods for adding OpenHAB HTTP clients to the service collection.
/// </summary>
public static class OpenHabHttpClientExtension
{
    /// <summary>
    /// Adds the OpenHAB HTTP clients to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to add the HTTP clients to.</param>
    public static void AddOpenHabHttpClients(this IServiceCollection services)
    {
        AddHttpClient(services, "local", options => options.LocalConnection);
        AddHttpClient(services, "remote", options => options.RemoteConnection);
    }

    private static void AddHttpClient(IServiceCollection services, string name, Func<ConnectionOptions, Connection.Models.Connection> getConnection)
    {
        services.AddHttpClient<OpenHABClient>(name, (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ConnectionOptions>>().Value;
            Connection.Models.Connection connection = CreateConnection(getConnection, options);

            ConfigureHttpClient(client, connection);
        }).ConfigurePrimaryHttpMessageHandler(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ConnectionOptions>>().Value;
            Connection.Models.Connection connection = CreateConnection(getConnection, options);
            return CreateHttpClientHandler(connection);
        });
    }

    private static Connection.Models.Connection CreateConnection(Func<ConnectionOptions, Connection.Models.Connection> getConnection, ConnectionOptions options)
    {
        // ponytail: must stay in sync with ConnectionService.DetectAndRetrieveConnection — both decide
        // "no connection configured -> demo" and the named HttpClient here must match what the service picks.
        bool noConnectionConfigured =
            string.IsNullOrWhiteSpace(options.LocalConnection?.Url) &&
            string.IsNullOrWhiteSpace(options.RemoteConnection?.Url);

        if (options.IsRunningInDemoMode.GetValueOrDefault() || noConnectionConfigured)
        {
            return new DemoConnectionProfile().CreateConnection();
        }

        return getConnection(options);
    }

    private static void ConfigureHttpClient(HttpClient client, Connection.Models.Connection connection)
    {
        if (string.IsNullOrEmpty(connection?.Url))
        {
            return;
        }

        client.BaseAddress = new Uri(connection.Url);

        if (!string.IsNullOrEmpty(connection.Username) && !string.IsNullOrEmpty(connection.Password))
        {
            string basicCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{connection.Username}:{connection.Password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicCredentials);
        }

        //client.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    private static HttpClientHandler CreateHttpClientHandler(Connection.Models.Connection connection)
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (errors == SslPolicyErrors.None)
                {
                    return true;
                }

                bool result = true;
                if (errors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors))
                {
                    result &= connection?.WillIgnoreSSLCertificate.GetValueOrDefault() ?? false;
                }

                if (errors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch))
                {
                    result &= connection?.WillIgnoreSSLHostname.GetValueOrDefault() ?? false;
                }

                if (errors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable))
                {
                    result = false;
                }

                return result;
            }
        };
    }
}
