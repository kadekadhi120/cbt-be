using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin;
using MediatR;

namespace cbt.be.Models.RequestModels.Admin
{
    public class GetListPacketUjianRequset : IRequest<MainResponse<GetListPacketUjianResponse>>
    {
        public int limit { get; set; } = 5;
    }
}
