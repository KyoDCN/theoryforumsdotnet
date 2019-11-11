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
    public class SubforumController : ControllerBase
    {
        private readonly IForumsRepository _Repo;

        public SubforumController(IForumsRepository repo)
        {
            _Repo = repo;
        }

        [AllowAnonymous]
        [HttpGet("forumId/{forumId}")]
        public async Task<IActionResult> GetSubforums(int forumId)
        {
            var query = from subforums in _Repo.Subforums
                        where subforums.ForumFK == forumId
                        select new
                        {
                            subforums.Id,
                            subforums.Title,
                            subforums.Slug,
                            subforums.Description,
                            subforums.Icon
                        };

            return Ok(await query.ToListAsync());
        }

        [AllowAnonymous]
        [HttpGet("{subforumId}")]
        public async Task<IActionResult> GetSubforum(int subforumId)
        {
            var query = from subforum in _Repo.Subforums
                        where subforum.Id == subforumId
                        select new ReturnSubforumDTO
                        {
                            Id = subforum.Id,
                            Title = subforum.Title, 
                            Slug = subforum.Slug,
                            Description = subforum.Description, 
                            Icon = subforum.Icon 
                        };

            return Ok(await query.FirstOrDefaultAsync());
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateSubforum(CreateSubforumDTO createSubforumDTO)
        {
            var forum = await _Repo.Forums.FirstOrDefaultAsync(x => x.Id == createSubforumDTO.ForumId);

            var newSubforum = new Subforum
            {
                Title = createSubforumDTO.Title,
                Slug = createSubforumDTO.Title.GenerateSlug(),
                Description = createSubforumDTO.Description ?? string.Empty,
                Icon = createSubforumDTO.Icon ?? string.Empty,
                ForumFK = forum.Id,
                Forum = forum
            };

            _Repo.Add(newSubforum);
            await _Repo.SaveChangesAsync();

            return await GetForumsAsync();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{subforumId}")]
        public async Task<IActionResult> DeleteSubforum(int subforumId)
        {
            Subforum forumToDelete = await _Repo.Subforums.FirstOrDefaultAsync(x => x.Id == subforumId);
            _Repo.Delete(forumToDelete);
            await _Repo.SaveChangesAsync();

            return await GetForumsAsync();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{subforumId}")]
        public async Task<IActionResult> UpdateSubforum(UpdateSubforumDTO updateSubforumDTO, int subforumId)
        {
            var subforumToUpdate = await _Repo.Subforums.FirstOrDefaultAsync(x => x.Id == subforumId);

            if (subforumToUpdate == null)
                return BadRequest("Subforum does not exist");

            subforumToUpdate.Title = updateSubforumDTO.Title;
            subforumToUpdate.Slug = updateSubforumDTO.Title.GenerateSlug();
            subforumToUpdate.Description = updateSubforumDTO.Description;
            subforumToUpdate.Icon = updateSubforumDTO.ImageUrl;

            _Repo.Update(subforumToUpdate);
            await _Repo.SaveChangesAsync();

            return await GetForumsAsync();
        }

        private async Task<IActionResult> GetForumsAsync()
        {
            var query = from forum in _Repo.Forums
                        select new
                        {
                            forum.Id,
                            forum.Title,
                            forum.Slug,
                            forum.Description,
                            Subforums = (from subforums in _Repo.Subforums
                                         where subforums.ForumFK == forum.Id
                                         select new
                                         {
                                             subforums.Id,
                                             subforums.Title,
                                             subforums.Slug,
                                             subforums.Description,
                                             subforums.Icon,
                                             LatestReply = (from thread in _Repo.Threads
                                                            where thread.SubforumFK == subforums.Id
                                                            orderby thread.LastUpdated descending
                                                            select new
                                                            {
                                                                ThreadId = thread.Id,
                                                                ThreadTitle = thread.Title,
                                                                ThreadSlug = thread.Slug,
                                                                PostReplyDate = thread.LastUpdated,
                                                                Author = "Author"
                                                            }).FirstOrDefault()
                                         }).ToList()
                        };

            return Ok(await query.ToListAsync());
        }
    }
}