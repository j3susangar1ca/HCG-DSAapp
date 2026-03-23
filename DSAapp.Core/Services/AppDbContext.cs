using Microsoft.EntityFrameworkCore;
using DSAapp.Core.Models;
using System;
using System.IO;

namespace DSAapp.Core.Services;

public class AppDbContext : DbContext
{
    // Esta es tu tabla de tareas
    public DbSet<Tarea> Tareas => Set<Tarea>();

    public DbSet<Oficio> Oficios => Set<Oficio>();
    public DbSet<ComentarioOficio> ComentariosOficio => Set<ComentarioOficio>();
    // 👇 1. ¡ESTO ES LO QUE FALTABA! Agrega este constructor 👇
    public AppDbContext() { }  // ✅ Sin EnsureCreated — Migrate() en App.xaml.cs lo maneja

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // TU RUTA INSTITUCIONAL EXACTA
        string servidor = @"\\10.2.1.92\FAA_divserv_admvos\APLICACIONES";
        string carpeta = "GestionProyectos";
        string nombreBD = "DSA_Tareas.db";

        string dbPath = Path.Combine(servidor, carpeta, nombreBD);
        string rutaCarpeta = Path.GetDirectoryName(dbPath)!;

        // GARANTÍA DE RED
        try
        {
            if (!Directory.Exists(rutaCarpeta))
            {
                Directory.CreateDirectory(rutaCarpeta);
                System.Diagnostics.Debug.WriteLine($"[Red] Carpeta creada exitosamente en: {rutaCarpeta}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ALERTA SMB] Error al acceder a la red {servidor}: {ex.Message}");
        }

        // CONEXIÓN SQLITE
        optionsBuilder.UseSqlite($"Data Source={dbPath};Cache=Shared;Mode=ReadWriteCreate");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuramos el nombre de la tabla explícitamente
        modelBuilder.Entity<Tarea>().ToTable("TareasInstitucionales");

        base.OnModelCreating(modelBuilder);
    }
}