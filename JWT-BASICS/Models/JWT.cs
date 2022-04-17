using JWT_BASICS.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_BASICS.Models
{
    public class JWT : IJWT
    {
        private static JWT instance_ = new JWT();
        private JWT() { }

        public static JWT Instance { get { return instance_; } }

        private readonly IConfiguration configuration_;

        public JWT(IConfiguration configuration_)
        {
            this.configuration_ = configuration_;
        }

        public string GenerateToken()
        {
            // Generamos una llave simetrica de seguridad con base a los bytes del parametro de configuracion Jwt:Key            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration_["Jwt:key"]));            

            // Generamos las credenciales de la firma  utilizando la lleve simetrica y le aplicamos un algoritmo
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Las Claims (reclamaciones) son esos campos personalizados que estaran dentro de nuestro payload
            

            // Creamos el token de seguridad y lo retornamos
            var token = new JwtSecurityToken(
                claims: null,
                expires: DateTime.Now.AddSeconds(30),
                //issuer: _config["Jwt:Issuer"],
                //audience: _config["Jwt:Audience"],
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateToken(Dictionary<string,object>? claimList)
        {
            // Generamos una llave simetrica de seguridad con base a los bytes del parametro de configuracion Jwt:Key            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration_["Jwt:key"]));
            //_logger.LogInformation(message: securityKey.);

            // Generamos las credenciales de la firma  utilizando la lleve simetrica y le aplicamos un algoritmo
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Las Claims (reclamaciones) son esos campos personalizados que estaran dentro de nuestro payload
            var claimsToken = new List<Claim>();
            if (claimList != null)
            {
                Claim claim;
                foreach(var c in claimList)
                {
                    claim = new Claim(c.Key, c.Value.ToString());
                    claimsToken.Add(claim);
                }
            }

            // Creamos el token de seguridad y lo retornamos
            var token = new JwtSecurityToken();
            if (claimsToken.Count > 0)
            {
                token = new JwtSecurityToken(
                claims: claimsToken,
                expires: DateTime.Now.AddSeconds(30),
                //issuer: _config["Jwt:Issuer"], 
                //audience: _config["Jwt:Audience"],
                signingCredentials: credentials
                );
            }
            else
            {
                token = new JwtSecurityToken(
                claims: null,
                expires: DateTime.Now.AddSeconds(30),
                //issuer: _config["Jwt:Issuer"],
                //audience: _config["Jwt:Audience"],
                signingCredentials: credentials
                );
            }

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
