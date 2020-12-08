using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private IConfiguration _config;
        static List<Customer> Customers;
        static CustomerController()
        {
            Customers = new List<Customer>()
            {
                new Customer(){Customer_Id=1,Customer_Name="Supriya",Password="sup123"},
                new Customer(){Customer_Id=2,Customer_Name="Meghana",Password="meg123"},
            };
        }
        public CustomerController( IConfiguration config)
        {
            
            this._config = config;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] Customer login)
        {

            
            IActionResult response = Unauthorized();
            var user = Customers.FirstOrDefault(c => c.Customer_Name == login.Customer_Name && c.Password == login.Password);
            if (user == null)
            {
                
                return NotFound();
            }
            // User user1=_context.Users.FirstOrDefault(u=>u.Username==)

            else
            {

              
                var tokenString = GenerateJSONWebToken(login);
                response = Ok(new { token = tokenString });
                

                return response;
            }
        }

        private string GenerateJSONWebToken(Customer userInfo)
        {
           
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                null,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

}