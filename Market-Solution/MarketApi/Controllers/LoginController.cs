using MarketApi.Dtos.LogIn;
using MarketApi.Errors;
using MarketCore.Entities;
using MarketCore.Repositries;
using MarketRepositry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketApi.Controllers
{
    
    public class LoginController : BaseApiController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public LoginController(
            UserManager<AppUser> _userManager,
            SignInManager<AppUser> _signInManager
            
            ) 
        {
            userManager = _userManager;
            signInManager = _signInManager;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.LoginFiled }, HasError = true });
            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password,false);
            if (!result.Succeeded)
               return Unauthorized(new ApiValidationErrorResponse() { Errors = new[] { ErrorMessages.LoginFiled }, HasError = true });


            user.Token = Guid.NewGuid().ToString();
            await userManager.UpdateAsync(user);
            
            HttpContext.Response.Headers.Add("token", user.Token);
            return Ok(new UserDto()
            {
                UserName = user.UserName,
                Email = user.Email,
                
            });
        }




    }
}
