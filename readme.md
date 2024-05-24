## angular-src/
Angular Application source code used for the front-end.

## front-end/
Static build files from the Angular application that are served from a web server.
Git is set to ignore this folder since build artifacts can easily be regenerated, and may often change while testing.

## back-end/
API running on the Python library FastAPI, communicating with the database.
- `Pipfile` and `Pipfile.lock` are used to track the dependencies for Python

## root
- `package.json` is the node project package file.