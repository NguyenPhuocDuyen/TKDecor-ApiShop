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

        public async Task<ApiResponse> Create(ArticleCreateDto dto, Guid userId)
        {
            var newSlug = Slug.GenerateSlug(dto.Title);
            var articleDb = await _context.Articles.FirstOrDefaultAsync(x => x.Slug == newSlug);

            //bool isAdd = true;
            if (articleDb == null)
                newSlug += new Random().Next(1000, 9999);

            articleDb = new Article();
            articleDb = _mapper.Map<Article>(dto);
            articleDb.Slug = newSlug;
            articleDb.UserId = userId;

            try
            {
                _context.Articles.Add(articleDb);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null || article.IsDelete)
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
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }

        public async Task<ApiResponse> GetAll()
        {
            var list = await _context.Articles.Include(x => x.User).ToListAsync();
            list = list
                .Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var result = _mapper.Map<List<ArticleGetDto>>(list);
            _response.Success = true;
            _response.Data = result;
            return _response;
        }

        public async Task<ApiResponse> GetAll(string sort = "default", int pageIndex = 1, int pageSize = 20)
        {
            var list = await _context.Articles.Include(x => x.User).ToListAsync();
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
            _response.Success = true;
            _response.Data = result;
            return _response;
        }

        public async Task<ApiResponse> GetById(Guid id)
        {
            var article = await _context.Articles.Include(x => x.User).FirstOrDefaultAsync(x => x.ArticleId == id);
            if (article == null || article.IsDelete)
            {
                _response.Message = ErrorContent.ArticleNotFound;
                return _response;
            }

            var result = _mapper.Map<ArticleGetDto>(article);
            _response.Success = true;
            _response.Data = result;
            return _response;
        }

        public async Task<ApiResponse> GetBySlug(string slug)
        {
            var article = await _context.Articles.Include(x => x.User).FirstOrDefaultAsync(x => x.Slug == slug);

            if (article == null || article.IsDelete || !article.IsPublish)
            {
                _response.Message = ErrorContent.ArticleNotFound;
                return _response;
            }

            var result = _mapper.Map<ArticleGetDto>(article);
            _response.Success = true;
            _response.Data = result;
            return _response;
        }

        public async Task<ApiResponse> SetPublish(ArticleSetPublishDto articleDto)
        {
            var article = await _context.Articles.FindAsync(articleDto.ArticleId);
            if (article == null || article.IsDelete)
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
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }

        public async Task<ApiResponse> Update(Guid id, ArticleUpdateDto dto)
        {
            if (id != dto.ArticleId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            var articleDb = await _context.Articles.FindAsync(id);
            if (articleDb == null || articleDb.IsDelete)
            {
                _response.Message = ErrorContent.ArticleNotFound;
                return _response;
            }

            var newSlug = Slug.GenerateSlug(dto.Title);
            var articleSlug = await _context.Articles.FirstOrDefaultAsync(x => x.Slug == newSlug);
            if (articleSlug != null && articleSlug.ArticleId != id)
            {
                _response.Message = "Vui lòng đổi tên do dữ liệu trùng lặp!";
                return _response;
            }

            // update info article
            articleDb.Title = dto.Title;
            articleDb.Content = dto.Content;
            articleDb.Thumbnail = dto.Thumbnail;
            articleDb.UpdatedAt = DateTime.Now;
            try
            {
                _context.Articles.Update(articleDb);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }
    }
}
