# 🔐 Token Service Microservice

A lightweight .NET microservice for generating and validating JWT access tokens, designed to work within distributed systems. It includes integration with an external user validation service.

## ✨ Features

* Generate JWT access tokens with customizable expiration
* Validate tokens and user existence via external API
* Azure Function endpoint for token validation
* Environment-variable-driven configuration
* Secure and exception-handled implementation

## 🧩 Tech Stack

* .NET 8 / C#
* Azure Functions
* JWT (System.IdentityModel.Tokens.Jwt)
* Newtonsoft.Json
* REST API calls with `HttpClient`

## 📦 Environment Variables

Ensure the following are set for the service to work:

| Variable    | Description            |
| ----------- | ---------------------- |
| `Issuer`    | JWT token issuer       |
| `Audience`  | JWT token audience     |
| `SecretKey` | Secret key for signing |

## 🚀 API Endpoints

### `POST /api/ValidateToken`

**Validates a JWT token.**

**Request body:**

```json
{
  "accessToken": "string",
  "userId": "string"
}
```

**Responses:**

* `200 OK` – Token is valid
* `400 Bad Request` – Token is invalid or request is malformed

## 📡 External Dependency

* **UserService**: Checks if the user exists by calling an external API.


