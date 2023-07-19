using AutoMapper;
using BE_TKDecor.Core.Dtos.Article;
using BE_TKDecor.Core.Response;
using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;
using Utility.SD;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleContent.Seller},{RoleContent.Admin}")]
    public class ManagementArticlesController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly IUserRepository _user;
        private readonly IArticleRepository _article;

        public ManagementArticlesController(IMapper mapper,
            IUserRepository user,
            IArticleRepository article)
        {
            _mapper = mapper;
            _user = user;
            _article = article;
        }

        // GET: api/ManagementArticles/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _article.GetAll();
            list = list.OrderByDescending(x => x.UpdatedAt).ToList();
            var result = _mapper.Map<List<ArticleGetDto>>(list);

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // POST: api/ManagementArticles/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(ArticleCreateDto articleDto)
        {
            var articleDb = await _article.FindByTitle(articleDto.Title);
            if (articleDb != null)
                return BadRequest(new ApiResponse { Message = "Article already exist!" });

            var newSlug = Slug.GenerateSlug(articleDto.Title);
            articleDb = await _article.FindBySlug(newSlug);
            if (articleDb != null)
                return BadRequest(new ApiResponse { Message = "Please change the name due to duplicate data!" });

            var user = await GetUser();
            if (user == null)
                return BadRequest(new ApiResponse { Message = ErrorContent.UserNotFound });

            // create new article info
            Article newArticle = _mapper.Map<Article>(articleDto);
            newArticle.UserId = user.UserId;
            newArticle.Slug = Slug.GenerateSlug(newArticle.Title);
            newArticle.IsPublish = false;
            try
            {
                await _article.Add(newArticle);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }

        }

        // PUT: api/ManagementArticles/Update/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, ArticleUpdateDto articleDto)
        {
            if (id != articleDto.ArticleId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var articleDb = await _article.FindById(id);
            if (articleDb == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ArticleNotFound });

            var newSlug = Slug.GenerateSlug(articleDto.Title);
            var articleSlug = await _article.FindBySlug(newSlug);
            if (articleSlug != null && articleSlug.ArticleId != id)
                return BadRequest(new ApiResponse { Message = "Please change the name due to duplicate data!" });

            // update info article
            articleDb.Title = articleDto.Title;
            articleDb.Content = articleDto.Content;
            articleDb.Thumbnail = articleDto.Thumbnail;
            articleDb.UpdatedAt = DateTime.Now;
            try
            {
                await _article.Update(articleDb);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // DELETE: api/ManagementArticles/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteArticle(Guid id)
        {
            var article = await _article.FindById(id);
            if (article == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ArticleNotFound });

            article.IsDelete = true;
            article.UpdatedAt = DateTime.Now;
            try
            {
                await _article.Update(article);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // api/ManagementArticles/SetPublish
        [HttpPost("SetPublish")]
        public async Task<IActionResult> SetPublish(ArticleSetPublishDto articleDto)
        {
            var article = await _article.FindById(articleDto.ArticleId);
            if (article == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ArticleNotFound });

            article.IsPublish = articleDto.Published;
            article.UpdatedAt = DateTime.Now;
            try
            {
                await _article.Update(article);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        private async Task<User?> GetUser()
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
                // get user by user id
                if (userId != null)
                    return await _user.FindById(Guid.Parse(userId));
            }
            return null;
        }
    }
}
