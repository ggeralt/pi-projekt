using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using SimpleSocialNetworkAPI.Model;
using SimpleSocialNetworkAPI.Services.Mail;
using SimpleSocialNetworkAPI.Services.User;
using SimpleSocialNetworkShared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimpleSocialNetworkAPI.Services.User
{
    public class UserService : IUserService
    {
        private UserManager<ApplicationUser> _userManager;
        private IConfiguration _configuration;
        private IMailService _mailService;
        public UserService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IMailService mailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
        }

        public async Task<ResponseManager> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ResponseManager
                {
                    Message = "User not found",
                    IsSuccess = false
                };
            }
            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
            {
                return new ResponseManager
                {
                    Message = "Email confirmed successfully",
                    IsSuccess = true
                };
            }

            return new ResponseManager
            {
                Message = "Email did not confir",
                IsSuccess = true,
                Errors = result.Errors.Select(error => error.Description)
            }; 
        }

        public async Task<ResponseManager> ForgetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ResponseManager
                {
                    IsSuccess = false,
                    Message = "User with that address was not found"
                };
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"http://localhost:5012/ResetPassword?email={email}&token={validToken}";
            await _mailService.SendEmail(email, "Reset Password", $"<p>To reset your password <a href='{url}'>CLICK HERE</a></p>");

            return new ResponseManager
            {
                IsSuccess = true,
                Message = "Reset password link has been sent to you succesfully!"
            };
        }

        public async Task<ResponseManager> LoginUser(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                return new ResponseManager
                {
                    IsSuccess = false,
                    Message = "Invalid username or password",
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (result == false) 
            {
                return new ResponseManager
                {
                    Message = "Invalid username or password",
                    IsSuccess = false,
                };
            }

            var claims = new[]
            {
                new Claim("Email", loginDTO.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

            var token = new JwtSecurityToken
            (
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            string stringToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new ResponseManager
            {
                Message = stringToken,
                IsSuccess = true
            };
        }

        public async Task<ResponseManager> RegisterUser(RegisterDTO registerDTO)
        {
            if (registerDTO == null)
            {
                throw new NullReferenceException("Register model is null");
            }
            if (registerDTO.Password != registerDTO.ConfirmPassword)
            {
                return new ResponseManager
                {
                    Message = "Confirm password doesn't match the password",
                    IsSuccess = false
                };
            }
            var identityUser = new ApplicationUser
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Username,
            };
            try
            {
                var result = await _userManager.CreateAsync(identityUser, registerDTO.Password);
                if (result.Succeeded)
                {
                    var confirmPasswordToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                    var encodedPasswordToken = Encoding.UTF8.GetBytes(confirmPasswordToken);
                    var validPasswordToken = WebEncoders.Base64UrlEncode(encodedPasswordToken);

                    //string url = $"{_configuration["BaseApiUrl"]}/api/auth/confirmemail?userid={identityUser.Id}&token={validEmailToken}";
                    string url = $"http://localhost:5012/api/Auth/ConfirmEmail?userId={identityUser.Id}&token={validPasswordToken}";

                    await _mailService.SendEmail(identityUser.Email, "Confirm your email", $"<h1>Welcome to SimpleSocialNetwork</h1><p>Please confirm your email by <a href='{url}'>CLICKING HERE</a></p>");

                    return new ResponseManager
                    {
                        Message = "User created successfully",
                        IsSuccess = true
                    };
                }
                return new ResponseManager
                {
                    Message = "User can not be created",
                    IsSuccess = false,
                    Errors = result.Errors.Select(error => error.Description)
                };
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<ResponseManager> ResetPassword([FromForm]ResetPassword model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new ResponseManager
                {
                    Message = "User with that address was not found",
                    IsSuccess = false
                };
            }

            var decodedPasswordToken = WebEncoders.Base64UrlDecode(model.Token);
            var passwordToken = Encoding.UTF8.GetString(decodedPasswordToken);
            var result = await _userManager.ResetPasswordAsync(user, passwordToken, model.NewPassword);

            if (result.Succeeded)
            {
                return new ResponseManager
                {
                    Message = "Password has been reset succesfully!",
                    IsSuccess = true
                };
            }
            return new ResponseManager
            {
                Message = "Something went wrong!",
                IsSuccess = false,
                Errors = result.Errors.Select(error => error.Description)
            };
        }
    }
}
