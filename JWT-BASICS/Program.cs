using JWT_BASICS.Interfaces;
using JWT_BASICS.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services

builder.Services.AddTransient<IJWT, JWT>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(
    opciones => 
    {
        opciones.AddDefaultPolicy(
            policy => 
            {
                policy.AllowAnyOrigin();
            }
        ); 
    }
);

builder.Services.AddHttpContextAccessor()
                .AddAuthorization(
                    opciones => 
                    {
                        opciones.AddPolicy("Admin", politica => politica.RequireClaim("Admin","true"));
                    }
                )
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

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    Global.appConfiguration = app.Configuration;
}
else
{
    Global.appConfiguration = null;
}
app.Run();
