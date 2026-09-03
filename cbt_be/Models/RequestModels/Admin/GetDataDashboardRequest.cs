using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin;
using MediatR;
using System.Security.Cryptography.X509Certificates;

namespace cbt.be.Models.RequestModels.Admin
{
    public class GetDataDashboardRequest : IRequest<MainResponse<GetDataDashboardResponse>>
    {
        
    }
}
