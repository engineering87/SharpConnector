// (c) 2020 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc;
using SharpConnector.Interfaces;

namespace SharpConnector.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConnectorController : ControllerBase
    {
        private readonly ISharpConnectorClient<string> _connectorClient;

        private readonly ILogger<ConnectorController> _logger;

        public ConnectorController(ILogger<ConnectorController> logger, ISharpConnectorClient<string> connectorClient)
        {
            _logger = logger;
            _connectorClient = connectorClient;
        }

        [HttpGet("{key}", Name = "Get")]
        public ActionResult<string> Get(string key)
        {
            var result = _connectorClient.Get(key);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpGet("async/{key}")]
        public async Task<ActionResult<string>> GetAsync(string key)
        {
            var result = await _connectorClient.GetAsync(key);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<string>> GetAll()
        {
            var results = _connectorClient.GetAll();
            return Ok(results);
        }

        [HttpPost("insert")]
        public ActionResult<bool> Insert(string key, [FromBody] string value)
        {
            var success = _connectorClient.Insert(key, value);
            return success ? Ok(success) : BadRequest("Insert failed.");
        }

        [HttpPost("insert/with-expiration")]
        public ActionResult<bool> Insert(string key, [FromBody] string value, TimeSpan expiration)
        {
            var success = _connectorClient.Insert(key, value, expiration);
            return success ? Ok(success) : BadRequest("Insert with expiration failed.");
        }

        [HttpPost("insert-async")]
        public async Task<ActionResult<bool>> InsertAsync(string key, [FromBody] string value)
        {
            var success = await _connectorClient.InsertAsync(key, value);
            return success ? Ok(success) : BadRequest("Async insert failed.");
        }

        [HttpPost("insert-async/with-expiration")]
        public async Task<ActionResult<bool>> InsertAsync(string key, [FromBody] string value, TimeSpan expiration)
        {
            var success = await _connectorClient.InsertAsync(key, value, expiration);
            return success ? Ok(success) : BadRequest("Async insert with expiration failed.");
        }

        [HttpPost("insert-many")]
        public ActionResult<bool> InsertMany([FromBody] Dictionary<string, string> values)
        {
            var success = _connectorClient.InsertMany(values);
            return success ? Ok(success) : BadRequest("Insert many failed.");
        }

        [HttpPost("insert-many/with-expiration")]
        public ActionResult<bool> InsertMany([FromBody] Dictionary<string, string> values, TimeSpan expiration)
        {
            var success = _connectorClient.InsertMany(values, expiration);
            return success ? Ok(success) : BadRequest("Insert many with expiration failed.");
        }

        [HttpDelete("{key}")]
        public ActionResult<bool> Delete(string key)
        {
            var success = _connectorClient.Delete(key);
            return success ? Ok(success) : NotFound("Delete failed.");
        }

        [HttpDelete("async/{key}")]
        public async Task<ActionResult<bool>> DeleteAsync(string key)
        {
            var success = await _connectorClient.DeleteAsync(key);
            return success ? Ok(success) : NotFound("Async delete failed.");
        }

        [HttpPut("{key}")]
        public ActionResult<bool> Update(string key, [FromBody] string value)
        {
            var success = _connectorClient.Update(key, value);
            return success ? Ok(success) : NotFound("Update failed.");
        }

        [HttpPut("async/{key}")]
        public async Task<ActionResult<bool>> UpdateAsync(string key, [FromBody] string value)
        {
            var success = await _connectorClient.UpdateAsync(key, value);
            return success ? Ok(success) : NotFound("Async update failed.");
        }
    }
}
