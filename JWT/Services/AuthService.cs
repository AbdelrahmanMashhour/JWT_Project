using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using testApi.Helpers;
using testApi.Models;

namespace testApi.Services
{
    public class AuthService : IAuthSevice
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwt;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;
        }



        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            //if i have same email in my database before
            if (await _userManager.FindByEmailAsync(model.Email)is not null)
            {
                //in this case IsAuthenticated is fals
                return new AuthModel { Message = "Email is eready exit!!!" };
            }
            //if i have same User Name in my database before
            else if (await _userManager.FindByNameAsync(model.UserName) is not null)
            {
                //in this case IsAuthenticated is fals

                return new AuthModel { Message = "Name is used !!" };
            }
            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            //hashing password
            var result= await _userManager.CreateAsync(user,model.Password);
            if (!result.Succeeded)
            {
                var error = string.Empty;
                foreach (var item in result.Errors)
                {
                    error += $"{item.Description}, ";
                }
                return new AuthModel { Message=error};
            }
            //Adding user to spacefic role
            await _userManager.AddToRoleAsync(user, "User");
            var jwtSecurityToken =await CreateJwtToken(user);
            return new AuthModel
            {
                Email = user.Email,
                ExpireOn = jwtSecurityToken.ValidTo,
                Roles = new List<string> { "User" },
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName,
            };
        }
        public async Task<AuthModel> GetTokenAsync(TokenRequsetModel model)
        {
            var authModel=new AuthModel();
            var user=await _userManager.FindByEmailAsync(model.Email);
            //if i don't have this email or password in my database 
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            authModel.ExpireOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();

            return authModel;

        }

        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            //return user if we have user with this id in my database else return null
            var user = await _userManager.FindByIdAsync(model.UserId);
            //return true if we have Role with this Name in my database
            var role = await _roleManager.RoleExistsAsync(model.RoleName);
            if (user is null||!role )
                return "Invalis User Id Or Role Name";

            if (await _userManager.IsInRoleAsync(user,model.RoleName))
            {
                return "User already assigned to this role";
            }
            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            if (result.Succeeded)//all thig is wright
            {
                return string.Empty;
            }
            return "Somethig went wrong...";

        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
