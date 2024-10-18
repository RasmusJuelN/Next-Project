# Project Structure Overview
This document provides an overview of the structure and purpose of the files and directories in this Angular project.

## src
The source directory for the application. It contains all the code and assets needed to build the project.

### src/app
The main application directory. This is where the core components, services, models, and configurations of the application are located.

- **app.component.ts**: The root component of the application. It acts as the main container for other components and sets up the base structure of the app.

- **app.component.html**: The HTML template associated with `app.component.ts`. It defines the layout and structure of the user interface.

- **app.component.css**: The CSS file for styling `app.component.html`. It contains styles specific to the root component.

- **app.config.ts**: Configuration file for application-wide settings and constants. It is utilized in `main.ts` for initializing configurations.

- **app.routes.ts**: Defines the routing configuration for the application, mapping URLs to respective components for navigation.
#### src/app/components
This directory contains all the reusable components of the application. Components are the building blocks of the UI, each encapsulating a specific piece of functionality and layout.

**Remember to switch between mock services and real services as needed for testing and production.**

#### src/app/models
This directory contains the data models used in the application. Models define the structure of the data being handled. Currently, it contains a single file consolidating all data models.

#### src/app/services
Services provide extended logic for components and manage data operations.

They also contain mock services and a mock Database with premade data. In app.config has a useClass for Data and Auth services whihc replaces it if environment in either environment.devolopment.ts or environment.ts is true for useMock.

**Mock needs to align that of the real version. So Names need to match. This is important as errors for return types or naming will give errors only in runtime for mock**

#### guards and interceptors
interceptors will give the htttp requests the token from local storage and put it in requests so backend can handle it.

Guards is used to protect routes from being accessed. Role is used if someone like for example student tries to access dashboard component while Auth is used to check for connection before being put over there. If there is no connection it will signal as error and be put over to error component

### src/assets
Contains static assets such as images and other resources. It also includes an **OLD** default mock data JSON file used when there is no data available in local storage.

# Other Stuff
## JWT Token
The JWT token can be found in **local storage** and is typiclly gotten using the service

## Api for backend.
for current devolpemnt, the api url is getting it from environment.ts/environment.developments.ts, however when using the proxy it will replaces the url from api call with the one from proxy.

http://localhost:4200/api/v1

becomes

http://127.0.0.1:8000