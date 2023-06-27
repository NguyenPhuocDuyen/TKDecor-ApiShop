using AutoMapper;
using BE_TKDecor.Core.Dtos.Article;
using BE_TKDecor.Core.Response;
using BusinessObject;
using DataAccess.Repository.IRepository;
using DataAccess.StatusContent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleContent.Seller},{RoleContent.Admin}")]
    public class ManagementArticlesController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IArticleRepository _articleRepository;

        public ManagementArticlesController(IMapper mapper,
            IUserRepository userRepository,
            IArticleRepository articleRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _articleRepository = articleRepository;
        }

        // GET: api/ManagementArticles/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _articleRepository.GetAll();
            list = list.OrderByDescending(x => x.UpdatedAt).ToList();
            var result = _mapper.Map<List<ArticleGetDto>>(list);

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // POST: api/ManagementArticles/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(Article article)
        {
            var articleDb = await _articleRepository.FindByTitle(article.Title);
            if (articleDb != null)
                return BadRequest(new ApiResponse { Message = "Article already exist!" });

            var user = await GetUser();
            if (user == null)
                return BadRequest(new ApiResponse { Message = ErrorContent.UserNotFound });

            // create new article info
            Article newArticle = _mapper.Map<Article>(article);
            newArticle.UserId = user.UserId;
            newArticle.Slug = Slug.GenerateSlug(newArticle.Title);
            newArticle.IsPublish = true;
            try
            {
                await _articleRepository.Add(newArticle);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }

        }

        // PUT: api/ManagementArticles/Update/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, ArticleUpdateDto articleDto)
        {
            if (id != articleDto.ArticleId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var articleDb = await _articleRepository.FindById(id);
            if (articleDb == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ArticleNotFound });

            // update info article
            articleDb.Title = articleDto.Title;
            articleDb.Content = articleDto.Content;
            articleDb.Thumbnail = articleDto.Thumbnail;
            articleDb.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _articleRepository.Update(articleDb);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // DELETE: api/ManagementArticles/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _articleRepository.FindById(id);
            if (article == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ArticleNotFound });

            article.IsDelete = true;
            article.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _articleRepository.Update(article);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // api/ManagementArticles/SetPublish
        [HttpPost("SetPublish")]
        public async Task<IActionResult> SetPublish(ArticleSetPublishDto articleDto)
        {
            var article = await _articleRepository.FindById(articleDto.ArticleId);
            if (article == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ArticleNotFound });

            article.IsPublish = articleDto.Published;
            article.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _articleRepository.Update(article);
                return NoContent();
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
                    return await _userRepository.FindById(int.Parse(userId));
            }
            return null;
        }
    }
}
