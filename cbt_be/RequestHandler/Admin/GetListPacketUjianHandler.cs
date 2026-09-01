using cbt.be.Models.RequestModels.Admin;
using cbt.be.Models.ResponseModels.Admin;
using cbt.entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace cbt.be.RequestHandler.Admin
{
    public class GetListPacketUjianHandler : IRequestHandler<GetListPacketUjianRequset, List<GetListPacketUjianResponse>>
    {
        public readonly AppDbContext _context;

        public GetListPacketUjianHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<GetListPacketUjianResponse>> Handle(GetListPacketUjianRequset request, CancellationToken cancellationToken)
        {
            var data = await _context.ExamPackages
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Take(request.limit)
                .Select(x => new GetListPacketUjianResponse
                {
                    Data = new List<GetlistPacketUjianDto>
                    {
                        new GetlistPacketUjianDto
                        {
                            Id = x.Id,
                            Title = x.Title,
                            Status = x.Status.ToString(),
                            Participant = x.ParticipantCount,
                            CreatedAt = x.CreatedAt,
                        }
                    }
                }).ToListAsync(cancellationToken);
            return data;
        }
    }
}
