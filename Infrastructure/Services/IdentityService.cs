using Application.Common.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services;

internal class IdentityService : IIdentityService {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public Task<bool> AuthorizeAsync(Guid userId) {
        throw new NotImplementedException();
    }

    public Task<object> CreateUserAsync() {
        throw new NotImplementedException();
    }

    public Task<string> GetUserNameAsync(Guid userId) {
        throw new NotImplementedException();
    }

    public Task<bool> IsInRoleAsync(Guid userId, string roleName) {
        throw new NotImplementedException();
    }
}