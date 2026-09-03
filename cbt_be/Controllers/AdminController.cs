using cbt.be.Models.RequestModels.Admin;
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
        public async Task<IActionResult> GetActivityLogs([FromQuery] GetActivityLogsRequest request)
        {
            var response = await _mediator.Send(request);

            return Ok(response);
        }

        [HttpGet("GetListPacketUjian")]
        public async Task<IActionResult> GetListPacketUjian([FromQuery] GetListPacketUjianRequset request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("GetDataDashboard")]
        public async Task<IActionResult> GetDataDashboard([FromQuery] GetDataDashboardRequest requset)
        {
            var response = await _mediator.Send(requset);
            return Ok(response);
        }

        [HttpGet("GetMaintanceStatus")]
        public async Task<IActionResult> GetMaintanceStatus([FromQuery] GetMaintanceStatusRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);

        }


    }
}
