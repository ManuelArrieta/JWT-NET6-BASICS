# JWT-NET6-BASICS (GENERIC JWT)
En este repositorio encontrara un paso a paso la implementacion **Basica** de JWT(Json Web Token) para .NET6 + Swagger

Esta API fue diseñada con un proposito didactico simlple.
___
### Tabla de contenido
* [Dependencias del proyecto](#Dependencias)
* [Definiciones de JWT](#Definiciones)
* [Definiciones de Servicios en .NET para el uso de JWT](#Servicios)
* [Secuencia de implementacion](#Imlpementacion)
* [Planteamiento de ejemplo](#ejemplo)

<a name="Dependencias"></a>
### Dependencias del proyecto
Verificar que el gestor de packetes NuGet se encuentre instalados los siguientes paquetes:
* `Microsoft.AspNetCore.Authentication.JwtBearer`


___

<a name="Definiciones"></a>
### Definiciones de JWT
|Definicion|Significado|Objetivo|
|---------|--------|----|
|**JWT**|Json Web Token||
|**Autorizacion**||Proteger los recursos del sistema permitiendo que sólo sean usados por aquellos consumidores a los que se les ha concedido autorización para ello|
|**Bearer**|Es un formato que nos permite la autorización en conjunto con la autenticación de usuarios. En la cabecera de la peticion http en el esquema de Authorization ira **Bearer <Token>**|Seleccionar el esquema de Autorizacion en la cabecera http|
|**Issuer**|Hace referencia al emisor del token. ejemplo, localhost, 178.68.25.4, prueba.com.co||
|**Subject**|Hace referencia al asunto, que coincide con el identificador la persona que se identifica.||
|**Audience**|La audiencia para la que se ha emitido. ejemplo, localhost, 0.0.0.0 .||
|**Expires**|Fecha de expiracion del token.|Con el fin de brindar seguridad al consumo de los recursos se hablita el **ValidateLifetime** con esto validaremos la fecha de caducidad de los tokens generados por el sistema.|
|**Claims**|Los **Claim** o reclamacion en ingles es una clase con un comportamiento similar a un diccionario el cual tiene la estructura **Claim<Tkey,Tvalue>**.||
|**ClockSkew**|||
|**signingCredentials**|Credenciales firmadas|Se utiliza para generar una firma a traves de un algoritmo que brindara autenticidad al token y issuer que emite el token. |
|**AllowAnonymous**||Se utiliza para conseder acceso a los recursos sin nesecidad de estar autenticado ni autorizado |
|**Authorize**||Se utiliza para restringir acceso a los recursos y para poder acceder a ellos se debe de estar autenticado y autorizado con un token valido. |

___
<a name="Servicios"></a>
### Definiciones de Servicios en .NET para el uso de JWT
|Definicion|Significado|Objetivo|
|---------|--------|----|
|**AddHttpContextAccessor**|Se inyecta el servicio de HttpContextAccessor|HttpContextAccessor nos permite acceder a **HttpContext**, clase cuyo proposito es mantener o retener distinta informacion como Request, Response, Server, Session, Item, Cache, informacion del usuario como la autenticacion y autorizacion|
|**AddAuthorization**|Se inyecta el servicio de autorizacion.|Se activan las politicas de autorizaciones de recursos a los cuales sean decorados con [Authorize]|
|**AddAuthentication**|Se inyecta el servicio de autenticacion con el cual se especificara en tipo de esquema a utilizar.|Seleccionar el esquema de Autorizacion en la cabecera HTTP que en este caso sera **Bearer**|
|**AddJwtBearer**|Se inyecta el servicio del esquema de autenticacion.|Configurar las distintas validaciones que se tendran en cuenta cuando un token haga parte |
|**AddSwaggerGen**|Se inyecta el servicio swagger |Se añade el middelware para de sawagger y se configura  las definiciones de seguridad a traves de **AddSecurityDefinition** y  **AddSecurityRequirement** con el fin de activar los esquemas de autorizacion|
|**SwaggerDoc**||Define uno o varios documentos swagger|
|**AddSecurityDefinition**||Define como la API es protegida para generarla en swagger|
|**AddSecurityRequirement**||Añade globalmente los requerimiento de seguridad a swagger|

___

<a name="Imlpementacion"></a>
### Secuencia de implementacion
Para realizar de forma acertiva la configuracion de los servicios de JWT y Swagger seguiremos la siguiente secuencia de implementacion:
1. Configuracion de JWT y Swagger en `Program.cs`.
2. Creacion y aplicacion de buenas practicas con una interfaz IJWT.
3. Creacion e implementacion de la clase JWT extendida de la interfaz IJWT.
4. Planteamiento de ejemplo.
___

### 1. Configuracion de JWT y Swagger.
* `Program.cs`
```c#

builder.Services.AddHttpContextAccessor()
                .AddAuthorization()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer   = false,
                        ValidateAudience = false,                        
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew        = TimeSpan.Zero,
                        //ValidIssuer      = builder.Configuration["Jwt:Issuer"],
                        //ValidAudience    = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWT Simple Authentication",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
});
```
### 2. Creacion y aplicacion de buenas practicas con una interfaz IJWT.
* `IJWT.cs`
* Declararemos dos metodos **GenerateToken()** y **GenerateToken(Dictionary<string, object> claimList)**.
    * Para este caso de **GenerateToken()** se generara un token cuyo unico proposito sera la validacion de tiempo de vida de el mismo, sin ningun tipo de **claim** adicional. 
    * Para este caso de **GenerateToken(Dictionary<string, object> claimList)** se generara un token cuyo unico proposito sera la validacion de tiempo de vida de el mismo pero adicionalmente añadiremos una lista de **claims** dinamicamente a traves del parametro con diccionario. 
```c#
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace JWT_BASICS.Interfaces
{
    public interface IJWT
    {
        public string GenerateToken();
        public string GenerateToken(Dictionary<string, object> claimList);
    }
}

```
### 3. Creacion e implementacion de la clase JWT.
* `JWT.cs`
```c#
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

```

<a name="ejemplo"></a>
### Planteamiento de ejemplo.

Dento de este proyecto tendremos adicionalmente en la carpeta de **controllers** el controlador `LoginExampleController.cs` los cuales contendran **tres** metodos:
1. **private bool AuthenticateUser(string userId, string password)**
```c#
private bool AuthenticateUser(string userId, string password)
{
    var usr = this.UsersList.FirstOrDefault(usr => usr.UserId == userId && usr.Password == password);
    if(usr != null)
        return true;
    return false;
}
```
Este metodo es utilizado para autenticar a traves del id y password un usuario que ha sido creado previamente en una lista de usuarios.

2. **public ActionResult<string> loginwithoutclaims(string userId, string password)**
Metodo que me devuelve un **Token** siempre y cuando el usuario y la contraseña sean validas.
```c#
[AllowAnonymous]
[HttpGet("loginwithoutclaims")]
public ActionResult<string> loginwithoutclaims(string userId, string password)
{
    if(this.AuthenticateUser(userId, password))
    {
        return Ok(ijwt_.GenerateToken());
    }
    else
    {
        return BadRequest();
    }
}
```
3. **public ActionResult<string> loginwithclaims(string userId, string password, Dictionary<string, object> claims)**
Metodo que me devuelve un **Token** siempre y cuando el usuario y la contraseña sean validas. Adicional a la validacion del usuario podras generar un diccionario dinamico con los claims que se requieran.
```c#
[AllowAnonymous]
[HttpPost("loginwithclaims")]
public ActionResult<string> loginwithclaims(string userId, string password, Dictionary<string, object> claims)
{
    if (this.AuthenticateUser(userId, password))
    {                                
        return Ok(ijwt_.GenerateToken(claims));
    }
    else
    {
        return BadRequest();
    }
}
```
4. **public ActionResult authexample()**
Metodo de ejemplo para la verificacion por autorizacion y validacion del Token generado anteriormente.
```c#
[Authorize]
[HttpGet("authexample")]
public ActionResult authexample()
{
    return Ok("Metodo publico de ejemplo");
}
```
