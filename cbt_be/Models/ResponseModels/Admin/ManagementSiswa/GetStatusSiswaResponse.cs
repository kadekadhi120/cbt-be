using cbt.entity.Models;
using MediatR;

namespace cbt.be.Models.ResponseModels.Admin.ManagementSiswa
{
    public class GetStatusSiswaResponse
    {
        public List<string> Status_Siswa { get; set; }
    }


}
