namespace cbt.be.Models.ResponseModels.Admin.ManagementSiswa
{
    public class GetListDataSiswaResponse
    {
        public List<GetListDataSiswaDto> ListDataSiswa { get; set; } = new List<GetListDataSiswaDto>();
    }

    public class GetListDataSiswaDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Class { get; set; }
        public string Status { get; set; }

    }
}
