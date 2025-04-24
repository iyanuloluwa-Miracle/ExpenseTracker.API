# Expense Management System API

A robust API for managing personal expenses, built with ASP.NET Core and MongoDB.

## Features

- User authentication and authorization
- Expense management
- Category management
- Dashboard and reporting
- Search and filtering capabilities

## Application Flow

### 1. Authentication Flow
```
User -> AuthController
  ├─ Signup: Creates user, sends verification email
  ├─ Login: Validates credentials, returns JWT token
  ├─ Verify Email: Confirms user's email
  └─ Password Reset: Handles forgotten password flow
```

### 2. Category Management Flow
```
User -> CategoryController -> CategoryService -> MongoDB
  ├─ Create Category
  │  └─ POST /api/categories
  │     └─ Creates new category with name and color
  │
  ├─ List Categories
  │  └─ GET /api/categories
  │     └─ Returns all categories for the user
  │
  ├─ Update Category
  │  └─ PUT /api/categories/{id}
  │     └─ Updates category name or color
  │
  └─ Delete Category
     └─ DELETE /api/categories/{id}
        └─ Checks if category is used by expenses before deletion
```

### 3. Expense Management Flow
```
User -> ExpenseController -> ExpenseService -> MongoDB
  ├─ Create Expense
  │  └─ POST /api/expenses
  │     └─ Creates new expense with amount, description, category
  │
  ├─ List Expenses
  │  └─ GET /api/expenses
  │     └─ Supports filtering by:
  │        ├─ Date range
  │        ├─ Amount range
  │        ├─ Category
  │        └─ Description
  │
  ├─ Get Single Expense
  │  └─ GET /api/expenses/{id}
  │     └─ Returns detailed expense info
  │
  ├─ Update Expense
  │  └─ PUT /api/expenses/{id}
  │     └─ Updates expense details
  │
  └─ Delete Expense
     └─ DELETE /api/expenses/{id}
        └─ Removes expense from database
```

### 4. Dashboard Flow
```
User -> DashboardController -> DashboardService -> MongoDB
  ├─ Get Summary
  │  └─ GET /api/dashboard/summary
  │     └─ Returns:
  │        ├─ Total expenses
  │        ├─ Average spending
  │        ├─ Highest expense
  │        ├─ Lowest expense
  │        └─ Total transactions
  │
  ├─ Recent Transactions
  │  └─ GET /api/dashboard/recent
  │     └─ Returns last 10 expenses
  │
  ├─ Category Summary
  │  └─ GET /api/dashboard/reports/category-summary
  │     └─ Returns:
  │        ├─ Category-wise totals
  │        ├─ Percentages
  │        └─ Colors for visualization
  │
  └─ Monthly Summary
     └─ GET /api/dashboard/reports/monthly-summary
        └─ Returns monthly spending trends
```

### 5. Data Flow
```
Client -> API -> Controller -> Service -> MongoDB
  ├─ Authentication: JWT token required for all requests
  ├─ Data Validation: At controller level
  ├─ Business Logic: In service layer
  └─ Data Storage: MongoDB collections
     ├─ Users
     ├─ Categories
     ├─ Expenses
     └─ Tokens
```

### 6. Security Flow
```
Request -> Middleware -> Controller
  ├─ JWT Validation
  ├─ User Authentication
  ├─ Authorization
  └─ Request Processing
```

### 7. Error Handling Flow
```
Error -> Controller -> Client
  ├─ 400: Bad Request (invalid input)
  ├─ 401: Unauthorized (missing/invalid token)
  ├─ 403: Forbidden (insufficient permissions)
  ├─ 404: Not Found (resource doesn't exist)
  └─ 500: Server Error (internal issues)
```

## API Endpoints

### Authentication
- `POST /api/auth/signup` - Register a new user
- `POST /api/auth/login` - Login and get JWT token
- `POST /api/auth/verify-email` - Verify user email
- `POST /api/auth/forgot-password` - Request password reset
- `POST /api/auth/reset-password` - Reset password with token

### Expense Management
- `POST /api/expenses` - Create a new expense
  ```json
  {
    "amount": 100.50,
    "description": "Groceries",
    "categoryId": "category_id",
    "date": "2024-03-20T10:00:00Z"
  }
  ```

- `GET /api/expenses` - List all expenses with optional filters
  - Query Parameters:
    - `startDate`: Filter by start date
    - `endDate`: Filter by end date
    - `minAmount`: Filter by minimum amount
    - `maxAmount`: Filter by maximum amount
    - `description`: Search by description
    - `categoryId`: Filter by category
    - `sortBy`: Sort by field (amount_asc, amount_desc, date_asc, category_asc, category_desc)

- `GET /api/expenses/{id}` - Get a specific expense
- `PUT /api/expenses/{id}` - Update an expense
- `DELETE /api/expenses/{id}` - Delete an expense

### Category Management
- `POST /api/categories` - Create a new category
  ```json
  {
    "name": "Groceries",
    "color": "#FF5733"
  }
  ```

- `GET /api/categories` - List all categories
- `PUT /api/categories/{id}` - Update a category
- `DELETE /api/categories/{id}` - Delete a category

### Dashboard & Reports
- `GET /api/dashboard/summary` - Get overall expense summary
  ```json
  {
    "totalExpenses": 1500.00,
    "averageSpending": 75.00,
    "highestExpense": 200.00,
    "lowestExpense": 10.00,
    "totalTransactions": 20
  }
  ```

- `GET /api/dashboard/recent` - Get recent transactions (last 10)
- `GET /api/dashboard/reports/category-summary` - Get expense totals by category
  ```json
  [
    {
      "categoryId": "cat1",
      "categoryName": "Groceries",
      "color": "#FF5733",
      "total": 500.00,
      "percentage": 33.33
    }
  ]
  ```

- `GET /api/dashboard/reports/monthly-summary` - Get monthly spending trends
  ```json
  [
    {
      "month": "2024-03",
      "total": 1500.00
    }
  ]
  ```

## Authentication

All endpoints except authentication endpoints require a valid JWT token in the Authorization header:
```
Authorization: Bearer <your_jwt_token>
```

## Environment Variables

The following environment variables are required:
- `JWT_KEY`: Secret key for JWT token generation
- `JWT_ISSUER`: JWT token issuer
- `JWT_AUDIENCE`: JWT token audience
- `MONGODB_CONNECTION_STRING`: MongoDB connection string
- `MONGODB_DATABASE_NAME`: MongoDB database name
- `EMAIL_SMTP_SERVER`: SMTP server address
- `EMAIL_SMTP_PORT`: SMTP server port
- `EMAIL_SMTP_USERNAME`: SMTP username
- `EMAIL_SMTP_PASSWORD`: SMTP password
- `EMAIL_FROM_ADDRESS`: Sender email address
- `EMAIL_FROM_NAME`: Sender name
- `EMAIL_ENABLE_SSL`: Enable SSL for email (true/false)

## Setup

1. Clone the repository
2. Set up environment variables
3. Install dependencies
4. Run the application:
   ```bash
   dotnet run
   ```

## Development

The API is built with:
- ASP.NET Core 7.0
- MongoDB.Driver
- JWT Authentication
- Swagger/OpenAPI

## Error Handling

The API returns appropriate HTTP status codes:
- 200: Success
- 400: Bad Request
- 401: Unauthorized
- 403: Forbidden
- 404: Not Found
- 500: Internal Server Error

## Security

- All endpoints are protected with JWT authentication
- Passwords are hashed using HMACSHA512
- Email verification is required for new accounts
- Category deletion is prevented if it's being used by expenses 