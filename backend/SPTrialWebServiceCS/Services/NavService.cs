using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace SwissPension.Trial.Tree.Services;

public class NavService : Nav.NavBase
{
    public override Task<HelloWorldResponse> SayHelloWorld(Empty request, ServerCallContext context) => Task.FromResult(new HelloWorldResponse { Message = "Hello, World!" });

    public override Task GetStiftungenStream(Empty request, IServerStreamWriter<NavData> responseStream, ServerCallContext context) => throw new NotImplementedException();

    public override Task<SetResponse> SetStiftung(SetRequest request, ServerCallContext context) => throw new NotImplementedException();

    // TODO: Add missing members
}