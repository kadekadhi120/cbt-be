using cbt.be.Models.ResponseModels;
using MediatR;

namespace cbt.be.Models.RequestModels.Admin.ManagementUjian
{
    public class UpdateClassPacketUjianRequest : IRequest<MainResponse<bool>>
    {
        public Guid PacketId {  get; set; }
        public List<string> ClassCode {  get; set; }
    }
}
