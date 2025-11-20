# Authentication System

## Overview

The NextQuestionnaire frontend implements JWT-based authentication.

## Technology Stack

- **JWT Tokens**: Stored in localStorage for session management
- **HTTP Interceptors**: Automatic token attachment to API requests
- **Route Guards**: Protection for unauthorized access

## Authentication Flow

1. User submits credentials on login page
2. Backend validates login and returns JWT token
3. Token stored in localStorage
4. Guards protect routes based on authentication status
5. Interceptors attach token to API requests automatically

## Token Management

- **Storage**: JWT tokens stored in browser localStorage
- **Automatic Attachment**: HTTP interceptors add `Authorization: Bearer <token>` headers
- **Expiration Handling**: Invalid tokens trigger redirect to home/login

## Route Protection

**Guards protect routes based on:**
- Authentication status (logged in/out)
- User roles and permissions
- Token validity

**User Roles:**
- **Students**: Access to assigned questionnaires only. Can respond to questionnaires and view their response history.
- **Teachers**: Can assign questionnaires to groups, view responses from assigned questionnaires, and manage user groups. Also retain student capabilities when assigned questionnaires.
- **Administrators**: Complete template management capabilities including creating, editing, and deleting questionnaire templates. Can manage application settings, view system logs, and perform all teacher functions.
- **Super Administrators**: All administrator permissions plus the ability to modify user permissions and roles. Complete system-level administrative control. 

## Key Features

- **Automatic Login State**: Guards check authentication on route changes
- **Token Persistence**: Login state maintained across browser sessions
- **Error Handling**: Failed authentication redirects to login with error message
- **Post-Login Redirect**: Users returned to intended page after login
- **Secure Logout**: Token removal and session cleanup