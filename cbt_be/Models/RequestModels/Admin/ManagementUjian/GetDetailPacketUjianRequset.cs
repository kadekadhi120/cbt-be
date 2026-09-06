using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin.ManagementUjian;
using MediatR;

namespace cbt.be.Models.RequestModels.Admin.ManagementUjian
{
    public class GetDetailPacketUjianRequset : IRequest<MainResponse<GetDetailPacketUjianResponse>>
    {
        public Guid PackagesId { get; set; }
    }
}
