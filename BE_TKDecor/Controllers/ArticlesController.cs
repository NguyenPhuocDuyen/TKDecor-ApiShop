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
        private readonly IArticleRepository _articleRepository;

        public ArticlesController(IMapper mapper,
            IArticleRepository articleRepository)
        {
            _mapper = mapper;
            _articleRepository = articleRepository;
        }

        // GET: api/Articles/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _articleRepository.GetAll();
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
            var article = await _articleRepository.FindBySlug(slug);
            if (article == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ArticleNotFound });

            var result = _mapper.Map<ArticleGetDto>(article);
            return Ok(new ApiResponse { Success = true, Data = result });
        }
    }
}
