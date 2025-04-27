using Devices.Infrastructure.Devices.Infrastraucture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial3_Task;

namespace RESTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly DeviceManager _manager;

        // Constructor that accepts DeviceManager via DI
        public DeviceController(DeviceManager manager)
        {
            _manager = manager;
        }

        
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
        [HttpGet("{id}", Name = nameof(GetDeviceById))]
        public IResult GetDeviceById(string id)
        {
            try
            {
                var device = _manager.GetDeviceById(id);
                return Results.Ok(device);
            }
            catch (ArgumentException ex)
            {
                return Results.NotFound(ex.Message);
            }
        }

        // POST: api/device/smartwatch
        [HttpPost("smartwatch")]
        public IResult AddSmartwatch([FromBody] Smartwatch smartwatch)
        {
            try
            {
                _manager.AddDevice(smartwatch, "smartwatch");
                return Results.CreatedAtRoute(nameof(GetDeviceById), new { id = smartwatch.Id }, smartwatch);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        // POST: api/device/pc
        [HttpPost("pc")]
        public IResult AddPersonalComputer([FromBody] PersonalComputer pc)
        {
            try
            {
                _manager.AddDevice(pc, "pc");
                return Results.CreatedAtRoute(nameof(GetDeviceById), new { id = pc.Id }, pc);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        // POST: api/device/embedded
        [HttpPost("embedded")]
        public IResult AddEmbeddedDevice([FromBody] Embedded embedded)
        {
            try
            {
                _manager.AddDevice(embedded, "embedded");
                return Results.CreatedAtRoute(nameof(GetDeviceById), new { id = embedded.Id }, embedded);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        // PUT: api/device/smartwatch/{id}
        [HttpPut("smartwatch/{id}")]
        public IResult EditSmartwatch(string id, [FromBody] Smartwatch smartwatch)
        {
            if (id != smartwatch.Id)
            {
                return Results.BadRequest("ID in URL does not match ID in body.");
            }

            try
            {
                _manager.EditDevice(smartwatch, "smartwatch");
                return Results.AcceptedAtRoute(nameof(GetDeviceById), new { id = smartwatch.Id }, smartwatch);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        // PUT: api/device/pc/{id}
        [HttpPut("pc/{id}")]
        public IResult EditPersonalComputer(string id, [FromBody] PersonalComputer pc)
        {
            if (id != pc.Id)
            {
                return Results.BadRequest("ID in URL does not match ID in body.");
            }

            try
            {
                _manager.EditDevice(pc, "pc");
                return Results.AcceptedAtRoute(nameof(GetDeviceById), new { id = pc.Id }, pc);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        // PUT: api/device/embedded/{id}
        [HttpPut("embedded/{id}")]
        public IResult EditEmbedded(string id, [FromBody] Embedded embedded)
        {
            if (id != embedded.Id)
            {
                return Results.BadRequest("ID in URL does not match ID in body.");
            }

            try
            {
                _manager.EditDevice(embedded, "embedded");
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

        // POST: api/device/{id}/turnon
        [HttpPost("{id}/turnon")]
        public IResult TurnOnDevice(string id)
        {
            try
            {
                _manager.TurnOnDevice(id);
                return Results.Ok($"Device with ID {id} is now turned on.");
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        // POST: api/device/{id}/turnoff
        [HttpPost("{id}/turnoff")]
        public IResult TurnOffDevice(string id)
        {
            try
            {
                _manager.TurnOffDevice(id);
                return Results.Ok($"Device with ID {id} is now turned off.");
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}

