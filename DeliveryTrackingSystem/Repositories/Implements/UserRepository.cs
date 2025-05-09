using DeliveryTrackingSystem.Data;
using DeliveryTrackingSystem.Models.Entities;
using DeliveryTrackingSystem.Repositories.GenericRepository;
using DeliveryTrackingSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DeliveryTrackingSystem.Repositories.Implements
{
    public class UserRepository(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext dbContext) : GenericRepository<User>(context), IUserRepository
    {

        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly AppDbContext _dbContext = dbContext;

        public Task<User> GetByEmailAsync(string email)
        {
            return _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
