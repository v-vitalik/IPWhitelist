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
using System.Linq;
using System.Text;
using System;
using System.Data.SqlClient;

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
                string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=IPWhitelist.Models.IPAddressesContext;Integrated Security=True;";
                string query = "SELECT Id, StartIP, EndIP, IsActive FROM [dbo].[WhitelistIPs] WHERE @IP >= StartIP AND @IP <= EndIP AND IsActive = 1";
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@IP", ipAddress.GetBytes());
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            IPAddressRange ip = new IPAddressRange()
                            {
                                Id = (int)reader["Id"],
                                StartAddress = (reader["StartIP"] as byte[]).IPAddressToString(),
                                EndAddress = (reader["EndIP"] as byte[]).IPAddressToString(),
                                IsActive = (bool)reader["IsActive"]
                            };
                            MemoryCacher.Add(ipAddress, ip);
                            return;
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                actionContext.Response.ReasonPhrase = "IP Address is not allowed";
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