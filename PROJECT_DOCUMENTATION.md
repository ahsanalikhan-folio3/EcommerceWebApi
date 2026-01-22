# EcommerceApp API – Comprehensive Project Documentation

## 1. Project Overview

**EcommerceApp API** is a full-featured, role-based e-commerce backend built with modern ASP.NET Core best practices. It serves as the business logic and data backbone for a marketplace platform where sellers can list and manage products, customers can browse and purchase items, and administrators oversee platform operations. The system supports three distinct user roles—**Admin**, **Seller**, and **Customer**—each with tailored functionality, security, and business workflows. The platform is designed for scalability, maintainability, and compliance with enterprise-grade architecture patterns.

**Main Purpose & Business Domain:**  
This is a B2C marketplace where multiple sellers operate independently within a centralized platform. Customers discover products across sellers, place orders, provide feedback, and manage their purchase history. Sellers manage their product catalogs, track incoming orders, and interact with customers through real-time chat. Administrators perform oversight, user management, and platform governance.

**Target Users:**
- **Customers**: Browse products, place orders, submit reviews/ratings, engage in seller communication
- **Sellers**: Register, publish products, manage inventory, view orders, accept/reject/ship orders, respond to customer queries
- **Admins**: Manage users (activate/deactivate), view all orders, oversee platform health

**Key High-Level Features:**
- User authentication & role-based authorization (JWT-based)
- Product catalog management with seller isolation
- Shopping cart abstraction through Order and SellerOrder entities
- Real-time bidirectional chat (SignalR-based)
- Order lifecycle management with status tracking
- Customer feedback and rating system
- Email notifications (welcome, account changes, order updates via Hangfire)
- Admin user deactivation/reactivation with automatic notifications

---

## 2. Technology Stack & Major Choices

| Component | Technology | Version/Details |
|-----------|-----------|-----------------|
| **Framework** | ASP.NET Core | Web API (minimal/attribute routing) |
| **Language** | C# | .NET runtime |
| **Database** | SQL Server | Local or Docker-based |
| **ORM** | Entity Framework Core | DbContext-based |
| **Authentication** | JWT (JSON Web Tokens) | HS256 symmetric signing |
| **Authorization** | Role-based (RBAC) | [Authorize(Roles = "...")] attributes |
| **Dependency Injection** | Built-in .NET DI | IServiceCollection |
| **Mapping** | AutoMapper | DTO ↔ Entity transformations |
| **Validation** | FluentValidation | Fluent API + Custom Validators |
| **Real-time** | SignalR | WebSocket-based chat hub |
| **Background Jobs** | Hangfire | SQL Server storage, email job scheduling |
| **Password Security** | AspNetCore.Identity | PasswordHasher<T> |
| **Logging** | Built-in ILogger | Default .NET logging |
| **Exception Handling** | Global Middleware | Centralized error response formatting |
| **API Documentation** | Swagger/OpenAPI | SwaggerGen with JWT bearer definition |

**Architectural Style:** The project follows **Clean Architecture** principles, layered into:
- **Domain**: Entities and domain logic (no external dependencies)
- **Application**: Business logic, DTOs, validators, services, and interfaces
- **Infrastructure**: Database context, repositories, external services (email, background jobs)
- **API**: Controllers, filters, middleware, HTTP pipeline configuration

This layered approach ensures separation of concerns, testability, and independence from frameworks.

---

## 3. Project Structure (Folder & Solution Layout)

```
EcommerceAppApi/
├── EcommerceApp.Domain/
│   └── Entities/
│       ├── ApplicationUser.cs      (Base user record with email, password, phone, role)
│       ├── Product.cs              (Seller's catalog items)
│       ├── Order.cs                (Customer's order header)
│       ├── SellerOrder.cs          (Line items: product + quantity per order)
│       ├── SellerProfile.cs        (Seller-specific data)
│       ├── CustomerProfile.cs      (Customer-specific data)
│       ├── AdminProfile.cs         (Admin-specific data)
│       ├── Chat.cs                 (Seller ↔ Customer conversation)
│       ├── Message.cs              (Individual messages in chat)
│       └── Feedback.cs             (Customer reviews for products)
│
├── EcommerceApp.Application/
│   ├── Services/
│   │   ├── AuthService.cs          (Registration, login, user activation)
│   │   ├── ProductService.cs       (CRUD operations, seller isolation)
│   │   ├── OrderService.cs         (Order creation, status updates, feedback)
│   │   ├── ChatService.cs          (Message exchange)
│   │   ├── JwtService.cs           (Token generation & validation)
│   │   └── UserService.cs          (Current user context, role checking)
│   ├── Interfaces/
│   │   ├── IProductService, IAuthService, etc. (Service contracts)
│   │   ├── Repositories/            (IProductRepository, IOrderRepository, etc.)
│   │   └── JobServices/             (IBackgroundJobService, IEmailJob)
│   ├── Dtos/
│   │   ├── ProductDto, GetProductDto
│   │   ├── OrderDto, SellerOrderDto
│   │   ├── CustomerProfileDto, SellerProfileDto, AdminProfileDto
│   │   ├── LoginDto, RegisterUserDto
│   │   ├── ChatDto, MessageDto
│   │   ├── FeedbackDto
│   │   └── (other data transfer objects)
│   ├── Validators/
│   │   ├── ProductValidator.cs
│   │   ├── AuthValidator.cs
│   │   ├── OrderValidator.cs
│   │   ├── ChatValidator.cs
│   │   ├── FeedbackValidator.cs
│   │   └── (FluentValidation rules per DTO)
│   ├── MappingProfiles/
│   │   └── MappingProfile.cs       (AutoMapper configurations)
│   ├── Common/
│   │   ├── ApiResponse.cs          (Standard response wrapper)
│   │   ├── AppRoles.cs             (String constants: "Admin", "Seller", "Customer")
│   │   ├── JwtOptions.cs           (JWT configuration POCO)
│   │   └── EmailSettings.cs        (SMTP configuration)
│   ├── Exceptions/
│   │   └── BusinessRuleException.cs (Custom exception for business logic failures)
│   └── DependencyInjection.cs      (Service registration extension method)
│
├── EcommerceApp.Infrastructure/
│   ├── Database/
│   │   └── ApplicationDbContext.cs (Entity Framework Core DbContext)
│   ├── Repositories/
│   │   ├── ProductRepository.cs, OrderRepository.cs, etc.
│   │   └── (Data access implementations with DbContext queries)
│   ├── Jobs/
│   │   └── EmailJob.cs             (Hangfire background job for sending emails)
│   ├── JobService/
│   │   └── HangfireBackgroundJobService.cs (Queueing emails via Hangfire)
│   ├── Hubs/
│   │   └── ChatHub.cs              (SignalR hub for real-time messaging)
│   ├── Realtime/
│   │   └── SignalRRealtimeChatNotifier.cs (Chat event broadcaster)
│   ├── Migrations/
│   │   └── (EF Core migration files for schema versioning)
│   ├── UnitOfWork.cs               (Aggregates all repositories)
│   ├── Security/
│   │   └── JwtService.cs           (Token generation logic)
│   └── DependencyInjection.cs      (DbContext, repository, and infrastructure service registration)
│
├── EcommerceApp.Api/
│   ├── Controllers/
│   │   ├── AuthController.cs       (POST /api/auth/register/*, /api/auth/login)
│   │   ├── ProductsController.cs   (GET/POST/PUT/DELETE /api/products)
│   │   ├── OrdersController.cs     (GET/POST /api/orders, feedback submission)
│   │   └── ChatController.cs       (GET/POST /api/chats/messages)
│   ├── Filters/
│   │   └── ValidationFilter.cs     (Auto-validates all action parameters via FluentValidation)
│   ├── Middlewares/
│   │   └── GlobalExceptionHandlerMiddleware.cs (Catches unhandled exceptions, formats error responses)
│   ├── Program.cs                  (ASP.NET Core startup configuration)
│   ├── appsettings.json            (Connection strings, JWT, email SMTP)
│   ├── Properties/launchSettings.json (Hosting profile)
│   └── EcommerceApp.Api.http       (REST client examples for manual testing)
│
├── DB-Schema-File.sql              (Initial database schema)
├── docker-compose.yml              (SQL Server + API containerization)
├── Dockerfile                      (API image definition)
├── init-db.sh, dbrestore.sh        (Database initialization scripts)
└── sql-backup/                     (Database backup files)
```

**Why This Structure?**
- **Domain** isolates business entities from infrastructure, enabling unit testing without database dependencies.
- **Application** concentrates business logic and service workflows, making features discoverable.
- **Infrastructure** encapsulates data access and external service interactions, simplifying swapping implementations (e.g., a different email provider).
- **API** remains thin, delegating logic to services and leaving HTTP concerns to filters/middleware.

This separation ensures the team can work on different layers in parallel and reduces merge conflicts.

---

## 4. Core Domain Entities & Relationships

| Entity | Purpose | Key Fields | Relationships |
|--------|---------|-----------|---|
| **ApplicationUser** | Base identity record for all users | Id, Email, HashedPassword, PhoneNumber, Role, FullName, IsActive, CreatedAt | 1→many Chat (seller/customer), 1→many Message |
| **Product** | Catalog item owned by a seller | Id, SellerId, Name, Description, ProductSlug, Category, StockQuantity, Price, IsAvailable | many→many SellerOrder |
| **Order** | Header-level order placed by customer | Id, UserId, OrderDate, TotalAmount | 1→many SellerOrder |
| **SellerOrder** | Line item (product instance in order) | Id, OrderId, ProductId, Quantity, Status (enum) | many→1 Order, many→1 Product, 1→many Feedback |
| **SellerProfile** | Seller-specific metadata | Id (references ApplicationUser) | N/A (inherits from ApplicationUser) |
| **CustomerProfile** | Customer-specific metadata | Id (references ApplicationUser) | N/A (inherits from ApplicationUser) |
| **AdminProfile** | Admin-specific metadata | Id (references ApplicationUser) | N/A (inherits from ApplicationUser) |
| **Chat** | Conversation thread | Id, SellerId, CustomerId, CreatedAt | 1→many Message |
| **Message** | Individual message in chat | Id, ChatId, SenderId, Content, TimeSent, IsRead | many→1 Chat |
| **Feedback** | Customer review for a product | Id, SellerOrderId, CustomerId, Rating, Comment | many→1 SellerOrder |

**Important Relationships:**
- **Order → SellerOrder (1-to-many)**: An order aggregates multiple line items, one per seller's product. This allows customers to buy from multiple sellers in a single checkout.
- **Product → SellerOrder (1-to-many)**: Products can appear in many seller orders (line items across different orders).
- **SellerOrder → Feedback (1-to-many)**: After delivery, customers submit feedback for each line item.
- **Chat → Message (1-to-many)**: Conversations contain sequences of messages.
- **ApplicationUser → Chat (1-to-many, dual role)**: Sellers and customers are both ApplicationUsers participating in chats.

**Domain Rules & Invariants:**
1. Products belong to exactly one seller; sellers cannot modify others' products.
2. Orders are immutable once placed; status updates happen via SellerOrder state transitions.
3. Feedback can only be submitted after a SellerOrder reaches **Delivered** status.
4. Users are deactivated/reactivated by admins; deactivation prevents login.
5. Each chat connects exactly one seller and one customer; no group chats.

---

## 5. Key Features & Implementation Highlights

### 5.1 User Authentication & Authorization
**Description:**  
Role-based access control using JWT tokens. Users register as Customer or Seller; Admins are created externally (commented-out endpoint). Passwords are hashed using ASP.NET Identity's PasswordHasher.

**Main Endpoints:**
- `POST /api/auth/register/customer` – Register as customer (auto-activated)
- `POST /api/auth/register/seller` – Register as seller (requires admin activation)
- `POST /api/auth/login` – Authenticate and receive JWT
- `GET /api/auth/users/profile` – Retrieve authenticated user profile
- `PATCH /api/auth/users/{id}` – Admin activates/deactivates users

**Implementation Location:**  
`AuthService.cs`, `AuthRepository.cs`, `AuthValidator.cs`, `JwtService.cs`

**Special Details:**
- JWT signed with HS256 (symmetric key); issuer and audience validated.
- `GlobalExceptionHandlerMiddleware` catches login errors and formats them consistently.
- Sellers require admin approval; customers are auto-approved.
- Background jobs (`IBackgroundJobService`) send welcome/activation emails asynchronously via Hangfire.

---

### 5.2 Product Catalog & Seller Management
**Description:**  
Sellers can independently publish, update, and delete their products. Customers and guests can browse all products. Search and filtering happen at the application/repository level.

**Main Endpoints:**
- `GET /api/products` – List all products (or seller's own products if `?mine=true` and authenticated as Seller)
- `GET /api/products/{id}` – Fetch single product details
- `POST /api/products` – Seller creates new product
- `PUT /api/products/{id}` – Seller updates their product
- `DELETE /api/products/{id}` – Seller soft-deletes their product
- `GET /api/products/{id}/feedbacks` – Seller views product reviews

**Implementation Location:**  
`ProductService.cs`, `ProductRepository.cs`, `ProductValidator.cs`

**Special Details:**
- Authorization checks `ProductBelongsToSellerAsync()` to enforce seller isolation.
- Products include slug for SEO-friendly URLs (not currently used in endpoints).
- Soft delete is supported (IsAvailable flag can mark products as inactive).
- Stock quantity decrements when orders are placed.

---

### 5.3 Shopping Cart & Order Placement
**Description:**  
The "cart" is implicit in the order creation flow. Customers specify products and quantities directly in an OrderDto; the API creates an Order header and SellerOrder line items in one transaction.

**Main Endpoints:**
- `POST /api/orders` – Customer creates order (specifies line items: product IDs and quantities)
- `GET /api/orders` – Customer, Seller, or Admin retrieves relevant orders

**Implementation Location:**  
`OrderService.cs`, `OrderRepository.cs`, `OrderValidator.cs`

**Special Details:**
- No persistent cart entity; orders are created atomically.
- Stock validation and decrement happen in `CreateOrderAsync()`.
- Role-based filtering: Admin sees all orders, Sellers see their items, Customers see their orders.
- Email notifications triggered on order creation (background job).

---

### 5.4 Order Management & Status Lifecycle
**Description:**  
Sellers manage individual SellerOrder items through a state machine. Customers can cancel orders or leave feedback post-delivery. Admins have full visibility and can override statuses.

**Order Status Enum:**
```
Pending → Processing → InWarehouse → Shipped → Delivered → (or Cancelled)
```

**Main Endpoints:**
- `PUT /api/orders/{id}` – Update SellerOrder status (different rules for Seller/Admin/Customer)
- `POST /api/orders/{id}/feedback` – Customer submits feedback after delivery

**Implementation Location:**  
`OrderService.cs`, `UpdateSellerOrderStatusDto`, `FeedbackDto`

**Special Details:**
- Sellers can accept (Processing) or reject (Cancelled) pending orders.
- Admins can force-update any status.
- Customers can only cancel if order is Pending; they can leave feedback only if Delivered.
- Feedback validation ensures no duplicate reviews per SellerOrder.

---

### 5.5 Real-Time Chat System
**Description:**  
Sellers and customers exchange messages via WebSocket (SignalR). Messages are persisted and retrievable by chat history. Real-time notifications push incoming messages to connected clients.

**Main Endpoints:**
- `GET /api/chats` – Retrieve chat list for user
- `POST /api/chats/messages` – Send message (WebSocket handled by SignalR Hub)

**SignalR Hub:**
- `ChatHub` at `/hubs/chat` – Manages client connections and broadcasts messages
- Methods: `SendMessage()`, `ReceiveMessage()`

**Implementation Location:**  
`ChatService.cs`, `ChatRepository.cs`, `MessageRepository.cs`, `ChatHub.cs`, `SignalRRealtimeChatNotifier.cs`

**Special Details:**
- Chat is one-to-one (seller ↔ customer), enforced by repository queries.
- Messages are transient until explicitly saved to database.
- `SignalRRealtimeChatNotifier` broadcasts via groups (e.g., "chat-{chatId}").
- Validation ensures users are participants in the chat before sending.

---

### 5.6 Customer Feedback & Rating System
**Description:**  
After a SellerOrder is delivered, customers submit ratings and comments. Sellers can view aggregated feedback for their products.

**Main Endpoints:**
- `POST /api/orders/{id}/feedback` – Submit review (already listed in 5.4)
- `GET /api/products/{id}/feedbacks` – Seller retrieves feedback for their product

**Implementation Location:**  
`FeedbackService` (implicit in `OrderService`), `FeedbackRepository.cs`, `FeedbackValidator.cs`

**Special Details:**
- Feedback is immutable once submitted (no edit/delete).
- Rating is typically numeric (1–5); comment is optional.
- Seller access verified via seller-product relationship.

---

### 5.7 Email Notifications & Background Jobs
**Description:**  
Time-consuming email operations (welcome, account changes, order updates) are offloaded to Hangfire background jobs, preventing request blocking.

**Job Types:**
- `EnqueueCustomerWelcomeEmailJob()` – Welcome email on customer registration
- `EnqueueAccountActivationEmailJob()` – Account re-enabled notification
- `EnqueueAccountDeactivationEmailJob()` – Account disabled notification

**Implementation Location:**  
`HangfireBackgroundJobService.cs`, `EmailJob.cs`, `EmailSettings` configuration

**Special Details:**
- Hangfire uses SQL Server (configured in appsettings as `HangFireDbConnection`).
- SMTP credentials (Gmail app password) stored in `EmailSettings`.
- Retry logic and dead-letter handling handled by Hangfire.

---

### 5.8 Admin User Governance
**Description:**  
Admins can activate/deactivate user accounts, triggering notifications. Deactivated users cannot log in.

**Main Endpoints:**
- `PATCH /api/auth/users/{id}` – Toggle user active status

**Implementation Location:**  
`AuthService.ChangeUserActivationStatus()`, `UserActivationDto`

**Special Details:**
- Validation ensures user exists before status change.
- Email notifications automatically triggered based on new status.
- Role enforcement via `[Authorize(Roles = AppRoles.Admin)]`.

---

## 6. Security & Authorization

**Authentication Mechanism:**  
JWT tokens are issued at login. Each token encodes the user ID, role, email, and expiration. Clients include the token in the `Authorization: Bearer <token>` header. The server validates the signature (HS256 with a shared secret key) and checks expiration.

**JWT Configuration (appsettings.json):**
```json
"Jwt": {
    "Key": "THIS_SHOULD_BE_A_LONG_RANDOM_SECRET_KEY_32+_CHARS",
    "Issuer": "EcommerceWebApi",
    "Audience": "EcommerceWebApiUsers",
    "AccessTokenExpirySeconds": 60
}
```

**Authorization Strategy:**  
Role-based access control (RBAC) using `[Authorize(Roles = "Admin,Seller")]` attributes on controller actions. The framework checks the user's role claim against the specified roles; if no match, a 403 Forbidden is returned.

**Protected Endpoints & Role Requirements:**
| Endpoint | Roles | Notes |
|----------|-------|-------|
| `POST /api/products` | Seller | Only sellers can create products |
| `PUT /api/products/{id}` | Seller | Sellers can only update their own products (checked via `ProductBelongsToSellerAsync()`) |
| `DELETE /api/products/{id}` | Seller | Same seller isolation as above |
| `POST /api/orders` | Customer | Only customers can place orders |
| `PUT /api/orders/{id}` | Seller, Admin, Customer | Role-specific logic inside the action |
| `PATCH /api/auth/users/{id}` | Admin | Only admins can change user statuses |
| `GET /api/auth/users/profile` | Any authenticated user | Returns own profile |

**Seller-Specific Authorization Logic:**  
Endpoints that modify seller-owned resources (products, orders) validate ownership:
```csharp
if (!(await productService.ProductBelongsToSellerAsync(id)))
    return Unauthorized(...);
```

This ensures a Seller cannot modify another Seller's products or orders.

**Public Endpoints:**
- `GET /api/products` – Browse all products (AllowAnonymous)
- `GET /api/products/{id}` – View single product (AllowAnonymous)
- `POST /api/auth/register/customer` – Registration (AllowAnonymous)
- `POST /api/auth/register/seller` – Registration (AllowAnonymous)
- `POST /api/auth/login` – Authentication (AllowAnonymous)

---

## 7. Data Access & Persistence

**Database:**  
SQL Server (LocalDB for development, Docker container for deployment). Configured via connection string in appsettings.

**Connection Strings (appsettings.json):**
```json
"ConnectionStrings": {
    "DbConnection": "Data Source=(localdb)\\MSSQLLocalDB;Database=EcommerceWebApiDb;...",
    "DbConnectionThroughDocker": "Server=sqlserver,1433;Database=EcommerceWebApiDb;...",
    "HangFireDbConnection": "..."
}
```

**Entity Framework Core Configuration:**  
DbContext (`ApplicationDbContext`) configured with:
- SQL Server provider via `UseSqlServer(connectionString)`
- Entity mappings and relationships defined via Fluent API (in OnModelCreating)
- Migration support for schema versioning

**Repository Pattern:**  
Each entity has a repository interface (e.g., `IProductRepository`) and implementation (e.g., `ProductRepository`). Repositories encapsulate DbContext queries.

```csharp
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> AddAsync(Product product);
    Task<bool> DeleteAsync(int id);
    // ...
}
```

**Unit of Work Pattern:**  
`UnitOfWork` aggregates all repositories and provides a single SaveChangesAsync() point:
```csharp
public class UnitOfWork : IUnitOfWork
{
    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }
    // ...
    public async Task SaveChangesAsync();
}
```

**Lazy Loading & Eager Loading:**  
Not explicitly configured; navigation properties are loaded on-demand or through explicit `.Include()` calls in repository queries. No global query filters observed.

**Data Validation & Constraints:**
- Entity properties marked `required` enforce non-null.
- Foreign key relationships enforced by EF Core conventions.
- Unique constraints on Email (via repository queries checking existence).

---

## 8. Error Handling & Validation

**Global Exception Handling:**  
All unhandled exceptions are caught by `GlobalExceptionHandlerMiddleware`, logged, and returned as a standardized JSON response. In development, the exception message is included; in production, a generic "internal server error" message is sent.

**Middleware Code Snippet:**
```csharp
public async Task InvokeAsync(HttpContext context)
{
    try {
        await _next(context);
    }
    catch (Exception ex) {
        _logger.LogError(ex, "Unhandled exception occurred.");
        await HandleExceptionAsync(context, ex);
    }
}
```

**Standard Response Shape:**
```json
{
    "success": true/false,
    "message": "Human-readable message",
    "data": { /* response data */ },
    "errors": null // or { "PropertyName": ["Error message"] }
}
```

**FluentValidation:**  
Input validation rules are defined in `*Validator.cs` classes (e.g., `ProductValidator`):
```csharp
public class ProductValidator : AbstractValidator<ProductDto>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(3);
        RuleFor(p => p.Price).GreaterThan(0);
        // ...
    }
}
```

**Validation Filter:**  
`ValidationFilter` (an `IAsyncActionFilter`) automatically validates all action parameters:
1. For each action argument, find a matching `IValidator<T>`.
2. Run validation.
3. If invalid, return 400 BadRequest with errors dictionary.
4. If valid, proceed to the action.

**Validation Response Example:**
```json
{
    "success": false,
    "message": "Validations Failed.",
    "errors": {
        "Name": ["'Name' must not be empty."],
        "Price": ["'Price' must be greater than 0."]
    }
}
```

**Business Logic Exceptions:**  
`BusinessRuleException` is a custom exception for domain rule violations (e.g., "Seller cannot access this product"). It is caught and handled by the global middleware.

---

## 9. Current Limitations & Known Technical Debt

1. **No Payment Integration**: Stripe integration is planned but not yet implemented. Orders are created but no actual payments are processed. (See Todos.txt)

2. **Admin Creation**: The `RegisterAdmin` endpoint is commented out. Admin users must be created manually in the database or via a back-office tool.

3. **Docker Setup Incomplete**: Dockerfiles exist, but the multi-container orchestration (SQL Server + API) via docker-compose is not fully tested in production.

4. **Email Verification for Sellers**: Sellers are not required to verify their email; they are auto-activated. This creates a security gap.

5. **No Rate Limiting**: Endpoints lack rate-limit protection; a malicious client could spam the API.

6. **Missing Audit Logging**: User actions (product updates, order status changes) are not logged for audit trails.

7. **No Full-Text Search**: Product search is basic (by ID or seller ID). Full-text search for product names/descriptions is not implemented.

8. **Soft Delete Not Fully Utilized**: Products have an `IsAvailable` flag, but soft deletion is inconsistently applied across entities.

9. **Limited Concurrency Handling**: No optimistic locking (ETag/version columns) on frequently-updated entities like Orders. Race conditions during concurrent status updates are possible.

10. **JWT Token Refresh**: Tokens have short expiry (60 seconds in config) but no refresh token mechanism; clients must re-authenticate frequently.

---

## 10. Getting Started / Setup Instructions

### Prerequisites
- **.NET SDK**: 6.0 or later
- **SQL Server**: LocalDB (Windows) or Docker
- **Visual Studio** or **VS Code** with C# extension
- **Git** (for cloning)

### Local Development Setup

#### 1. Clone & Restore Dependencies
```bash
git clone <repository-url>
cd EcommerceAppApi
dotnet restore
```

#### 2. Database Configuration
**Option A: Using LocalDB (Windows)**
- Connection string is already configured in `appsettings.json` for LocalDB.
- Ensure SQL Server Express LocalDB is installed.

**Option B: Using Docker**
- Run SQL Server container:
  ```bash
  docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Strong!Pass123" \
    -p 1433:1433 --name sqlserver \
    mcr.microsoft.com/mssql/server:latest
  ```
- Update `appsettings.json` connection strings to use the Docker connection.

#### 3. Apply Migrations
```bash
cd EcommerceApp.Api
dotnet ef database update --project ../EcommerceApp.Infrastructure
```
This creates the database schema.

#### 4. Run the Application
```bash
dotnet run
```
API starts on `https://localhost:5001` (or port specified in `launchSettings.json`).

#### 5. Access Swagger UI
Navigate to `https://localhost:5001/swagger` to explore endpoints interactively.

### Important Environment Variables / Appsettings Keys

| Key | Purpose | Example |
|-----|---------|---------|
| `ConnectionStrings.DbConnection` | Database connection (LocalDB) | `Data Source=(localdb)\\...` |
| `Jwt.Key` | Secret key for token signing | `THIS_SHOULD_BE_A_LONG_RANDOM_...` |
| `Jwt.Issuer` | Token issuer claim | `EcommerceWebApi` |
| `Jwt.Audience` | Token audience claim | `EcommerceWebApiUsers` |
| `Jwt.AccessTokenExpirySeconds` | Token validity (seconds) | `60` |
| `EmailSettings.Host` | SMTP server | `smtp.gmail.com` |
| `EmailSettings.Port` | SMTP port | `587` |
| `EmailSettings.Username` | SMTP account | `your-email@gmail.com` |
| `EmailSettings.AppPassword` | SMTP app password (Gmail) | `xxxx xxxx xxxx xxxx` |

### Seed Data
Currently, no automated seed data script exists. To populate test data:
1. Create users via `/api/auth/register/*` endpoints.
2. Create products via `/api/products` (as Seller).
3. Place orders via `/api/orders` (as Customer).

### Launching in VS Code
1. Install **C# Dev Kit** extension.
2. Open the workspace: `code .`
3. Press `F5` to start debugging (or `Ctrl+F5` for running without debugging).
4. Set breakpoints and use the Debug Console as needed.

### Key Launch Settings (launchSettings.json)
```json
"profiles": {
    "EcommerceApp.Api": {
        "commandName": "Project",
        "launchBrowser": true,
        "launchUrl": "swagger",
        "applicationUrl": "https://localhost:7001;http://localhost:5001",
        "environmentVariables": {
            "ASPNETCORE_ENVIRONMENT": "Development"
        }
    }
}
```

---

## Conclusion

The **EcommerceApp API** is a well-structured, enterprise-ready e-commerce backend leveraging modern C# and .NET patterns. Its Clean Architecture separates concerns effectively, making the codebase maintainable and testable. The role-based authorization, real-time chat, and async email jobs demonstrate thoughtful engineering. Future enhancements—payment processing, full-text search, audit logging, and rate limiting—will further strengthen the platform's capabilities and security posture.

For questions or contributions, refer to the [Contributing Guidelines] (if any) or contact the development team.
