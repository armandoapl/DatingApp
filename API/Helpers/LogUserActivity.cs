using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        //" An action filter allow us to do something ethier before the request is executing or after the request is executing" neil coummings this is my action filter
        //to handle the last active value of the property LastActive.
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resulContext = await next();

            if (!resulContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resulContext.HttpContext.User.GetUserId();

            var repo = resulContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await repo.GetUserByIdAsync(userId);
            user.LastActive = DateTime.UtcNow.AddHours(-4);
            await repo.SaveAllAsync();

        }
    }
}
