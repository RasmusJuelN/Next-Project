# Design

> [!TIP]
> If you find it difficult to plan ahead and design the controller/endpoint first, try reversing the entire order! Try and start designing the very last thing that will be called/used at the end of the chain, and design backwards from there. If you already know what the database, service or other components will look like, it might be easier to design the controller/endpoint around that.

## Controller
**Location: `Backend/API/Controllers/`**

Remember to follow the naming convention of ending the controller name with "Controller" (e.g., `UserController.cs`). This helps maintain consistency and clarity in the codebase.

## DTO
**Location: `Backend/API/DTO/`**

When designing a new controller or endpoint, start by defining the necessary Data Transfer Objects (DTOs). DTOs are used to encapsulate the data that will be sent to and from the API. They help ensure that only the required data is exposed and can also aid in validation. Additionally, Swagger uses these DTOs to generate accurate API documentation, aiding both development and client integration.
> [!NOTE]
> DTOs still have to follow C# naming conventions, meaning PascalCase for class names and properties. They will be automatically converted to camelCase in the JSON response.

> [!NOTE]
> These DTOs do not necessarily have to be used or exposed over the API. Anything that might travel between the API and other layers (e.g., services, repositories) can also use DTOs to ensure data integrity and separation of concerns.

### Request DTO
**Location: `Backend/API/DTO/Requests/`**

The request DTO defines the structure of the data that the client must send to the API when making a request. This includes any parameters, body content, or query strings required for the endpoint to function correctly.

### Response DTO
**Location: `Backend/API/DTO/Responses/`**

The response DTO defines the structure of the data that the API will return to the client. This includes any data that the client needs to receive after making a request. Unless explicitly desired, status/error responses are not necessary to create, as these are handled globally.

> [!TIP]
> If you find yourself needing multiple response DTOs with varying amounts of data, consider using inheritance to create a base response DTO and then extend it for more specific use cases. This can help reduce code duplication and improve maintainability.

## Service

**Location: `Backend/API/Services/`**

The service layer contains the business logic for the application. When designing a new controller or endpoint, consider what services will be needed to handle the request and response. Services should be designed to be reusable and modular, allowing them to be easily tested and maintained.

> [!TIP]
> Consider designing an interface for the service in the `Interfaces` folder. This can further abstract the implementation details and make it easier to swap out implementations if needed.