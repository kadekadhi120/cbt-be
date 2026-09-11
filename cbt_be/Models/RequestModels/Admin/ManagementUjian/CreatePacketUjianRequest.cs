using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin.ManagementUjian;
using MediatR;
using System.Reflection;

namespace cbt.be.Models.RequestModels.Admin.ManagementUjian
{
    public class CreatePacketUjianRequest : IRequest<MainResponse<CreatePacketUjianResponse>>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public List<string> ClassCode { get; set; }
    }
}
