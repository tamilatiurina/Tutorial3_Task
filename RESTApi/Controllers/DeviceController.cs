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
        public IResult GetAllDevices()
        {
            var devices = _manager
                .GetAllDevices()
                .Select(d => new { d.Id, d.Name, d.IsEnabled });

            return Results.Ok(devices);
        }
        

        // GET: api/device/{id}
        [HttpGet("{id}")]
        public IResult GetDeviceById(string id)
        {
            var device = _manager.GetDeviceById(id);
            if (device == null)
                return Results.NotFound($"Device with ID {id} not found.");

            return Results.Ok(device);
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
        public IResult AddSmartwatch([FromBody] Smartwatch smartwatch)
        {
            try
            {
                _manager.AddDevice(smartwatch);
                return Results.CreatedAtRoute(nameof(GetDeviceById), new { id = smartwatch.Id }, smartwatch);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        [HttpPost("pc")]
        public IResult AddPersonalComputer([FromBody] PersonalComputer pc)
        {
            try
            {
                _manager.AddDevice(pc);
                return Results.CreatedAtRoute(nameof(GetDeviceById), new { id = pc.Id }, pc);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        [HttpPost("embedded")]
        public IResult AddEmbeddedDevice([FromBody] Embedded embedded)
        {
            try
            {
                _manager.AddDevice(embedded);
                return Results.CreatedAtRoute(nameof(GetDeviceById), new { id = embedded.Id }, embedded);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
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
        public IResult EditSmartwatch(string id, [FromBody] Smartwatch smartwatch)
        {
            if (id != smartwatch.Id)
            {
                return Results.BadRequest("ID in URL does not match ID in body.");
            }

            try
            {
                _manager.EditDevice(smartwatch);
                return Results.AcceptedAtRoute(nameof(GetDeviceById), new { id = smartwatch.Id }, smartwatch);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
        
        [HttpPut("pc/{id}")]
        public IResult EditPersonalComputer(string id, [FromBody] PersonalComputer pc)
        {
            if (id != pc.Id)
            {
                return Results.BadRequest("ID in URL does not match ID in body.");
            }

            try
            {
                _manager.EditDevice(pc);
                return Results.AcceptedAtRoute(nameof(GetDeviceById), new { id = pc.Id }, pc);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
        
        [HttpPut("embedded/{id}")]
        public IResult EditEmbedded(string id, [FromBody] Embedded embedded)
        {
            if (id != embedded.Id)
            {
                return Results.BadRequest("ID in URL does not match ID in body.");
            }

            try
            {
                _manager.EditDevice(embedded);
                return Results.AcceptedAtRoute(nameof(GetDeviceById), new { id = embedded.Id }, embedded);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }


        // DELETE: api/device/{id}
        [HttpDelete("{id}")]
        public IResult DeleteDevice(string id)
        {
            try
            {
                _manager.RemoveDeviceById(id);
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.NotFound(ex.Message);
            }
        }
    }
}

