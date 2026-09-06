namespace cbt.be.Models.ResponseModels.Admin.ManagementUjian
{
    public class GetDetailPacketUjianResponse
    {
        public Guid Id { get; set; }
        public int DurationTime { get; set; }
        public int Participant_Ammount {  get; set; }
        public int Question_Ammount { get; set; }
        public List<string> Class { get; set; }
        public List<GetDetailQuestion> ListData { get; set; } = new List<GetDetailQuestion>();

    }

    public class GetDetailQuestion
    {
        public string Question { get; set; }
        public List<GetDetailPacketUjianDto> Question_Options { get; set; } = new List<GetDetailPacketUjianDto>();
    }



    public class GetDetailPacketUjianDto
    {
        public string Question_Option { get; set; }
        public string Label { get; set; }
        public bool isCorrect { get; set; }
    }
}
