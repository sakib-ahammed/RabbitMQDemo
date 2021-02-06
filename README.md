# RabbitMQDemo In ASP.NET Core

To run the rabbitmq management poratal in Docker container `[default port: 15672]`

`docker run -p 5672:5672 -p 15672:15672 rabbitmq:management`

For learn about rabbitmq, need to know about publisher and consumer as well as Exchanges, routing keys and bindings. Follow this articles:

- [Consuming RabbitMQ Messages In ASP.NET Core](https://www.c-sharpcorner.com/article/consuming-rabbitmq-messages-in-asp-net-core/)
- [Publishing RabbitMQ Message In ASP.NET Core](https://www.c-sharpcorner.com/article/publishing-rabbitmq-message-in-asp-net-core/)
- [RabbitMQ Headers Exchange in C# to Publish or Consume Messages](https://www.tutlane.com/tutorial/rabbitmq/csharp-rabbitmq-headers-exchange)

You can run the project in different port. To run, go to the project folder in CMD and execute the command `dotnet run [port]`. Example: `dotnet run 5001` `dotnet run 5002` and so on
