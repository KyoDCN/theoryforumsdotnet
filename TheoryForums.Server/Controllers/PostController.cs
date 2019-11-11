using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using TheoryForums.Shared.DataTransferObjects;
using TheoryForums.Shared.Models;
using TheoryForums.Shared.Repositories;

namespace TheoryForums.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IForumsRepository _Repo;
        private readonly IUsersRepository _UsersRepo;
        private readonly UserManager<User> _UserManager;

        public PostController(IForumsRepository repo, IUsersRepository usersRepo, UserManager<User> userManager)
        {
            _Repo = repo;
            _UsersRepo = usersRepo;
            _UserManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet("threadId/{threadId}")]
        public async Task<IActionResult> GetPosts(int threadId)
        {
            var postsList = await (from posts in _Repo.Posts
                                   where posts.ThreadFK == threadId
                                   orderby posts.CreatedOn ascending
                                   select new ReturnPostDTO
                                   {
                                       Id = posts.Id,
                                       Content = posts.Content,
                                       CreatedOn = posts.CreatedOn,
                                       Edited = posts.Edited,
                                       LastEditDate = posts.LastEditDate,
                                       Author = new AuthorDTO { Id = posts.AuthorId }
                                   }).ToListAsync();

            foreach (var post in postsList)
            {
                post.Author = await (from user in _UsersRepo.Users
                                     where user.Id == post.Author.Id
                                     select new AuthorDTO
                                     {
                                         Id = user.Id,
                                         DisplayName = user.DisplayName,
                                         AvatarUrl = user.AvatarUrl
                                     }).FirstOrDefaultAsync();
            }
            return Ok(postsList);
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPost(int postId)
        {
            var postReturn = await (from post in _Repo.Posts
                                    where post.Id == postId
                                    select new ReturnPostDTO
                                    {
                                        Id = post.Id,
                                        Content = post.Content,
                                        CreatedOn = post.CreatedOn,
                                        Edited = post.Edited,
                                        LastEditDate = post.LastEditDate,
                                        Author = new AuthorDTO { Id = post.AuthorId }
                                    }).FirstOrDefaultAsync();

            postReturn.Author = await (from user in _UsersRepo.Users
                                 where user.Id == postReturn.Author.Id
                                 select new AuthorDTO
                                 {
                                     Id = user.Id,
                                     DisplayName = user.DisplayName,
                                     AvatarUrl = user.AvatarUrl
                                 }).FirstOrDefaultAsync();

            return Ok(postReturn);
        }

        [Authorize(Policy = "MemberOnly")]
        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostDTO createPostDTO)
        {
            var userId = GetUserId();

            // Does User exist?
            if (await UserExists(userId) != true)
                return Unauthorized();

            var thread = await _Repo.Threads.FirstOrDefaultAsync(x => x.Id == createPostDTO.ThreadId);

            var newPost = new Post
            {
                Content = JsonSerializer.Serialize(createPostDTO.Content),
                CreatedOn = DateTime.Now,
                ThreadFK = thread.Id,
                Thread = thread,
                AuthorId = int.Parse(userId)
            };

            thread.LastUpdated = DateTime.Now;
            
            _Repo.Add(newPost);
            _Repo.Update(thread);

            await _Repo.SaveChangesAsync();

            return await GetPosts(createPostDTO.ThreadId);
        }

        [Authorize(Policy = "MemberOnly")]
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var userId = GetUserId();

            // Does User Exist?
            if (await UserExists(userId) != true)
                return Unauthorized();

            Post postToDelete = await _Repo.Posts.FirstOrDefaultAsync(x => x.Id == postId);

            // Does Post Exist?
            if (postToDelete == null)
                return BadRequest("Post does not exist");

            // Is User an Admin or the Post's Owner?
            if (IsAdmin() || postToDelete.AuthorId != int.Parse(userId))
                return Unauthorized();

            _Repo.Delete(postToDelete);
            await _Repo.SaveChangesAsync();

            return Ok("Post successfully deleted");
        }

        [Authorize(Policy = "MemberOnly")]
        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(UpdatePostDTO updatePostDTO, int postId)
        {
            var userId = GetUserId();

            // Does User exist?
            if (await UserExists(userId) != true)
                return Unauthorized();

            var postToUpdate = await _Repo.Posts.FirstOrDefaultAsync(x => x.Id == postId);

            // Does Post exist?
            if (postToUpdate == null)
                return BadRequest("Post does not exist");

            // Is User an Admin or the Post's Owner?
            if (IsAdmin() == false || postToUpdate.AuthorId != int.Parse(userId))
                return Unauthorized();

            if (postToUpdate.Content == JsonSerializer.Serialize(updatePostDTO.Content))
                return await GetPost(postToUpdate.Id);

            postToUpdate.Content = JsonSerializer.Serialize(updatePostDTO.Content);
            postToUpdate.Edited = true;
            postToUpdate.LastEditDate = DateTime.Now;

            _Repo.Update(postToUpdate);
            await _Repo.SaveChangesAsync();

            return await GetPost(postToUpdate.Id);
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier).Value;

        private bool IsAdmin() => User.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Administrator");

        private async Task<bool> UserExists(string userId) => await _UserManager.FindByIdAsync(userId) != null;
    }
}