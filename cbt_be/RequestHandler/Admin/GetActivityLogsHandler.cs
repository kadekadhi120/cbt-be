using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using cbt.entity.Models;
using cbt.be.Models.RequestModels.Admin;
using cbt.be.Models.ResponseModels.Admin;
using Microsoft.IdentityModel.Tokens;
using cbt.entity;

namespace cbt.be.RequestHandler.Admin
{
    public class GetActivityLogsHandler : IRequestHandler<GetActivityLogsRequest, List<GetActivityLogsResponse>>
    {
        private readonly AppDbContext _context;

        public GetActivityLogsHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<GetActivityLogsResponse>> Handle(GetActivityLogsRequest request, CancellationToken cancellationToken)
        {
            var data = await _context.ActivityLogs
                .AsNoTracking()
                .OrderByDescending(x => x.OccurredAt)
                .Take(request.limit)
                .Select(x => new GetActivityLogsResponse
                {
                    Data = new List<ActivityLogDto>
                    {
                        new ActivityLogDto
                        {
                            Id = x.Id,
                            Type = x.Type.ToString(),
                            Message = x.Message,
                            OccuratedAt = x.OccurredAt
                        }
                    }
                })
                .ToListAsync(cancellationToken);

            return data;
        }
    }
}
