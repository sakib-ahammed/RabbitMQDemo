namespace GHRabbitMQDemo.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IRabbitManager _manager;

        public ValuesController(IRabbitManager manager)
        {
            _manager = manager;
        }

        // GET api/values  
        [HttpGet]
        public ActionResult<string> Get()
        {
            // other opreation  

            // if above operation succeed, publish a message to RabbitMQ  

            var num = new System.Random().Next(9000);
            var message = new
            {
                field1 = $"Hello-{num}",
                field2 = $"rabbit-{num}",
                machineID = _manager.GetRabbitMQMachineID()
            };
            // publish message  
            //_manager.Publish( message,"demo.exchange.fanout", "fanout", "demo.queue.*");
            _manager.Publish(message, "demo.exchange.header", "headers", "");
            return message.ToString();
            //return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
