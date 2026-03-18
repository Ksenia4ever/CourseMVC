using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CourseInfrastructure.Controllers
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var accountId = context.HttpContext.Session.GetInt32("CurrentAccountId");

            if (accountId == null)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }

            base.OnActionExecuting(context);
        }
    }
}