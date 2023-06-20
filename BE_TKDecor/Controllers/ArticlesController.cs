using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using AutoMapper;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Dtos.Article;
using BE_TKDecor.Core.Response;
using Microsoft.AspNetCore.Authorization;
using DataAccess.StatusContent;
using Utility;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleContent.Seller + "," + RoleContent.Admin)]
    public class ArticlesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IArticleRepository _articleRepository;

        public ArticlesController(IMapper mapper,
            IUserRepository userRepository,
            IArticleRepository articleRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _articleRepository = articleRepository;
        }

        // GET: api/Articles
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var list = await _articleRepository.GetAll();
            list = list.Where(x => x.IsDelete == false && x.IsPublish == true).ToList();
            var result = _mapper.Map<List<ArticleGetDto>>(list);

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Articles
        [HttpGet("GetBySlug/{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var article = await _articleRepository.FindBySlug(slug);
            if (article == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ArticleNotFound });

            var result = _mapper.Map<ArticleGetDto>(article);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // PUT: api/Articles/5
        [HttpPut("Update")]
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

        // POST: api/Articles
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

        // DELETE: api/Articles/5
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

        // api/Articles/SetPublish
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
