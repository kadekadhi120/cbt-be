using cbt.be.Models.RequestModels.Admin.ManagementUjian;
using cbt.be.Models.ResponseModels;
using cbt.entity;
using cbt.entity.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace cbt.be.RequestHandler.Admin.ManagementUjian
{
    public class CreatePacketUjianRequestHandler : IRequestHandler<CreatePacketUjianRequest, MainResponse<bool>>
    {
        public readonly AppDbContext _db;
        public readonly IHttpContextAccessor _httpContextAccessor;

        public CreatePacketUjianRequestHandler(AppDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MainResponse<bool>> Handle(CreatePacketUjianRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = _httpContextAccessor.HttpContext
                    .User
                    .Identity?
                    .Name ?? "SYSTEM";

                var newGuidPacketId = Guid.NewGuid();


                var PacketClass = request.ClassCode.Select(code => new PackageClass
                {
                    Id = newGuidPacketId,
                    ExamPackageId = newGuidPacketId,
                    KodeClass = code
                });


                var newPacket = new ExamPackage
                {
                    Id = newGuidPacketId,
                    Title = request.Title,
                    Description = request.Description,
                    DurationMinutes = request.DurationMinutes,
                    Status = ExamStatus.draft,
                    CreatedBy = user,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _db.ExamPackages.AddAsync(newPacket, cancellationToken);
                await _db.PackageClasses.AddRangeAsync(PacketClass, cancellationToken);

                await _db.SaveChangesAsync(cancellationToken);

                return new MainResponse<bool>
                {
                    Status = 200,
                    IsSuccess = true,
                    Message = "Success"
                };

            }
            catch (Exception ex)
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
