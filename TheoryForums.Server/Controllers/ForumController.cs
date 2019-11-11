using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TheoryForums.Shared.DataTransferObjects;
using TheoryForums.Shared.Models;
using TheoryForums.Shared.Repositories;
using TheorySlugify;

namespace TheoryForums.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForumController : ControllerBase
    {
        private readonly IForumsRepository _Repo;
        private readonly IUsersRepository _UserRepo;

        public ForumController(IForumsRepository repo, IUsersRepository userRepo)
        {
            _Repo = repo;
            _UserRepo = userRepo;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetForums()
        {
            var forumsList =
                await (from forum in _Repo.Forums
                       select new ReturnForumDTO
                       {
                           Id = forum.Id,
                           Title = forum.Title,
                           Slug = forum.Slug,
                           Description = forum.Description,
                           Subforums =
                           (from subforums in _Repo.Subforums
                            where subforums.ForumFK == forum.Id
                            select new ReturnSubforumDTO
                            {
                                Id = subforums.Id,
                                Title = subforums.Title,
                                Slug = subforums.Slug,
                                Description = subforums.Description,
                                Icon = subforums.Icon,
                                LatestReply =
                                (from thread in _Repo.Threads
                                 where thread.SubforumFK == subforums.Id
                                 orderby thread.LastUpdated descending
                                 select new LatestReply
                                 {
                                     ThreadId = thread.Id,
                                     ThreadTitle = thread.Title,
                                     ThreadSlug = thread.Slug,
                                     PostReplyDate = thread.LastUpdated,
                                     Author = new AuthorDTO() { Id = thread.AuthorId }
                                 }).FirstOrDefault()
                            }).ToList()
                       }).ToListAsync();

            foreach (var forum in forumsList)
            {
                foreach (var subforum in forum.Subforums)
                {
                    subforum.LatestReply.Author =
                        await (from user in _UserRepo.Users
                               where user.Id == subforum.LatestReply.Author.Id
                               select new AuthorDTO
                               {
                                   Id = user.Id,
                                   DisplayName = user.DisplayName,
                                   AvatarUrl = user.AvatarUrl
                               }).FirstOrDefaultAsync();
                }
            }

            return Ok(forumsList);
        }

        [HttpGet("{forumId}")]
        public async Task<IActionResult> GetForum(int forumId)
        {
            var query = from forum in _Repo.Forums
                        where forum.Id == forumId
                        select new { forum.Id, forum.Title, forum.Description };

            return Ok(await query.FirstOrDefaultAsync());
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateForum(CreateForumDTO createForumDTO)
        {
            var newForum = new Forum
            {
                Title = createForumDTO.Title,
                Slug = createForumDTO.Title.GenerateSlug(),
                Description = createForumDTO.Description ?? string.Empty
            };

            _Repo.Add(newForum);
            await _Repo.SaveChangesAsync();

            return await GetForums();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{forumId}")]
        public async Task<IActionResult> DeleteForum(int forumId)
        {
            Forum forumToDelete = await _Repo.Forums.FirstOrDefaultAsync(x => x.Id == forumId);
            _Repo.Delete(forumToDelete);
            await _Repo.SaveChangesAsync();

            return await GetForums();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{forumId}")]
        public async Task<IActionResult> UpdateForum(UpdateForumDTO updateForumDTO, int forumId)
        {
            var forumToUpdate = await _Repo.Forums.FirstOrDefaultAsync(x => x.Id == forumId);

            if (forumToUpdate == null)
                return BadRequest("Forum not found!");

            forumToUpdate.Title = updateForumDTO.Title;
            forumToUpdate.Slug = updateForumDTO.Title.GenerateSlug();
            forumToUpdate.Description = updateForumDTO.Description ?? string.Empty;

            _Repo.Update(forumToUpdate);
            await _Repo.SaveChangesAsync();

            return await GetForums();
        }
    }
}