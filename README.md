# CalamattaChat
This repository contains the solution for a backend developer test, that is about creating a system that assigns support chat requests to agents.

Technologies used:
- ASP.NET Core
- RabbitMQ
- Serilog
- SignalR
- Docker

To test the application, there two HTML files served by the ChatAPI: chatWindow.html and chatSessionTester.html.
<br/><br/>
For the application to work, it needs a RabbitMQ instance running on the local machine (although this can be configured).
<br/>
Currently, the RabbitMQ server is accessed simply through a guest account.
