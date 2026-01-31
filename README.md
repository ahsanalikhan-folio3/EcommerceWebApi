# EcommerceApp API – Backend Architecture & Comprehensive Documentation

## 1. Project Overview

**EcommerceApp API** is a robust, role-based e-commerce backend built with modern ASP.NET Core best practices. It serves as the complete business logic and data backbone for a DARAZ-like, warehouse-based B2C marketplace platform, where multiple sellers operate independently within a centralized system. The platform supports three distinct user roles — **Admin**, **Seller**, and **Customer** — each with tailored functionality, security, and business workflows, enabling efficient product management, order processing, and real-time interactions across the marketplace.

**Core Purpose:**  
This platform enables a multi-vendor marketplace where:

- **Customers** discover products across sellers, place orders, provide feedback, and manage purchase history
- **Sellers** register and manage their product catalogs, track incoming orders, and interact with customers via real-time chat
- **Admins** oversee user management, platform health, and system governance

**Key High-Level Features:**

- User authentication & role-based authorization (JWT-based)
- Product catalog management with seller isolation
- Order management with state machine status tracking
- Real-time bidirectional chat (SignalR-based)
- Customer feedback and rating system
- Asynchronous email notifications (Hangfire-based)
- Admin user governance (activate/deactivate accounts)

---

## 2. Technology Stack & Major Choices

| Component                | Technology            | Version/Details                          |
| ------------------------ | --------------------- | ---------------------------------------- |
| **Framework**            | ASP.NET Core          | Web API (minimal/attribute routing)      |
| **Language**             | C#                    | .NET runtime                             |
| **Database**             | SQL Server            | Local or Docker-based                    |
| **ORM**                  | Entity Framework Core | DbContext-based                          |
| **Authentication**       | JWT (JSON Web Tokens) | HS256 symmetric signing                  |
| **Authorization**        | Role-based (RBAC)     | [Authorize(Roles = "...")] attributes    |
| **Dependency Injection** | Built-in .NET DI      | IServiceCollection                       |
| **Mapping**              | AutoMapper            | DTO ↔ Entity transformations             |
| **Validation**           | FluentValidation      | Fluent API + Custom Validators           |
| **Real-time**            | SignalR               | WebSocket-based chat hub                 |
| **Background Jobs**      | Hangfire              | SQL Server storage, email job scheduling |
| **Password Security**    | AspNetCore.Identity   | PasswordHasher<T>                        |
| **Logging**              | Built-in ILogger      | Default .NET logging                     |
| **Exception Handling**   | Global Middleware     | Centralized error response formatting    |
| **API Documentation**    | Swagger/OpenAPI       | SwaggerGen with JWT bearer definition    |

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
├── docker-compose.yml              (SQL Server + API containerization)
├── Dockerfile                      (API image definition)
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

| Entity              | Purpose                               | Key Fields                                                                                | Relationships                                 |
| ------------------- | ------------------------------------- | ----------------------------------------------------------------------------------------- | --------------------------------------------- |
| **ApplicationUser** | Base identity record for all users    | Id, Email, HashedPassword, PhoneNumber, Role, FullName, IsActive, CreatedAt               | 1→many Chat (seller/customer), 1→many Message |
| **Product**         | Catalog item owned by a seller        | Id, SellerId, Name, Description, ProductSlug, Category, StockQuantity, Price, IsAvailable | many→many SellerOrder                         |
| **Order**           | Header-level order placed by customer | Id, UserId, OrderDate, TotalAmount                                                        | 1→many SellerOrder                            |
| **SellerOrder**     | Line item (product instance in order) | Id, OrderId, ProductId, Quantity, Status (enum)                                           | many→1 Order, many→1 Product, 1→many Feedback |
| **SellerProfile**   | Seller-specific metadata              | Id (references ApplicationUser)                                                           | N/A (inherits from ApplicationUser)           |
| **CustomerProfile** | Customer-specific metadata            | Id (references ApplicationUser)                                                           | N/A (inherits from ApplicationUser)           |
| **AdminProfile**    | Admin-specific metadata               | Id (references ApplicationUser)                                                           | N/A (inherits from ApplicationUser)           |
| **Chat**            | Conversation thread                   | Id, SellerId, CustomerId, CreatedAt                                                       | 1→many Message                                |
| **Message**         | Individual message in chat            | Id, ChatId, SenderId, Content, TimeSent, IsRead                                           | many→1 Chat                                   |
| **Feedback**        | Customer review for a product         | Id, SellerOrderId, CustomerId, Rating, Comment                                            | many→1 SellerOrder                            |

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

## 5. Complete API Endpoint Reference

### 5.1 Authentication Endpoints

#### POST /api/Auth/Register/Customer

**Access Level:** Public (AllowAnonymous)  
**Purpose:** Register a new customer account

**Request Body (`CustomerProfileDto`):**

```json
{
  "email": "customer@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!",
  "role": "Customer",
  "phoneNumber": "+1234567890",
  "fullName": "John Doe",
  "houseNumber": "123",
  "streetNumber": "Main St",
  "city": "Springfield",
  "state": "IL",
  "postalCode": "62701",
  "country": "USA",
  "gender": "Male"
}
```

**Response (200 OK):**

- Customer profile with `IsActive = true` (auto-activated)
- Can immediately log in
- Welcome email sent asynchronously via Hangfire

**Validations:**

- Email must be unique
- Password must meet complexity requirements
- confirmPassword must match password
- All required fields present

---

#### POST /api/Auth/Register/Seller

**Access Level:** Public (AllowAnonymous)  
**Purpose:** Register a new seller account (pending admin approval)

**Request Body (`SellerProfileDto`):**

```json
{
  "email": "seller@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!",
  "role": "Seller",
  "phoneNumber": "+1234567890",
  "fullName": "Jane Smith",
  "storename": "Smith's Electronics",
  "city": "Springfield",
  "state": "IL",
  "postalCode": "62701",
  "country": "USA"
}
```

**Response (200 OK):**

- Seller profile with `IsActive = false` (awaiting admin approval)
- Cannot log in until activated by admin
- Admin notification email sent asynchronously

**Validations:**

- Email must be unique
- Store name must be unique
- Password complexity validated

---

#### POST /api/Auth/Login

**Access Level:** Public (AllowAnonymous)  
**Purpose:** Authenticate user and receive JWT token

**Request Body (`LoginDto`):**

```json
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": 1,
      "email": "user@example.com",
      "fullName": "John Doe",
      "role": "Customer",
      "isActive": true
    }
  }
}
```

**Error Responses:**

- `401 Unauthorized` – Invalid credentials or user not found
- `401 Unauthorized` – User account is deactivated

**Token Details:**

- Signed with HS256
- Includes claims: userId, role, email
- Expires after `Jwt.AccessTokenExpirySeconds` (default: 60 seconds)
- Include in subsequent requests: `Authorization: Bearer <token>`

---

#### GET /api/Auth/Users/Profile

**Access Level:** Authenticated (any role)  
**Purpose:** Retrieve current authenticated user's profile

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Profile retrieved",
  "data": {
    "id": 1,
    "email": "user@example.com",
    "fullName": "John Doe",
    "role": "Customer",
    "phoneNumber": "+1234567890",
    "isActive": true,
    "createdAt": "2024-01-20T10:30:00Z"
  }
}
```

---

#### PATCH /api/Auth/Users/{id}

**Access Level:** Authenticated (Admin only)  
**Purpose:** Activate or deactivate a user account  
**Path Parameter:** `id` (integer) – User ID to modify

**Request Body (`UserActivationDto`):**

```json
{
  "isActive": true
}
```

**Response (200 OK):**

- Updated user with new IsActive status
- Status change email sent to user (activation or deactivation notification)

**Authorization Checks:**

- User must be Admin role
- Target user must exist

---

### 5.2 Product Management Endpoints

#### GET /api/Products

**Access Level:** Public (AllowAnonymous)  
**Purpose:** List all available products

**Query Parameters:**

- `mine` (boolean, optional) – If true and authenticated as Seller, returns only that seller's products

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Products retrieved",
  "data": [
    {
      "id": 1,
      "sellerId": 2,
      "name": "Wireless Mouse",
      "productSlug": "wireless-mouse-logitech",
      "description": "Precision wireless mouse",
      "category": "Accessories",
      "stockQuantity": 150,
      "price": 29.99,
      "isAvailable": true,
      "createdAt": "2024-01-15T08:00:00Z"
    }
  ]
}
```

---

#### GET /api/Products/{id}

**Access Level:** Public (AllowAnonymous)  
**Purpose:** Retrieve details of a specific product  
**Path Parameter:** `id` (integer) – Product ID

**Response (200 OK):** Single product object (same schema as list endpoint)

---

#### POST /api/Products

**Access Level:** Authenticated (Seller only)  
**Purpose:** Create a new product in seller's catalog

**Request Body (`ProductDto`):**

```json
{
  "name": "USB-C Cable",
  "productSlug": "usb-c-cable-braided",
  "description": "Durable braided USB-C charging cable",
  "category": "Cables & Adapters",
  "stockQuantity": 500,
  "price": 14.99,
  "isAvailable": true
}
```

**Response (200 OK):**

- Created product with auto-generated `id` and `sellerId` from JWT token

**Validations:**

- All required fields present
- Price > 0
- Stock quantity >= 0
- Product name and slug unique per seller (enforced at repository level)

---

#### PUT /api/Products?id={id}

**Access Level:** Authenticated (Seller only)  
**Purpose:** Update an existing product  
**Query Parameter:** `id` (integer) – Product ID

**Request Body:** Same as POST (ProductDto)

**Response (200 OK):** Updated product

**Authorization Check:** Seller can only update own products (`ProductBelongsToSellerAsync()` verification)

---

#### DELETE /api/Products/{id}

**Access Level:** Authenticated (Seller only)  
**Purpose:** Soft-delete a product from catalog  
**Path Parameter:** `id` (integer) – Product ID

**Response (200 OK):** Confirmation message

**Behavior:**

- Sets `IsAvailable = false` (soft delete)
- Existing orders referencing this product remain unaffected
- Product data retained for audit trails

**Authorization Check:** Seller can only delete own products

---

#### GET /api/Products/{id}/Feedbacks

**Access Level:** Authenticated (Seller only)  
**Purpose:** Retrieve all customer feedback for a product  
**Path Parameter:** `id` (integer) – Product ID

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Feedbacks retrieved",
  "data": [
    {
      "id": 1,
      "sellerOrderId": 10,
      "customerId": 3,
      "rating": 5,
      "comment": "Excellent product, fast delivery!",
      "createdAt": "2024-01-18T14:30:00Z"
    }
  ]
}
```

**Authorization Check:** Only the product's seller can access

---

#### GET /api/Products/{id}/Orders

**Access Level:** Authenticated (Seller only)  
**Purpose:** Retrieve all seller orders (line items) for a product  
**Path Parameter:** `id` (integer) – Product ID

**Response (200 OK):** Array of seller orders for this product

**Authorization Check:** Only the product's seller can access

---

### 5.3 Order Management Endpoints

#### GET /api/Orders

**Access Level:** Authenticated (any role)  
**Purpose:** Retrieve orders (filtered by user role)

**Response Filtering:**

- **Customer:** Returns only their orders
- **Seller:** Returns all SellerOrder items from their products
- **Admin:** Returns all orders in system

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Orders retrieved",
  "data": [
    {
      "id": 100,
      "userId": 3,
      "orderDate": "2024-01-24T12:00:00Z",
      "totalAmount": 1059.97,
      "sellerOrders": [
        {
          "id": 1,
          "productId": 1,
          "productName": "Wireless Mouse",
          "quantity": 2,
          "unitPrice": 29.99,
          "status": 1,
          "createdAt": "2024-01-24T12:00:00Z"
        }
      ]
    }
  ]
}
```

---

#### POST /api/Orders

**Access Level:** Authenticated (Customer only)  
**Purpose:** Create a new order

**Request Body (`OrderDto`):**

```json
{
  "sellerOrders": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 5,
      "quantity": 1
    }
  ]
}
```

**Response (200 OK):**

- Created order with SellerOrder line items (one per seller)
- Initial status: `Pending`

**Validations:**

- All products exist and are available
- Stock quantity sufficient for each product
- Quantity > 0

**Transaction:** Atomic operation

- Order header created
- SellerOrder items created
- Stock decremented for each product
- All committed or all rolled back

**Side Effects:**

- Order confirmation email queued for customer
- Seller notification for incoming orders

---

#### PUT /api/Orders/{id}

**Access Level:** Authenticated (Seller, Admin, or Customer)  
**Purpose:** Update seller order status  
**Path Parameter:** `id` (integer) – SellerOrder ID

**Request Body (`UpdateSellerOrderStatusDto`):**

```json
{
  "status": 2
}
```

**Status Enum Values:**

- `1` = Pending
- `2` = Processing (Seller accepted)
- `3` = InWarehouse
- `4` = Shipped
- `5` = Delivered
- `6` = Cancelled

**Allowed Transitions by Role:**

| Role         | Allowed Transitions                       |
| ------------ | ----------------------------------------- |
| **Seller**   | Pending→Processing, Pending→Cancelled     |
| **Admin**    | Any transition (override capability)      |
| **Customer** | Pending→Cancelled (before seller accepts) |

**Response (200 OK):** Updated SellerOrder

**Error Responses:**

- `409 Conflict` – Invalid status transition
- `403 Forbidden` – User not authorized to update

**Side Effects:**

- Email notification sent on key transitions
- Feedback becomes available after status reaches Delivered

---

#### POST /api/Orders/{id}/Feedback

**Access Level:** Authenticated (Customer only)  
**Purpose:** Submit feedback/review for a delivered product  
**Path Parameter:** `id` (integer) – SellerOrder ID

**Request Body (`FeedbackDto`):**

```json
{
  "rating": 5,
  "comment": "Excellent quality and fast shipping!"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Feedback submitted successfully",
  "data": {
    "id": 50,
    "sellerOrderId": 10,
    "customerId": 3,
    "rating": 5,
    "comment": "Excellent quality and fast shipping!",
    "createdAt": "2024-01-24T13:00:00Z"
  }
}
```

**Validations:**

- SellerOrder must be in **Delivered** status
- Rating must be 1-5
- Customer must own the parent Order
- No duplicate feedback for same SellerOrder
- Feedback is immutable (no edit/delete after creation)

**Error Responses:**

- `400 BadRequest` – Order not yet delivered or validation failed
- `409 Conflict` – Feedback already exists for this SellerOrder

---

### 5.4 Real-Time Chat Endpoints

#### POST /api/Chat

**Access Level:** Authenticated (any role)  
**Purpose:** Create or retrieve one-to-one chat

**Request Body (`CreateChatDto`):**

```json
{
  "sellerId": 2
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Chat created or retrieved",
  "data": {
    "id": 15,
    "sellerId": 2,
    "customerId": 3,
    "createdAt": "2024-01-20T09:00:00Z",
    "isActive": true,
    "messages": []
  }
}
```

**Behavior:**

- Creates new chat if none exists between seller and current user
- Returns existing chat if already created
- Enforces one-to-one relationship

---

#### GET /api/Chat/{id}

**Access Level:** Authenticated (chat participants only)  
**Purpose:** Retrieve chat history with messages  
**Path Parameter:** `id` (integer) – Chat ID

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Chat retrieved",
  "data": {
    "id": 15,
    "sellerId": 2,
    "customerId": 3,
    "createdAt": "2024-01-20T09:00:00Z",
    "messages": [
      {
        "id": 1,
        "chatId": 15,
        "senderId": 3,
        "content": "Hi, I'm interested in your product",
        "timeSent": "2024-01-20T09:10:00Z",
        "isRead": true
      },
      {
        "id": 2,
        "chatId": 15,
        "senderId": 2,
        "content": "Thanks! I can help you with this.",
        "timeSent": "2024-01-20T09:15:00Z",
        "isRead": true
      }
    ]
  }
}
```

**Authorization Check:** User must be chat participant (seller or customer)

---

#### POST /api/Chat/{id}/Message

**Access Level:** Authenticated (chat participants only)  
**Purpose:** Send message (persisted and broadcast via SignalR)  
**Path Parameter:** `id` (integer) – Chat ID

**Request Body (`SendMessageDto`):**

```json
{
  "content": "When will this item be shipped?"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Message sent successfully",
  "data": {
    "id": 3,
    "chatId": 15,
    "senderId": 3,
    "content": "When will this item be shipped?",
    "timeSent": "2024-01-20T09:20:00Z",
    "isRead": false
  }
}
```

**Real-Time Behavior:**

- Message persisted to database
- Broadcast to SignalR group $`{userId}`
- Connected clients receive notification instantly

**Validations:**

- Sender must be chat participant
- Content not empty
- Chat must be active (not closed)

---

#### PATCH /api/Chat/{id}/MarkAsRead

**Access Level:** Authenticated (chat participants only)  
**Purpose:** Mark all unread messages as read  
**Path Parameter:** `id` (integer) – Chat ID

**Response (200 OK):** Confirmation message

---

#### PATCH /api/Chat/{id}/Close

**Access Level:** Authenticated (chat participants only)  
**Purpose:** Close a chat thread  
**Path Parameter:** `id` (integer) – Chat ID

**Response (200 OK):** Confirmation message

**Behavior:**

- Marks chat as inactive
- Participants cannot send new messages
- Chat history remains retrievable

---

### 5.5 Product Image Management Endpoints

#### POST /api/Products/{id}/Images

**Access Level:** Authenticated (Seller only)  
**Purpose:** Upload product images  
**Path Parameter:** `id` (integer) – Product ID

**Request Body:** `ProductImageUploadDto` (multipart/form-data)

```
Files: [image1.jpg, image2.png, ...]
```

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Product images added successfully",
  "data": null
}
```

**Validations:**

- Files must be valid image formats (jpg, png, etc.)
- Product must belong to authenticated seller
- Total images per product should not exceed limit (if configured)

**Authorization Check:** Only the product's seller can upload images

---

#### DELETE /api/Products/{id}/Images/{imageId}

**Access Level:** Authenticated (Seller only)  
**Purpose:** Delete a product image  
**Path Parameters:**

- `id` (integer) – Product ID
- `imageId` (integer) – Image ID

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Product image deleted successfully",
  "data": null
}
```

**Authorization Check:** Only the product's seller can delete images

---

## 6. Implementation Architecture & Key Services

### 6.1 User Authentication Service

**Implementation Location:** `AuthService.cs`, `AuthRepository.cs`, `AuthValidator.cs`, `JwtService.cs`

**Key Workflows:**

1. **Customer Registration**
   - Validates email uniqueness
   - Hashes password via `PasswordHasher<ApplicationUser>`
   - Creates ApplicationUser + CustomerProfile in transaction
   - Sets IsActive = true (auto-activated)
   - Enqueues welcome email job
   - Customer can immediately log in

2. **Seller Registration**
   - Validates email and store name uniqueness
   - Hashes password
   - Creates ApplicationUser + SellerProfile
   - Sets IsActive = false (awaiting admin approval)
   - Enqueues admin notification email
   - Seller cannot log in until admin activates

3. **Login Process**
   - Validates email exists and credentials correct
   - Checks IsActive flag (rejects deactivated users)
   - Generates JWT token with claims (userId, role, email)
   - Token signed with HS256 using key from `Jwt.Key`
   - Token expires after `Jwt.AccessTokenExpirySeconds`

4. **Token Validation**
   - Verified on every protected endpoint
   - Signature validated with HS256 key
   - Expiration checked; expired tokens rejected
   - Claims extracted for authorization checks

**Special Details:**

- JWT issued at login; bearer scheme for subsequent requests
- `GlobalExceptionHandlerMiddleware` catches auth errors and formats responses
- Deactivated users cannot log in regardless of valid credentials
- Email notifications sent asynchronously (no blocking on welcome/activation emails)

---

### 6.2 Product Management Service

**Implementation Location:** `ProductService.cs`, `ProductRepository.cs`, `ProductValidator.cs`

**Key Features:**

- **Seller Isolation:** All product operations verify `ProductBelongsToSellerAsync()` before allowing access
- **Stock Management:** Stock decremented atomically when orders created; validated before order processing
- **Soft Deletion:** Products marked unavailable (IsAvailable = false) retained for audit; orders unaffected
- **SEO Slugs:** Products include human-readable slugs for future URL routing

---

### 6.3 Order Processing Service

**Implementation Location:** `OrderService.cs`, `OrderRepository.cs`, `OrderValidator.cs`

**Order Creation Workflow:**

1. Receives OrderDto with line items (productId + quantity pairs)
2. Validates all products exist, are available, and have sufficient stock
3. Creates Order header (aggregates multiple sellers' products)
4. Creates SellerOrder items (one per unique seller in cart)
5. Decrements stock for each product
6. All operations in single DbContext transaction (atomic)
7. Enqueues order confirmation email
8. Returns 400 BadRequest if any validation fails

**Status State Machine:**

- **Seller:** Pending→Processing, Pending→Cancelled
- **Admin:** Any transition (override capability)
- **Customer:** Pending→Cancelled only

**Feedback Processing:**

- Only available after SellerOrder status = Delivered
- Validates customer owns parent Order
- Prevents duplicate feedback per SellerOrder
- Feedback is immutable (no edit/delete)

---

### 6.4 Real-Time Chat Service

**Implementation Location:** `ChatService.cs`, `ChatRepository.cs`, `ChatHub.cs`, `SignalRRealtimeChatNotifier.cs`

**Chat Creation:**

- One-to-one relationship enforced (seller ↔ customer pair)
- Queries for existing chat before creating new
- Returns existing if duplicate creation attempted

**Chat Rules & Constraints:**

- **Only customers** can create a chat with a seller
- **Only customers** have the authority to close a chat with a seller
- Chat can only exist between a seller and a customer (no group chats)
- Both sellers and customers can view their chat history

**Message Broadcasting:**

- Messages persisted to database
- Broadcast via SignalR group $`{userId}`
- Connected clients receive real-time notifications using the SignalR library
- IsRead flag tracks message state for UI

**WebSocket Hub:** ChatHub at `/hubs/chat` handles real-time connections and broadcast logic

---

### 6.5 Email & Background Jobs

**Implementation Location:** `HangfireBackgroundJobService.cs`, `EmailJob.cs`, `EmailSettings` configuration

**Background Job Queue:**

- `EnqueueCustomerWelcomeEmailJob()` – Welcome email on customer registration
- `EnqueueAccountActivationEmailJob()` – Account re-enabled notification
- `EnqueueAccountDeactivationEmailJob()` – Account disabled notification

**Hangfire Configuration:**

- Uses SQL Server for persistence (configured via `HangFireDbConnection`)
- SMTP credentials stored in `EmailSettings` (Gmail app password)
- Automatic retry logic and dead-letter queue for failed jobs
- No request blocking; all email operations asynchronous

---

### 6.6 Rate Limiting Policy

**Implementation Location:** `Program.cs` (ASP.NET Core Rate Limiter configuration)

**Rate Limiter Type:** RoleBasedRateLimiterPolicy

**Core Concept: Partitioning**  
The rate limiter uses a partitioning strategy combining **Role** and **UserId** to create role-specific rate limits with per-user isolation:

- Partitions like `"Sellers"` group all sellers together (shared bucket)
- Partitions like `"Seller:10"` create per-user buckets (isolated traffic)
- This ensures one seller's heavy traffic doesn't affect another seller

**Rate Limits by Role:**

| Role          | Type         | Limit      | Behavior                                                                    |
| ------------- | ------------ | ---------- | --------------------------------------------------------------------------- |
| **Admin**     | Concurrency  | 10         | Max 10 concurrent requests; 11th is blocked immediately until one completes |
| **Seller**    | Fixed Window | 300/minute | 300 requests per minute; resets automatically after 60 seconds              |
| **Customer**  | Fixed Window | 200/minute | 200 requests per minute; resets automatically after 60 seconds              |
| **Anonymous** | Fixed Window | 100/minute | 100 requests per minute; identified by IP address                           |

**Key Behaviors:**

- **Rejection:** Users exceeding limits receive `429 Too Many Requests`
- **No Queue:** `QueueLimit = 0` means requests are rejected immediately rather than queued (saves server memory)
- **Recovery:** Buckets automatically refill when the time window expires
- **Anonymous Tracking:** Unauthenticated users are identified by IP address

**Implementation Details:**

1. Extract Role and UserId from JWT claims
2. For anonymous users, use `"Anonymous" + IP Address` as partition key
3. Build partition string: e.g., `"Seller:123"`, `"Customer:456"`, `"Admin:789"`
4. Pass partition key to the rate limiter
5. Rate limiter checks against configured limits and accepts/rejects

---

---

## 9. Error Handling & Validation

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
| `POST /api/products/{id}/Images` | Seller | Only sellers can upload images for their products |
| `DELETE /api/products/{id}/Images/{imageId}` | Seller | Only sellers can delete images for their products |
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

## 8. Data Access & Persistence

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

## 9. Error Handling & Validation

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

## 9.1 Docker & Containerization

The entire application stack has been containerized using Docker for consistent development and deployment environments.

### Docker Configuration Files

**Dockerfile (Multi-Stage Build):**

- **Stage 1 (Build):** Uses `mcr.microsoft.com/dotnet/sdk:8.0` to compile and publish the application
- **Stage 2 (Runtime):** Uses `mcr.microsoft.com/dotnet/aspnet:8.0` for optimized runtime image
- **Benefits:** Reduces final image size by excluding SDK tools from runtime container
- **Entry Point:** Runs `dotnet EcommerceApp.Api.dll` on container startup

**Docker Compose (docker-compose.yml):**
Orchestrates two services:

1. **dockersqlserver** (SQL Server 2022)
   - Image: `mcr.microsoft.com/mssql/server:2022-latest`
   - Container Name: `dockersqlserver`
   - Port: `1434:1433` (external:internal)
   - Environment: Accepts EULA, sets SA password to `Strong!Pass123`
   - Volume: `sqlserver-data` (persists database files)
   - Network: `app-network` (internal bridge for service communication)

2. **webapi** (Ecommerce API)
   - Build: Builds from local `Dockerfile`
   - Container Name: `ecommerceapi`
   - Port: `5208:80` (external:internal)
   - Depends On: `dockersqlserver` (ensures SQL starts first)
   - Environment:
     - `ASPNETCORE_ENVIRONMENT`: Docker
     - `MSSQL_SA_USER`: sa
     - `MSSQL_SA_PASSWORD`: Strong!Pass123
   - Command: Runs `wait-for-sql.sh` script before starting API (ensures database readiness)
   - Volume: `product-images` (persists uploaded product images)
   - Network: `app-network` (communicates with SQL Server container)

**Shared Resources:**

- **Volumes:**
  - `sqlserver-data`: Persists SQL Server data files
  - `product-images`: Persists product image uploads
- **Network:** `app-network` (bridge driver for internal service discovery)

### Running with Docker

```bash
# Build and start all services
docker-compose up --build

# Stop all services
docker-compose down

# View logs
docker-compose logs -f webapi
docker-compose logs -f dockersqlserver

# Access API
# http://localhost:5208/swagger
```

### Wait-for-SQL Script (wait-for-sql.sh)

- Ensures SQL Server is fully initialized before the API attempts database migration and startup
- Prevents connection failures due to SQL Server not being ready
- Uses TCP connection test to verify database availability

---

## 10. Current Limitations & Known Technical Debt

1. **No Payment Integration**: Stripe integration is planned but not yet implemented. Orders are created but no actual payments are processed. (See [Todos.txt](Todos.txt))

2. **Admin Creation**: The `RegisterAdmin` endpoint is commented out. Admin users must be created manually in the database or via a back-office tool.

3. **Email Verification for Sellers**: Sellers are not required to verify their email; they are auto-activated. This creates a security gap.

4. **Missing Audit Logging**: User actions (product updates, order status changes) are not logged for audit trails.

5. **No Full-Text Search**: Product search is basic (by ID or seller ID). Full-text search for product names/descriptions is not implemented.

6. **Soft Delete Not Fully Utilized**: Products have an `IsAvailable` flag, but soft deletion is inconsistently applied across entities.

7. **Limited Concurrency Handling**: No optimistic locking (ETag/version columns) on frequently-updated entities like Orders. Race conditions during concurrent status updates are possible.

8. **JWT Token Refresh**: Tokens have short expiry (60 seconds in config) but no refresh token mechanism; clients must re-authenticate frequently.

---

## 11. Getting Started / Setup Instructions

### Prerequisites

- **.NET SDK**: 8.0 or later
- **SQL Server**: LocalDB (Windows) or Docker
- **Visual Studio** or **VS Code** with C# extension
- **Git** (for cloning)

### Local Development Setup

#### 1. Clone & Restore Dependencies

```bash
git clone https://github.com/ahsanalikhan-folio3/EcommerceWebApi
cd EcommerceWebApi
dotnet restore
```

#### 2. Database Configuration

**Option A: Using LocalDB (Windows)**

- Connection string is already configured in `appsettings.json` for LocalDB.
- Ensure SQL Server Express LocalDB is installed.

**Option B: Using Docker (Currently not working/stable)**

- Run SQL Server container:
  ```bash
  docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=PASS!WEAK4356" \
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

API starts on `https://localhost:5208` (or port specified in `launchSettings.json`).

#### 5. Access Swagger UI

Navigate to `https://localhost:5208/swagger` to explore endpoints interactively.

#### 6. Using Docker for Complete Stack

Alternatively, to run the entire stack (API + SQL Server) in Docker:

```bash
docker-compose up --build
```

This will:

- Build the API image
- Start SQL Server 2022 on port 1434
- Start the API on port 5208
- Apply database migrations automatically

**Note:** For Docker usage, ensure Docker Desktop is installed and running.

### Important Environment Variables / Appsettings Keys

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DbConnection": "CONNECTION_STRING_SQL_SERVER_DB",
    "DbConnectionThroughDocker": "CONNECTION_STRING_SQL_SERVER_DB_IN_DOCKER",
    "HangFireDbConnection": "CONNECTION_STRING_HANGFIRE_DB"
  },
  "Jwt": {
    "Key": "THIS_SHOULD_BE_A_LONG_RANDOM_SECRET_KEY_32+_CHARS",
    "Issuer": "EcommerceWebApi",
    "Audience": "EcommerceWebApiUsers",
    "AccessTokenExpirySeconds": 60
  },
  "EmailSettings": {
    "Host": "YOUR_HOST_GMAIL",
    "Port": 123,
    "Username": "USERNAME",
    "AppPassword": "XXXX XXXX XXXX XXXX",
    "EnableSsl": true,
    "FromName": "NAME",
    "FromEmail": "EMAIL"
  }
}
```

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
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger/index.html",
      "applicationUrl": "http://localhost:5208",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
        "commandName": "Project",
        "dotnetRunMessages": true,
        "launchBrowser": true,
        "launchUrl": "swagger/index.html",
        "applicationUrl": "https://localhost:7131;http://localhost:5208",
        "environmentVariables": {
            "ASPNETCORE_ENVIRONMENT": "Development"
        }
    },
    "IIS Express": {
        "commandName": "IISExpress",
        "launchBrowser": true,
        "launchUrl": "swagger/index.html",
        "environmentVariables": {
            "ASPNETCORE_ENVIRONMENT": "Development"
        }
    }
}
```

---

## Conclusion

The **EcommerceApp API** is a professionally-architected, enterprise-ready e-commerce backend demonstrating modern C# and .NET best practices. Its Clean Architecture separates concerns effectively across domain, application, infrastructure, and API layers, making the codebase maintainable, testable, and extensible.

**Architectural Highlights:**

- Comprehensive role-based authorization with JWT security
- Atomic order processing with multi-seller support
- Real-time communication via SignalR WebSocket
- Asynchronous email processing (no request blocking)
- Standardized error handling and validation
- Unit of Work pattern for data access consistency

**Future Enhancement Opportunities:**

- Payment processing integration (Stripe/PayPal)
- Full-text search for product discovery
- Audit logging for compliance
- JWT refresh token mechanism (currently limited to 60-second expiry)
- Optimistic locking for concurrent order updates
- Email verification for seller registration

For questions, contributions, or additional documentation, refer to code comments or contact the development team.
