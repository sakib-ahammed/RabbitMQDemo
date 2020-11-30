using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GHRabbitMQDemo
{
    public interface IRabbitManager
    {
        string GetRabbitMQMachineID();
        void SetRabbitMQMachineID(string value);
        void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey)
            where T : class;
    }
}
