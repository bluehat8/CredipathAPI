using CredipathAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CredipathAPI.Data
{
    public static class PermissionsInitializer
    {
        public static async Task InitializePermissionsAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            
            try
            {
                var context = services.GetRequiredService<DataContext>();
                var logger = services.GetRequiredService<ILogger<DataContext>>();
                
                // Asegurarse de que la base de datos está creada
                await context.Database.EnsureCreatedAsync();
                
                // Inicializar los permisos
                await InitializeAsync(context, logger);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<DataContext>>();
                logger.LogError(ex, "Ocurrió un error al inicializar los permisos.");
            }
        }
        
        private static async Task InitializeAsync(DataContext context, ILogger logger)
        {
            // Definir los permisos necesarios
            var requiredPermissions = new List<(string Module, string Action)>
            {
                // Permisos para préstamos
                ("Prestamos", "Agregar"),
                ("Prestamos", "Editar"),
                ("Prestamos", "Eliminar"),
                
                // Permisos para pagos
                ("Pagos", "Agregar"),
                ("Pagos", "Editar"),
                ("Pagos", "Eliminar"),
                
                // Permisos para módulos
                ("Colaboradores", "Ver"),
                ("PagosVencidos", "Ver"),
                ("PagosProximos", "Ver"),
                ("PagoPrestamos", "Ver"),
                ("Reportes", "Ver")
            };
            
            // Verificar y crear los permisos que faltan
            foreach (var (module, action) in requiredPermissions)
            {
                var permissionExists = await context.Permissions
                    .AnyAsync(p => p.Module == module && p.Action == action);
                    
                if (!permissionExists)
                {
                    logger.LogInformation($"Creando permiso: {module} - {action}");
                    
                    context.Permissions.Add(new Permission
                    {
                        Module = module,
                        Action = action,
                        Active = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }
            
            // Guardar cambios si hay permisos nuevos
            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync();
                logger.LogInformation("Se han creado los permisos faltantes en la base de datos.");
            }
            else
            {
                logger.LogInformation("Todos los permisos necesarios ya existen en la base de datos.");
            }
        }
    }
}
