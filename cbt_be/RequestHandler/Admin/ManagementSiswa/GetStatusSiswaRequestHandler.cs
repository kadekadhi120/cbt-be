using cbt.be.Models.RequestModels.Admin.ManagementSiswa;
using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin.ManagementSiswa;
using cbt.entity;
using cbt.entity.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace cbt.be.RequestHandler.Admin.ManagementSiswa
{
    public class GetStatusSiswaRequestHandler : IRequestHandler<GetStatusSiswaRequest, MainResponse<GetStatusSiswaResponse>>
    {
        public readonly AppDbContext _db;

        public GetStatusSiswaRequestHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<MainResponse<GetStatusSiswaResponse>> Handle(GetStatusSiswaRequest request, CancellationToken cancellationToken)
        {

            try
            {
                var daftarStatus = Enum.GetNames(typeof(UserStatus)).ToList();
                

                if (daftarStatus == null || daftarStatus.Count == 0)
                {
                    return new MainResponse<GetStatusSiswaResponse>
                    {
                        Status = 404,
                        IsSuccess = false,
                        Message = "Data status siswa tidak ditemukan",
                        Data = null
                    };
                }

                return new MainResponse<GetStatusSiswaResponse>
                {
                    Status = 200,
                    IsSuccess = true,
                    Message = "Data status siswa berhasil ditemukan",
                    Data = new GetStatusSiswaResponse
                    {
                        Status_Siswa = daftarStatus
                    }
                };
            }
            catch (Exception ex)
            {
               return new MainResponse<GetStatusSiswaResponse>
               {
                   Status = 500,
                   IsSuccess = false,
                   Message = $"Terjadi kesalahan: {ex.Message}",
                   Data = null
               };
            }


        }
    }
}