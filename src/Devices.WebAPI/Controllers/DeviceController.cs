
using Devices.Infrastructure;
using Devices.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial3_Task;


namespace RESTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly DevicesRep _repository;

        public DeviceController(DevicesRep repository)
        {
            _repository = repository;
        }

        
       // GET: api/devices
        [HttpGet]
        public IActionResult GetAllDevices()
        {
            var devices = _repository.GetAllDevices()
                .Select(d => new {
                    Id = d.Id,
                    Name = d.Name,
                    IsEnabled = d.IsEnabled
                })
                .ToList();

            return Ok(devices);
        }

        // GET: api/devices/{id}
        [HttpGet("{id}")]
        public IActionResult GetDeviceById(string id)
        {
            var device = _repository.GetAllDevices().FirstOrDefault(d => d.Id == id);
            if (device == null)
            {
                return NotFound($"Device with id {id} not found.");
            }

            return Ok(device);
        }

        // POST: api/devices
        [HttpPost]
        public IActionResult CreateDevice([FromBody] DeviceCreateRequest request)
        {
            /*if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newId = request.DeviceType.ToLower() switch
            {
                "pc" => _repository.GenerateNextId("pc"),
                "embedded" => _repository.GenerateNextId("embedded"),
                "smartwatch" => _repository.GenerateNextId("smartwatch"),
                _ => throw new ArgumentException("Unknown device type")
            };
            Device newDevice = request.DeviceType.ToLower() switch
            {
                "pc" => new PersonalComputer(newId, request.Name, request.IsEnabled, request.OperationSystem),
                "embedded" => new Embedded(newId, request.Name, request.IsEnabled, request.IpAddress, request.NetworkName),
                "smartwatch" => new Smartwatch(newId, request.Name, request.IsEnabled, request.BatteryLevel ?? 0),
                _ => null
            };

            if (newDevice == null)
            {
                return BadRequest("Invalid device type.");
            }


    		try
    		{
        		_repository.AddEmbedded(newDevice);
        		return CreatedAtAction(nameof(GetDeviceById), new { id = newDevice.Id }, newDevice);
    		}
   			 catch (Exception ex)
    		{
        		// You could log ex.Message here
        		return StatusCode(500, "Failed to create device: " + ex.Message);
    		}*/
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var supportedTypes = new[] { "pc", "embedded", "smartwatch" };

            if (!supportedTypes.Contains(request.DeviceType, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest("Unsupported device type. Supported types are: 'pc', 'embedded', and 'smartwatch'.");
            }

            var newId = request.DeviceType.ToLower() switch
            {
                "pc" => _repository.GenerateNextId("pc"),
                "embedded" => _repository.GenerateNextId("embedded"),
                "smartwatch" => _repository.GenerateNextId("smartwatch"),
                _ => throw new ArgumentException("Unknown device type")
            };

            Device newDevice = request.DeviceType.ToLower() switch
            {
                "pc" => new PersonalComputer(newId, request.Name, request.IsEnabled, request.OperationSystem),
                "embedded" => new Embedded(newId, request.Name, request.IsEnabled, request.IpAddress, request.NetworkName),
                "smartwatch" => new Smartwatch(newId, request.Name, request.IsEnabled, request.BatteryLevel ?? 0),
                _ => null
            };

            if (newDevice == null)
            {
                return BadRequest("Invalid device type.");
            }

            try
            {
                _repository.AddDevice(newDevice, request.DeviceType.ToLower());
                return CreatedAtAction(nameof(GetDeviceById), new { id = newDevice.Id }, newDevice);
            }
            catch (Exception ex)
            {
                // You could log ex.Message here
                return StatusCode(500, "Failed to create device: " + ex.Message);
            }
        }

        // PUT: api/devices/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateDevice(string id, [FromBody] DeviceUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var device = _repository.GetAllDevices().ToList().FirstOrDefault(d => d.Id == id);
            if (device == null)
            {
                return NotFound($"Device with id {id} not found.");
            }
            
            
            device.Name = request.Name;
            device.IsEnabled = request.IsEnabled;
            device.RowVersion = request.RowVersion;

            if (device is PersonalComputer pc && request.OperationSystem != null)
            {
                pc.OperationSystem = request.OperationSystem;
            }
            if (device is Embedded emb && request.IpAddress != null && request.NetworkName != null)
            {
                emb.IpAddress = request.IpAddress;
                emb.NetworkName = request.NetworkName;
            }
            if (device is Smartwatch sw && request.BatteryLevel.HasValue)
            {
                sw.BatteryLevel = request.BatteryLevel.Value;
            }
            
            Console.WriteLine(request.RowVersion);
            if (device.RowVersion == null || device.RowVersion.Length == 0)
            {
                throw new ArgumentException("RowVersion must be provided for concurrency check.");
            }

            try
            {
                if (_repository.UpdateDevice(device))
                {
                    return Ok(device);
                }
                else
                {
                    return BadRequest("Could not update device.");
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/devices/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteDevice(string id)
        {
            try
            {
                _repository.DeleteDevice(id);
                return NoContent(); // 204 - No Content
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}

