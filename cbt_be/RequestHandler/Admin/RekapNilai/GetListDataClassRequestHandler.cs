using cbt.be.Models.RequestModels.Admin.RekapNilai;
using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin.RekapNilai;
using cbt.entity;
using cbt.entity.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace cbt.be.RequestHandler.Admin.RekapNilai
{
    public class GetListDataClassRequestHandler : IRequestHandler<GetListDataClassRequest, MainResponse<GetListDataClassResponse>>
    {
        public readonly AppDbContext _db;

        public GetListDataClassRequestHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<MainResponse<GetListDataClassResponse>> Handle(GetListDataClassRequest request, CancellationToken cancellationToken)
        {
            try
            {

                var dataClass = await _db.Classes
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Where(c => _db.Users.Any(u => u.Class == c.KodeClass && u.Role == UserRole.student))
                    .Select(c => new GetListDataClassDto
                    {
                        CodeClass = c.KodeClass,
                        ClassName = c.ClassName,


                        Siswa_Ammount = _db.Users.Count(x => x.Class == c.KodeClass && x.Role == UserRole.student),
                        Packet_Ammount = _db.PackageClasses.Count(x => x.KodeClass == c.KodeClass),

                        Packets = (from pc in _db.PackageClasses
                                   where pc.KodeClass == c.KodeClass
                                   join ep in _db.ExamPackages on pc.ExamPackageId equals ep.Id
                                   select new GetListPacketdto
                                   {
                                       Id = ep.Id,
                                       Title = ep.Title,
                                       Description = ep.Description,
                                       Status = ep.Status.ToString(),
                                       Duration_Minute = ep.DurationMinutes,
                                       Question_Ammount = ep.QuestionCount,
                                       Participant_Ammount = _db.ExamAttempts.Count(x => x.ExamPackageId == ep.Id)
                                   }).ToList()
                    })
                    .ToListAsync(cancellationToken); 

                if (dataClass == null || dataClass.Count == 0)
                {
                    return new MainResponse<GetListDataClassResponse>
                    {
                        Status = 404,
                        IsSuccess = false,
                        Message = "Data Class not found",
                        Data = null
                    };
                }

                return new MainResponse<GetListDataClassResponse>
                {
                    Status = 200,
                    IsSuccess = true,
                    Message = "Data Class found",
                    Data = new GetListDataClassResponse
                    {
                        ListDataClass = dataClass
                    }
                };
            }
            catch(Exception ex)
            {
                string realError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                return new MainResponse<GetListDataClassResponse>
                {
                    Status = 500,
                    IsSuccess = false,
                    Message = $"Gagal di DB: {realError}",
                    Data = null
                };
            }

        }
    }
}
