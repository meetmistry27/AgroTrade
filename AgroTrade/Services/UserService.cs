using AgroTrade.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AgroTrade.Services
{
    public class UserService
    {
        private ApplicationDbContext context;

        public UserService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> RegisterUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            if (await context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return false;   
            }

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return true;
        }



        public async Task<User> AuthenticateUser(string email, string password)
        {
            var user = await context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user != null && user.Password == password)
            {
                return user;  
            }

            return null;  
        }

        public async Task<User> GetUserProfile(int userId)
        {
            return await context.Users.FindAsync(userId);
        }

        public async Task<bool> UpdateUserProfile(User user)
        {
            var existingUser = await context.Users.FindAsync(user.UserId);
            if (existingUser == null) return false;

            existingUser.Phone = user.Phone;
            existingUser.Username = user.Username;

            await context.SaveChangesAsync();
            return true;
        }
    }
}
