using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using DSAapp.Contracts.ViewModels;
using DSAapp.Core.Models;
using DSAapp.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml; // Necesario para DispatcherTimer

namespace DSAapp.ViewModels;

public partial class CuadrículaDeDatosViewModel : ObservableRecipient, INavigationAware
{
    private readonly AppDbContext _db;
    private DispatcherTimer? _pollingTimer;

    public ObservableCollection<Oficio> Source { get; } = new ObservableCollection<Oficio>();

    [ObservableProperty]
    private bool _isLoading;

    public CuadrículaDeDatosViewModel(AppDbContext db)
    {
        _db = db;
    }

    public async void OnNavigatedTo(object parameter)
    {
        // 1. Carga inicial (con animación de carga)
        await CargarDatosAsync(mostrarCarga: true);

        // 2. Configurar y arrancar el Polling (cada 30 segundos)
        _pollingTimer = new DispatcherTimer();
        _pollingTimer.Interval = TimeSpan.FromSeconds(30);
        _pollingTimer.Tick += async (s, e) => await CargarDatosAsync(mostrarCarga: false);
        _pollingTimer.Start();
    }

    public void OnNavigatedFrom()
    {
        // MUY IMPORTANTE: Detener el timer al salir de la página para no consumir memoria
        _pollingTimer?.Stop();
        _pollingTimer = null;
    }

    private async Task CargarDatosAsync(bool mostrarCarga)
    {
        if (mostrarCarga) IsLoading = true;

        try
        {
            // Usamos AsNoTracking() porque es una consulta de solo lectura, mejora mucho el rendimiento
            var data = await _db.Oficios.AsNoTracking().ToListAsync();

            // Sincronizar la colección preservando los elementos (evita parpadeos en la UI)
            Source.Clear();
            foreach (var item in data)
            {
                Source.Add(item);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en polling de oficios: {ex.Message}");
        }
        finally
        {
            if (mostrarCarga) IsLoading = false;
        }
    }
}
