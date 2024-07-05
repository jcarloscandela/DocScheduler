# DocScheduler Project

## Overview

The DocScheduler project facilitates scheduling and managing appointments. It is structured into several components, each serving distinct roles within the application ecosystem.

### Components
  - **/src** Contains the source code for the project:
      - **DocScheduler.API**: This layer serves as the entry point for the application, handling HTTP requests and responses. It is configured with Swagger for API documentation and testing.
      - **DocScheduler.Application**: Contains core business logic and services related to scheduling and managing appointments. It is utilized by the API layer.
      - **DocScheduler.SlotService**: Manages time slots for appointments, interacting with core application services to ensure slot availability.
      - **DocScheduler.Utils**: Provides utility functions and helpers used across the application, including date manipulation and validation functions.

  - **/test** Contains unit tests for the project.
      - **DocScheduler.API.Test**: Contains API unit tests.
      - **DocScheduler.Application.Test**: Contains application layer unit tests.
      - **DocScheduler.SlotService.Test**: Contains slot service unit tests.
      - **DocScheduler.Utils.Test**: Contains utility unit tests.

## Running the Project

Before running the project, ensure the required configuration is set in the `appsettings.json` file located within the `DocScheduler.API` project:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "SlotService": {
    "BaseUrl": "url-to-slot-service",
    "Username": "username",
    "Password": "password"
  },
  "AllowedHosts": "*"
}
```

To run the DocScheduler project, ensure you have .NET 8.0 installed on your machine, then follow these steps:

1. Navigate to the `DocScheduler.API` directory.
2. Run `dotnet build` to build the project.
3. Run `dotnet run` to start the application.

The API will start, and you can access it by navigating to `http://localhost:5038`.

## Testing with Swagger

DocScheduler.API is configured with Swagger, allowing easy testing of API endpoints directly through your browser.

1. Ensure the API is running (follow steps in the "Running the Project" section).
2. Navigate to `http://localhost:5038/swagger` in your web browser. You will see the Swagger UI listing all available API endpoints.
3. Choose an endpoint, click on it, and then click "Try it out".
4. Fill in required parameters and execute the request. Swagger will display the request response, status code, and headers.

### Test Endpoints

- **GET /api/slots/available**: Retrieves available slots for a given date.
   - Request:
     - Parameters:
       - **MondayDate**: Date format: "yyyy-MM-dd", e.g., "2024-07-08"
   - Response:
     ```json
     [
       {
         "facilityId": "7960f43f-67dc-4bc5-a52b-9a3494306749",
         "start": "2024-07-08T10:40:00",
         "end": "2024-07-08T10:50:00"
       },
       {
         "facilityId": "7960f43f-67dc-4bc5-a52b-9a3494306749",
         "start": "2024-07-08T10:50:00",
         "end": "2024-07-08T11:00:00"
       },
       {
         "facilityId": "7960f43f-67dc-4bc5-a52b-9a3494306749",
         "start": "2024-07-08T11:10:00",
         "end": "2024-07-08T11:20:00"
       }
     ]
     ```

- **POST /api/slots/book**: Books a slot (available slots retrieved from the previous endpoint).
   - Request Body:
     ```json
     {      
         "facilityId": "7960f43f-67dc-4bc5-a52b-9a3494306749",
         "start": "2024-07-08T09:30:00",
         "end": "2024-07-08T09:40:00",
         "name": "John",
         "secondName": "Doe",
         "email": "test@mail.com",
         "phone": "666999666",
         "comments": "Test booking"
     }
     ```
   - Response:
     ```text
     Slot booked successfully
     ```

## Conclusion

The DocScheduler project provides a comprehensive solution for appointment scheduling, leveraging .NET technologies. Its clear, purpose-driven components ensure efficient scheduling and management. Follow the provided instructions to run the project and utilize Swagger for API testing.
