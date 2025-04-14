using LoanApp_REST_APIs.Models;
using LoanApp_REST_APIs.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanApp_REST_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly LoanManagementDbContext _context;

        public UsersController(LoanManagementDbContext context)
        {
            _context = context;
        }

        // GET: api/users?role=Customer
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetUsers([FromQuery] string role)
        {
            if (role != "Customer" && role != "Officer")
                return BadRequest("Invalid role. Use 'Customer' or 'Officer'.");

            var users = await _context.Users
                .Join(_context.Roles,
                    u => u.RoleId,
                    r => r.RoleId,
                    (u, r) => new { User = u, Role = r })
                .Where(ur => ur.Role.RoleName == role)
                .Select(ur => new UserViewModel
                {
                    UserId = ur.User.UserId,
                    RoleName = ur.Role.RoleName,
                    Username = ur.User.Username,
                    Name = ur.User.Name,
                    Email = ur.User.Email,
                    MobileNumber = ur.User.MobileNumber,
                    IsActive = ur.User.IsActive
                })
                .ToListAsync();

            return Ok(users);
        }

        // PUT: api/users/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserViewModel>> UpdateUserStatus(int id, [FromBody] UserUpdateStatusViewModel model)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id && (u.Role.RoleName == "Customer" || u.Role.RoleName == "Officer"));

            if (user == null)
                return NotFound("User not found or invalid role.");

            user.IsActive = model.IsActive;
            await _context.SaveChangesAsync();

            var updatedUser = new UserViewModel
            {
                UserId = user.UserId,
                RoleName = user.Role.RoleName,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                IsActive = user.IsActive
            };

            return Ok(updatedUser);
        }

        // POST: api/users
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserViewModel>> CreateUser([FromBody] UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == model.RoleName && (r.RoleName == "Customer" || r.RoleName == "Officer"));

            if (role == null)
                return BadRequest("Invalid role. Use 'Customer' or 'Officer'.");

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Email);

            if (existingUser != null)
                return BadRequest("Username or email already exists.");

            var user = new User
            {
                RoleId = role.RoleId,
                Username = model.Username,
                Password = model.Password, // Stored in plain text as per requirement
                Name = model.Name,
                Email = model.Email,
                MobileNumber = model.MobileNumber,
                IsActive = false, // Starts inactive, Admin must approve
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = new UserViewModel
            {
                UserId = user.UserId,
                RoleName = role.RoleName,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                IsActive = user.IsActive
            };

            return CreatedAtAction(nameof(GetUsers), new { role = role.RoleName }, result);
        }
    }
}