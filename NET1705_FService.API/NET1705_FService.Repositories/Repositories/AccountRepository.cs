﻿using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NET1705_FService.Repositories.Data;
using NET1705_FService.Repositories.Interface;
using NET1705_FService.Repositories.Models;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text;

namespace FServiceAPI.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly FserviceApiDatabaseContext _context;
        private readonly UserManager<Accounts> accountManager;
        private readonly SignInManager<Accounts> signInManager;
        private readonly IConfiguration configuration;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountRepository(
            UserManager<Accounts> accountManager, 
            SignInManager<Accounts> signInManager, 
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            IUserRepository userRepository)
        {
            this.accountManager = accountManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.roleManager = roleManager;
        }

        public async Task<ResponseModel> ConfirmEmail(string token, string email)
        {
            var user = await accountManager.FindByNameAsync(email);
            if (user != null) 
            {
                var result = await accountManager.ConfirmEmailAsync(user, token);
                if(result.Succeeded)
                {
                    return new ResponseModel
                    {
                        Status = "Success",
                        Message = "Email Verification Successfully!"
                    };
                }
                return new ResponseModel
                {
                    Status = "Error",
                    Message = "Email Verification Failed, please try again!"
                };
            }
            return new ResponseModel
            {
                Status = "Error",
                Message = "User does not existed!"
            };
        }

        public async Task<Accounts> GetAccountByUserName(string userName)
        {
            var account = await accountManager.FindByNameAsync(userName);
            return account;
        }

        public async Task<List<Accounts>> GetAllStaffsAsync()
        {
            bool roleExists = await roleManager.RoleExistsAsync("staff");

            if (!roleExists)
            {
                return null;
            }

            var staffAccounts = await accountManager.GetUsersInRoleAsync("staff");

            return staffAccounts.ToList();
        }

        public async Task<AuthenticationResponseModel> SignInAsync(SignInModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded) 
            {
                var account = await accountManager.FindByNameAsync(model.Email);
                var roles = await accountManager.GetRolesAsync(account);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var role in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));

                var token = new JwtSecurityToken(
                    issuer: configuration["JWT:ValidIssuer"],
                    audience: configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMinutes(30),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha512Signature)
                    );

                return new AuthenticationResponseModel 
                { 
                    Status = true,
                    Message = "Login successfully!",
                    JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                    Expired = token.ValidTo
                };
            } else if (result.IsNotAllowed)
            {
                return new AuthenticationResponseModel
                {
                    Status = false,
                    Message = "Please confirm your email before login!",
                    JwtToken = null,
                    Expired = null
                };
            } else
            {
                return new AuthenticationResponseModel
                {
                    Status = false,
                    Message = "Login failed! Incorrect username or password!",
                    JwtToken = null,
                    Expired = null
                };
            }
        }

        //public async Task<ResponseModel> SignUpAdminAsync(SignUpModel model)
        //{
        //    var userExist = await accountManager.FindByNameAsync(model.Email);
        //    if (userExist == null)
        //    {
        //        var user = new Accounts
        //        {
        //            Name = model.Name,
        //            PhoneNumber = model.PhoneNumber,
        //            Address = model.Address,
        //            DateOfBirth = model.DateOfBirth,
        //            Email = model.Email,
        //            UserName = model.Email,
        //            Status = true
        //        };

        //        var result = await accountManager.CreateAsync(user, model.Password);

        //        if (result.Succeeded)
        //        {
        //            if (!await roleManager.RoleExistsAsync(RoleModel.ADMIN.ToString()))
        //            {
        //                await roleManager.CreateAsync(new IdentityRole(RoleModel.ADMIN.ToString()));
        //            }
        //            if (await roleManager.RoleExistsAsync(RoleModel.ADMIN.ToString()))
        //            {
        //                await accountManager.AddToRoleAsync(user, RoleModel.ADMIN.ToString());
        //            }
        //            //Tạo TK Admin tự động xác thực email
        //            var token = accountManager.GenerateEmailConfirmationTokenAsync(user);
        //            var confirm = await accountManager.ConfirmEmailAsync(user, token.Result);
        //            if(confirm.Succeeded)
        //            {
        //                return new ResponseModel { Status = "Success", Message = "Register Admin Successfully!" };
        //            }
        //            return new ResponseModel { Status = "Error", Message = "Cannot authentication Admin Account!" };
        //        }
        //        return new ResponseModel { Status = "Error", Message = "Cannot create account in database!" };
        //    }
        //    return new ResponseModel { Status = "Error", Message = "Username is already exist" };
        //}

        public async Task<ResponseModel> SignUpAsync(SignUpModel model)
        {
            var userExist = await accountManager.FindByNameAsync(model.Email);
            if (userExist == null)
            {
                var user = new Accounts
                {
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    DateOfBirth = model.DateOfBirth,
                    Email = model.Email,
                    UserName = model.Email,
                    Status = true
                };

                var result = await accountManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!await roleManager.RoleExistsAsync(RoleModel.USER.ToString()))
                    {
                        await roleManager.CreateAsync(new IdentityRole(RoleModel.USER.ToString()));
                    }
                    if (await roleManager.RoleExistsAsync(RoleModel.USER.ToString()))
                    {
                        await accountManager.AddToRoleAsync(user, RoleModel.USER.ToString());
                    }
                    var token = accountManager.GenerateEmailConfirmationTokenAsync(user);
                    return new ResponseModel { Status = "Success", Message = "Register Successfully! Please check your email to confirm your account!", ConfirmEmailToken = token };
                }
                return new ResponseModel { Status = "Error", Message = "Cannot create account in database!" };
            }
            return new ResponseModel { Status = "Error", Message = "Username is already exist" };

        }

        public async Task<ResponseModel> SignUpInternalAsync(SignUpModel model, RoleModel role)
        {
            var userExist = await accountManager.FindByNameAsync(model.Email);
            if (userExist == null)
            {
                var user = new Accounts
                {
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    DateOfBirth = model.DateOfBirth,
                    Email = model.Email,
                    UserName = model.Email,
                    Status = true
                };


                if (role.Equals(RoleModel.ADMIN) || role.Equals(RoleModel.STAFF) || role.Equals(RoleModel.USER))
                {
                    await accountManager.CreateAsync(user, model.Password);

                    if (!await roleManager.RoleExistsAsync(role.ToString()))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role.ToString()));
                    }
                    if (await roleManager.RoleExistsAsync(role.ToString()))
                    {
                        await accountManager.AddToRoleAsync(user, role.ToString());
                    }
                    //Tạo TK Admin tự động xác thực email
                    var token = accountManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirm = await accountManager.ConfirmEmailAsync(user, token.Result);
                    if (confirm.Succeeded)
                    {
                        return new ResponseModel { Status = "Success", Message = $"Register {role} Successfully!" };
                    }
                    return new ResponseModel { Status = "Error", Message = $"Cannot authentication {role} Account!" };
                }
                return new ResponseModel { Status = "Error", Message = $"Cannot create account in database! Role {role} not support or you have missing a typo!" };
            }
            return new ResponseModel { Status = "Error", Message = "Username is already exist" };
        }

        //public async Task<ResponseModel> SignUpStaffAsync(SignUpModel model)
        //{
        //    var userExist = await accountManager.FindByNameAsync(model.Email);
        //    if (userExist != null)
        //    {
        //        return new ResponseModel { Status = "Error", Message = "Username is already exist!" };
        //    }
        //    var user = new Accounts
        //    {
        //        Name = model.Name,
        //        PhoneNumber = model.PhoneNumber,
        //        Address = model.Address,
        //        DateOfBirth = model.DateOfBirth,
        //        Email = model.Email,
        //        UserName = model.Email,
        //        Status = true
        //    };
        //    var result = await accountManager.CreateAsync(user, model.Password);
        //    if (result.Succeeded)
        //    {
        //        if (!await roleManager.RoleExistsAsync(RoleModel.STAFF.ToString()))
        //        {
        //            await roleManager.CreateAsync(new IdentityRole(RoleModel.STAFF.ToString()));
        //        }
        //        if (await roleManager.RoleExistsAsync(RoleModel.STAFF.ToString()))
        //        {
        //            await accountManager.AddToRoleAsync(user, RoleModel.STAFF.ToString());
        //        }
        //    }

        //    return new ResponseModel { Status = "Sucess", Message = "Register Staff successfully!" };
        //}
    }
}
