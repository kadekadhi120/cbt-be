namespace cbt.be.Models.ResponseModels.Admin
{
    public class GetListPacketUjianResponse
    {
        public List<GetlistPacketUjianDto> Data { get; set; } = new List<GetlistPacketUjianDto>();
    }

    public class GetlistPacketUjianDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public int Participant { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
