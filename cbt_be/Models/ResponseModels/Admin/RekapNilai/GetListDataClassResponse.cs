namespace cbt.be.Models.ResponseModels.Admin.RekapNilai
{
    public class GetListDataClassResponse
    {
        public new List<GetListDataClassDto> ListDataClass { get; set; }
       
    }

    public class  GetListDataClassDto
    {
        public string CodeClass { get; set; }
        public string ClassName { get; set; }
        public int Siswa_Ammount { get; set; }
        public int Packet_Ammount { get; set; }
        public new IEnumerable<GetListPacketdto> Packets { get; set; }
    }

    public class GetListPacketdto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration_Minute { get; set; }
        public string Status { get; set; }
        public int Question_Ammount { get; set; }
        public int Participant_Ammount { get; set; }
    }
}
