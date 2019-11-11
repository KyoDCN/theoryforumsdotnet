using System.Linq;
using System.Threading.Tasks;
using TheoryForums.Shared.Models;

namespace TheoryForums.Shared.Repositories
{
    public interface IForumsRepository
    {
        IQueryable<Forum> Forums { get; }
        IQueryable<Post> Posts { get; }
        IQueryable<Subforum> Subforums { get; }
        IQueryable<Thread> Threads { get; }

        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveChangesAsync();
        void Update<T>(T entity) where T : class;
    }
}