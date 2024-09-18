using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using SwissPension.Trial.Common.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SwissPension.Trial.Common;

[SuppressMessage("Usage", "CA2253:Named placeholders should not be numeric values")]
[SuppressMessage("Performance", "CA1869:Cache and reuse \'JsonSerializerOptions\' instances")]
public static class WebApplication
{
    private const string CorsPolicyName = "ClientPolicy";
    private const string HttpClientName = "CustomHttpClient";

    public static (WebApplicationBuilder builder, string corsPolicyName) CreateGrpcService(string certificateSettingsEnvironmentVariable, string apiPortEnvironmentVariable, bool isPublic = false)
    {
        var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder();

        var certSettingsJson = Environment.GetEnvironmentVariable(certificateSettingsEnvironmentVariable) ?? throw new InvalidOperationException($"{certificateSettingsEnvironmentVariable} environment variable not set");

        // Get the certificate settings from the environment variable
        var certSettings = JsonSerializer.Deserialize<CertificateSettings>(certSettingsJson, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })!;

        // Get the API port from the environment variable
        var apiPort = int.Parse(Environment.GetEnvironmentVariable(apiPortEnvironmentVariable) ?? throw new InvalidOperationException($"{apiPortEnvironmentVariable} environment variable not set"));

        // Configure the Kestrel server with the certificate and the API port
        builder.WebHost.ConfigureKestrel(options => options.ListenLocalhost(apiPort, listenOptions =>
        {
            listenOptions.UseHttps(new X509Certificate2(Path.Combine("..", "..", $"{certSettings.Path}.pfx"), certSettings.Password));
            // Enable HTTP/2 and HTTP/1.1 for gRPC-Web compatibility
            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        }));

        // Allow all origins
        builder.Services.AddCors(o => o.AddPolicy(CorsPolicyName, policyBuilder =>
        {
            policyBuilder
                // Allow all methods and headers
                .AllowAnyMethod()
                .AllowAnyHeader();

            // Only allow localhost if gRPC-Web is not enabled (microservice communication)
            if (!isPublic)
            {
                policyBuilder
                    // Allow all ports on localhost
                    .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");

                return;
            }

            policyBuilder
                // Allow all origins
                .AllowAnyOrigin()
                // Expose the gRPC-Web headers
                .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
        }));

        builder.Services.AddGrpc();

        builder.Services.AddSingleton(certSettings);

        // Add custom HttpClient with the certificate handler to talk to the gRPC services
        // - The certificate is loaded from the environment variable and does not need to be installed on the machine
        // - First we need to create a factory for the HttpClient with the custom handler
        builder.Services.AddHttpClient(HttpClientName).ConfigurePrimaryHttpMessageHandler(serviceProvider =>
        {
            var certificateSettings = serviceProvider.GetRequiredService<CertificateSettings>();
            var logger = serviceProvider.GetRequiredService<ILogger<GeneralLogContext>>();

            // Load the certificate from the environment variable
            var certificate = new X509Certificate2(Path.Combine("..", "..", $"{certificateSettings.Path}.crt"));

            // Expected thumbprint and issuer of the certificate for validation
            var expectedThumbprint = certificate.Thumbprint;
            var expectedIssuer = certificate.Issuer;

            logger.LogInformation("Creating custom HttpClient with certificate handler for {0}", expectedIssuer);

            // Create the gRPC channels and clients with the custom certificate handler
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(certificate);

            handler.ServerCertificateCustomValidationCallback = (_, cert, _, _) =>
                cert?.Issuer == expectedIssuer && cert.Thumbprint == expectedThumbprint;

            return handler;
        });

        // Add the custom HttpClient to the service provider
        // - The HttpClient is created using the factory with the custom handler
        builder.Services.AddTransient(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var logger = serviceProvider.GetRequiredService<ILogger<GeneralLogContext>>();

            logger.LogInformation("Creating custom HttpClient with certificate handler");

            return httpClientFactory.CreateClient(HttpClientName);
        });

        return (builder, CorsPolicyName);
    }

    // Dummy class for logging
    private abstract class GeneralLogContext;
}