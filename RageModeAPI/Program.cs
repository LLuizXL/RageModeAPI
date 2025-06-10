using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RageModeAPI.Data;
using RageModeAPI.Data.Authorization;

var builder = WebApplication.CreateBuilder(args);
// Configurar a conexão com o banco de dados
builder.Services.AddDbContext<RageModeApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Configurar a CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    }
    );
});



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//Add Swagger com JWT Bearer
// Adionar o Swagger com JWT Bearer
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
    {
        new OpenApiSecurityScheme
        {
        Reference = new OpenApiReference
            {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header,

        },
        new List<string>()
        }
    });
});


// Serviço de EndPoints do Identity Framework
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 4;
})
    .AddEntityFrameworkStores<RageModeApiContext>()
    .AddDefaultTokenProviders(); // Adiocionando o provedor de tokens padrão

// Add serviço de Autenticação e Autorização.
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync("admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("admin"));
    }
}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOrOwner", policy =>
        policy.Requirements.Add(new AdminOrOwnerRequirement()));
});

// Registre o handler no DI
builder.Services.AddScoped<IAuthorizationHandler, AdminOrOwnerHandler>();

//Swagger em ambiente de produção
app.UseSwagger();
app.UseSwaggerUI();
//Mapear os EndPoints padrão do Identity Framework
app.MapGroup("/Users").MapIdentityApi<IdentityUser>();

app.UseHttpsRedirection();

//Permitir a autenticação e autorização de qualquer origem
app.UseAuthentication();

app.UseAuthorization();
app.UseCors("AllowAll");

app.MapControllers();
app.UseStaticFiles();

app.Run();