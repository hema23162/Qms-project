using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using QMS.DTOs;
using QMS.Data;
using QMS.Models;

namespace QMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly QmsDbContext _context;
        private readonly IConfiguration _config;

        public UserController(QmsDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

       
        // REGISTER USER
        
        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                return BadRequest("Password and Confirm Password do not match");

            var existingUser = _context.Users
                .FirstOrDefault(u => u.Email == dto.Email);

            if (existingUser != null)
                return BadRequest("User already exists");

            var user = new User
            {
                Email = dto.Email.Trim(),
                Password = dto.Password.Trim(), // plain text (OK for project)
                Role = dto.Role.Trim()
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new
            {
                user.UserId,
                user.Email,
                user.Role
            });
        }

        // ===============================
        //  LOGIN → GENERATE JWT TOKEN
        // ===============================
        [HttpPost("login")]
        public IActionResult Login(Logindto dto)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Email.ToLower() == dto.Email.Trim().ToLower() &&
                u.Password == dto.Password.Trim() &&
                u.Role.ToLower() == dto.Role.Trim().ToLower());

            if (user == null)
                return Unauthorized("Invalid email, password, or role");

            var token = GenerateToken(user);

            return Ok(new
            {
                token,
                role = user.Role
            });
        }

        // ===============================
        // GET ALL USERS (OPTIONAL)
        // Only Analyst can view
        // ===============================
        // [Authorize(Roles = "Analyst")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users
                .Select(u => new
                {
                    u.UserId,
                    u.Email,
                    u.Role
                })
                .ToList();

            return Ok(users);
        }

        // ===============================
        // ✅ JWT TOKEN HELPER METHOD
        // ===============================
        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(_config["Jwt:DurationInMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}