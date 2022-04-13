﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soulgram.Eventbus.Interfaces;
using soulgram.identity.Data;
using Soulgram.Identity.IntegrationEvents;
using soulgram.identity.Models;
using Soulgram.Identity.Models;

namespace Soulgram.Identity.ApiControllers;

[Route("api/account")]
[Authorize(IdentityServerConstants.LocalApi.PolicyName)]
public class AccountApiController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountApiController(
        UserManager<ApplicationUser> userManager,
        IEventBus eventBus,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _eventBus = eventBus;
        _dbContext = dbContext;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterViewModel userModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var user = new ApplicationUser
        {
            Email = userModel.Email,
            UserName = userModel.Nickname
        };

        var result = await _userManager.CreateAsync(user, userModel.Password);
        if (!result.Succeeded)
        {
            return BadRequest(string.Join(",", result.Errors.Select(ie => ie.Description)));
        }

        var userCreatedEvent = new SuccessedUserRegistrationEvent(
            userId: user.Id,
            email: user.Email,
            nickname: userModel.Nickname,
            birthday: userModel.Birthday
        );

        _eventBus.Publish(userCreatedEvent);
        return Ok();
    }

    [HttpGet]
    public async Task<ApplicationUser> GetUser(CancellationToken cancellationToken)
    {
        var userId = User.Claims.First(c => c.Type == SoulgramClaimTypes.UserId).Value;

        var user = await _userManager.Users
            .Where(u => u.Id == userId)
            .FirstAsync(cancellationToken);

        return user;
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser(CancellationToken cancellationToken)
    {
        var user = await GetUser(cancellationToken);
        // var result = await _userManager.DeleteAsync(user);
        //
        // if (!result.Succeeded)
        // {
        //     return BadRequest(string.Join(",", result.Errors.Select(ie => ie.Description)));
        // }

        var userDeleteEvent = new DeletedUserEvent(user.Id);
        _eventBus.Publish(userDeleteEvent);

        return Ok();
    }
}