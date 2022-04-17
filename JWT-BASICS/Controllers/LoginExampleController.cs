using JWT_BASICS.Interfaces;
using JWT_BASICS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace JWT_BASICS.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginExampleController : ControllerBase
    {
        private readonly IJWT ijwt_;

        private List<User> UsersList = new List<User>()
        {
            new User(){UserId = "1" ,UserName="Usuario 1", Email="usuario1@gmail.com", Password="1"},
            new User(){UserId = "2" ,UserName="Usuario 2", Email="usuario2@gmail.com", Password="2"}
        };        

        public LoginExampleController(IJWT ijwt_)
        {
            this.ijwt_ = ijwt_;
        }


        [AllowAnonymous]
        [HttpGet("loginwithoutclaims")]
        public ResponseAuthentication loginwithoutclaims(string userId, string password)
        {
            ResponseAuthentication response ;
            try
            {
                if (this.AuthenticateUser(userId, password))
                {                    
                    response = new ResponseAuthentication()
                    {
                        Token = ijwt_.GenerateToken(),
                        HttpStatusCode = ((int)HttpStatusCode.OK),
                        HttpStatusTitle = HttpStatusCode.OK.ToString()
                    }; 
                    Console.WriteLine("paso");
                }
                else
                {
                    response = new ResponseAuthentication()
                    {
                        Token = "",
                        HttpStatusCode = ((int)HttpStatusCode.Unauthorized),
                        HttpStatusTitle = HttpStatusCode.Unauthorized.ToString()
                    };
                    Console.WriteLine("no paso");
                }
            }
            catch (Exception ex)
            {                
                response = new ResponseAuthentication()
                {
                    Token = "",
                    HttpStatusCode = ((int)HttpStatusCode.BadRequest),
                    HttpStatusTitle = HttpStatusCode.BadRequest.ToString()
                };
            }  
            return response;
        }

        [AllowAnonymous]
        [HttpPost("loginwithclaims")]
        public ActionResult<string> loginwithclaims(string userId, string password, Dictionary<string, object> claims)
        {

            if (this.AuthenticateUser(userId, password))
            {
                claims.Add("Admin",true);                    
                return Ok(ijwt_.GenerateToken(claims));
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpGet("authexample")]
        public ActionResult authexample()
        {
            return Ok("Metodo publico de ejemplo");
        }
        
        [Authorize(Policy = "Admin")]
        [HttpGet("authexampleAdmin")]
        public ActionResult authexampleAdmin()
        {
            return Ok("Metodo publico solo para administradores!");
        }

        private bool AuthenticateUser(string userId, string password)
        {
            var usr = this.UsersList.FirstOrDefault(usr => usr.UserId == userId && usr.Password == password);
            if(usr != null)
                return true;
            return false;
        }


    }
}
