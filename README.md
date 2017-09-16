# inventory-example #

## Requirements ##
  - Visual Studio 2015 Community
  - with [SpecFlow for Visual Studio 2015](http://specflow.org/getting-started/#InstallSetup) extension in order to run acceptance tests.

## Running the solution ##

There are four main actors in the system, all of which must run at the same time:

- Inventory.WebApi
- Inventory.BackgroundWorker
- Inventoyr.Consumer
- RabbitMQ

The Visual Studio solution is configured for "Multiple Startup Projects" and all applications should run simultaneously in debug mode.

### WebApi

This is the layer accepting HTTP requests to manage the underlying inventory of Items.

I recommend [Postman](https://www.getpostman.com/) or [Curl](https://curl.haxx.se/) to send requests.

The api has a few requirements:

 - it only accepts https requests (configured at port 44301 in Visual Studio's IIS Express)
 - it requires basic user authentication 

   -  via http header: [{"key":"Authorization","value":"Basic cGNoaWNvY2l2OnBjaGljb2Npdg==","description":""}]
   -  or, if called from a browser it will prompt for user and password (pchicociv - pchicociv). 
     Not every browser have support for this behaviour but it has been tested in the latest version of Chrome.
 - it requires app authentication via an access token in the query string:
   - apikey = 88DA3BBAC33F4C35A7B6453185F38BB2
   - token = F46B9A9B642943C182E319880F85A554
   - Eg:
   
      https://localhost:44301/api/inventory?apikey=88DA3BBAC33F4C35A7B6453185F38BB2&token=F46B9A9B642943C182E319880F85A554

- it excepts Item objects in json format

```json
{
	"Label" : "Etiqueta2",
	"ItemType": "Type1",
	"ExpirationDate" : "01/01/2020"
}
```
- it keeps inventory in memory and it will be empty at start

#### Api usage

- To add an item to the inventory 
 

Request:
```
POST /api/inventory?apikey=88DA3BBAC33F4C35A7B6453185F38BB2&amp;token=F46B9A9B642943C182E319880F85A554 HTTP/1.1
Host: localhost:44301
Content-Type: application/json
Authorization: Basic cGNoaWNvY2l2OnBjaGljb2Npdg==
{
	"Label" : "any_label",
	"ItemType": "any_type",
	"ExpirationDate" : "01/01/2020"
}
```
Response: inserted item in json format
```json
{
    "Id": 1,
    "Label": "any_label",
    "ItemType": "any_type",
    "ExpirationDate": "01/01/2020",
    "Url": "https://localhost:44301/api/inventory/any_label"
}
``` 


- Obtain the whole inventory

Request:
```
GET /api/inventory?apikey=88DA3BBAC33F4C35A7B6453185F38BB2&amp;token=F46B9A9B642943C182E319880F85A554 HTTP/1.1
Host: localhost:44301
Content-Type: application/json
Authorization: Basic cGNoaWNvY2l2OnBjaGljb2Npdg==
```
Response: array with all items in json format
```json
[
    {
        "Id": 1,
        "Label": "any_label",
        "ItemType": "any_type",
        "ExpirationDate": "01/01/2020",
        "Url": "https://localhost:44301/api/inventory/any_label"
    }
]
```

- Obtain an item by Label

Request:
```
GET /api/inventory/any_label?apikey=88DA3BBAC33F4C35A7B6453185F38BB2&amp;token=F46B9A9B642943C182E319880F85A554 HTTP/1.1
Host: localhost:44301
Content-Type: application/json
Authorization: Basic cGNoaWNvY2l2OnBjaGljb2Npdg==
```
Response: item requested or 'HTTP 404 Not Found' if no item matches label 
```json
{
    "Id": 1,
    "Label": "any_label",
    "ItemType": "any_type",
    "ExpirationDate": "01/01/2020",
    "Url": "https://localhost:44301/api/inventory/any_label"
}
```

- Take an item from the inventory by Label

Request:
```
DELETE /api/inventory/any_label?apikey=88DA3BBAC33F4C35A7B6453185F38BB2&amp;token=F46B9A9B642943C182E319880F85A554 HTTP/1.1
Host: localhost:44301
Content-Type: application/json
Authorization: Basic cGNoaWNvY2l2OnBjaGljb2Npdg==
```
Response: item taken or 'HTTP 404 Not Found' if no item matches label
```json
{
    "Id": 1,
    "Label": "any_label",
    "ItemType": "any_type",
    "ExpirationDate": "01/01/2020",
    "Url": "https://localhost:44301/api/inventory/any_label"
}
```


### BackgroundWorker

Is a simple task scheduler that notifies the system when a day is over, every day, so item expiration can be checked.

It uses [Hangfire](https://www.hangfire.io/) and the task is configured to send this notification every minute
 (instead of every day, for obvious testing purposes).

### Consumer

Console application subscribed to all types of events to see what's happening

### RabbitMQ

By default, the system uses a cloud based service at [CloudAMQP](https://www.cloudamqp.com/) so no further 
steps need to be taken for the system to run.
See instructions below to use a local RabbitMQ server with Docker.

## Testing the requirements ##

Acceptance tests have been written using the BDD paradigm with the Gherkin syntax with [SpecFlow](http://specflow.org/).

Tests are defined in Inventory.Specs project under Features folder.
Implementation of the tests can be found under Steps folder. [FluentAssertions](http://fluentassertions.com/) syntax has been used for condition assertment.

Once we have built the solution, the tests requested by the exercise can be found at Test Explorer window in Visual Studio.

Unfortunately, given the high level of decoupling of the system, tests can not be run in parallel nor with  "Run All" option. 
Instead they have to be run **one at a time** to give xUnit enough time to dispose the web server and the connections to RabbitMQ service.

- ### Requirement 1: 
  *Add an item to the inventory*

    - An item object is instantiated
    - The **OWIN web server** is instantiated
    - A POST request is sent to the server with the item
    - A GET request is sent to retrieve the item by label
    - The label of the item received must match the label of the item instantiated in first place
  
- ### Requirement 2:
  *Take an item from the inventory by Label*

    - An item object is instantiated
    - The OWIN web server is instantiated
    - A POST request is sent to the server with the item (so it exists later)
    - A DELETE request is sent to take the item out by label
    - A GET request is sent to retrieve the item by label
    - The response should raise an HTTP 404 Not Found error

- ### Requirement 3:
  *Notification that an item has been taken out*

    - An item object is instantiated
    - The OWIN web server is instantiated
    - A POST request is sent to the server with the item (so it exists later)
    - A DELETE request is sent to take the item out by label
    - A connection to RabbitMQ message broker with a subscription to the ItemTaken event is stablished
    - A notification should be received of type ItemTaken containing information about the item deleted. Label should match the label of the item taken out previously

- ### Requirement 4:
  *Notification that an item has expired*

    - An item object with a past expiration date is instantiated
    - The OWIN web server is instantiated
    - A POST request is sent to the server with the item (so it exists later)
    - A DayFinished event is raised
    - A connection to RabbitMQ message broker with a subscription to the ItemExpired event is stablished
    - A notification should be received of type ItemExpired containing information about the expired item. Label should match the label of the item added previously


## Notes on design, code structure, assumptions and reasonings

### Call contexts

An object of type CallContext is injected into every http request and then passed to internal calls and events. 
This allows us to identify and group calls and logs under a unique context. It could be very useful for further
analysis and statistics, as well as error debugging. 

We may include new useful information as the system evolves, such as user identity information.

### Persistent repositories

Thanks to IoC (with [StructureMap](http://structuremap.github.io/)), it is quite simple to susbstitute the actual repositories being injected
in the controllers or in the test methods. If we want to implement a persisten repository, only IoC configuration has to be modified, changing the association
between Interfaces and Concrete Classes

### Browser cache

To avoid data to be cached by the browser we instruct it not to keep any responses with HTTP headers in the responses

```
[{"key":"cache-control","value":"no-store, must-revalidate, no-cache, max-age=0"}]
```

### Security

Security has been simplified and there isn't any implementation for user or client registering, token generation,
or any cryptograhpic function for user validation.

The system does implement the logic for basic user authentication and token validation, with predefined and constant values for keys.

OAuth2 and OpenId Connect usage could be easily added with a third party authorizarion service such as [IdentityServer](https://github.com/IdentityServer/IdentityServer3)

### Serilog

Logging in a production environment requires a lot of decissions. Some logs may be just written
to a text file while others may require to be sent to an alert system, or be processed by a 
big data engine.

For this demo, logs are written to debug output window. In a production environment we may go 
for an structured application log engine like [Seq](https://getseq.net/) instead. 
Probably cloud based.

Also we may implement a wrapper around Serilog for sharing purposes as well as hiding Serilog 
behind an interface to ease future changes.

### SpecFlow

Tests in Inventory.Specs must run secuentially in order to avoid problems with the items in the inventory for each test.
We can configure this behaviour in AssemblyInfo.cs:

```c#
[assembly:CollectionBehavior(DisableTestParallelization = true)]
```

### RabbitMQ with docker

In case we don't want to use CloudAMQP and have Docker installed, we can run the following command line to start
a RabbitMQ image.

```
docker run -d --hostname my-rabbit --name inventory-rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management
```
Then in web.config, appSettings, change UseLocalRabbit to true.
Depending on the OS connection to the guest system may vary. In Linux we can use localhost directly, but in Windows
a virtual adapter is created and we need to use its IP.
The IP used to connect to RabbitMQ service can be also configured in web.config->appSettings->LocalRabbitIP

### Xml Documentation

There are several simple comments in the code but every public method and property should be properly decorated
with xml comments to document functionality.

### API Versioning

At some point the API will evolve and we will have to support more than one version at the same time.
This can be accomplished with various techniques, such as Accept Headers, Media Types or even directly in the Url.


### Why not ASP.NET Core?

No real reason. I just think in some cases is better to choose a mature technology to get faster to the market, 
find documentation or hire realiable resources.