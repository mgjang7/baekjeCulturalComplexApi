using BaekjeCulturalComplexApi.Data;
using BaekjeCulturalComplexApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BaekjeCulturalComplexApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly BaekjeDbContext _dbCon;
        private readonly IConfiguration _config;

        public AccountController(BaekjeDbContext context, IConfiguration config)
        {
            _dbCon = context;
            _config = config;
        }

        /// <summary>
        /// 로그인
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [EnableCors("CorsPolicyName")]
        public IActionResult Login([FromBody] Manager login)
        {
            SHA256Managed sha256Managed = new SHA256Managed();
            byte[] encryptBytes = sha256Managed.ComputeHash(Encoding.UTF8.GetBytes(login.Password));
            //base64
            String encryptPassword = Convert.ToBase64String(encryptBytes);

            IActionResult response = Unauthorized();

            Hashtable ht = AuthenticateUser(login, encryptPassword);
            //Hashtable ht = AuthenticateUser(login, login.password);

            string errorType = ht["ErrorType"].ToString();
            Manager user = ht["User"] as Manager;

            if (string.IsNullOrWhiteSpace(errorType))
            {
                var tokenString = GenerateJWTToken(user);
                response = Ok(new
                {
                    msg = "success",
                    token = tokenString,
                    loginUserId = user.Id,
                    // loginUserName = user.name
                });
            }
            else
            {
                if (errorType == "NoUsers")
                {
                    response = Ok(new
                    {
                        msg = "NoUsers"
                    });
                }
                else if (errorType == "PasswordWrong")
                {
                    response = Ok(new
                    {
                        msg = "PasswordWrong"
                    });
                }
            }

            return response;
        }

        /// <summary>
        /// token 인증요청, 성공시 UserId 반환
        /// </summary>
        /// <returns></returns>
        [HttpGet("reqpermission")]
        [EnableCors("CorsPolicyName")]
        public JsonResult GetUserId()        
        {

            var loginUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {

                //User rUSer = _dbCon.Users.Where(p => p.UserId == loginUserId).FirstOrDefault();
                Manager rUSer = _dbCon.Managers.Where(p => p.Id == loginUserId).FirstOrDefault();

                //if (dt.Rows.Count > 0)
                if (rUSer != null)
                {
                    Manager user = new Manager();

                    user.Id = rUSer.Id;
                    // user.UserName = rUSer.UserName;

                    return new JsonResult(user);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private Hashtable AuthenticateUser(Manager loginCredentials, string encryptPassword)
        {
            Hashtable ht = new Hashtable();

            try
            {
                var list = _dbCon.Managers.ToList();

                //ID 체크
                int existUserIdCnt = _dbCon.Managers.Where(p => p.Id == loginCredentials.Id).Count();

                if (existUserIdCnt > 0) //ID 존재하면 true
                {
                    //Password 체크
                    int existUserCnt = _dbCon.Managers.Where(p => p.Id == loginCredentials.Id && p.Password == encryptPassword).Count();

                    if (existUserCnt > 0)  //Password 가 일치하면
                    {
                        Manager user = _dbCon.Managers.Where(p => p.Id == loginCredentials.Id && p.Password == encryptPassword).FirstOrDefault();

                        ht.Add("ErrorType", "");
                        ht.Add("User", user);
                        return ht;
                    }
                    else
                    {
                        ht.Add("ErrorType", "PasswordWrong");
                        ht.Add("User", null);
                        return ht;
                    }
                }
                else
                {
                    ht.Add("ErrorType", "NoUsers");
                    ht.Add("User", null);
                    return ht;
                }
            }
            catch (Exception ex)
            {
                ht.Add("ErrorType", ex.Message);
                ht.Add("User", null);
                return ht;
            }
        }

        private string GenerateJWTToken(Manager userInfo)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userInfo.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}