using cbt.be.Models.ResponseModels.Admin;
using MediatR;

namespace cbt.be.Models.RequestModels.Admin
{
    public class GetListPacketUjianRequset : IRequest<List<GetListPacketUjianResponse>>
    {
        public int limit { get; set; } = 5;
    }
}
