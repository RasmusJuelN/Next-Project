## Two-Language Translation Setup (Angular + ngx-translate)
This document explains exactly how language translation was added to this project — from installation to usage.
## nstall Required Packages
- npm install @ngx-translate/core @ngx-translate/http-loader

## Create Translation Files
Inside src/assets, create a folder called i18n and add JSON files for each language.

src/assets/i18n/en.json
## All static text is stored in:
-/assets/i18n/en.json
-/assets/i18n/da.json

## Configure ngx-translate in app.config.ts

## Add Language Switch Buttons Only After Login but planning to move in header where the next logo is located
it is onin homecomponent now