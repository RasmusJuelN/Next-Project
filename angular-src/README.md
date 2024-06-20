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
#### src/app/models
This directory contains the data models used in the application. Models define the structure of the data being handled. Currently, it contains a single file consolidating all data models.
#### src/app/services
Services provide extended logic for components and manage data operations. They also contain mock data files that use local storage to simulate backend data during development. 

Remember to switch between mock services and real services as needed for testing and production.
### src/assets
Contains static assets such as images and other resources. It also includes a default mock data JSON file used when there is no data available in local storage.
# FrontendProj
This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 17.3.5.

  

## Development server
Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.
## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build
Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.
## Running unit tests
Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).
## Running end-to-end tests
Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a package that implements end-to-end testing capabilities.

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.io/cli) page.