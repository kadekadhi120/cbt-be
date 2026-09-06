using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin.RekapNilai;
using MediatR;

namespace cbt.be.Models.RequestModels.Admin.RekapNilai
{
    public class GetListDataClassRequest : IRequest<MainResponse<GetListDataClassResponse>>
    {

    }
}
