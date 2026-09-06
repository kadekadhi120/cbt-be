using cbt.be.Models.RequestModels.Admin.Dashboard;
using cbt.be.Models.RequestModels.Admin.ManagementSiswa;
using cbt.be.Models.RequestModels.Admin.ManagementUjian;
using cbt.be.Models.RequestModels.Admin.RekapNilai;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace cbt.be.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("GetAvtivityLogs")]
        [Tags("Admin Dashboard")]
        public async Task<IActionResult> GetActivityLogs([FromQuery] GetActivityLogsRequest request)
        {
            var response = await _mediator.Send(request);

            return Ok(response);
        }

        [HttpGet("GetListPacketUjian")]
        [Tags("Admin Dashboard")]
        public async Task<IActionResult> GetListPacketUjian([FromQuery] GetListPacketUjianRequset request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("GetDataDashboard")]
        [Tags("Admin Dashboard")]
        public async Task<IActionResult> GetDataDashboard([FromQuery] GetDataDashboardRequest requset)
        {
            var response = await _mediator.Send(requset);
            return Ok(response);
        }

        [HttpGet("GetMaintanceStatus")]
        [Tags("Admin Dashboard")]
        public async Task<IActionResult> GetMaintanceStatus([FromQuery] GetMaintanceStatusRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("ManagementUjian/GetDetailListPacketUjian")]
        [Tags("Admin Management Ujian")]
        public async Task<IActionResult> GetDetailDataListPacketUjian([FromQuery] GetDetailListPacketUjianRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("ManagementUjian/GetDetailPacketUjian")]
        [Tags("Admin Management Ujian")]
        public async Task<IActionResult> GetDetailPacketUjian([FromQuery] GetDetailPacketUjianRequset requset)
        {
            var response = await _mediator.Send(requset);
            return Ok(response);
        }


        [HttpGet("ManagementSiswa/GetListDataSiswa")]
        [Tags("Admin Management Siswa")]
        public async Task<IActionResult> GetListDataSiswa([FromQuery] GetListDataSiswaRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("ManagementSiswa/GetStatusSiswa")]
        [Tags("Admin Management Siswa")]
        public async Task<IActionResult> GetStatusSiswa([FromQuery] GetStatusSiswaRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("ManagementSiswa/GetlistDataClass")]
        [Tags("Admin Rekap Nilai")]
        public async Task<IActionResult> GetListDataClass([FromQuery] GetListDataClassRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

    }
}
