using cbt.be.Models.ResponseModels.Admin;
using MediatR;

namespace cbt.be.Models.RequestModels.Admin
{
    public class GetActivityLogsRequest : IRequest<List<GetActivityLogsResponse>>
    {
      public int limit { get; set; } = 5;
    
    }
}
