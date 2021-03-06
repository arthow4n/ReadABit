using System.Linq;
using System;
using Microsoft.AspNetCore.Identity;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;
using System.Threading.Tasks;

namespace ReadABit.Web.Test.Helpers
{
    public class RequestContextMock : IRequestContext
    {
        public RequestContextMock(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task SignIn(string userName)
        {
            var existing = await _userManager.FindByNameAsync(userName);

            if (existing is not null)
            {
                _currentUser = existing;
                return;
            }

            var r = await _userManager.CreateAsync(new ApplicationUser
            {
                UserName = userName,
                Email = $"{userName}@localhost",
            });

            if (!r.Succeeded)
            {
                throw new Exception(string.Join("; ", r.Errors.Select(x => $"{x.Code}: {x.Description}")));
            }

            _currentUser = await _userManager.FindByNameAsync(userName);
        }

        public Task SignInWithDefaultUser()
        {
            return SignIn(_defaultUserName);
        }

        public Task SignInWithAnotherUser()
        {
            return SignIn(_anotherUserName);
        }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _defaultUserName = $"default-{Guid.NewGuid()}";
        private readonly string _anotherUserName = $"another-{Guid.NewGuid()}";
        private ApplicationUser? _currentUser;
        public Guid? UserId { get => _currentUser!.Id; set => throw new NotSupportedException(); }
    }
}
