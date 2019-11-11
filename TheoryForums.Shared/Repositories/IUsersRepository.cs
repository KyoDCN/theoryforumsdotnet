using System.Linq;
using System.Threading.Tasks;
using TheoryForums.Shared.Models;

namespace TheoryForums.Shared.Repositories
{
    public interface IUsersRepository
    {
        IQueryable<Role> Roles { get; }
        IQueryable<UserRole> UserRoles { get; }
        IQueryable<User> Users { get; }

        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveChangesAsync();
        void Update<T>(T entity) where T : class;
    }
}