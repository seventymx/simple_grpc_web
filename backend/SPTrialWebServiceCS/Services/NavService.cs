using System.Data;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Data.SqlClient;
using SwissPension.Trial.Common;

namespace SwissPension.Trial.Tree.Services;

public class NavService : Nav.NavBase
{
    public override Task<HelloWorldResponse> SayHelloWorld(Empty request, ServerCallContext context) => Task.FromResult(new HelloWorldResponse { Message = "Hello, World!" });

    public override async Task GetStiftungenStream(Empty request, IServerStreamWriter<NavData> responseStream, ServerCallContext context)
    {
        //var stiftungen = new List<NavData>
        //{
        //    new NavData
        //    {
        //        Id = 1,
        //        Text = "Stiftung 1",
        //        ImageName = ResourceAccessor.ResourceName.stiftung.ToString()
        //    },
        //    new NavData
        //    {
        //        Id = 2,
        //        Text = "Stiftung 2",
        //        ImageName = ResourceAccessor.ResourceName.stiftung.ToString()
        //    },
        //    new NavData
        //    {
        //        Id = 3,
        //        Text = "Stiftung 3",
        //        ImageName = ResourceAccessor.ResourceName.stiftung.ToString()
        //    }
        //};
        var stiftungen = new List<NavData>();

        using (SqlConnection connection = new SqlConnection("Server=localhost;Database=SP7;Trusted_Connection=yes;Connection Timeout=60;Encrypt=False;"))
        {
            await connection.OpenAsync();
            using (SqlCommand command = new SqlCommand("SELECT STID AS ID, STName AS Text FROM Stiftungen WHERE STID > 0", connection))
            {
                using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                {
                    while (await reader.ReadAsync())
                    {
                        var navData = new NavData();

                        if (!(await reader.IsDBNullAsync(0)))
                        {
                            navData.Id = reader.GetInt32(0); // Assuming ID is an integer
                        }

                        if (!(await reader.IsDBNullAsync(1)))
                        {
                            navData.Text = reader.GetString(1); // Assuming Text is a string
                        }

                        stiftungen.Add(navData);
                    }
                }
            }
        }

        foreach (var stiftung in stiftungen)
        {
            await responseStream.WriteAsync(stiftung);
            await Task.Delay(1000);
        }
    }

    public override Task<SetResponse> SetStiftung(SetRequest request, ServerCallContext context) => throw new NotImplementedException();

    // TODO: Add missing members
}