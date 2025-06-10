using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RageModeAPI.Data;
using RageModeAPI.Models; // Adicione este using para sua model Usuarios
//using Microsoft.AspNetCore.Authentication.JwtBearer; // Adicione este using
using Microsoft.IdentityModel.Tokens; // Adicione este using
using System.Text; // Adicione este using

var builder = WebApplication.CreateBuilder(args);

// Configurar a conexão com o banco de dados
builder.Services.AddDbContext<RageModeApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar a CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", corsBuilder =>
    {
        corsBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Adicione os serviços de Controllers
builder.Services.AddControllers();

// Configuração do ASP.NET Core Identity com sua model Usuarios e IdentityRole
builder.Services.AddIdentityApiEndpoints<Usuarios>(options =>
{
    // Configure seus requisitos de senha aqui
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false; // Ajuste conforme sua política
    options.Password.RequireDigit = false;          // Ajuste conforme sua política
    options.Password.RequireUppercase = false;      // Ajuste conforme sua política
    options.Password.RequireLowercase = false;      // Ajuste conforme sua política
    options.Password.RequiredLength = 4;            // Ajuste conforme sua política
    options.User.RequireUniqueEmail = true; // Garante que e-mails sejam únicos
})
    .AddRoles<IdentityRole>()
.AddEntityFrameworkStores<RageModeApiContext>() // Conecta Identity ao seu DbContext
.AddDefaultTokenProviders(); // Necessário para gerar tokens de redefinição de senha, etc.

// *** SERVIÇO PARA GERAR JWT (Adicione esta interface e classe no seu projeto!) ***
// Exemplo: Crie uma pasta 'Services' e dentro dela, um arquivo 'ITokenService.cs' e 'TokenService.cs'
//builder.Services.AddScoped<ITokenService, TokenService>(); // Registra seu serviço de token

//// Adicione serviço de Autenticação JWT Bearer
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true, // Validar o emissor do token
//        ValidateAudience = true, // Validar o público do token
//        ValidateLifetime = true, // Validar o tempo de vida do token (expiração)
//        ValidateIssuerSigningKey = true, // Validar a chave de assinatura

//        ValidIssuer = builder.Configuration["Jwt:Issuer"], // Obter do appsettings.json
//        ValidAudience = builder.Configuration["Jwt:Audience"], // Obter do appsettings.json
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Obter do appsettings.json
//    };
//});

// Adiciona o serviço de Autorização (DEVE VIR APÓS AddAuthentication)
builder.Services.AddAuthorization();

// Configuração do Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RageModeAPI", Version = "v1" });

    // Configuração para JWT Bearer no Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Insira o token JWT no formato 'Bearer {token}'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configuração do middleware
if (app.Environment.IsDevelopment()) // Geralmente você quer o Swagger apenas em desenvolvimento
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RageModeAPI v1");
    });
}

app.UseHttpsRedirection();

// Use CORS antes de UseAuthentication e UseAuthorization, se for uma API Pública.
app.UseCors("AllowAll");

// Middleware de autenticação (DEVE VIR ANTES DE UseAuthorization e MapControllers)
app.UseAuthentication();

// Middleware de autorização (DEVE VIR APÓS UseAuthentication)
app.UseAuthorization();

app.MapGroup("/Identity").MapIdentityApi<Usuarios>();

// Mapie os controllers, incluindo o AuthController
app.MapControllers();

// *** REMOVA ISTO: app.MapGroup("/Users").MapIdentityApi<IdentityUser>(); ***
// Isso cria os endpoints mínimos do Identity com IdentityUser, e você quer usar seu AuthController.

app.Run();