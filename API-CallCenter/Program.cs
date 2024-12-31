using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SoapService;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
// Add services for JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://dev-aykzsyocig5der37.us.auth0.com"; //  Auth0 domain
        options.Audience = "https://dev-aykzsyocig5der37.us.auth0.com/api/v2/"; // API identifier
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://dev-aykzsyocig5der37.us.auth0.com", // Auth0 domain
            ValidateAudience = true,
            ValidAudience = "https://dev-aykzsyocig5der37.us.auth0.com/api/v2/", // API audience la no auth0
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType = "https://example.com/roles" // Configurar o claim de roles
        };
    });
builder.Services.AddSwaggerGen(options =>
{
    // Configurar o esquema de segurança JWT
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {token}"
    });

    // Associar o esquema de segurança a todas as operações protegidas
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {} // Scopes permitidos
        }
    });
});
// Add CORS for Swagger
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwaggerUI", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddAuthorization(); //adicionado
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
/*
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
*/

app.UseCors("AllowSwaggerUI");
app.UseHttpsRedirection();

app.UseAuthentication();  //adicionado 

app.UseAuthorization();

app.MapControllers();

app.Run();
