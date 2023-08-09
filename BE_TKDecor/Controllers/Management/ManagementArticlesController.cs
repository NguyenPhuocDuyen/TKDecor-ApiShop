using BE_TKDecor.Core.Dtos.Article;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
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
        private readonly IUserService _user;

        public ManagementArticlesController(IArticleService article,
            IUserService user)
        {
            _article = article;
            _user = user;
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
        public async Task<IActionResult> Create(ArticleCreateDto articleDto)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return BadRequest(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _article.Create(articleDto, user.UserId);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // PUT: api/ManagementArticles/Update/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, ArticleUpdateDto articleDto)
        {
            var res = await _article.Update(id, articleDto);
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
        public async Task<IActionResult> SetPublish(ArticleSetPublishDto articleDto)
        {
            var res = await _article.SetPublish(articleDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        private async Task<User?> GetUser()
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
                // get user by user id
                if (userId != null)
                    return await _user.GetById(Guid.Parse(userId));
            }
            return null;
        }
    }
}
