using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Server_side.Data;
using MyModel.Models.Entitties;
using MyModel.Models.DTOs;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server_side.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public UserRepository(AppDbContext dbContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        public async Task<ApiResponse<string>> CreateAccount(RegisterDTO registerDTO)
        {
            if (registerDTO is null)
                return ApiResponse<string>.ErrorResponse("Request is empty.", 400);

            var newUser = new AppUser()
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                PhoneNumber = registerDTO.PhoneNumber,
                Email = registerDTO.Email,
                PasswordHash = registerDTO.Password,
                UserName = registerDTO.Email
            };

            var user = await _userManager.FindByEmailAsync(newUser.Email);

            if (user is not null)
                return ApiResponse<string>.ErrorResponse("User with this email account is already registered.", 400);

            if (registerDTO.Role != "Admin" && registerDTO.Role != "Manager" && registerDTO.Role != "Worker")
                return ApiResponse<string>.ErrorResponse("Invalid role.", 400);


            List<IdentityError>? errors = null;
            var isValid = true;
            foreach (var v in _userManager.PasswordValidators)
            {
                var result = await v.ValidateAsync(_userManager, newUser, registerDTO.Password);
                if (!result.Succeeded)
                {
                    if (result.Errors.Any())
                    {
                        errors ??= new List<IdentityError>();
                        errors.AddRange(result.Errors);
                    }

                    isValid = false;
                }
            }

            if (!isValid)
                return ApiResponse<string>.ErrorResponse("Invalid password.", 400, errors?.Select(e => e.Description).ToList());

            var createUser = await _userManager.CreateAsync(newUser, registerDTO.Password);

            if (!createUser.Succeeded)
                return ApiResponse<string>.ErrorResponse("Error occurred during user creation.", 500);

            if (await _roleManager.FindByNameAsync("Admin") is null)
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
            }

            if (await _roleManager.FindByNameAsync("Manager") is null)
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = "Manager" });
            }

            if (await _roleManager.FindByNameAsync("Worker") is null)
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = "Worker" });
            }

            await _userManager.AddToRoleAsync(newUser, registerDTO.Role);

            return ApiResponse<string>.SuccessResponse("Success.");
        }

        public async Task<ApiResponse<LoginResponseDTO>> LoginAccount(LoginDTO loginDTO)
        {
            if (loginDTO is null)
                return ApiResponse<LoginResponseDTO>.ErrorResponse("Request is empty.", 400);

            var getUser = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (getUser is null)
                return ApiResponse<LoginResponseDTO>.ErrorResponse("User with this email is not found.", 400);

            bool checkUserPasswords = await _userManager.CheckPasswordAsync(getUser, loginDTO.Password);

            if (!checkUserPasswords)
                return ApiResponse<LoginResponseDTO>.ErrorResponse("Invalid email or password.", 400);

            var getUserRole = await _userManager.GetRolesAsync(getUser);
            var userSession = new UserSession
            {
                Id = getUser.Id,
                FirstName = getUser.FirstName,
                LastName = getUser.LastName,
                Email = getUser.Email,
                PhoneNumber = getUser.PhoneNumber,
                Role = getUserRole.First(),
            };
            string token = GenerateToken(userSession);

            var data = new LoginResponseDTO { 
                Token = token, 
            };

            return ApiResponse<LoginResponseDTO>.SuccessResponse(data);
        }

        private string GenerateToken(UserSession user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserSession?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (user is null)
                return null;

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            return new UserSession {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Role = role,
            };
        }

        public async Task<IReadOnlyList<UserSession>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            var usersFromMan = await _userManager.Users.ToListAsync(cancellationToken);
            var users = new List<UserSession>();
            foreach (var user in usersFromMan)
            {
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                users.Add(new UserSession {
                    Id =  user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Role =role,
                });
            }
            return users;
        }

        public async Task<IReadOnlyList<UserSession>> ListAsync(Expression<Func<AppUser, bool>> filter, CancellationToken cancellationToken = default)
        {
            var usersFromMan = await _userManager.Users.Where(filter).ToListAsync(cancellationToken);
            var users = new List<UserSession>();
            foreach (var user in usersFromMan)
            {
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                users.Add(new UserSession {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Role = role,
                });
            }
            return users;
        }

        public async Task<ApiResponse<string>> UpdateAsync(string id, RegisterDTO user, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userManager.FindByIdAsync(id);

            if (existingUser is null)
                return ApiResponse<string>.ErrorResponse("There is no user with such id.", 404);

            if (await _userManager.FindByEmailAsync(user.Email) is not null && existingUser.Email != user.Email)
            {
                return ApiResponse<string>.ErrorResponse("There is user with such email.", 400);
            }

            if (user.Role != "Admin" && user.Role != "Manager" && user.Role != "Worker")
                return ApiResponse<string>.ErrorResponse("Invalid role.", 400);

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Email = user.Email;
            existingUser.UserName = user.Email;

            if (!user.Password.IsNullOrEmpty())
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                var result = await _userManager.ResetPasswordAsync(existingUser, token, user.Password);

                if (!result.Succeeded)
                    return ApiResponse<string>.ErrorResponse("Error occurred during password update.", 500);
            }

            var updateResult = await _userManager.UpdateAsync(existingUser);

            if (!updateResult.Succeeded)
                return ApiResponse<string>.ErrorResponse("Error occurred during user update.", 500);

            await _dbContext.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Success.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return ApiResponse<string>.ErrorResponse("There is no user with such id.", 404);

            var res = await _userManager.DeleteAsync(user);

            if (!res.Succeeded)
                return ApiResponse<string>.ErrorResponse("Error occurred during user deletion.", 500);

            return ApiResponse<string>.SuccessResponse("Success");
        }

        public async Task<UserSession?> FirstOrDefaultAsync(Expression<Func<AppUser, bool>> filter, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(filter, cancellationToken);

            if (user is null)
                return null;

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            return new UserSession {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Role = role,
            };
        }
    }
}
