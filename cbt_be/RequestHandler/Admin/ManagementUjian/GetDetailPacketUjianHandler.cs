using cbt.be.Models.RequestModels.Admin.ManagementUjian;
using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin.ManagementUjian;
using cbt.entity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace cbt.be.RequestHandler.Admin.ManagementUjian
{
    public class GetDetailPacketUjianHandler : IRequestHandler<GetDetailPacketUjianRequset, MainResponse<GetDetailPacketUjianResponse>>
    {
        public readonly AppDbContext _db;

        public GetDetailPacketUjianHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<MainResponse<GetDetailPacketUjianResponse>> Handle(GetDetailPacketUjianRequset request, CancellationToken cancellationToken)
        {

            if (request.PackagesId == Guid.Empty)
            {
                
            }

            try
            {
                var baseData = await _db.ExamPackages
                    .Where(ep => ep.Id == request.PackagesId)
                    .Select(x => new
                    {
                        x.DurationMinutes,
                        x.QuestionCount,
                        x.ParticipantCount,
                    }
                    ).FirstOrDefaultAsync();

                if (baseData == null)
                {
                    return new MainResponse<GetDetailPacketUjianResponse>
                    {
                        Status = 404,
                        IsSuccess = false,
                        Message = "Data Not Found",
                        Data = null
                    };
                }

                var accsesPacket = await _db.PackageClasses
                    .AsNoTracking()
                    .Where(pc => pc.ExamPackageId == request.PackagesId)
                    .Select(pc => pc.KodeClass)
                    .ToListAsync(cancellationToken);



                var rawQuestions = await (from qs in _db.Questions.AsNoTracking()
                                          where qs.ExamPackageId == request.PackagesId
                                          join qo in _db.QuestionOptions.AsNoTracking() on qs.Id equals qo.QuestionId
                                          select new
                                          {
                                              qs.Id,
                                              qs.QuestionText,
                                              qo.OptionText,
                                              qo.Label,
                                              qo.IsCorrect
                                          })
                                          .ToListAsync(cancellationToken);

                var questionDataList = rawQuestions
                    .GroupBy(q => new { q.Id, q.QuestionText })
                    .Select(g => new GetDetailQuestion
                    {
                        Question = g.Key.QuestionText,
                        Question_Options = g.Select(o => new GetDetailPacketUjianDto
                        {
                            Question_Option = o.OptionText,
                            Label = o.Label.ToString(),
                            isCorrect = o.IsCorrect
                        }).ToList()
                    })
                    .ToList();

                var responseData = new GetDetailPacketUjianResponse
                {
                    Id = request.PackagesId,
                    DurationTime = baseData.DurationMinutes,
                    Question_Ammount = baseData.QuestionCount,
                    Participant_Ammount = baseData.ParticipantCount,
                    Class = accsesPacket,
                    ListData = questionDataList 
                };


                return new MainResponse<GetDetailPacketUjianResponse>
                {  
                    Status = 200,
                    IsSuccess = true,
                    Message = "Success",
                    Data = responseData
                };

            }
            catch (Exception ex)
            {
                return new MainResponse<GetDetailPacketUjianResponse>
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
