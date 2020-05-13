using RealEstateCore.Infrastructure.DataContext;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace RealEstateCore.Filters
{
    public class UserIdFilter : IActionFilter
    {
        private readonly DatabaseContext _db;

        public UserIdFilter(DatabaseContext db)
        {
            _db = db;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Guid userId = Guid.Empty;

            if (context.ActionArguments.ContainsKey("userid"))
            {
                userId = (Guid)context.ActionArguments["userid"];

                if (!string.IsNullOrEmpty(userId.ToString()))
                {
                    var user = _db.Users.Find(userId);

                    if (user != null)
                        context.HttpContext.Items.Add("uid", user.Id);
                }
                else
                {
                    context.Result = new NotFoundResult();
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult("Bad id parameter");
                return;
            }
        }
    }
}
