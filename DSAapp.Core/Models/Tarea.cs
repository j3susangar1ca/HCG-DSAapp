#nullable enable
using System;
using System.ComponentModel.DataAnnotations; // Necesario para las etiquetas [Key] y [Required]

namespace DSAapp.Core.Models;

/// <summary>
/// Representa la entidad principal de nuestra base de datos en la red institucional.
/// </summary>
public class Tarea
{
    // [Key] le dice a SQLite que este es el ID único y se autoincrementará solo.
    [Key]
    public int Id
    {
        get; set;
    }

    // 'required' es de C# 13 (.NET 9). Obliga a que toda tarea tenga un título al crearse.
    [Required(ErrorMessage = "El título es obligatorio para el seguimiento institucional.")]
    public required string Titulo
    {
        get; set;
    }

    public string? Descripcion
    {
        get; set;
    }

    // Aquí guardaremos la ruta UNC (ej: \\SERVIDOR\Carpeta\Documento.pdf)
    // El '?' significa que puede estar vacío si la tarea no tiene archivo adjunto.
    public string? RutaArchivoAdjunto
    {
        get; set;
    }

    // 'init' significa que la fecha solo se pone al crear la tarea y no se puede cambiar después.
    public DateTime FechaCreacion { get; init; } = DateTime.Now;

    public bool EstaCompletada { get; set; } = false;

    // Identifica automáticamente quién está usando la app en ese momento.
    public string CreadoPor { get; set; } = Environment.UserName;
}