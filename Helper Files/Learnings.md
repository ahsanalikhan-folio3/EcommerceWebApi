**-> Core REST Principle (important)**

    Routes represent resources, not actions or roles

    Resource → orders

    Action → HTTP verb (POST, PATCH, DELETE)

    Authorization → NOT in the URL

**-> For Business level Authorization instead of bombarding ifs in controllers, you can create a simple Filter by implementing the IAsyncAuthorizationFilter interface and can use it like this on top of controller functions. This is efficient as we can reuse these filters as well.**



**Example:**

```csharp
public interface ISellerAuthorizationService

{

   Task<bool> OwnsProductAsync(int sellerId, int productId);

   Task<bool> OwnsSellerOrderAsync(int sellerId, int sellerOrderId);

}



public class SellerAuthorizationService : ISellerAuthorizationService

{

   private readonly IUnitOfWork _uow;



   public SellerAuthorizationService(IUnitOfWork uow)

   {

       _uow = uow;

   }



   public async Task<bool> OwnsProductAsync(int sellerId, int productId)

   {

       return await _uow.Products

           .IsProductOwnedBySeller(productId, sellerId);

   }



   public async Task<bool> OwnsSellerOrderAsync(int sellerId, int sellerOrderId)

   {

       return await _uow.SellerOrders

           .IsSellerOrderOwnedBySeller(sellerOrderId, sellerId);

   }

}



public class SellerOwnsProductFilter : IAsyncAuthorizationFilter

{

   private readonly ISellerAuthorizationService _authService;



   public SellerOwnsProductFilter(ISellerAuthorizationService authService)

   {

       _authService = authService;

   }



   public async Task OnAuthorizationAsync(AuthorizationFilterContext context)

   {

       if (!context.RouteData.Values.TryGetValue("id", out var productIdObj) ||

           !int.TryParse(productIdObj?.ToString(), out int productId))

       {

           context.Result = new BadRequestResult();

           return;

       }



       int sellerId = context.HttpContext.User.GetUserIdInt();



       bool authorized =

           await _authService.OwnsProductAsync(sellerId, productId);



       if (!authorized)

       {

           context.Result = new ForbidResult();

       }

   }

}


services.AddScoped<SellerOwnsProductFilter>();


[ServiceFilter(typeof(SellerOwnsProductFilter))]

[HttpGet("products/{id}/orders")]
public IActionResult GetOrders(int id)

{

   ...

}



[ServiceFilter(typeof(SellerOwnsProductFilter))]
[HttpPut("products/{id}")]
public IActionResult UpdateProduct(int id)

{

  ...

}
```



**-> By default every fetched entity or list of entities are being tracked by EF core and we can fetch it change it and Save it which will update the entity/entities, but for reading, it is performance efficient that we fetch without tracking like: db.AsNoTracking()**



**-> Write db linq queries in repository layer, business logic in service layer, http results in api layer, entities in domain layer.**



**-> Keep common code paths as fast as possible. (like a middleware that is executed on every request must be fast.)**



**-> Complete long running tasks outside of the http requests. (background job/service must handle these.)**



**-> Compress responses as much as possible since large responses increase latency. \[Pagination for bulk amount of entity]**



**-> If performance is priority then update to the newer version of asp dotnet core since every new version is the optimized version of the previous version.**



**-> Exceptions must be rare, Dont use exceptions to catch normal program flow, use exceptions only for catching unexpected errors.**



**-> Dont read http request body synchronously as it blocks the reading thread, read asynchronously instead.**



**-> Keep in mind the Out Of Memory (OOM) Error which can occur when reading a large payload entirely in memory which exceeds memory limit. OOM can result in DOS.**



**-> In .NET, every object allocation greater than or equal to 85,000 bytes ends up in the large object heap (LOH).**



**-> Do not use the HttpContext in parallel calls.**



**-> Do not use the HttpContext after the request is complete.**



**-> Do not capture the HttpContext in background threads**

**-> We can also add Db constraint by Data Annontating the class properties.**

**-> Best Practice that I found is we should write basic property level validations in Validators while more db level validations or business level validations we can validate these in the service level and return false from the service level if any of the validation fails. We can then return bad request and state Invalid request or something without exposing the more sensitive validations and yes we can maintain logs to keep track if any contradiction occurred. But still have to explore how to expose these responses as well if the usecases demand so keep searching . . . . .**

**-> Before starting every project it is good to revise how dotnet handles the a client request and what stages it has and can go through in short before every project understand request hadnling pipeline.**

---

# 🔁 High-Level Flow (Big Picture)

```
Client
  ↓
Kestrel (Web Server)
  ↓
Middleware Pipeline
  ↓
Routing
  ↓
Authentication
  ↓
Authorization
  ↓
MVC Filters
  ↓
Controller Action
  ↓
Action Result
  ↓
Filters (reverse order)
  ↓
Middleware (reverse order)
  ↓
Response → Client
```

---

# 🧠 Step-by-Step Breakdown

## 1️⃣ Client Sends Request

* Browser / Mobile App / Postman sends HTTP request
* Includes:

  * Method (GET, POST, PUT…)
  * Headers
  * Body (JSON, form data)
  * Cookies / Tokens

---

## 2️⃣ Kestrel (Web Server)

* ASP.NET Core’s built-in web server
* Listens on a port (e.g. `https://localhost:5001`)
* Converts raw HTTP → `HttpContext`

📌 At this point:

```csharp
HttpContext.Request
HttpContext.Response
```

are created.

---

## 3️⃣ Middleware Pipeline (VERY IMPORTANT)

Middleware are executed **in the order they are registered** in `Program.cs`.

Example:

```csharp
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

### How middleware works

Each middleware can:

* Do something **before**
* Call `next()`
* Do something **after**

```csharp
app.Use(async (context, next) =>
{
    // Before
    await next();
    // After
});
```

### Common middleware stages

| Middleware         | Purpose                |
| ------------------ | ---------------------- |
| Exception handling | Catch errors globally  |
| HTTPS redirection  | Force HTTPS            |
| CORS               | Cross-origin rules     |
| Authentication     | Validate JWT / cookies |
| Authorization      | Check permissions      |
| Routing            | Match endpoint         |
| Endpoints          | Execute controller     |

⚠️ **Order matters** a LOT.

---

## 4️⃣ Routing

Routing decides **which endpoint** should handle the request.

```http
GET /api/orders/5
```

Matches:

```csharp
[HttpGet("api/orders/{id}")]
```

If no route matches → **404**

---

## 5️⃣ Authentication

This stage:

* Reads token / cookie
* Validates it
* Creates `ClaimsPrincipal`

```csharp
HttpContext.User
```

❌ If invalid token → **401 Unauthorized**

📌 Authentication does NOT check permissions — only identity.

---

## 6️⃣ Authorization

Checks **what the user is allowed to do**.

Examples:

```csharp
[Authorize]
[Authorize(Roles = "Admin")]
[Authorize(Policy = "CanEditOrder")]
```

❌ If user is authenticated but not allowed → **403 Forbidden**

---

## 7️⃣ MVC Filter Pipeline (Inside Controller)

Once authorization passes, MVC kicks in.

### Filter order (VERY IMPORTANT)

```
Authorization Filters
  ↓
Resource Filters
  ↓
Model Binding
  ↓
Model Validation
  ↓
Action Filters
  ↓
Action Method
  ↓
Result Filters
  ↓
Response
```

---

## 8️⃣ Model Binding

* Converts incoming data → C# objects

Example:

```json
{
  "name": "Phone",
  "price": 500
}
```

→

```csharp
CreateProductDto dto
```

Sources:

* Route
* Query string
* Body
* Headers

---

## 9️⃣ Model Validation

If you use:

```csharp
[ApiController]
```

Then:

* Data annotations
* FluentValidation

❌ Invalid model → **400 Bad Request**
✔ Controller action never runs

---

## 🔟 Controller Action Executes

Your actual business logic runs here:

```csharp
public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
```

You:

* Call services
* Save data
* Enqueue background jobs
* Return response

---

## 1️⃣1️⃣ Action Result Execution

You return:

```csharp
return Ok(orderDto);
```

ASP.NET Core:

* Serializes object → JSON
* Applies JSON settings
* Sets HTTP status code

---

## 1️⃣2️⃣ Filters Run (Reverse Order)

After action:

* Result filters
* Resource filters
* Authorization filters

Good for:

* Logging
* Wrapping responses
* Caching

---

## 1️⃣3️⃣ Middleware Runs (Reverse Order)

Response goes back through middleware:

* Logging
* Compression
* Headers
* Exception handling

---

## 1️⃣4️⃣ Response Sent to Client

* Status code
* Headers
* JSON body

Client receives response 🎉

---

# 🔁 Visual Mental Model (Think Like This)

```
Request →
  Middleware →
    Routing →
      Auth →
        Controller Filters →
          Action →
        Filters →
    Middleware →
Response
```

---

# ⚠️ Common Mistakes (Very Important)

❌ Wrong middleware order
❌ Doing business logic in middleware
❌ Returning EF entities directly
❌ Ignoring validation errors
❌ Mixing auth logic into controllers

---

# 🧩 Where Your Recent Work Fits In

From your recent work:

| Feature        | Pipeline Stage            |
| -------------- | ------------------------- |
| JWT auth       | Authentication middleware |
| Role checks    | Authorization filters     |
| Email jobs     | Controller → Service      |
| Hangfire       | After action              |
| DTO validation | Model validation          |
| AutoMapper     | Action execution          |
| EF Core        | Action execution          |


**For detailed descriptions:** [**https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-8.0**](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-8.0)

