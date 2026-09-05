namespace cbt.be.Models.ResponseModels.Admin.ManagementUjian
{
    public class GetDetailListPacketUjianResponse
    {
        public List<GetDetailListPacketUjianDto> Data { get; set; }
    }

    public class GetDetailListPacketUjianDto { 
    
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration_Minute { get; set; }
        public string Status { get; set; }
        public string Question_Ammount { get; set; }
        public string Participant { get; set; }

    }
}
