using BE_TKDecor.Core.Dtos.Article;
using BE_TKDecor.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleAdmin)]
    public class ManagementArticlesController : ControllerBase
    {
        private readonly IArticleService _article;

        public ManagementArticlesController(IArticleService article)
        {
            _article = article;
        }

        // GET: api/ManagementArticles/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _article.GetAll();
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/ManagementArticles/GetById/id
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var res = await _article.GetById(id);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/ManagementArticles/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(ArticleCreateDto dto)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _article.Create(dto, userId);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // PUT: api/ManagementArticles/Update/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, ArticleUpdateDto dto)
        {
            var res = await _article.Update(id, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // DELETE: api/ManagementArticles/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteArticle(Guid id)
        {
            var res = await _article.Delete(id);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // api/ManagementArticles/SetPublish
        [HttpPost("SetPublish")]
        public async Task<IActionResult> SetPublish(ArticleSetPublishDto dto)
        {
            var res = await _article.SetPublish(dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
