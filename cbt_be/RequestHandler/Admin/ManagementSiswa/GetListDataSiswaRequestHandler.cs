using cbt.be.Models.RequestModels.Admin.ManagementSiswa;
using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin.ManagementSiswa;
using cbt.entity;
using cbt.entity.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace cbt.be.RequestHandler.Admin.ManagementSiswa
{
    public class GetListDataSiswaRequestHandler : IRequestHandler<GetListDataSiswaRequest, MainResponse<GetListDataSiswaResponse>>
    {
        public readonly AppDbContext _db;

        public GetListDataSiswaRequestHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<MainResponse<GetListDataSiswaResponse>> Handle(GetListDataSiswaRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var querry = from u in _db.Users.AsNoTracking()
                                where u.Role == UserRole.student
                                join c in _db.Classes.AsNoTracking() on u.Class equals c.KodeClass
                                select new GetListDataSiswaDto
                                {
                                    Id = u.Id,
                                    Name = u.Name,
                                    Email = u.Email,
                                    Class = c.ClassName,
                                    Status = u.Status.ToString()
                                };

                var dataSiswa = await querry.ToListAsync(cancellationToken);

                if (dataSiswa == null)
                {
                    return new MainResponse<GetListDataSiswaResponse>
                    {
                        Status = 404,
                        IsSuccess = false,
                        Message = "No student data found.",
                        Data = null
                    };
                }

                return new MainResponse<GetListDataSiswaResponse>
                {
                    Status = 200,
                    IsSuccess = true,
                    Message = "Success",
                    Data = new GetListDataSiswaResponse
                    {
                        ListDataSiswa = dataSiswa
                    }
                };

            }
            catch (Exception ex)
            {
                return new MainResponse<GetListDataSiswaResponse>
                {
                    Status = 500,
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
