using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin.Dashboard;
using MediatR;
using System.Security.Cryptography.X509Certificates;

namespace cbt.be.Models.RequestModels.Admin.Dashboard
{
    public class GetDataDashboardRequest : IRequest<MainResponse<GetDataDashboardResponse>>
    {
        
    }
}
