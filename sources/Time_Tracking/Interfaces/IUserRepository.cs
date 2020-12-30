using System.Collections.Generic;
using System.Threading.Tasks;
using Time_Tracking.Models;

namespace Time_Tracking.Interfaces
{
    public interface IUserRepository
    {
        public Task<List<User>> GetAllAsync();

        public Task<User> FirstOrDefaultByEmailAsync(User user);

        public Task<User> FirstOrDefaultByIdAsync(int? id);

        public Task<User> CheckUniqueEmail(User user, string currentEmail);

        public void Remove(User user);

        public void Update(User user);

        public Task AddAsync(User user);

        public Task SaveChanges();

    }
}
