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
        private static DeviceManager _manager = new(@"C:\Users\Home\Downloads\S30844_REST\tutorial3_template\Tutorial3_Task\input.txt");
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
        /*[HttpPost]
        public IActionResult CreateDevice([FromBody] Device device)
        {
            try
            {
               _manager.AddDevice(device);
                return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, device);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex){
                return StatusCode(500, ex.Message);
            }
        }*/
        
        [HttpPost("smartwatch")]
        public IActionResult AddSmartwatch([FromBody] Smartwatch smartwatch)
        {
            try
            {
                _manager.AddDevice(smartwatch);
                return CreatedAtAction(nameof(GetDeviceById), new { id = smartwatch.Id }, smartwatch);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("pc")]
        public IActionResult AddPersonalComputer([FromBody] PersonalComputer pc)
        {
            try
            {
                _manager.AddDevice(pc);
                return CreatedAtAction(nameof(GetDeviceById), new { id = pc.Id }, pc);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("embedded")]
        public IActionResult AddEmbeddedDevice([FromBody] Embedded embedded)
        {
            try
            {
                _manager.AddDevice(embedded);
                return CreatedAtAction(nameof(GetDeviceById), new { id = embedded.Id }, embedded);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/device/{id}
        /*[HttpPut("{id}")]
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
        }*/
        
        [HttpPut("smartwatch/{id}")]
        public IActionResult EditSmartwatch(string id, [FromBody] Smartwatch smartwatch)
        {
            if (id != smartwatch.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            try
            {
                _manager.EditDevice(smartwatch);
                return AcceptedAtAction(nameof(GetDeviceById), new { id = smartwatch.Id }, smartwatch);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("pc/{id}")]
        public IActionResult EditPersonalComputer(string id, [FromBody] PersonalComputer pc)
        {
            if (id != pc.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            try
            {
                _manager.EditDevice(pc);
                return AcceptedAtAction(nameof(GetDeviceById), new { id = pc.Id }, pc);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("embedded/{id}")]
        public IActionResult EditEmbedded(string id, [FromBody] Embedded embedded)
        {
            if (id != embedded.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            try
            {
                _manager.EditDevice(embedded);
                return AcceptedAtAction(nameof(GetDeviceById), new { id = embedded.Id }, embedded);
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

