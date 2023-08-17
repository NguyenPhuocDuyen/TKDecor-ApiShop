using AutoMapper;
using BE_TKDecor.Core.Dtos.Article;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace BE_TKDecor.Service
{
    public class ArticleService : IArticleService
    {
        private readonly TkdecorContext _context;
        private readonly IMapper _mapper;
        private ApiResponse _response;

        public ArticleService(TkdecorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        // create article
        public async Task<ApiResponse> Create(ArticleCreateDto dto, Guid userId)
        {
            // check slug already exists
            var newSlug = Slug.GenerateSlug(dto.Title);
            var articleDb = await _context.Articles.FirstOrDefaultAsync(x => x.Slug == newSlug);
            // auto - regenerate slug
            if (articleDb is not null)
                newSlug += new Random().Next(1000, 9999);

            try
            {
                articleDb = new Article();
                articleDb = _mapper.Map<Article>(dto);
                articleDb.Slug = newSlug;
                articleDb.UserId = userId;

                _context.Articles.Add(articleDb);
                await _context.SaveChangesAsync();

                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // delete article by id
        public async Task<ApiResponse> Delete(Guid id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article is null || article.IsDelete)
            {
                _response.Message = ErrorContent.ArticleNotFound;
                return _response;
            }

            article.IsDelete = true;
            article.UpdatedAt = DateTime.Now;
            try
            {
                _context.Articles.Update(article);
                await _context.SaveChangesAsync();

                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }

            return _response;
        }

        // get list of articles for admin
        public async Task<ApiResponse> GetAll()
        {
            var list = await _context.Articles
                .Include(x => x.User)
                .Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            try
            {
                var result = _mapper.Map<List<ArticleGetDto>>(list);
                _response.Data = result;
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get list of articles for customer have filter
        public async Task<ApiResponse> GetAll(string sort = "default", int pageIndex = 1, int pageSize = 20)
        {
            var list = await _context.Articles
                .Include(x => x.User)
                .Where(x => !x.IsDelete && x.IsPublish)
                .ToListAsync();

            try
            {
                var listArticleGet = _mapper.Map<List<ArticleGetDto>>(list);
                // filter sort
                listArticleGet = sort switch
                {
                    "date-old" => listArticleGet.OrderBy(x => x.CreatedAt).ToList(),
                    _ => listArticleGet.OrderByDescending(x => x.CreatedAt).ToList(),
                };
                // paging
                PaginatedList<ArticleGetDto> pagingArticle = PaginatedList<ArticleGetDto>.CreateAsync(
                    listArticleGet, pageIndex, pageSize);

                var result = new
                {
                    articles = pagingArticle,
                    pagingArticle.PageIndex,
                    pagingArticle.TotalPages,
                    pagingArticle.TotalItem
                };
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get article by id for admin
        public async Task<ApiResponse> GetById(Guid id)
        {
            var article = await _context.Articles
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.ArticleId == id && !x.IsDelete);

            if (article is null)
            {
                _response.Message = ErrorContent.ArticleNotFound;
                return _response;
            }

            try
            {
                var result = _mapper.Map<ArticleGetDto>(article);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get article by slug for customer
        public async Task<ApiResponse> GetBySlug(string slug)
        {
            var article = await _context.Articles
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Slug == slug && !x.IsDelete);

            if (article is null || !article.IsPublish)
            {
                _response.Message = ErrorContent.ArticleNotFound;
                return _response;
            }

            try
            {
                var result = _mapper.Map<ArticleGetDto>(article);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // set publish for article
        public async Task<ApiResponse> SetPublish(ArticleSetPublishDto articleDto)
        {
            var article = await _context.Articles.FindAsync(articleDto.ArticleId);

            if (article is null || article.IsDelete)
            {
                _response.Message = ErrorContent.ArticleNotFound;
                return _response;
            }

            article.IsPublish = articleDto.Published;
            article.UpdatedAt = DateTime.Now;
            try
            {
                _context.Articles.Update(article);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // update article
        public async Task<ApiResponse> Update(Guid id, ArticleUpdateDto dto)
        {
            if (id != dto.ArticleId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            var articleDb = await _context.Articles.FindAsync(id);
            if (articleDb is null || articleDb.IsDelete)
            {
                _response.Message = ErrorContent.ArticleNotFound;
                return _response;
            }

            // check slug already exists
            var newSlug = Slug.GenerateSlug(dto.Title);
            var articleSlug = await _context.Articles.FirstOrDefaultAsync(x => x.Slug == newSlug);

            // auto - regenerate slug
            if (articleSlug is not null && articleSlug.ArticleId != id)
                newSlug += new Random().Next(1000, 9999);

            // update info article
            articleDb.Title = dto.Title;
            articleDb.Content = dto.Content;
            articleDb.Thumbnail = dto.Thumbnail;
            articleDb.Slug = newSlug;
            articleDb.UpdatedAt = DateTime.Now;
            try
            {
                _context.Articles.Update(articleDb);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }
    }
}
