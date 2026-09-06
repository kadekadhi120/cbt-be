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
    }
}
