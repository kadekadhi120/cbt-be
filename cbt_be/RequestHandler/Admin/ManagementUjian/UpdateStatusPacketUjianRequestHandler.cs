using cbt.be.Models.RequestModels.Admin.ManagementUjian;
using cbt.be.Models.ResponseModels;
using cbt.be.Models.ResponseModels.Admin.ManagementUjian;
using cbt.entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace cbt.be.RequestHandler.Admin.ManagementUjian
{
    public class UpdateStatusPacketUjianRequestHandler : IRequestHandler<UpdateStatusPacketUjianRequest, MainResponse<bool>>
    {
        public readonly AppDbContext _db;

        public UpdateStatusPacketUjianRequestHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<MainResponse<bool>> Handle(UpdateStatusPacketUjianRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if(request == null)
                {
                    return new MainResponse<bool>
                    {
                        Status = 404,
                        IsSuccess = false,
                        Message = "Request Not Found",
                        Data = false
                    };
                }

                var currentPacket = await _db.ExamPackages
                    .FirstOrDefaultAsync(ep => ep.Id == request.PacketId);

                currentPacket.Status = request.Status;
                currentPacket.UpdatedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return new MainResponse<bool>
                {
                    Status = 200,
                    IsSuccess = true,
                    Message = "Succes",
                    Data = true
                };
                 
            }catch (Exception ex)
            {
                return new MainResponse<bool>
                {
                    Status = 500,
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = false
                };
            }
                

        }
    }
}
