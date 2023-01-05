using LoginPage.Data;
using LoginPage.Helper;
using LoginPage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace LoginPage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly LoginDBContext _authloginDbContext;
        public LoginController(LoginDBContext  loginDBContext)
        {
            _authloginDbContext = loginDBContext;   
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Login loginObj)
         
        {
            if (loginObj == null) { return BadRequest(); }

            var login = await _authloginDbContext.LoginPage.FirstOrDefaultAsync(x => x.Username == loginObj.Username);

            if (login == null) 
            { 
                return NotFound(new
                { Message = "User Not Found!" }); 
            }

            if (!PasswordHasher.VerifyPassword(loginObj.Password, login.Password))
            {
                return BadRequest(new
                { Message = "Password is Incorrect" });
            }

            login.Token = CreateJwt(login); 
         
            return Ok(new
            { 
                Token = login.Token,
                Message = "Login Successfull!" });
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> UserSignUp([FromBody] Login signupObj)
        {
            if(signupObj == null) 
            {
                return BadRequest();
            }
            //validation check for username
            if(await CheckUserNameExistAsync(signupObj.Username))
            {
                return BadRequest(new { Message = "Username already Exist" });
            }

            //validation check for email
            if(await CheckEmailExistAsync(signupObj.Email))
            {
                return BadRequest(new { Message = "Email already Exist" });
            }

            //validation check for password
            var pass = CheckPasswordStrength(signupObj.Password);
            if (!string.IsNullOrEmpty(pass))
                {
                return BadRequest(new
                {
                    Message = pass.ToString()
                });
            }

            signupObj.Password = PasswordHasher.HashPassword(signupObj.Password);
            signupObj.Role = "User";
            signupObj.Token = "";
            
            await _authloginDbContext.LoginPage.AddAsync(signupObj);
            await _authloginDbContext.SaveChangesAsync();
            return Ok(new
            {
                Message = "Sign Up Successfull!"
            });
        }

        private async Task<bool> CheckUserNameExistAsync(string username)
        {
            return await _authloginDbContext.LoginPage.AnyAsync(x => x.Username == username);           
        }

        private async Task<bool> CheckEmailExistAsync(string email)
        {
            return await _authloginDbContext.LoginPage.AnyAsync(x => x.Email == email);
        }

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sbcheckPasswordStrength = new StringBuilder();
            if (password.Length < 8)
                sbcheckPasswordStrength.Append("Minimum Password length should be 8" + Environment.NewLine);      

            if ((Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                sbcheckPasswordStrength.Append("Password Should be Alphanumeric" + Environment.NewLine);            

            if (!Regex.IsMatch(password, "[<,>,@]"))
                sbcheckPasswordStrength.Append("Password Should Contain Special Characters" + Environment.NewLine);

            return sbcheckPasswordStrength.ToString();
        }


        private string CreateJwt(Login user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, $"{user.FullName}"),
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }

        [Authorize]
        [HttpGet("GetUser")]
        public async Task<ActionResult<Login>> GetAllUsers()
        {
            return Ok(await _authloginDbContext.LoginPage.ToListAsync());
        }

    }
}
