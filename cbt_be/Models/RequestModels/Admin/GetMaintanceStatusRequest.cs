using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin;
using MediatR;

namespace cbt.be.Models.RequestModels.Admin
{
    public class GetMaintanceStatusRequest : IRequest<MainResponse<GetMaintanceStatusResponse>>
    {
    }
}
