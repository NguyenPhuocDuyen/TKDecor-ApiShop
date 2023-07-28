using AutoMapper;
using BE_TKDecor.Core.Dtos.Article;
using BE_TKDecor.Core.Dtos.Notification;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Hubs;
using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Utility;
using Utility.SD;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = $"{RoleContent.Seller},{RoleContent.Admin}")]
    public class ManagementArticlesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _user;
        private readonly IArticleRepository _article;
        private readonly INotificationRepository _notification;
        private readonly IHubContext<NotificationHub> _hub;

        public ManagementArticlesController(IMapper mapper,
            IUserRepository user,
            IArticleRepository article,
            INotificationRepository notification,
            IHubContext<NotificationHub> hub)
        {
            _mapper = mapper;
            _user = user;
            _article = article;
            _notification = notification;
            _hub = hub;
        }

        // GET: api/ManagementArticles/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _article.GetAll();
            list = list
                .Where(x => !x.IsDelete)
                .OrderByDescending(x => x.UpdatedAt)
                .ToList();

            var result = _mapper.Map<List<ArticleGetDto>>(list);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // POST: api/ManagementArticles/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(ArticleCreateDto articleDto)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return BadRequest(new ApiResponse { Message = ErrorContent.UserNotFound });

            var newSlug = Slug.GenerateSlug(articleDto.Title);
            var articleDb = await _article.FindBySlug(newSlug);

            //bool isAdd = true;

            if (articleDb == null)
                newSlug += Guid.NewGuid();
            //{
            // create new article info
            articleDb = new Article();
            articleDb = _mapper.Map<Article>(articleDto);
            articleDb.Slug = newSlug;
            articleDb.UserId = user.UserId;
            //}
            //else
            //{
            //    // if article exits and not delete 
            //    if (!articleDb.IsDelete)
            //        return BadRequest(new ApiResponse { Message = "Please change the name due to duplicate data!" });

            //    articleDb.IsDelete = false;
            //    isAdd = false;

            //    articleDb.Title = articleDto.Title;
            //    articleDb.Content = articleDto.Content;
            //    articleDb.Thumbnail = articleDto.Thumbnail;
            //    articleDb.IsPublish = articleDto.IsPublish;
            //    articleDb.UpdatedAt = DateTime.Now;
            //}

            try
            {
                //if (isAdd)
                //{
                    await _article.Add(articleDb);
                //}
                //else
                //{
                //    await _article.Update(articleDb);
                //}
                //// add notification for user
                //Notification newNotification = new()
                //{
                //    UserId = user.UserId,
                //    User = user,
                //    Message = $"Thêm bài viết mới thành công. Tiêu đề: " + articleDb.Title
                //};
                //await _notification.Add(newNotification);
                //// notification signalR
                //await _hub.Clients.User(user.UserId.ToString())
                //    .SendAsync(Common.NewNotification,
                //    _mapper.Map<NotificationGetDto>(newNotification));

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
            if (articleDb == null || articleDb.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ArticleNotFound });

            var newSlug = Slug.GenerateSlug(articleDto.Title);
            var articleSlug = await _article.FindBySlug(newSlug);
            if (articleSlug != null && articleSlug.ArticleId != id)
                return BadRequest(new ApiResponse { Message = "Vui lòng đổi tên do dữ liệu trùng lặp!" });

            // update info article
            articleDb.Title = articleDto.Title;
            articleDb.Content = articleDto.Content;
            articleDb.Thumbnail = articleDto.Thumbnail;
            articleDb.UpdatedAt = DateTime.Now;
            try
            {
                await _article.Update(articleDb);

                //// add notification for user
                //Notification newNotification = new()
                //{
                //    UserId = user.UserId,
                //    User = user,
                //    Message = $"Cập nhật bài viết mới thành công. Tiêu đề: " + articleDb.Title
                //};
                //await _notification.Add(newNotification);
                //// notification signalR
                //await _hub.Clients.User(user.UserId.ToString())
                //    .SendAsync(Common.NewNotification,
                //    _mapper.Map<NotificationGetDto>(newNotification));

                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // DELETE: api/ManagementArticles/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteArticle(Guid id)
        {
            var article = await _article.FindById(id);
            if (article == null || article.IsDelete)
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
            if (article == null || article.IsDelete)
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
