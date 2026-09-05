using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin.Dashboard;
using MediatR;

namespace cbt.be.Models.RequestModels.Admin.Dashboard
{
    public class GetActivityLogsRequest : IRequest<MainResponse<GetActivityLogsResponse>>
    {
      public int limit { get; set; } = 5;
    }
}
