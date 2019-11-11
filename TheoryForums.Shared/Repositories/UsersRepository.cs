using System.Linq;
using System.Threading.Tasks;
using TheoryForums.Shared.Data;
using TheoryForums.Shared.Models;

namespace TheoryForums.Shared.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IdentityDataContext _Context;

        public IQueryable<User> Users { get; private set; }
        public IQueryable<UserRole> UserRoles { get; private set; }
        public IQueryable<Role> Roles { get; private set; }

        public UsersRepository(IdentityDataContext context)
        {
            _Context = context;
            Users = _Context.Users;
            UserRoles = _Context.UserRoles;
            Roles = _Context.Roles;
        }

        public void Add<T>(T entity) where T : class => _Context.Add(entity);

        public void Delete<T>(T entity) where T : class => _Context.Remove(entity);

        public void Update<T>(T entity) where T : class => _Context.Update(entity);

        public async Task<bool> SaveChangesAsync() => await _Context.SaveChangesAsync() > 0;
    }
}
