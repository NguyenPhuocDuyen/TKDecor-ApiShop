using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using AutoMapper;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly IMapper _mapper;

        public CouponsController(IMapper mapper)
        {
            _mapper = mapper;
        }

        //// GET: api/Coupons
        //[HttpGet]
        //public async Task<IActionResult> GetCoupons()
        //{
        //    return await _context.Coupons.ToListAsync();
        //}

        //// GET: api/Coupons/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Coupon>> GetCoupon(int id)
        //{
        //  if (_context.Coupons == null)
        //  {
        //      return NotFound();
        //  }
        //    var coupon = await _context.Coupons.FindAsync(id);

        //    if (coupon == null)
        //    {
        //        return NotFound();
        //    }

        //    return coupon;
        //}

        //// PUT: api/Coupons/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutCoupon(int id, Coupon coupon)
        //{
        //    if (id != coupon.CouponId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(coupon).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CouponExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Coupons
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Coupon>> PostCoupon(Coupon coupon)
        //{
        //  if (_context.Coupons == null)
        //  {
        //      return Problem("Entity set 'TkdecorContext.Coupons'  is null.");
        //  }
        //    _context.Coupons.Add(coupon);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetCoupon", new { id = coupon.CouponId }, coupon);
        //}

        //// DELETE: api/Coupons/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteCoupon(int id)
        //{
        //    if (_context.Coupons == null)
        //    {
        //        return NotFound();
        //    }
        //    var coupon = await _context.Coupons.FindAsync(id);
        //    if (coupon == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Coupons.Remove(coupon);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool CouponExists(int id)
        //{
        //    return (_context.Coupons?.Any(e => e.CouponId == id)).GetValueOrDefault();
        //}
    }
}
