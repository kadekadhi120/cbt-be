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
                var listClass = from u in _db.Users
                                where u.Role == UserRole.student
                                join c in _db.Classes on u.Class equals c.KodeClass
                                join p in _db.PackageClasses on u.Class equals p.KodeClass
                                select new GetListDataClassDto
                                {
                                    CodeClass = c.KodeClass,
                                    ClassName = c.ClassName,
                                    Siswa_Ammount = _db.Users.Count(x => x.Class == c.KodeClass && x.Role == UserRole.student),
                                    Packet_Ammount = _db.PackageClasses.Count(x => x.KodeClass == c.KodeClass)
                                };
                
                if(listClass == null)
                {
                    return new MainResponse<GetListDataClassResponse>
                    {
                        Status = 404,
                        IsSuccess = false,
                        Message = "Data Class not found",
                        Data = null
                    };
                }

                var dataClass = await listClass.ToListAsync();

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
            catch
            {
                return new MainResponse<GetListDataClassResponse>
                {
                    Status = 500,
                    IsSuccess = false,
                    Message = "Internal Server Error",
                    Data = null
                };
            }

        }
    }
}
