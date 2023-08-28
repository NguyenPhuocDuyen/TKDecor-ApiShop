using BE_TKDecor.Core.Dtos.Product3DModel;
using BE_TKDecor.Core.Response;
using BusinessObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataBaseController : ControllerBase
    {
        private readonly TkdecorContext _context;
        private readonly ApiResponse _apiResponse;

        public DataBaseController(TkdecorContext context)
        {
            _context = context;
            _apiResponse = new ApiResponse();
        }

        [HttpGet("ResetModelDelete")]
        public async Task<IActionResult> ResetModelDelete()
        {
            var models = await _context.Product3Dmodels.Where(x => x.IsDelete).ToListAsync();
            foreach (var item in models)
            {
                item.IsDelete = false;
            }
            try
            {
                _context.Product3Dmodels.UpdateRange(models);
                await _context.SaveChangesAsync();
                _apiResponse.Success = true;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Message = ex.Message;
                return BadRequest(_apiResponse);
            }
        }

        [HttpGet("ResetProductDelete")]
        public async Task<IActionResult> ResetProductDelete()
        {
            var products = await _context.Products.Where(x => x.IsDelete).ToListAsync();
            foreach (var item in products)
            {
                item.IsDelete = false;
            }
            try
            {
                _context.Products.UpdateRange(products);
                await _context.SaveChangesAsync();
                _apiResponse.Success = true;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Message = ex.Message;
                return BadRequest(_apiResponse);
            }
        }

        [HttpPut("UpdateModel3D/{id}")]
        public async Task<IActionResult> UpdateModel3D(Guid id, Product3DModelCreateDto dto)
        {
            var model = await _context.Product3Dmodels.FindAsync(id);
            if (model is null)
                return NotFound();

            model.ModelName = dto.ModelName;
            model.VideoUrl = dto.VideoUrl;
            model.ModelUrl = dto.ModelUrl;
            model.ThumbnailUrl = dto.ThumbnailUrl;
            try
            {
                _context.Product3Dmodels.Update(model);
                await _context.SaveChangesAsync();
                _apiResponse.Success = true;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Message = ex.Message;
                return BadRequest(_apiResponse);
            }
        }

        //[HttpGet("GetAll")]
        //public IActionResult GetAll()
        //{
        //    return Ok(_context.Users);
        //}

        //[HttpGet("ResetAccount")]
        //public IActionResult ResetAccount()
        //{
        //    var list = _context.Users.ToList();
        //    foreach (var account in list)
        //    {
        //        account.IsDelete = false;
        //    }
        //    _context.Users.UpdateRange(list);
        //    try
        //    {
        //        _context.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    return NoContent();
        //}

        //[HttpPut]
        //public async Task<IActionResult> Update(Guid id)
        //{
        //    try
        //    {
        //        var user = await _context.Users.FindAsync(id);
        //        if (user != null)
        //        {
        //            user.Email = 1 + user.Email;
        //            _context.Users.Update(user);
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex);
        //    }
        //    return NoContent();
        //}

        //[HttpDelete]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    try
        //    {
        //        var notifications = _context.Notifications.Where(x => x.UserId == id).ToList();
        //        _context.RemoveRange(notifications);
        //        _context.SaveChanges();

        //        var user = await _context.Users.FindAsync(id);
        //        if (user != null)
        //        {
        //            _context.Users.Remove(user);
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    return NoContent();
        //}
    }
}
