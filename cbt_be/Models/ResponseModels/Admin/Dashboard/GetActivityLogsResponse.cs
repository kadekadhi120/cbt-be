namespace cbt.be.Models.ResponseModels.Admin.Dashboard
{
    public class GetActivityLogsResponse
    {
        public List<ActivityLogDto> Data { get; set; } = new List<ActivityLogDto>();
        //public int TotalPages { get; set; }
        //public int CurrentPage { get; set; }
        //public int UnreadCount { get; set; }
    }

    public class ActivityLogDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public DateTime OccuratedAt { get; set; }
    }
}
