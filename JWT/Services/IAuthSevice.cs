using testApi.Models;

namespace testApi.Services
{
    public interface IAuthSevice
    {
        /// <summary>
        /// Making Sign Up 
        /// </summary>
        /// <param name="model"> Data To Make Sign Up</param>
        /// <returns></returns>
        Task<AuthModel> RegisterAsync(RegisterModel model);

        /// <summary>
        /// Making Log in
        /// </summary>
        /// <param name="model">Take data to make log in</param>
        /// <returns></returns>
        Task<AuthModel> GetTokenAsync(TokenRequsetModel model);

        /// <summary>
        /// To adding spacefic user to spacefic Role("User","Admin"...)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> AddRoleAsync(AddRoleModel model);
    }
}
