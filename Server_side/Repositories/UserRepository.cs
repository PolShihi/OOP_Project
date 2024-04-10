using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Server_side.Models.DTOs;
using static Server_side.Models.DTOs.ServiceResponces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Server_side.Data;

namespace Server_side.Repositories
{
    public class UserRepository(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config) : IUserRepository
    {

        public async Task<GeneralResponse> CreateAccount(UserDTO userDTO)
        {
            if (userDTO is null) return new GeneralResponse(false, "Model is empty");
            var newUser = new AppUser()
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                PhoneNumber = userDTO.PhoneNumber,
                Email = userDTO.Email,
                PasswordHash = userDTO.Password,
                UserName = userDTO.Email
            };

            var user = await userManager.FindByEmailAsync(newUser.Email);
            if (user is not null) return new GeneralResponse(false, "User registered already");

            if (userDTO.Role != "Admin" && userDTO.Role != "Manager" && userDTO.Role != "Worker")
                return new GeneralResponse(false, "Invalid Role");

            var createUser = await userManager.CreateAsync(newUser!, userDTO.Password);
            if (!createUser.Succeeded) return new GeneralResponse(false, "Error occured.. please try again");

            if (await roleManager.FindByNameAsync("Admin") is null)
            {
                await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
            }

            if (await roleManager.FindByNameAsync("Manager") is null)
            {
                await roleManager.CreateAsync(new IdentityRole() { Name = "Manager" });
            }

            if (await roleManager.FindByNameAsync("Worker") is null)
            {
                await roleManager.CreateAsync(new IdentityRole() { Name = "Worker" });
            }

            await userManager.AddToRoleAsync(newUser, userDTO.Role);
            return new GeneralResponse(true, "Account Created");
        }

        public async Task<LoginResponse> LoginAccount(LoginDTO loginDTO)
        {
            if (loginDTO == null)
                return new LoginResponse(false, null!, "Login container is empty");

            var getUser = await userManager.FindByEmailAsync(loginDTO.Email);
            if (getUser is null)
                return new LoginResponse(false, null!, "User not found");

            bool checkUserPasswords = await userManager.CheckPasswordAsync(getUser, loginDTO.Password);
            if (!checkUserPasswords)
                return new LoginResponse(false, null!, "Invalid email/password");

            var getUserRole = await userManager.GetRolesAsync(getUser);
            var userSession = new UserSession(getUser.Id, getUser.FirstName, getUser.LastName, getUser.PhoneNumber, getUser.Email, getUserRole.First());
            string token = GenerateToken(userSession);
            return new LoginResponse(true, token!, "Login completed");
        }

        public async Task<List<UserSession>> GetUsers()
        {
            var usersFromMan = userManager.Users.ToList();
            var users = new List<UserSession>();
            foreach (var user in usersFromMan)
            {
                var role = (await userManager.GetRolesAsync(user)).FirstOrDefault();
                users.Add(new UserSession(
                    Id: user.Id,
                    FirstName: user.FirstName,
                    LastName: user.LastName,
                    PhoneNumber: user.PhoneNumber,
                    Email: user.Email,
                    Role: role
                    ));
            }
            return users;
        }

        public async Task<GeneralResponse> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user is null)
            {
                return new GeneralResponse(false, "There is no user with such Id");
            }

            var res = await userManager.DeleteAsync(user);

            if (!res.Succeeded)
            {
                return new GeneralResponse(false, "Error occured.. please try again");
            }

            return new GeneralResponse(true, "Deletion complete");
        }

        private string GenerateToken(UserSession user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
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
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
