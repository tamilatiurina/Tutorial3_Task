using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial3_Task;
using static Tutorial3_Task.DeviceStorage;

namespace RESTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        // GET: api/device
        [HttpGet]
        public IActionResult GetAllDevices()
        {
            var devices = _manager
                .GetAllDevices()
                .Select(d => new { d.Id, d.Name, d.IsEnabled });

            return Ok(devices);
        }

        // GET: api/device/{id}
        [HttpGet("{id}")]
        public IActionResult GetDeviceById(string id)
        {
            var device = _manager.GetDeviceById(id);
            if (device == null)
                return NotFound($"Device with ID {id} not found.");

            return Ok(device);
        }

        // POST: api/device
        [HttpPost]
        public IActionResult CreateDevice([FromBody] Device device)
        {
            try
            {
               _manager.AddDevice(device);
                return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, device);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/device/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateDevice(string id, [FromBody] Device updatedDevice)
        {
            if (id != updatedDevice.Id)
                return BadRequest("ID mismatch.");

            try
            {
                _manager.EditDevice(updatedDevice);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/device/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteDevice(string id)
        {
            try
            {
                _manager.RemoveDeviceById(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}

