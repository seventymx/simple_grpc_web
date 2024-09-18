using SwissPension.Trial.Common.Extensions;
using SwissPension.Trial.Common.Model;
using Microsoft.EntityFrameworkCore;
using WebApplication = SwissPension.Trial.Common.WebApplication;

var (builder, corsPolicyName) = WebApplication.CreateGrpcService("CERTIFICATE_SETTINGS", "BACKEND_PORT", true);

var app = builder.Build();

// Configure the HTTP request pipeline.

// Enable the HTTPS redirection - only use HTTPS
app.UseHttpsRedirection();

// Enable CORS - allow all origins and add gRPC-Web headers
app.UseCors(corsPolicyName);

// Enable gRPC-Web for all services
app.UseGrpcWeb(new() { DefaultEnabled = true });

// Add all services in the Services namespace
app.MapGrpcServices();

app.Run();