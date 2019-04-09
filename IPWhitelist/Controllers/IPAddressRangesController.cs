using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using IPWhitelist.Models;
using IPWhitelist.Cache;
using IPWhitelist.Extensions;
using System.Collections.Generic;

namespace IPWhitelist.Controllers
{
    public class IPAddressRangesController : ApiController
    {
        private IPAddressesContext db = new IPAddressesContext();

        // GET: api/IPAddressRanges
        public IEnumerable<IPAddressRange> GetIPAddressRanges()
        {

            return db.WhitelistIPs.ToList().Select(i => new IPAddressRange
            {
                Id = i.Id,
                StartAddress = i.StartIP.IPAddressToString(),
                EndAddress = i.EndIP.IPAddressToString(),
                IsActive = i.IsActive,
                RuleName = i.RuleName
            });
        }

        // GET: api/IPAddressRanges/5
        [ResponseType(typeof(IPAddressRange))]
        public async Task<IHttpActionResult> GetIPAddressRange(int id)
        {
            WhitelistIP whitelistIP = await db.WhitelistIPs.FindAsync(id);
            if (whitelistIP == null)
            {
                return NotFound();
            }

            return Ok(new IPAddressRange
            {
                Id = whitelistIP.Id,
                StartAddress = whitelistIP.StartIP.IPAddressToString(),
                EndAddress = whitelistIP.EndIP.IPAddressToString(),
                IsActive = whitelistIP.IsActive,
                RuleName = whitelistIP.RuleName
            });
        }

        // PUT: api/IPAddressRanges/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutIPAddressRange(int id, IPAddressRange iPAddressRange)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != iPAddressRange.Id)
            {
                return BadRequest();
            }

            if (!iPAddressRange.ValidAddresses())
            {
                return BadRequest();
            }
            WhitelistIP whitelistIP = new WhitelistIP()
            {
                Id = iPAddressRange.Id,
                RuleName = iPAddressRange.RuleName,
                StartIP = iPAddressRange.StartAddress.GetBytes(),
                EndIP = iPAddressRange.EndAddress.GetBytes(),
                IsActive = iPAddressRange.IsActive
            };
            db.Entry(whitelistIP).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
                MemoryCacher.UpdateOrDelete(iPAddressRange);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IPAddressRangeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/IPAddressRanges
        [ResponseType(typeof(IPAddressRange))]
        public async Task<IHttpActionResult> PostIPAddressRange(IPAddressRange iPAddressRange)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!iPAddressRange.ValidAddresses())
            {
                return BadRequest();
            }

            WhitelistIP whitelistIP = new WhitelistIP()
            {
                RuleName = iPAddressRange.RuleName,
                StartIP = iPAddressRange.StartAddress.GetBytes(),
                EndIP = iPAddressRange.EndAddress.GetBytes(),
                IsActive = iPAddressRange.IsActive
            };

            db.WhitelistIPs.Add(whitelistIP);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = iPAddressRange.Id }, iPAddressRange);
        }

        // DELETE: api/IPAddressRanges/5
        [ResponseType(typeof(IPAddressRange))]
        public async Task<IHttpActionResult> DeleteIPAddressRange(int id)
        {
            WhitelistIP whitelistIP = await db.WhitelistIPs.FindAsync(id);
            if (whitelistIP == null)
            {
                return NotFound();
            }

            db.WhitelistIPs.Remove(whitelistIP);

            await db.SaveChangesAsync();

            IPAddressRange iPAddressRange = new IPAddressRange
            {
                Id = whitelistIP.Id,
                StartAddress = whitelistIP.StartIP.IPAddressToString(),
                EndAddress = whitelistIP.EndIP.IPAddressToString(),
                IsActive = whitelistIP.IsActive,
                RuleName = whitelistIP.RuleName
            };

            MemoryCacher.DeleteIfContains(iPAddressRange);

            return Ok(iPAddressRange);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool IPAddressRangeExists(int id)
        {
            return db.WhitelistIPs.Count(e => e.Id == id) > 0;
        }
    }
}