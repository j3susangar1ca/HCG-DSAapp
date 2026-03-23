using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DSAapp.Core.Models;
using DSAapp.Core.Services;
using Microsoft.EntityFrameworkCore;
using System.IO;
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
                UsuarioAsignado = UsuarioAsignado,
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
}