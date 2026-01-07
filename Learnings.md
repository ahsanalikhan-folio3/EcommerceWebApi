**-> For Business level Authorization instead of bombarding ifs in controllers, you can create a simple Filter by implementing the IAsyncAuthorizationFilter interface and can use it like this on top of controller functions. This is efficient as we can reuse these filters as well.**



**Example:**



public interface ISellerAuthorizationService

{

&nbsp;   Task<bool> OwnsProductAsync(int sellerId, int productId);

&nbsp;   Task<bool> OwnsSellerOrderAsync(int sellerId, int sellerOrderId);

}



public class SellerAuthorizationService : ISellerAuthorizationService

{

&nbsp;   private readonly IUnitOfWork \_uow;



&nbsp;   public SellerAuthorizationService(IUnitOfWork uow)

&nbsp;   {

&nbsp;       \_uow = uow;

&nbsp;   }



&nbsp;   public async Task<bool> OwnsProductAsync(int sellerId, int productId)

&nbsp;   {

&nbsp;       return await \_uow.Products

&nbsp;           .IsProductOwnedBySeller(productId, sellerId);

&nbsp;   }



&nbsp;   public async Task<bool> OwnsSellerOrderAsync(int sellerId, int sellerOrderId)

&nbsp;   {

&nbsp;       return await \_uow.SellerOrders

&nbsp;           .IsSellerOrderOwnedBySeller(sellerOrderId, sellerId);

&nbsp;   }

}



public class SellerOwnsProductFilter : IAsyncAuthorizationFilter

{

&nbsp;   private readonly ISellerAuthorizationService \_authService;



&nbsp;   public SellerOwnsProductFilter(ISellerAuthorizationService authService)

&nbsp;   {

&nbsp;       \_authService = authService;

&nbsp;   }



&nbsp;   public async Task OnAuthorizationAsync(AuthorizationFilterContext context)

&nbsp;   {

&nbsp;       if (!context.RouteData.Values.TryGetValue("id", out var productIdObj) ||

&nbsp;           !int.TryParse(productIdObj?.ToString(), out int productId))

&nbsp;       {

&nbsp;           context.Result = new BadRequestResult();

&nbsp;           return;

&nbsp;       }



&nbsp;       int sellerId = context.HttpContext.User.GetUserIdInt();



&nbsp;       bool authorized =

&nbsp;           await \_authService.OwnsProductAsync(sellerId, productId);



&nbsp;       if (!authorized)

&nbsp;       {

&nbsp;           context.Result = new ForbidResult();

&nbsp;       }

&nbsp;   }

}



services.AddScoped<SellerOwnsProductFilter>();



\[ServiceFilter(typeof(SellerOwnsProductFilter))]

\[HttpGet("products/{id}/orders")]

public IActionResult GetOrders(int id)

{

&nbsp;   ...

}



\[ServiceFilter(typeof(SellerOwnsProductFilter))]

\[HttpPut("products/{id}")]

public IActionResult UpdateProduct(int id)

{

&nbsp;   ...

}



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





**For detailed descriptions:** [**https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-8.0**](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-8.0)

