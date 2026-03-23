using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SistemaOrdenesReparacion.BL;
using SistemaOrdenesReparacion.DA;
using SistemaOrdenesReparacion.SI;
using SistemaOrdenesReparacion.Model;

var builder = WebApplication.CreateBuilder(args);

var connectionUrl = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionUrl) && connectionUrl.StartsWith("postgres://"))
{
    var uri = new Uri(connectionUrl);
    var userInfo = uri.UserInfo.Split(':');
    connectionUrl = $"Host={uri.Host};Database={uri.LocalPath.Substring(1)};Username={userInfo[0]};Password={userInfo[1]};Port={(uri.Port > 0 ? uri.Port : 5432)};SSL Mode=Require;Trust Server Certificate=true;";
}
builder.Services.AddDbContext<TallerDbContext>(options => options.UseNpgsql(connectionUrl));


builder.Services.AddScoped<IGestorClientes, GestorClientes>();
builder.Services.AddScoped<IGestorMecanicos, GestorMecanicos>();
builder.Services.AddScoped<IGestorOrdenesDeTrabajo, GestorDeOrdenesDeTrabajo>();
builder.Services.AddScoped<IGestorInventarioDeServicios, GestorInventarioDeServicios>();
builder.Services.AddScoped<IGestorDeRepuestos, GestorDeRepuestos>();
builder.Services.AddScoped<IGestorDeOrdenesEnProceso, GestorDeOrdenesEnProceso>();
builder.Services.AddScoped<GestorEmail>();



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "ApiKey necesaria para acceder a los endpoints. Debe enviarse en el header 'X-Api-Key'.",
        In = ParameterLocation.Header,
        Name = "x-api-key",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TallerDbContext>();
    dbContext.Database.Migrate();

    if (!dbContext.Usuarios.Any())
    {
        var cliente = new Cliente { Identificacion = "11111111", Nombre = "Juan", Apellidos = "Perez", CorreoElectronico = "cliente@taller.com" };
        var mecanico = new Mecanico { Identificacion = 22222222, Nombre = "Carlos", Apellidos = "Mecanico", CorreoElectronico = "mecanico@taller.com" };
        
        dbContext.Clientes.Add(cliente);
        dbContext.Mecanicos.Add(mecanico);
        dbContext.SaveChanges();

        dbContext.Usuarios.AddRange(
            new Usuario { NombreUsuario = "admin", Clave = "admin123", CorreoElectronico = "admin@taller.com", Rol = 1, Activo = true, IntentosFallidos = 0 },
            new Usuario { NombreUsuario = "cliente", Clave = "cliente123", CorreoElectronico = "cliente@taller.com", Rol = 2, Activo = true, IntentosFallidos = 0, Id_Cliente = cliente.Id },
            new Usuario { NombreUsuario = "mecanico", Clave = "mecanico123", CorreoElectronico = "mecanico@taller.com", Rol = 3, Activo = true, IntentosFallidos = 0, Id_Mecanico = mecanico.Id }
        );
        dbContext.SaveChanges();
    }
}

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAll"); 

app.UseMiddleware<ApiKeyMiddleware>();


app.UseAuthorization();

app.MapControllers();

app.Run();



