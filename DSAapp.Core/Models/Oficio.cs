#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSAapp.Core.Models;

/// <summary>
/// Representa el registro de un oficio físico o digital que ingresa a la institución, 
/// incluyendo su archivo PDF escaneado y su seguimiento.
/// </summary>
public class Oficio
{
    // ID único autoincrementable para SQLite
    [Key]
    public int Id
    {
        get; set;
    }

    // Folio estandarizado que generará la app (Ej: OF-2026-0001)
    [Required(ErrorMessage = "El folio interno es obligatorio para el control institucional.")]
    public required string FolioInterno
    {
        get; set;
    }

    // El número que trae el documento impreso (puede venir sin número, por eso es opcional '?')
    public string? FolioOriginal
    {
        get; set;
    }

    [Required(ErrorMessage = "Debe registrar la dependencia o persona que remite el oficio.")]
    public required string Remitente
    {
        get; set;
    }

    [Required(ErrorMessage = "El asunto es obligatorio para identificar el propósito del oficio.")]
    public required string Asunto
    {
        get; set;
    }

    // Fecha en la que se recibió y registró en el sistema (no se puede cambiar después)
    public DateTime FechaRecepcion { get; init; } = DateTime.Now;

    // Fecha límite para darle respuesta (opcional)
    public DateTime? FechaLimite
    {
        get; set;
    }

    // A qué usuario o departamento del hospital se le turna para su atención
    [Required(ErrorMessage = "Es necesario asignar este oficio a un responsable.")]
    public required string UsuarioAsignado
    {
        get; set;
    }

    // Estados sugeridos: "Pendiente", "En Proceso", "Atendido", "Archivado"
    public string Estatus { get; set; } = "Pendiente";

    // Ruta UNC donde guardaremos el PDF en el servidor \\10.2.1.92\...
    public string? RutaArchivoRed
    {
        get; set;
    }

    // Registra automáticamente qué usuario de Windows capturó este oficio
    public string CapturadoPor { get; init; } = Environment.UserName;

    // Propiedades para control de concurrencia (bloqueo en edición)
    public string? BloqueadoPor { get; set; }
    public DateTime? BloqueadoDesde { get; set; }

    public DateTime FechaIngreso { get; set; } = DateTime.Now;

    // ── Módulo Archivístico (Físico) ──
    public string? UbicacionLefort { get; set; }
    public string? UbicacionCaja { get; set; }
    public string? UbicacionEstante { get; set; }

    // ── Clasificación CADIDO ──
    public string? CadidoFondo { get; set; }
    public string? CadidoSeccion { get; set; }
    public string? CadidoSerie { get; set; }

    // ── Relación para el Chat Contextual ──
    public virtual ICollection<ComentarioOficio> Comentarios { get; set; } = new List<ComentarioOficio>();

    // ── Propiedad Calculada para el Semáforo SLA (No se guarda en BD) ──
    [NotMapped]
    public string ColorSemaforo
    {
        get
        {
            var diasTranscurridos = (DateTime.Now - FechaIngreso).TotalDays;
            if (diasTranscurridos < 3) return "Green";       // 🟢 < 2 días
            if (diasTranscurridos <= 5) return "Goldenrod";  // 🟡 3 - 5 días
            return "Red";                                    // 🔴 > 5 días
        }
    }
}