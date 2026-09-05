using MediatR;

namespace cbt.be.Models.ResponseModels.Admin.Dashboard
{
    public class GetMaintanceStatusResponse
    {
        public bool IsMaintanceMode { get; set; }
    }
}
