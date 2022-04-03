using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using soulgram.identity.Models;
using Soulgram.Eventbus.Interfaces;
using Soulgram.Identity.IntegrationEvents;
using Soulgram.Identity.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Soulgram.File.Manager.Interfaces;
using soulgram.identity.Data;
using soulgram.identity.Services;

namespace Soulgram.Identity.ApiControllers
{
	[Route("api/account")]
	[Authorize(IdentityServerConstants.LocalApi.PolicyName)]
	public class AccountApiController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IFileManager _fileManager;
		private readonly IEventBus _eventBus;
		private readonly ApplicationDbContext _dbContext;
		
		public AccountApiController(
			UserManager<ApplicationUser> userManager,
			IEventBus eventBus,
			IFileManager fileManager, ApplicationDbContext dbContext)
		{
			_userManager = userManager;
			_eventBus = eventBus;
			_fileManager = fileManager;
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
				UserName = userModel.Email
			};

			var result = await _userManager.CreateAsync(user, userModel.Password);
			if (!result.Succeeded)
			{
				return BadRequest(string.Join(",", result.Errors.Select(ie => ie.Description)));
			}

			var userCreatedEvent = new SuccessUserRegistrationEvent(
				userId: user.Id,
				userName: user.UserName,
				email: user.Email
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
		
		[HttpGet("compact")]
		public async Task<CompactUserInfo> GetUserCompact(CancellationToken cancellationToken)
		{
			var userId = User.Claims.First(c => c.Type == SoulgramClaimTypes.UserId).Value;
			var user = await _dbContext.Users
				.Where(u => u.Id == userId)
				.Select(ui => new CompactUserInfo
				(
					ui.Fullname,
					ui.UserName,
					ui.Hobbies,
					ui.ProfileImg
				))
				.FirstOrDefaultAsync(cancellationToken);
			
			return user;
		}

		[HttpPut]
		//TODO add input parameters validation
		public async Task<IActionResult> UpdateUser([FromBody] ApplicationUser user, CancellationToken cancellationToken)
		{
			var userToUpdate = await GetUser(cancellationToken);
			userToUpdate.CheckThenUpdate(user);
			
			var result = await _userManager.UpdateAsync(userToUpdate);
			if (!result.Succeeded)
			{
				return BadRequest(string.Join(",", result.Errors.Select(ie => ie.Description)));
			}

			return Ok();
		}

		[HttpPut("profile-img")]
		public async Task<IActionResult> UpdateProfilePicture(
			[FromForm] IFormFile picture,
			CancellationToken cancellationToken)
		{
			var user = await GetUser(cancellationToken);

			var fileInfo = picture.ToFileInfo();
			var uploadedPicture = await _fileManager.UploadFileAsync(fileInfo, user.Id);

			user.ProfileImg = uploadedPicture;

			await UpdateUser(user, cancellationToken);
			
			return Ok();
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteUser(CancellationToken cancellationToken)
		{
			var user = await GetUser(cancellationToken);
			var result = await _userManager.DeleteAsync(user);

			if (!result.Succeeded)
			{
				return BadRequest(string.Join(",", result.Errors.Select(ie => ie.Description)));
			}

			var userDeleteEvent = new DeleteUserEvent(user.Id);
			_eventBus.Publish(userDeleteEvent);

			return Ok();
		}
	}
}
