# Expense Tracker

A secure authentication API built with .NET 8.0, featuring JWT authentication, email verification, and password reset functionality.

## Features

- User registration with email verification
- JWT-based authentication
- Password reset functionality
- CORS support
- Swagger API documentation
- MongoDB integration
- Email service integration

## Prerequisites

- .NET 8.0 SDK
- MongoDB
- SMTP server for email functionality

## Setup

1. Clone the repository
2. Create a `.env` file in the root directory with the following variables:
```
# MongoDB Settings
MONGODB_CONNECTION_STRING=your_mongodb_connection_string
MONGODB_DATABASE_NAME=your_database_name

# JWT Settings
JWT_KEY=your_jwt_secret_key_at_least_32_characters_long
JWT_ISSUER=your_jwt_issuer
JWT_AUDIENCE=your_jwt_audience

# Email Settings
EMAIL_SMTP_SERVER=your_smtp_server
EMAIL_SMTP_PORT=587
EMAIL_SMTP_USERNAME=your_smtp_username
EMAIL_SMTP_PASSWORD=your_smtp_password
EMAIL_FROM_ADDRESS=your_from_email
EMAIL_FROM_NAME=your_from_name
EMAIL_ENABLE_SSL=true

# CORS Settings
ALLOWED_ORIGINS=http://localhost:3000
```

3. Install dependencies:
```bash
dotnet restore
```

4. Run the application:
```bash
dotnet run
```

## API Documentation

The API documentation is available through Swagger UI when running the application in development mode:

1. Start the application
2. Navigate to `https://localhost:5001/swagger` or `http://localhost:5000/swagger`

### Available Endpoints

#### Authentication

- `POST /api/auth/signup` - Register a new user
- `POST /api/auth/verify-email` - Verify user's email
- `POST /api/auth/login` - Login and get JWT token
- `POST /api/auth/forgot-password` - Request password reset
- `POST /api/auth/reset-password` - Reset password with token

### Login Response

```json
{
    "token": "jwt_token",
    "userId": "user_id",
    "email": "user@example.com",
    "expiresAt": "2024-03-21T12:00:00Z",
    "message": "Login successful"
}
```

## Security Features

- Password hashing with salt
- JWT token authentication
- Email verification
- Secure password reset flow
- CORS protection

## Development

- The application runs on .NET 8.0
- Uses MongoDB for data storage
- Implements JWT for authentication
- Includes Swagger for API documentation
- Supports CORS for frontend integration

## License

MIT 