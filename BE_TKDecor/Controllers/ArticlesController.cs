using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Dtos.Article;
using BE_TKDecor.Core.Response;
using Utility;
using BE_TKDecor.Service.IService;
using System.Drawing.Printing;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _article;

        public ArticlesController(IArticleService article)
        {
            _article = article;
        }

        // GET: api/Articles/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(
            string sort = "default",
            int pageIndex = 1,
            int pageSize = 20
            )
        {
            var res = await _article.GetAll( sort, pageIndex, pageSize );
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/Articles/GetBySlug/abc-def
        [HttpGet("GetBySlug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var res = await _article.GetBySlug(slug);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
