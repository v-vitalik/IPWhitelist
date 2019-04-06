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
        private Dictionary<string, IPAddressRange> cache;

        public IPAddressFilterAttribute()
        {
            cache = new Dictionary<string, IPAddressRange>();
        }

        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return Task.Run(() => 
            {
                var ipAddress = GetClientIpAddress(actionContext.Request);
                IPAddressRange res;
                if (cache.TryGetValue(ipAddress, out res))
                {
                    return;
                }
                using (var dbContext = new IPAddressesContext())
                {
                    foreach (IPAddressRange range in dbContext.Ranges)
                    {
                        if (ipAddress.MoreOrEqualTo(range.StartAddress) && ipAddress.LessOrEqualTo(range.EndAddress))
                        {
                            cache.Add(ipAddress, range);
                            return;
                        }
                    }
                }
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        }

        private string GetClientIpAddress(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                string ipString = ((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                IPAddress ipAddress = IPAddress.Parse(ipString);
                return ipAddress.MapToIPv4().ToString();
            }
            return string.Empty;
        }
    }
}