using System;

namespace DSAapp.Core.Models;

public class ComentarioOficio
{
    public int Id { get; set; }
    public int OficioId { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; } = DateTime.Now;

    public virtual Oficio? Oficio { get; set; }
}
