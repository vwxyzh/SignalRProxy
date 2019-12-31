# SignalR Proxy

SignalR Proxy is a prototype for bridge the SignalR to any web server.

## Architecture

```
   ________               __________          ________
  |        |_____________|  SignalR |________|  Web   |
  | Client |  WebSocket  |  Server  |  REST  | Server |
  |________|             |__________|        |________|

```

## API Contract

### Proxy API

* Broadcast: `POST /hub/all/{method}`, body is object.
* Send to one client: `POST /hub/clients/{connectionId}/{method}`, body is object.
* Send to user: `POST /hub/users/{userId}/{method}`, body is object.
* Send to group: `POST /hub/groups/{groupName}/{method}`, body is object.
* Join group: `PUT /hub/groups/{groupName}/clients/{connectionId}`.
* Leave group: `DELETE /hub/groups/{groupName}/clients/{connectionId}`.

### Web Server API

* Open connection: `PUT {baseUrl}/clients/{connectionId}`:

  Head:
  * `x-hub`: the hub path.
  * `x-user`: the user identity.  
* Close connection: `DELETE {baseUrl}/clients/{connectionId}`

  Head:
  * `x-hub`: the hub path.
  * `x-user`: the user identity.

* Message: `POST {baseUrl}/messages/{method}`

  Head:
  * `x-hub`: the hub path.
  * `x-user`: the user identity.
  * `x-connection-id`: the connection id.
  
  Body:
  object.
