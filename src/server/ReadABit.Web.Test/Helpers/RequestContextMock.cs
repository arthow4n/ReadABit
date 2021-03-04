using System.Linq;
using System;
using Microsoft.AspNetCore.Identity;
using ReadABit.Core.Utils;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Web.Test.Helpers
{
    public class RequestContextMock : IRequestContext
    {
        public RequestContextMock(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public void SignIn(string? userName = null)
        {
            userName ??= $"default-{Guid.NewGuid()}";
            var r = _userManager.CreateAsync(new ApplicationUser
            {
                UserName = userName,
                Email = "example@localhost",
            }).GetAwaiter().GetResult();

            if (!r.Succeeded)
            {
                throw new Exception(string.Join("; ", r.Errors.Select(x => $"{x.Code}: {x.Description}")));
            }

            _currentUser = _userManager.FindByNameAsync(userName).GetAwaiter().GetResult();
        }

        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationUser? _currentUser;
        public Guid? UserId { get => _currentUser!.Id; set => throw new NotSupportedException(); }
    }
}
