using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using IPWhitelist.Models;
using System.Net.Http;
using System.Net;
using IPWhitelist.Extensions;

namespace IPWhitelist.Filters
{
    public class IPAddressFilterAttribute : ActionFilterAttribute
    {
        private readonly IPAddressesContext dbContext;

        public IPAddressFilterAttribute()
        {
            dbContext = new IPAddressesContext();
        }

        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return Task.Run(() => 
            {
                var ipAddress = GetClientIpAddress(actionContext.Request);
                foreach(IPAddressRange range in dbContext.Ranges)
                {
                    if (ipAddress.MoreOrEqualTo(range.StartAddress) && ipAddress.LessOrEqualTo(range.EndAddress))
                        return;
                }
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
                //if (!dbContext.Ranges.Any(r => ipAddress.MoreOrEqualTo(r.StartAddress) && ipAddress.LessOrEqualTo(r.EndAddress)))
                //    actionContext.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.Run(() => 
            {

            });
        }

        private string GetClientIpAddress(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                string ip = ((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                if (ip == "::1")
                    return "127.0.0.1";
                return ip;
            }
            return string.Empty;
        }
    }
}