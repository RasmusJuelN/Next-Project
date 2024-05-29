## angular-src/
Angular Application source code used for the front-end.

## front-end/
Static build files from the Angular application that are served from a web server.
Git is set to ignore this folder since build artifacts can easily be regenerated, and may often change while testing.

## back-end/
API running on the Python library FastAPI, communicating with the database.
- `Pipfile` & `Pipfile.lock` are used to track the dependencies for Python

## Scripts/
- `build-angular.bat` Batch script for building Angular app and automatically moving it to `front-end/`. Takes arg "prod" (`build-angular.bat prod`) to indicate it should build for production.
- `build-angular.sh` Bash shell script for building Angular app and automatically moving it to `front-end/`. Takes arg "prod" (`build-angular.sh prod`) to indicate it should build for production.

## root
- `package.json` is the node project package file.

## Running
- `npm run install-dependencies` to install dependencies for both Node and Python.
- `npm run start` to both build and serve the Angular app, and start the API.
- `npm run start-backend` to only start the API.
- `npm run start-frontend` to only build and serve the Angular app.