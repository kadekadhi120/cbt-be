using cbt.be.Models.RequestModels.Admin.Dashboard;
using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin.Dashboard;
using cbt.entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace cbt.be.RequestHandler.Admin.Dashboard
{
    public class GetListPacketUjianHandler : IRequestHandler<GetListPacketUjianRequset, MainResponse<GetListPacketUjianResponse>>
    {
        public readonly AppDbContext _db;

        public GetListPacketUjianHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<MainResponse<GetListPacketUjianResponse>> Handle(GetListPacketUjianRequset request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _db.ExamPackages
                    .AsNoTracking()
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(request.limit)
                    .Select(x => new GetlistPacketUjianDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Status = x.Status.ToString(),
                        Participant = x.ParticipantCount,
                        Duration = x.DurationMinutes,
                        Question_ammount = x.QuestionCount
                    }).ToListAsync(cancellationToken);

                return new MainResponse<GetListPacketUjianResponse>
                {
                    Status = 200,
                    IsSuccess = true,
                    Message = "Success",
                    Data = new GetListPacketUjianResponse
                    {
                        Data = data
                    }
                };
            }
            catch (Exception ex)
            {
                return new MainResponse<GetListPacketUjianResponse>
                {
                    Status = 500,
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
            
    

