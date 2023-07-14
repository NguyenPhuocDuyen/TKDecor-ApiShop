using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabasesController : ControllerBase
    {
        private readonly IProductRepository _product;

        public DatabasesController(IProductRepository product)
        {
            _product = product;
        }

        //[HttpGet("updateProduct")]
        //public async Task<IActionResult> UpdateProduct()
        //{
        //    var list = await _product.GetAll();
        //    foreach (var item in list)
        //    {
        //        if (item.Product3DModel != null)
        //        {
        //            item.Product3DModel.ModelUrl = "https://cdn-luma.com/e13d5b281b9c97e6fbc3defda9a2812bcca09f323705dff6b77646f0d5655dc9.glb";
        //            await _product.Update(item);
        //        }
        //    }
        //    return NoContent();
        //}
    }
}
