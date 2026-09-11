using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin.ManagementUjian;
using cbt.entity.Models;
using MediatR;

namespace cbt.be.Models.RequestModels.Admin.ManagementUjian
{
    public class UpdateStatusPacketUjianRequest : IRequest<MainResponse<bool>>
    {
        public Guid PacketId { get; set; }
        public ExamStatus Status { get; set; }
    }
}
