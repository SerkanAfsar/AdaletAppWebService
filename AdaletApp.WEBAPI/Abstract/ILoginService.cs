using AdaletApp.WEBAPI.Utilities;
using AdaletApp.WEBAPI.ViewModels;

namespace AdaletApp.WEBAPI.Abstract
{
    public interface ILoginService
    {
        Task<ResponseResult<TokenDTO>> TokenResult(UserLoginDTO loginViewModel);
    }
}
