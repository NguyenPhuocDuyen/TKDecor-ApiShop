using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IAuthenService
    {
        Task<ApiResponse> Register(UserRegisterDto dto);
        Task<ApiResponse> ConfirmMail(UserConfirmMailDto dto);
        Task<ApiResponse> ResendConfirmationEmail(UserEmailDto dto);
        Task<ApiResponse> Login(UserLoginDto dto);
        Task<ApiResponse> ForgotPassword(UserEmailDto dto);
        Task<ApiResponse> ConfirmForgotPassword(UserConfirmForgotPasswordDto dto);
        Task<ApiResponse> RenewToken(TokenModel model);
    }
}
