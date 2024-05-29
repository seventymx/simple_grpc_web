using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SwissPension.Trial.Common;

namespace SwissPension.Trial.Tree.Services;

public class NavService : Nav.NavBase
{
    public override Task<HelloWorldResponse> SayHelloWorld(Empty request, ServerCallContext context) => Task.FromResult(new HelloWorldResponse { Message = "Hello, World!" });

    public override async Task GetStiftungenStream(Empty request, IServerStreamWriter<NavData> responseStream, ServerCallContext context)
    {
        var stiftungen = new List<NavData>
        {
            new NavData
            {
                Id = 1,
                Text = "Stiftung 1",
                ImageName = ResourceAccessor.ResourceName.stiftung.ToString()
            },
            new NavData
            {
                Id = 2,
                Text = "Stiftung 2",
                ImageName = ResourceAccessor.ResourceName.stiftung.ToString()
            },
            new NavData
            {
                Id = 3,
                Text = "Stiftung 3",
                ImageName = ResourceAccessor.ResourceName.stiftung.ToString()
            }
        };
        
        foreach (var stiftung in stiftungen)
        {
            await responseStream.WriteAsync(stiftung);
            await Task.Delay(1000);
        }
    }

    public override Task<SetResponse> SetStiftung(SetRequest request, ServerCallContext context) => throw new NotImplementedException();

    // TODO: Add missing members
}