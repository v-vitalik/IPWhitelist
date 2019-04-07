using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using IPWhitelist.Models;
using IPWhitelist.Cache;

namespace IPWhitelist.Controllers
{
    public class IPAddressRangesController : ApiController
    {
        private IPAddressesContext db = new IPAddressesContext();

        // GET: api/IPAddressRanges
        public IQueryable<IPAddressRange> GetIPAddressRanges()
        {
            return db.Ranges;
        }

        // GET: api/IPAddressRanges/5
        [ResponseType(typeof(IPAddressRange))]
        public async Task<IHttpActionResult> GetIPAddressRange(int id)
        {
            IPAddressRange iPAddressRange = await db.Ranges.FindAsync(id);
            if (iPAddressRange == null)
            {
                return NotFound();
            }

            return Ok(iPAddressRange);
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

            db.Entry(iPAddressRange).State = EntityState.Modified;

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

            db.Ranges.Add(iPAddressRange);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = iPAddressRange.Id }, iPAddressRange);
        }

        // DELETE: api/IPAddressRanges/5
        [ResponseType(typeof(IPAddressRange))]
        public async Task<IHttpActionResult> DeleteIPAddressRange(int id)
        {
            IPAddressRange iPAddressRange = await db.Ranges.FindAsync(id);
            if (iPAddressRange == null)
            {
                return NotFound();
            }

            db.Ranges.Remove(iPAddressRange);

            await db.SaveChangesAsync();

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
            return db.Ranges.Count(e => e.Id == id) > 0;
        }
    }
}