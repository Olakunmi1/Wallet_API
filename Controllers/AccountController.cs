using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Wallet.Data.Entities;
using Wallet.Data.Interface;
using Wallet_API.ReadDTO;
using Wallet_API.WriteDTO;

namespace Wallet_API.Controllers
{
    
    [ApiController]
    [Route("api/Account")]
    public class AccountController : ControllerBase
    {
        private readonly ISystemuserRepo _systemuser;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(ISystemuserRepo systemuser, UserManager<ApplicationUser> userManager, IConfiguration config, SignInManager<ApplicationUser> signInManager)
        {
            _systemuser = systemuser;
            _userManager = userManager;
            _config = config;
            _signInManager = signInManager;
        }

        //Register with Identity 
        [AllowAnonymous]
        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterClient(SystemuserDTOW model)
        {
            try
            {
                StringBuilder strbld2 = new StringBuilder();
                var err2 = new List<string>();
                if (!ModelState.IsValid)
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            err2.Add(error.ErrorMessage);
                            err2.ForEach(err => { strbld2.AppendFormat("•{0}", error.ErrorMessage); });
                        }
                    }

                    return BadRequest(new { message = strbld2 });
                }
                //create Account
                var user = new ApplicationUser { UserName = model.username, Email = model.email, PhoneNumber = model.phonenumber };

                IdentityResult result = await _userManager.CreateAsync(user, model.password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors;
                    var message = string.Join(", ", errors.Select(x => x.Code + "," + " " + x.Description));
                    //ModelState.AddModelError("", message);

                    return BadRequest(new { message = message });
                }

                ////On Successful Registration -  store role as Client
                //var result1 = await _userManager.AddToRoleAsync(user, "Client");

                var NewUser = new SystemUser
                {
                    Username = model.username,
                    PhoneNumber = model.phonenumber,
                    Created_at = DateTime.Now   
                };
                try
                {
                    // create systemUser
                    var newUserr = _systemuser.Create(NewUser);
                    if(newUserr != null)
                    {
                       await _systemuser.SaveChanges();  //save changes
                        return Ok(new
                        {
                            success = true,
                            message = "Registration was Succesful, You will be required to log in with your Email & Password",
                            Username = newUserr.Username,
                            Email = user.Email

                        });
                    }
                    return Ok(new
                    {
                        success = false,
                        message = "Registration was not Succesful, Pls try again"
                        
                    });

                }
                catch (Exception ex)
                {
                    // return error message if there was an exception
                    return BadRequest(new { message = ex.Message });
                }

            }

            catch (Exception ex)
            {
                var messg = ex.Message;
                return BadRequest();
            }
        }

        //Login with Identity and grab a token with JWt
        [AllowAnonymous]
        [HttpPost("Authenticate/token")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel model)
        {
            try
            {

                StringBuilder strbld2 = new StringBuilder();
                var err2 = new List<string>();
                if (!ModelState.IsValid)
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            err2.Add(error.ErrorMessage);
                            err2.ForEach(err => { strbld2.AppendFormat("•{0}", error.ErrorMessage); });
                        }
                    }

                    return BadRequest(new { message = strbld2 });
                }

                //var user = _userService.Authenticate_2(model.Username, model.Password);

                var user = await _userManager.FindByEmailAsync(model.email);
                if (user != null && (await _userManager.CheckPasswordAsync(user, model.Password)))
                {
                    //var rolename = await _userManager.GetRolesAsync(user);  //--get users Role

                    //var userRole = rolename.FirstOrDefault(); //

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_config["AppSettings:Secret"]);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                    new Claim(JwtRegisteredClaimNames.Sub, model.email),
                   // new Claim(ClaimTypes.Role, userRole),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        }),
                        Expires = DateTime.UtcNow.AddMinutes(15),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),

                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);

                    var tokenString = tokenHandler.WriteToken(token);
                    var expiresWhen = tokenDescriptor.Expires;

                    //to validate token 
                    //var validate = tokenHandler.ValidateToken(tokenString);

                    // return basic user info and authentication token
                    return Ok(new
                    {
                        //Id = user.Id,
                        Username = user.UserName,
                        Email = user.Email,
                        //Role = userRole,
                        Token = tokenString,
                        expires = expiresWhen
                    });

                }
                return BadRequest(new { message = "Email or password is incorrect" });
            }

            catch (Exception ex)
            {
                return Ok(new
                {
                    message = ex.Message
                });
            }
        }

    }
}