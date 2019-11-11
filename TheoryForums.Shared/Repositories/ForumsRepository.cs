using System.Linq;
using System.Threading.Tasks;
using TheoryForums.Shared.Data;
using TheoryForums.Shared.Models;

namespace TheoryForums.Shared.Repositories
{
    public class ForumsRepository : IForumsRepository
    {
        private readonly DataContext _Context;
        public IQueryable<Forum> Forums { get; }
        public IQueryable<Subforum> Subforums { get; }
        public IQueryable<Thread> Threads { get; }
        public IQueryable<Post> Posts { get; }

        public ForumsRepository(DataContext context)
        {
            _Context = context;
            Forums = _Context.Forums;
            Subforums = _Context.Subforums;
            Threads = _Context.Threads;
            Posts = _Context.Posts;
        }

        public void Add<T>(T entity) where T : class => _Context.Add(entity);

        public void Delete<T>(T entity) where T : class => _Context.Remove(entity);

        public void Update<T>(T entity) where T : class => _Context.Update(entity);

        public async Task<bool> SaveChangesAsync() => await _Context.SaveChangesAsync() > 0;
    }
}
