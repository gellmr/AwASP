using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AngularWithASP.Server.Infrastructure
{
  // To customize the authorization failure response in .NET Core,
  // you can implement the IAuthorizationMiddlewareResultHandler interface.
  // This interface allows you to intercept the authorization process and customize the response.
  // This allows you to handle the authorization result and modify the HTTP response accordingly.
  public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
  {
    private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

    // This method is called when authorization fails.
    public async Task HandleAsync( RequestDelegate next,
      HttpContext context,
      AuthorizationPolicy policy,
      PolicyAuthorizationResult authorizeResult // Tells the reason for failure ( Forbidden / Unauthorized )
    )
    {
      int code = context.Response.StatusCode;

      // Check what reason was the authorization failure ?
      if (authorizeResult.Challenged == true)
      {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.Headers["Location"] = "/admin"; // Your desired login URL
        return;
      }

      // If not forbidden, let the default handler handle it. We don't need any custom logic.
      await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
  }
}
