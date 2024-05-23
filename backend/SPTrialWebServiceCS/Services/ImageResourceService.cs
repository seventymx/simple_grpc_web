using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SwissPension.Trial.Common;

namespace SwissPension.Trial.Tree.Services;

public class ImageResourceService : ImageResource.ImageResourceBase
{
    public override Task<ImageResponse> GetImageByName(ImageRequest request, ServerCallContext context)
    {
        if (!System.Enum.TryParse(request.Name, out ResourceAccessor.ResourceName name))
            throw new RpcException(new(StatusCode.InvalidArgument, "The requested image does not exist"));

        var image = name.GetSvg();

        return Task.FromResult(new ImageResponse { Image = image });
    }
}