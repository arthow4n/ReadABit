using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ReadABit.Infrastructure.Models;

namespace ReadABit.Core.Utils
{
    public class RequestContext : IRequestContext
    {
        public RequestContext(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private Guid? _userId;
        public Guid? UserId
        {
            get
            {
                if (_userId is null)
                {
                    var success = Guid.TryParse(_userManager.GetUserId(_httpContextAccessor.HttpContext.User), out var userId);
                    if (success)
                    {
                        _userId = userId;
                        return userId;
                    }
                };
                return _userId;
            }
            set => throw new NotImplementedException();
        }
    }

    public interface IRequestContext
    {
        Guid? UserId { get; set; }
    }
}
