using BusinessObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE_TKDecor.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class DataBaseController : ControllerBase
    //{
    //    private readonly TkdecorContext _context;

    //    public DataBaseController(TkdecorContext context)
    //    {
    //        _context = context;
    //    }

    //    [HttpGet("GetAll")]
    //    public IActionResult GetAll()
    //    {
    //        return Ok(_context.Users);
    //    }

    //    [HttpGet("ResetAccount")]
    //    public IActionResult ResetAccount()
    //    {
    //        var list = _context.Users.ToList();
    //        foreach (var account in list)
    //        {
    //            account.IsDelete = false;
    //        }
    //        _context.Users.UpdateRange(list);
    //        try
    //        {
    //            _context.SaveChanges();
    //        }
    //        catch (Exception ex)
    //        {
    //            return BadRequest(ex.Message);
    //        }
    //        return NoContent();
    //    }

    //    [HttpPut]
    //    public async Task<IActionResult> Update(Guid id)
    //    {
    //        try
    //        {
    //            var user = await _context.Users.FindAsync(id);
    //            if (user != null)
    //            {
    //                user.Email = 1 + user.Email;
    //                _context.Users.Update(user);
    //                _context.SaveChanges();
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            return BadRequest(ex);
    //        }
    //        return NoContent();
    //    }

    //    [HttpDelete]
    //    public async Task<IActionResult> Delete(Guid id)
    //    {
    //        try
    //        {
    //            var notifications = _context.Notifications.Where(x => x.UserId == id).ToList();
    //            _context.RemoveRange(notifications);
    //            _context.SaveChanges();

    //            var user = await _context.Users.FindAsync(id);
    //            if (user != null)
    //            {
    //                _context.Users.Remove(user);
    //                _context.SaveChanges();
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            return BadRequest(ex.Message);
    //        }
    //        return NoContent();
    //    }
    //}
}
