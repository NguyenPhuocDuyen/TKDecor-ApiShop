using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Dtos.Article;
using BE_TKDecor.Core.Response;
using Utility;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IArticleRepository _article;

        public ArticlesController(IMapper mapper,
            IArticleRepository article)
        {
            _mapper = mapper;
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
            var list = await _article.GetAll();
            list = list.Where(x => !x.IsDelete && x.IsPublish)
                .ToList();

            var listArticleGet = _mapper.Map<List<ArticleGetDto>>(list);
            // filter sort
            listArticleGet = sort switch
            {
                "date-old" => listArticleGet.OrderBy(x => x.CreatedAt).ToList(),
                _ => listArticleGet.OrderByDescending(x => x.CreatedAt).ToList(),
            };

            PaginatedList<ArticleGetDto> pagingArticle = PaginatedList<ArticleGetDto>.CreateAsync(
                listArticleGet, pageIndex, pageSize);

            var result = new
            {
                articles = pagingArticle,
                pagingArticle.PageIndex,
                pagingArticle.TotalPages,
                pagingArticle.TotalItem
            };

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Articles/GetBySlug/abc-def
        [HttpGet("GetBySlug/{slug}")]
        public async Task<ApiResponse> GetBySlug(string slug)
        {
            var article = await _article.FindBySlug(slug);
            if (article == null || article.IsDelete || !article.IsPublish)
                return new ApiResponse { Message = ErrorContent.ArticleNotFound };

            var result = _mapper.Map<ArticleGetDto>(article);
            return new ApiResponse { Success = true, Data = result };
        }
    }
}
