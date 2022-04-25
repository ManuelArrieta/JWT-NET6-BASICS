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
            new User(){UserId = "1" ,UserName="Usuario 1", Email="usuario1@gmail.com", Password="1", rol = "Administrador"},
            new User(){UserId = "2" ,UserName="Usuario 2", Email="usuario2@gmail.com", Password="2", rol = "Operario"}
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
            User user = null;
            try
            {
                user = this.AuthenticateUser(userId, password);
                if (user != null)
                {   
                    Dictionary<string,object> administrative_claims = new Dictionary<string,object>();
                    administrative_claims.Add("Admin", "true");
                    //administrative_claims.Add(user.rol, true);
                    response = new ResponseAuthentication()
                    {
                        Token = ijwt_.GenerateToken(administrative_claims),
                        HttpStatusCode = ((int)HttpStatusCode.OK),
                        HttpStatusTitle = HttpStatusCode.OK.ToString()
                    };                     
                }
                else
                {
                    response = new ResponseAuthentication()
                    {
                        Token = "",
                        HttpStatusCode = ((int)HttpStatusCode.Unauthorized),
                        HttpStatusTitle = HttpStatusCode.Unauthorized.ToString()
                    };                    
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
        public ResponseAuthentication loginwithclaims(string userId, string password, Dictionary<string, object> claims)
        {
            ResponseAuthentication response;
            User user = null;
            try
            {
                user = this.AuthenticateUser(userId, password);
                if (user != null)
                {
                    //claims.Add(user.rol,true);
                    claims.Add("Admin", false);
                    response = new ResponseAuthentication()
                    {
                        Token = ijwt_.GenerateToken(claims),
                        HttpStatusCode = ((int)HttpStatusCode.OK),
                        HttpStatusTitle = HttpStatusCode.OK.ToString()
                    };
                }
                else
                {
                    response = new ResponseAuthentication()
                    {
                        Token = "",
                        HttpStatusCode = ((int)HttpStatusCode.Unauthorized),
                        HttpStatusTitle = HttpStatusCode.Unauthorized.ToString()
                    };
                }
            }
            catch(Exception ex)
            {
                response = new ResponseAuthentication()
                {
                    Token="",
                    HttpStatusCode= ((int)HttpStatusCode.BadRequest),
                    HttpStatusTitle= HttpStatusCode.BadRequest.ToString()
                };
            }
            return response;
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

        private User AuthenticateUser(string userId, string password)
        {
            var usr = this.UsersList.FirstOrDefault(usr => usr.UserId == userId && usr.Password == password);
            if(usr != null)
                return usr;
            return null;
        }
    }
}
