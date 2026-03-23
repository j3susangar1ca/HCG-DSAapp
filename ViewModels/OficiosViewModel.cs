using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DSAapp.Core.Models;
using DSAapp.Core.Services;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Principal;
using DSAapp.Contracts.Services;
using System.Data;

namespace DSAapp.ViewModels;

public partial class OficiosViewModel : ObservableObject
{
    private readonly AppDbContext _db;
    private readonly ILocalSettingsService _localSettingsService;
    private readonly IAppNotificationService _notificationService;

    // ── Campos del formulario ──
    [ObservableProperty] private string _remitente = string.Empty;
    [ObservableProperty] private string _asunto = string.Empty;
    [ObservableProperty] private string _usuarioAsignado = string.Empty;
    [ObservableProperty] private string _folioOriginal = string.Empty;

    // ── Archivo adjunto ──
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneArchivoSeleccionado))]
    [NotifyPropertyChangedFor(nameof(NombreArchivoSeleccionado))]
    private string _rutaArchivoLocal = string.Empty;

    // ── Feedback: último folio generado ──
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneUltimoFolio))]
    private string _ultimoFolioGenerado = string.Empty;

    // ── Propiedades calculadas para los bindings de la vista ──
    public bool TieneArchivoSeleccionado => !string.IsNullOrEmpty(RutaArchivoLocal);
    public string NombreArchivoSeleccionado => Path.GetFileName(RutaArchivoLocal);
    public bool TieneUltimoFolio => !string.IsNullOrEmpty(UltimoFolioGenerado);

    // ── Propiedades para el manejo del Bloqueo en la Interfaz ──
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PuedeEditar))]
    private bool _esSoloLectura;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneBloqueoActivo))]
    private string _mensajeBloqueo = string.Empty;
    
    public bool TieneBloqueoActivo => !string.IsNullOrEmpty(MensajeBloqueo);
    public bool PuedeEditar => !EsSoloLectura;

    public OficiosViewModel(AppDbContext db, ILocalSettingsService localSettingsService, IAppNotificationService notificationService)
    {
        _db = db;
        _localSettingsService = localSettingsService;
        _notificationService = notificationService;
    }

    [RelayCommand]
    public async Task GuardarOficioAsync()
    {
        if (string.IsNullOrWhiteSpace(Remitente) ||
            string.IsNullOrWhiteSpace(Asunto) ||
            string.IsNullOrWhiteSpace(UsuarioAsignado))
        {
            System.Diagnostics.Debug.WriteLine("Faltan campos obligatorios.");
            return;
        }

        try
        {
            // 1. Validar directorio en segundo plano (Fuera de la transacción)
            var rutaServidorBase = await _localSettingsService.ReadSettingAsync<string>("RutaServidorRed") 
                ?? @"\\10.2.1.92\FAA_divserv_admvos\APLICACIONES\GestionProyectos\OficiosPDF";

            await Task.Run(() =>
            {
                if (!Directory.Exists(rutaServidorBase))
                    Directory.CreateDirectory(rutaServidorBase);
            });

            // 2. Transacción ultra-rápida (Solo Base de Datos)
            using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var añoActual = DateTime.Now.Year.ToString();
            var cantidadActual = await _db.Oficios
                                          .Where(o => o.FolioInterno.Contains(añoActual))
                                          .CountAsync();
            var nuevoFolio = $"OF-{añoActual}-{(cantidadActual + 1):D4}";

            var nombreArchivo = $"{nuevoFolio}.pdf";
            var rutaDestinoRed = Path.Combine(rutaServidorBase, nombreArchivo);

            var nuevoOficio = new Oficio
            {
                FolioInterno = nuevoFolio,
                FolioOriginal = FolioOriginal,
                Remitente = Remitente,
                Asunto = Asunto,
                UsuarioAsignado = ObtenerUsuarioActual(),
                RutaArchivoRed = string.IsNullOrEmpty(RutaArchivoLocal) ? "Sin archivo adjunto" : rutaDestinoRed
            };

            _db.Oficios.Add(nuevoOficio);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            // 3. Copiar archivo (Una vez que la DB ya está liberada)
            if (!string.IsNullOrEmpty(RutaArchivoLocal) && File.Exists(RutaArchivoLocal))
            {
                await Task.Run(() => File.Copy(RutaArchivoLocal, rutaDestinoRed, overwrite: true));
            }

            // 4. Feedback visual + limpiar formulario
            UltimoFolioGenerado = nuevoFolio;
            LimpiarFormulario();

            System.Diagnostics.Debug.WriteLine($"Oficio {nuevoFolio} guardado correctamente.");
            _notificationService.Show($@"<toast><visual><binding template=""ToastGeneric""><text>Éxito</text><text>Oficio {nuevoFolio} guardado correctamente.</text></binding></visual></toast>");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al guardar el oficio: {ex.Message}");
            var safeMsg = ex.Message.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
            _notificationService.Show($@"<toast><visual><binding template=""ToastGeneric""><text>Error al guardar</text><text>{safeMsg}</text></binding></visual></toast>");
        }
    }

    private void LimpiarFormulario()
    {
        Remitente = string.Empty;
        Asunto = string.Empty;
        UsuarioAsignado = string.Empty;
        FolioOriginal = string.Empty;
        RutaArchivoLocal = string.Empty;
    }

    // Método que se llama cuando el usuario selecciona un oficio para abrirlo
    public async Task AbrirOficioParaEdicionAsync(int oficioId)
    {
        var usuarioActual = ObtenerUsuarioActual();
        
        var oficio = await _db.Oficios.FindAsync(oficioId);
        if (oficio == null) return;

        // 1. Comprobar si está bloqueado por otro usuario
        var tiempoExpiracion = TimeSpan.FromMinutes(30);
        bool estaBloqueado = oficio.BloqueadoDesde.HasValue && 
                            (DateTime.Now - oficio.BloqueadoDesde.Value) < tiempoExpiracion && 
                            oficio.BloqueadoPor != usuarioActual;

        if (estaBloqueado)
        {
            // Modo Solo Lectura: Otro usuario lo tiene abierto
            EsSoloLectura = true;
            MensajeBloqueo = $"Este expediente está siendo editado por {oficio.BloqueadoPor} desde las {oficio.BloqueadoDesde:HH:mm}.";
            
            // Cargar datos a los TextBox pero no permitir guardar
            CargarDatosAlFormulario(oficio);
        }
        else
        {
            // Modo Edición: Está libre, procedemos a "adueñarnos" del bloqueo
            EsSoloLectura = false;
            MensajeBloqueo = string.Empty;

            oficio.BloqueadoPor = usuarioActual;
            oficio.BloqueadoDesde = DateTime.Now;
            await _db.SaveChangesAsync();

            CargarDatosAlFormulario(oficio);
        }
    }

    // Método a llamar cuando el usuario guarda los cambios o cierra la ventana
    public async Task LiberarBloqueoAsync(int oficioId)
    {
        var oficio = await _db.Oficios.FindAsync(oficioId);
        if (oficio != null && oficio.BloqueadoPor == ObtenerUsuarioActual())
        {
            oficio.BloqueadoPor = null;
            oficio.BloqueadoDesde = null;
            await _db.SaveChangesAsync();
        }
    }

    private void CargarDatosAlFormulario(Oficio oficio)
    {
        Remitente = oficio.Remitente;
        Asunto = oficio.Asunto;
        UsuarioAsignado = oficio.UsuarioAsignado;
        FolioOriginal = oficio.FolioOriginal ?? string.Empty;
        // La ruta de archivo no la cargamos de la red para edición, a menos que sea requerido
    }

    /// <summary>
    /// Obtiene el usuario de Active Directory/Windows conectado a la PC.
    /// Formato típico: "HOSPITAL\j3susangar1ca"
    /// </summary>
    private string ObtenerUsuarioActual()
    {
        var identity = WindowsIdentity.GetCurrent();
        return identity.Name;
    }

    /// <summary>
    /// Genera un documento Word de respuesta a partir de una plantilla maestra,
    /// inyectando automáticamente los datos del oficio actual (Folio, Remitente, Fecha).
    /// </summary>
    [RelayCommand]
    public async Task GenerarRespuestaWordAsync()
    {
        if (string.IsNullOrWhiteSpace(UltimoFolioGenerado) && string.IsNullOrWhiteSpace(FolioOriginal))
        {
            _notificationService.Show(@"<toast><visual><binding template=""ToastGeneric""><text>Sin folio</text><text>Primero guarda un oficio o carga uno existente para generar la respuesta.</text></binding></visual></toast>");
            return;
        }

        try
        {
            await Task.Run(() =>
            {
                // 1. Ruta de la plantilla maestra guardada en el servidor
                string rutaPlantilla = @"\\10.2.1.92\SIA\Plantillas\Plantilla_Respuesta.docx";

                // 2. Ruta donde se guardará el nuevo documento
                var folioParaArchivo = !string.IsNullOrWhiteSpace(UltimoFolioGenerado) 
                    ? UltimoFolioGenerado 
                    : FolioOriginal;
                string rutaNuevoDoc = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    $"Respuesta_{folioParaArchivo}.docx"
                );

                // Copiar la plantilla para no alterar la original
                File.Copy(rutaPlantilla, rutaNuevoDoc, true);

                // 3. Abrir y reemplazar placeholders con DocumentFormat.OpenXml
                using (var document = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(rutaNuevoDoc, true))
                {
                    var body = document.MainDocumentPart?.Document?.Body;
                    if (body != null)
                    {
                        var textoCompleto = body.InnerText;
                        
                        foreach (var paragraph in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
                        {
                            foreach (var run in paragraph.Descendants<DocumentFormat.OpenXml.Wordprocessing.Run>())
                            {
                                foreach (var text in run.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
                                {
                                    if (text.Text.Contains("<<FOLIO_ORIGINAL>>"))
                                        text.Text = text.Text.Replace("<<FOLIO_ORIGINAL>>", folioParaArchivo);
                                    if (text.Text.Contains("<<REMITENTE>>"))
                                        text.Text = text.Text.Replace("<<REMITENTE>>", Remitente);
                                    if (text.Text.Contains("<<FECHA_ACTUAL>>"))
                                        text.Text = text.Text.Replace("<<FECHA_ACTUAL>>", DateTime.Now.ToString("dd/MM/yyyy"));
                                    if (text.Text.Contains("<<ASUNTO>>"))
                                        text.Text = text.Text.Replace("<<ASUNTO>>", Asunto);
                                    if (text.Text.Contains("<<USUARIO_ASIGNADO>>"))
                                        text.Text = text.Text.Replace("<<USUARIO_ASIGNADO>>", UsuarioAsignado);
                                }
                            }
                        }

                        document.MainDocumentPart!.Document.Save();
                    }
                }

                // 4. Abrir el documento generado automáticamente para el usuario
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = rutaNuevoDoc,
                    UseShellExecute = true
                });
            });

            _notificationService.Show(@"<toast><visual><binding template=""ToastGeneric""><text>Documento generado</text><text>Se ha creado la respuesta Word en Mis Documentos.</text></binding></visual></toast>");
        }
        catch (FileNotFoundException)
        {
            _notificationService.Show(@"<toast><visual><binding template=""ToastGeneric""><text>Plantilla no encontrada</text><text>No se pudo acceder a la plantilla en el servidor. Verifique la conexión de red.</text></binding></visual></toast>");
        }
        catch (Exception ex)
        {
            var safeMsg = ex.Message.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
            _notificationService.Show($@"<toast><visual><binding template=""ToastGeneric""><text>Error al generar documento</text><text>{safeMsg}</text></binding></visual></toast>");
        }
    }
}