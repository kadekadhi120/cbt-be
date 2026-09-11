using cbt.be.Models.RequestModels.Admin.ManagementUjian;
using cbt.be.Models.ResponseModels;
using cbt.entity;
using cbt.entity.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace cbt.be.RequestHandler.Admin.ManagementUjian
{
    public class UpdateClassPacketUjianRequestHandler : IRequestHandler<UpdateClassPacketUjianRequest, MainResponse<bool>>
    {
        public readonly AppDbContext _db;

        public UpdateClassPacketUjianRequestHandler (AppDbContext db)
        {
            _db = db;
        }

        public async Task<MainResponse<bool>> Handle(UpdateClassPacketUjianRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var rawPacketClass = await _db.PackageClasses
                     .AsNoTracking()
                     .Where(pc => pc.ExamPackageId == request.PacketId)
                     .ToListAsync(cancellationToken);

                var existingClass = rawPacketClass.Select(ep => ep.KodeClass)
                    .ToList();

                var classToRemove = rawPacketClass
                    .Where(pc => !request.ClassCode.Contains(pc.KodeClass));

                if(classToRemove.Any())
                {
                    _db.PackageClasses.RemoveRange(classToRemove);
                }

                var classToAdd = request.ClassCode
                    .Where(cc => !existingClass.Contains(cc))
                    .ToList();

                if (classToAdd.Any())
                {
                    var newRecord = classToAdd.Select(kc => new PackageClass
                    {
                        ExamPackageId = request.PacketId,
                        KodeClass = kc
                    });
                    await _db.PackageClasses.AddRangeAsync(newRecord, cancellationToken);
                }

                await _db.SaveChangesAsync(cancellationToken);

                return new MainResponse<bool>
                {
                    Status = 200,
                    IsSuccess = true,
                    Message = "Succes",
                    Data = true

                };
            }
            catch (Exception ex)
            {
                return new MainResponse<bool>
                {
                    Status = 500,
                    IsSuccess= false,
                    Message = ex.Message,
                    Data = false
                };
            }
        }
    }
}
