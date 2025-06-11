using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RageModeAPI.Data;
using RageModeAPI.Models; // Adicione este using para sua model Usuarios
//using Microsoft.AspNetCore.Authentication.JwtBearer; // Adicione este using
using Microsoft.IdentityModel.Tokens; // Adicione este using
using System.Text;
using RageModeAPI.Data.Authorization; // Adicione este using

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
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Add services to the container.

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

// Serviço de EndPoints do Identity Framework


builder.Services.AddIdentityApiEndpoints<Usuarios>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 4;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<RageModeApiContext>()
    .AddDefaultTokenProviders(); // Adiocionando o provedor de tokens padrão

// Add serviço de Autenticação e Autorização.
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOrOwner", policy =>
        policy.Requirements.Add(new AdminOrOwnerRequirement()));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync("admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("admin"));
    }
}



//Swagger em ambiente de produção



//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();


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
app.UseStaticFiles();

// *** REMOVA ISTO: app.MapGroup("/Users").MapIdentityApi<IdentityUser>(); ***
// Isso cria os endpoints mínimos do Identity com IdentityUser, e você quer usar seu AuthController.



app.Run();