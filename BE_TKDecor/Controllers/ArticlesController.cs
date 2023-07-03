using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Dtos.Article;
using BE_TKDecor.Core.Response;

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
        public async Task<IActionResult> GetAll()
        {
            var list = await _article.GetAll();
            list = list.Where(x => x.IsDelete == false && x.IsPublish == true)
                .OrderByDescending(x => x.UpdatedAt)
                .ToList();
            var result = _mapper.Map<List<ArticleGetDto>>(list);

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Articles/GetBySlug/abc-def
        [HttpGet("GetBySlug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var article = await _article.FindBySlug(slug);
            if (article == null || article.IsDelete == true)
                return NotFound(new ApiResponse { Message = ErrorContent.ArticleNotFound });

            var result = _mapper.Map<ArticleGetDto>(article);
            return Ok(new ApiResponse { Success = true, Data = result });
        }
    }
}
