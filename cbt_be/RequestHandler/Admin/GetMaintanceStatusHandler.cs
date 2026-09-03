using cbt.be.Models.RequestModels.Admin;
using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin;
using cbt.entity;
using MediatR;

namespace cbt.be.RequestHandler.Admin
{
    public class GetMaintanceStatusHandler : IRequestHandler<GetMaintanceStatusRequest, MainResponse<GetMaintanceStatusResponse>>
    {
        public readonly AppDbContext _db;

        public GetMaintanceStatusHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<MainResponse<GetMaintanceStatusResponse>> Handle(GetMaintanceStatusRequest request, CancellationToken cancellationToken)
        {
            var response = new MainResponse<GetMaintanceStatusResponse>();
            try
            {
                var maintanceStatus = _db.AppSettings.FirstOrDefault();
                if (maintanceStatus == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Maintenance status not found.";
                    return response;
                }

                response.Status = 200;
                response.IsSuccess = true;
                response.Message = "Maintenance status retrieved successfully.";

                response.Data = new GetMaintanceStatusResponse
                {
                    IsMaintanceMode = maintanceStatus.MaintenanceMode
                };
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred while retrieving maintenance status: {ex.Message}";
            }
            return response;
        }
    }
}
