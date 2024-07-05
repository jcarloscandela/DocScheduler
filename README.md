# DocScheduler Project

## Overview

The DocScheduler project is designed to facilitate scheduling and managing appointments. It is structured into several components, each serving a distinct role within the application ecosystem.

### Components

- **DocScheduler.API**: The API layer that serves as the entry point for the application. It handles HTTP requests and responses, routing them to the appropriate services. It is also configured with Swagger for API documentation and testing.

- **DocScheduler.Application**: Contains the core business logic and services of the application. It is referenced by the API layer to perform operations related to scheduling and managing appointments.

- **DocScheduler.SlotService**: A service dedicated to managing time slots for appointments. It interacts with the core application services to ensure the availability and scheduling of slots.

- **DocScheduler.Utils**: Provides utility functions and helpers used across the application. This could include date manipulation, validation functions, and more.

- **DocScheduler.Test**: Contains unit tests for the application, ensuring that the core functionality works as expected.

## Running the Project

To run the DocScheduler project, you will need to have .NET 8.0 installed on your machine. Follow these steps to get the API up and running:

1. Navigate to the `DocScheduler.API` directory.
2. Run `dotnet build` to build the project.
3. Run `dotnet run` to start the application.

The API will start, and you can access it by navigating to `http://localhost:5038`

## Testing with Swagger

DocScheduler.API is configured with Swagger, making it easy to test the API endpoints directly through your browser.

1. Ensure the API is running by following the steps in the "Running the Project" section.
2. Navigate to `http://localhost:5038/swagger` in your web browser. You will be presented with the Swagger UI, listing all the available API endpoints.
3. Choose an endpoint you wish to test, click on it, and then click the "Try it out" button.
4. Fill in the required parameters and execute the request. Swagger will display the request response, status code, and headers.

## Conclusion

The DocScheduler project is a comprehensive solution for appointment scheduling, leveraging .NET technologies and structured into clear, purpose-driven components. By following the instructions above, you can run the project and use Swagger to test the API endpoints.