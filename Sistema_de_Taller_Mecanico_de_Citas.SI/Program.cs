using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Sistema_de_Taller_Mecanico_de_Citas.BL;
using Sistema_de_Taller_Mecanico_de_Citas.DA;
using Sistema_de_Taller_Mecanico_de_Citas.SI;
using Sistema_de_Taller_Mecanico_de_Citas.Model;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TallerDbContext>(options =>
    options.UseSqlServer(connectionString));


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
    dbContext.Database.EnsureCreated();

    var adminUser = await dbContext.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == "admin");
    if (adminUser == null)
    {
        var cliente = await dbContext.Clientes.FirstOrDefaultAsync(c => c.Identificacion == "11111111");
        if (cliente == null)
        {
            cliente = new Cliente { Identificacion = "11111111", Nombre = "Juan", Apellidos = "Perez", CorreoElectronico = "cliente@taller.com" };
            dbContext.Clientes.Add(cliente);
            await dbContext.SaveChangesAsync();
        }

        var mecanico = await dbContext.Mecanicos.FirstOrDefaultAsync(m => m.Identificacion == 22222222);
        if (mecanico == null)
        {
            mecanico = new Mecanico { Identificacion = 22222222, Nombre = "Carlos", Apellidos = "Mecanico", CorreoElectronico = "mecanico@taller.com" };
            dbContext.Mecanicos.Add(mecanico);
            await dbContext.SaveChangesAsync();
        }
        
        dbContext.Usuarios.AddRange(
            new Usuario { NombreUsuario = "admin", Clave = "admin123", CorreoElectronico = "admin@taller.com", Rol = 1, Activo = true, IntentosFallidos = 0 },
            new Usuario { NombreUsuario = "cliente", Clave = "cliente123", CorreoElectronico = "cliente@taller.com", Rol = 2, Activo = true, IntentosFallidos = 0, Id_Cliente = cliente.Id },
            new Usuario { NombreUsuario = "mecanico", Clave = "mecanico123", CorreoElectronico = "mecanico@taller.com", Rol = 3, Activo = true, IntentosFallidos = 0, Id_Mecanico = mecanico.Id }
        );
        await dbContext.SaveChangesAsync();
    }
    else
    {
        // Forzar clave admin123 para asegurar acceso
        adminUser.Clave = "admin123";
        adminUser.Activo = true;
        adminUser.IntentosFallidos = 0;
        await dbContext.SaveChangesAsync();
    }

    if (!dbContext.InventarioDeRepuestos.Any())
    {
        dbContext.InventarioDeRepuestos.AddRange(
            new InventarioDeRepuesto { Nombre = "Aceite Sintético 5W30 (1L)", Descripcion = "Aceite de alto rendimiento para motores modernos", Precio = 7500 },
            new InventarioDeRepuesto { Nombre = "Filtro de Aceite", Descripcion = "Filtro de aceite compatible con múltiples marcas", Precio = 4500 },
            new InventarioDeRepuesto { Nombre = "Pastillas de Freno", Descripcion = "Juego de pastillas delanteras cerámicas", Precio = 25000 },
            new InventarioDeRepuesto { Nombre = "Batería 12V 550CCA", Descripcion = "Batería libre de mantenimiento", Precio = 65000 },
            new InventarioDeRepuesto { Nombre = "Líquido de Frenos Dot 4", Descripcion = "Frasco de 500ml", Precio = 5500 }
        );
        await dbContext.SaveChangesAsync();
    }

    if (!dbContext.InventarioDeServicios.Any())
    {
        dbContext.InventarioDeServicios.AddRange(
            new InventarioDeServicios { Nombre = "Cambio de Aceite y Filtro", Descripcion = "Incluye mano de obra y revisión de niveles", Precio = 15000 },
            new InventarioDeServicios { Nombre = "Alineación y Balanceo", Descripcion = "Ajuste de ángulos de dirección y plomeo", Precio = 20000 },
            new InventarioDeServicios { Nombre = "Lavado Premium", Descripcion = "Lavado exterior, aspirado y detallado de motor", Precio = 12000 },
            new InventarioDeServicios { Nombre = "Escaneo Computarizado", Descripcion = "Diagnóstico integral con scanner profesional", Precio = 15000 },
            new InventarioDeServicios { Nombre = "Revisión de Frenos", Descripcion = "Limpieza y ajuste de todo el sistema de frenado", Precio = 10000 }
        );
        await dbContext.SaveChangesAsync();
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



