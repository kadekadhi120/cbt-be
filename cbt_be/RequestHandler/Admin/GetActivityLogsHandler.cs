using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using cbt.entity.Models;
using cbt.be.Models.RequestModels.Admin;
using cbt.be.Models.ResponseModels.Admin;
using Microsoft.IdentityModel.Tokens;
using cbt.entity;
using cbt.be.Models.ResponseModels;

namespace cbt.be.RequestHandler.Admin
{
    public class GetActivityLogsHandler : IRequestHandler<GetActivityLogsRequest, MainResponse<GetActivityLogsResponse>>
    {
        private readonly AppDbContext _db;

        public GetActivityLogsHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<MainResponse<GetActivityLogsResponse>> Handle(GetActivityLogsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var activityLogs = await _db.ActivityLogs
                    .OrderByDescending(a => a.OccurredAt)
                    .Take(request.limit)
                    .Select(a => new ActivityLogDto
                    {
                        Id = a.Id,
                        Type = a.Type.ToString(),
                        Message = a.Message,
                        OccuratedAt = a.OccurredAt
                    }).ToListAsync(cancellationToken);

                return new MainResponse<GetActivityLogsResponse>
                {
                    Status = 200,
                    IsSuccess = true,
                    Message = "Activity logs retrieved successfully.",
                    Data = new GetActivityLogsResponse
                    {
                        Data = activityLogs
                    }
                };
            }
            catch (Exception ex)
            {
                return new MainResponse<GetActivityLogsResponse>
                {
                    Status = 500,
                    IsSuccess = false,
                    Message = $"An error occurred while retrieving activity logs: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}