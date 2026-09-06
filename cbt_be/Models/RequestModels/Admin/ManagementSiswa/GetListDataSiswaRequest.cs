using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin.ManagementSiswa;
using MediatR;

namespace cbt.be.Models.RequestModels.Admin.ManagementSiswa
{
    public class GetListDataSiswaRequest : IRequest<MainResponse<GetListDataSiswaResponse>>
    {

    }
}
