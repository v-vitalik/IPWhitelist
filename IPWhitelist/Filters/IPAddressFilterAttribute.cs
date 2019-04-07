using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using IPWhitelist.Models;
using System.Net.Http;
using System.Net;
using IPWhitelist.Extensions;
using IPWhitelist.Cache;

namespace IPWhitelist.Filters
{
    public class IPAddressFilterAttribute : ActionFilterAttribute
    {
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return Task.Run(() => 
            {
                var ipAddress = GetClientIpAddress(actionContext.Request);
                if (MemoryCacher.Contains(ipAddress))
                {
                    return;
                }
                using (var dbContext = new IPAddressesContext())
                {
                    foreach (IPAddressRange range in dbContext.Ranges)
                    {
                        if (ipAddress.MoreOrEqualTo(range.StartAddress) && ipAddress.LessOrEqualTo(range.EndAddress))
                        {
                            MemoryCacher.Add(ipAddress, range);
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