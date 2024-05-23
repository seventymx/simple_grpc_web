using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using SwissPension.Trial.Tree.Extensions;
using SwissPension.Trial.Tree.Model;

namespace SwissPension.Trial.Tree;

// This class exists independently to enable WebService to be a partial, overridable, or generic class.
public class WebService
{
    public virtual void StartApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // TODO: Configure with a real certificate for production
        // TODO: Store the certificate password in a secure location
        var certSettings = builder.Configuration.GetSection("Certificate").Get<CertificateSettings>()!;
        builder.WebHost.ConfigureKestrel(options => options.ListenLocalhost(5001, listenOptions =>
        {
            listenOptions.UseHttps(new X509Certificate2(certSettings.Path, certSettings.Password));
            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        }));

        builder.Services.AddGrpc();

        // TODO: Add the client origins to the configuration file to set up CORS
        var clients = builder.Configuration.GetSection("Clients").Get<string[]>()!;
        builder.Services.AddCors(o => o.AddPolicy("ClientPolicy", policyBuilder =>
        {
            policyBuilder.WithOrigins(clients)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
        }));

        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseHttpsRedirection();

        app.UseGrpcWeb(new() { DefaultEnabled = true });

        app.UseCors("ClientPolicy");

        // TODO: Add Authentication and Authorization middleware

        app.MapGrpcServices();

        app.MapControllers();

        app.Run();
    }
}