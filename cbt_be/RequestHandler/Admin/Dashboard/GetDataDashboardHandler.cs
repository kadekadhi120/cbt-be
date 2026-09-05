using cbt.be.Models.RequestModels.Admin.Dashboard;
using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin.Dashboard;
using cbt.entity;
using cbt.entity.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace cbt.be.RequestHandler.Admin.Dashboard
{
    public class GetDataDashboardHandler : IRequestHandler<GetDataDashboardRequest, MainResponse<GetDataDashboardResponse>>
    {
        public readonly AppDbContext _db;

        public GetDataDashboardHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<MainResponse<GetDataDashboardResponse>> Handle(GetDataDashboardRequest request, CancellationToken cancellationToken)
        {


            try
            {
                var totalExamPackages = await _db.ExamPackages.CountAsync();

                var totalActiveExamPackages = await _db.ExamPackages
                    .Where(x => x.Status == ExamStatus.published)
                    .CountAsync();

                var totalStudents = await _db.Users
                    .Where(x => x.Role == UserRole.student)
                    .CountAsync();

                var totalExamAttempts = await _db.ExamAttempts
                    .Where(x => x.SubmittedAt == DateTime.UtcNow)
                    .CountAsync();

                if(totalExamAttempts == null)
                {
                    totalExamAttempts = 0;
                }

                return new MainResponse<GetDataDashboardResponse>
                {
                    Status = 200,
                    IsSuccess = true,
                    Message = "Success",
                    Data = new GetDataDashboardResponse
                    {
                        Total_Students = totalStudents,
                        Total_Exams = totalExamPackages,
                        Active_Exams = totalActiveExamPackages,
                        Exam_attempts = totalActiveExamPackages
                    }
                };
            }
            catch (Exception ex)
            {
                return new MainResponse<GetDataDashboardResponse>
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
