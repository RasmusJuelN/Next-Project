#!/bin/bash

# Navigate to the Angular source directory
cd angular-src

# Install dependencies
npm install

# Check if the first argument is "prod"
if [ "$1" == "prod" ]
then
    # Build the Angular application for production
    ng build --prod
else
    # Build the Angular application for development
    ng build
fi

# Enable dotglob so * matches hidden files
shopt -s dotglob

# Move the built files to the front-end directory
mv -f dist/angular-src/* ../front-end

# Navigate back to the root directory
cd ..