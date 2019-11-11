using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TheoryForums.Shared.Data;
using TheoryForums.Shared.DataTransferObjects;
using TheoryForums.Shared.Models;
using TheoryForums.Shared.Repositories;

namespace TheoryForums.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThreadController : ControllerBase
    {
        private readonly IForumsRepository _Repo;
        private readonly IUsersRepository _UserRepo;
        private readonly UserManager<User> _UserManager;
        private readonly SignInManager<User> _SignInManager;

        public ThreadController(IForumsRepository repo, IUsersRepository userRepo, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _Repo = repo;
            _UserRepo = userRepo;
            _UserManager = userManager;
            _SignInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpGet("subforumId/{subforumId}")]
        public async Task<IActionResult> GetThreads(int subforumId)
        {
            var threadsList = await (from threads in _Repo.Threads
                                     where threads.SubforumFK == subforumId
                                     orderby threads.LastUpdated descending
                                     select new ReturnThreadListDTO
                                     {
                                         Id = threads.Id,
                                         Title = threads.Title,
                                         Slug = threads.Slug,
                                         Author = new AuthorDTO() { Id = threads.AuthorId },
                                         CreatedOn = threads.CreatedOn,
                                         Replies = (from post in _Repo.Posts where post.ThreadFK == threads.Id select post).Count(),
                                         Views = threads.Views,
                                         LastReplyDate = threads.LastUpdated
                                     }).ToListAsync();

            foreach (var thread in threadsList)
            {
                thread.Author = await (from user in _UserRepo.Users
                                       where user.Id == thread.Author.Id
                                       select new AuthorDTO
                                       {
                                           Id = user.Id,
                                           DisplayName = user.DisplayName,
                                           AvatarUrl = user.AvatarUrl
                                       }).FirstOrDefaultAsync();
            }

            return Ok(threadsList);
        }

        [AllowAnonymous]
        [HttpGet("{threadId}")]
        public async Task<IActionResult> GetThread(int threadId)
        {
            var returnThread = await (from thread in _Repo.Threads
                                      where thread.Id == threadId
                                      select new ReturnThreadDTO
                                      {
                                          Id = thread.Id,
                                          Title = thread.Title,
                                          Content = thread.Content,
                                          CreatedOn = thread.CreatedOn,
                                          Edited = thread.Edited,
                                          LastEditDate = thread.LastEditDate,
                                          Subforum = (from subforum in _Repo.Subforums
                                                      where subforum.Id == thread.SubforumFK
                                                      select new ReturnSubforumDTO
                                                      {
                                                          Id = subforum.Id,
                                                          Title = subforum.Title,
                                                          Slug = subforum.Slug,
                                                          Icon = subforum.Icon,
                                                      }).FirstOrDefault(),
                                          Author = new AuthorDTO { Id = thread.AuthorId }
                                      }).FirstOrDefaultAsync();

            returnThread.Author = await (from user in _UserRepo.Users
                                         where user.Id == returnThread.Author.Id
                                         select new AuthorDTO
                                         {
                                             Id = returnThread.Author.Id,
                                             DisplayName = user.DisplayName,
                                             AvatarUrl = user.AvatarUrl
                                         }).FirstOrDefaultAsync();

            return Ok(returnThread);
        }

        [Authorize(Policy = "MemberOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateThread(CreateThreadDTO createThreadDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (await UserExists(userId) != true) 
                return Unauthorized();

            var subforum = await _Repo.Subforums.FirstOrDefaultAsync(x => x.Id == createThreadDTO.SubforumId);

            var newThread = new Thread
            {
                Title = createThreadDTO.Title,
                Content = createThreadDTO.Content,
                CreatedOn = DateTime.Now,
                LastUpdated = DateTime.Now,
                SubforumFK = subforum.Id,
                Subforum = subforum,
                Edited = false,
                AuthorId = int.Parse(userId)
            };

            _Repo.Add(newThread);
            await _Repo.SaveChangesAsync();

            return await GetThreads(createThreadDTO.SubforumId);
        }

        [Authorize(Policy = "MemberOnly")]
        [HttpDelete("{threadId}")]
        public async Task<IActionResult> DeleteThread(int threadId)
        {
            Thread threadToDelete = await _Repo.Threads.FirstOrDefaultAsync(x => x.Id == threadId);

            if (threadToDelete == null)
                return BadRequest("Thread does not exist");

            var userId = GetUserId();

            if (await UserExists(userId) != true) 
                return Unauthorized();

            if (IsAdmin() != true || threadToDelete.AuthorId != int.Parse(userId))
                return Unauthorized();

            _Repo.Delete(threadToDelete);
            await _Repo.SaveChangesAsync();

            return Ok("Thread successfully deleted");
        }

        [Authorize(Policy = "MemberOnly")]
        [HttpPut("{threadId}")]
        public async Task<IActionResult> UpdateThread(UpdateThreadDTO updateThreadDTO, int threadId)
        {
            var threadToUpdate = await _Repo.Threads.FirstOrDefaultAsync(x => x.Id == threadId);

            if (threadToUpdate == null)
                return BadRequest("Thread does not exist");

            var userId = GetUserId();

            if (await UserExists(userId) == false)
                return Unauthorized();

            if (IsAdmin() != true || threadToUpdate.AuthorId != int.Parse(userId))
                return Unauthorized();

            threadToUpdate.Title = updateThreadDTO.Title;

            if (threadToUpdate.Content != updateThreadDTO.Content)
            {
                threadToUpdate.Content = updateThreadDTO.Content;
                threadToUpdate.Edited = true;
                threadToUpdate.LastEditDate = DateTime.Now;
            }

            _Repo.Update(threadToUpdate);
            await _Repo.SaveChangesAsync();

            return Ok("Thread successfully updated");
        }

        [AllowAnonymous]
        [HttpPost("{threadId}/view")]
        public async Task<IActionResult> IncrementView(int threadId)
        {
            var thread = await _Repo.Threads.Where(x => x.Id == threadId).FirstOrDefaultAsync();
            thread.Views++;
            _Repo.Update(thread);
            await _Repo.SaveChangesAsync();

            return Ok(thread.Views);
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier).Value;

        private bool IsAdmin() => User.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Administrator");

        private async Task<bool> UserExists(string userId) => await _UserManager.FindByIdAsync(userId) != null;
    }
}