using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using AutoMapper;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using BE_TKDecor.Core.Dtos.UserAddress;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Core.Dtos.Notification;
using Microsoft.AspNetCore.SignalR;
using BE_TKDecor.Hubs;
using Utility.SD;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserAddressesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _user;
        private readonly IUserAddressRepository _userAddress;
        private readonly INotificationRepository _notification;
        private readonly IHubContext<NotificationHub> _hub;

        public UserAddressesController(IMapper mapper,
            IUserRepository user,
            IUserAddressRepository userAddress,
            INotificationRepository notification,
            IHubContext<NotificationHub> hub
            )
        {
            _mapper = mapper;
            _user = user;
            _userAddress = userAddress;
            _notification = notification;
            _hub = hub;
        }

        // GET: api/UserAddresses/GetUserAddresses
        [HttpGet("GetUserAddresses")]
        public async Task<IActionResult> GetUserAddresses()
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var list = (await _userAddress.FindByUserId(user.UserId))
                .Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt);
            var result = _mapper.Map<List<UserAddressGetDto>>(list);

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/UserAddresses/GetUserAddressDefault
        [HttpGet("GetUserAddressDefault")]
        public async Task<IActionResult> GetUserAddressDefault()
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var address = (await _userAddress.FindByUserId(user.UserId))
                .FirstOrDefault(x => !x.IsDelete && x.IsDefault);

            var result = _mapper.Map<UserAddressGetDto>(address);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/UserAddresses/SetDefault
        [HttpPost("SetDefault")]
        public async Task<IActionResult> SetDefault(UserAddressSetDefaultDto dto)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var address = await _userAddress.FindById(dto.UserAddressId);
            if (address == null || address.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.AddressNotFound });

            var listAddress = await _userAddress.FindByUserId(user.UserId);
            //address.IsDefault = true;
            //address.UpdatedAt = DateTime.Now;
            try
            {
                foreach (var ad in listAddress)
                {
                    if (ad.UserAddressId == dto.UserAddressId)
                    {
                        ad.IsDefault = true;
                        ad.UpdatedAt = DateTime.Now;
                    }
                    else if (ad.IsDefault)
                    {
                        ad.IsDefault = false;
                        ad.UpdatedAt = DateTime.Now;
                    }
                    await _userAddress.Update(ad);
                }

                //// add notification for user
                //Notification newNotification = new()
                //{
                //    UserId = user.UserId,
                //    Message = $"Thay đổi địa chỉ mặc định thành công"
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

        // POST: api/UserAddresses/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(UserAddressCreateDto userAddressDto)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            UserAddress newAddress = _mapper.Map<UserAddress>(userAddressDto);
            newAddress.UserId = user.UserId;
            try
            {
                await _userAddress.Add(newAddress);

                var listAddress = await _userAddress.FindByUserId(user.UserId);
                if (listAddress.Count == 1)
                {
                    listAddress[0].IsDefault = true;
                    await _userAddress.Update(listAddress[0]);
                }

                //// add notification for user
                //Notification newNotification = new()
                //{
                //    UserId = user.UserId,
                //    Message = $"Thêm địa chỉ mới thành công"
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

        // POST: api/UserAddresses/Update/1
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateUserAddress(Guid id, UserAddressUpdateDto userAddressDto)
        {
            if (id != userAddressDto.UserAddressId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var userAddressDb = await _userAddress.FindById(id);
            if (userAddressDb == null || userAddressDb.IsDelete || userAddressDb.UserId != user.UserId)
                return NotFound(new ApiResponse { Message = ErrorContent.AddressNotFound });

            userAddressDb.FullName = userAddressDto.FullName;
            //userAddressDb.Address = userAddressDto.Address;
            userAddressDb.Phone = userAddressDto.Phone;
            userAddressDb.CityCode = userAddressDto.CityCode;
            userAddressDb.City = userAddressDto.City;
            userAddressDb.DistrictCode = userAddressDto.DistrictCode;
            userAddressDb.District = userAddressDto.District;
            userAddressDb.WardCode = userAddressDto.WardCode;
            userAddressDb.Ward = userAddressDto.Ward;
            userAddressDb.Street = userAddressDto.Street;
            userAddressDb.UpdatedAt = DateTime.Now;
            try
            {
                await _userAddress.Update(userAddressDb);

                //// add notification for user
                //Notification newNotification = new()
                //{
                //    UserId = user.UserId,
                //    Message = $"Cập nhật địa chỉ thành công"
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

        // DELETE: api/UserAddresses/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteUserAddress(Guid id)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var userAddress = await _userAddress.FindById(id);
            if (userAddress == null || userAddress.UserId != user.UserId)
                return NotFound(new ApiResponse { Message = ErrorContent.AddressNotFound });

            if (userAddress.IsDefault)
                return BadRequest(new ApiResponse { Message = "Không thể xóa địa chỉ mặc định!" });

            userAddress.IsDelete = true;
            userAddress.UpdatedAt = DateTime.Now;
            try
            {
                await _userAddress.Update(userAddress);

                //// add notification for user
                //Notification newNotification = new()
                //{
                //    UserId = user.UserId,
                //    Message = $"Xoá địa chỉ thành công"
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
