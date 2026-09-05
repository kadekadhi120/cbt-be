using cbt.be.Models.RequestModels.Admin.ManagementUjian;
using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin.ManagementUjian;
using cbt.entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace cbt.be.RequestHandler.Admin.ManajementUjian
{
    public class GetDetailListPacketUjianHandler : IRequestHandler<GetDetailListPacketUjianRequest, MainResponse<GetDetailListPacketUjianResponse>>
    {
        public readonly AppDbContext _db;

        public GetDetailListPacketUjianHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<MainResponse<GetDetailListPacketUjianResponse>> Handle(GetDetailListPacketUjianRequest request, CancellationToken cancellationToken)
        {

            try
            {
                var data = await _db.ExamPackages
                    .AsNoTracking()
                    .Select(x => new GetDetailListPacketUjianDto
                     {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        Duration_Minute = x.DurationMinutes,
                        Status = x.Status.ToString(),
                        Participant = x.ParticipantCount.ToString(),

                    }).ToListAsync(cancellationToken);

                if(data == null || data.Count == 0)
                {
                    return new MainResponse<GetDetailListPacketUjianResponse>
                    {
                        Status = 404,
                        IsSuccess = false,
                        Message = "Data not found",
                        Data = null
                    };
                }

                return new MainResponse<GetDetailListPacketUjianResponse>
                {
                    Status = 200,
                    IsSuccess = true,
                    Message = "Success",
                    Data = new GetDetailListPacketUjianResponse
                    {
                        Data = data
                    }
                };
            }
            catch (Exception ex)
            {
                return new MainResponse<GetDetailListPacketUjianResponse>
                {
                    Status = 500,
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }


    }
}
