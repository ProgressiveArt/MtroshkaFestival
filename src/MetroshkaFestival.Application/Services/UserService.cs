using System.Threading.Tasks;
using Interfaces.Application;
using MetroshkaFestival.Core.Exceptions.ExceptionCodes;
using MetroshkaFestival.Data;
using MetroshkaFestival.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MetroshkaFestival.Application.Services
{
    public class UserService : IService
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserService(DataContext dataContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        private async Task<User> GetOrDefaultAsync(string username)
        {
            return await _dataContext.Users.FirstOrDefaultAsync(x => x.NormalizedEmail == username.ToUpper());
        }

        public async Task<string> SignInAsync(string username, string password, bool isPersistent)
        {
            var user = await GetOrDefaultAsync(username);
            if (user == null)
            {
                return UserExceptionCodes.FailedLoginAttempt;
            }

            var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (!emailConfirmed)
            {
                return UserExceptionCodes.EmailNotConfirmed;
            }

            var singInResult = await _signInManager.PasswordSignInAsync(username, password, isPersistent, false);
            return !singInResult.Succeeded ? UserExceptionCodes.FailedLoginAttempt : null;
        }

        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }
    }
}