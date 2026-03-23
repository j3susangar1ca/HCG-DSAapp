using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DSAapp.Contracts.ViewModels;
using DSAapp.Core.Models;
using DSAapp.Core.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace DSAapp.ViewModels;

public partial class CuadrículaDeDatosViewModel : ObservableRecipient, INavigationAware
{
    private readonly AppDbContext _db;
    private HubConnection? _hubConnection;

    public ObservableCollection<Oficio> Source { get; } = new ObservableCollection<Oficio>();

    [ObservableProperty]
    private bool _isLoading;

    // ── Módulo de Búsqueda ──
    [ObservableProperty]
    private string _textoBusqueda = string.Empty;

    public CuadrículaDeDatosViewModel(AppDbContext db)
    {
        _db = db;
    }

    public async void OnNavigatedTo(object parameter)
    {
        // 1. Carga inicial (con animación de carga)
        await CargarDatosAsync(mostrarCarga: true);

        // 2. Iniciar conexión SignalR para actualizaciones en tiempo real
        await IniciarSignalRAsync();
    }

    private async Task IniciarSignalRAsync()
    {
        // La URL de tu servidor backend que aloje el Hub de SignalR
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://10.2.1.92:5000/oficiosHub")
            .WithAutomaticReconnect() // Reconexión automática si el WiFi falla
            .Build();

        // Escuchar el evento "OficioActualizado" enviado por el servidor
        _hubConnection.On("OficioActualizado", () =>
        {
            // Ejecutar en el hilo de la UI para evitar excepciones de concurrencia
            App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
            {
                try
                {
                    await CargarDatosAsync(mostrarCarga: false);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error actualizando desde SignalR: {ex.Message}");
                }
            });
        });

        try
        {
            await _hubConnection.StartAsync();
            System.Diagnostics.Debug.WriteLine("Conectado a SignalR.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error SignalR: {ex.Message}");
        }
    }

    public void OnNavigatedFrom()
    {
        // Fire-and-forget controlado: capturamos la tarea para evitar excepciones no observadas
        _ = LiberarConexionAsync();
    }

    private async Task LiberarConexionAsync()
    {
        if (_hubConnection is null) return;
        try
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al cerrar SignalR: {ex.Message}");
        }
    }

    /// <summary>
    /// Comando de búsqueda: filtra por Folio, Asunto o Remitente usando IQueryable dinámico.
    /// Se ejecuta al presionar Enter en el AutoSuggestBox o al hacer clic en el ícono de búsqueda.
    /// </summary>
    [RelayCommand]
    public async Task BuscarOficiosAsync()
    {
        IsLoading = true;
        Source.Clear();

        try
        {
            // Usar IQueryable para construir la consulta dinámicamente en la BD
            var query = _db.Oficios.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                var termino = TextoBusqueda.ToLower();
                query = query.Where(o =>
                    o.FolioInterno.ToLower().Contains(termino) ||
                    o.Asunto.ToLower().Contains(termino) ||
                    o.Remitente.ToLower().Contains(termino));
            }

            // Ordenar por los más recientes primero
            query = query.OrderByDescending(o => o.FechaIngreso);

            // Limitar a los últimos 100 resultados para máximo rendimiento
            var resultados = await query.Take(100).ToListAsync();

            foreach (var item in resultados)
            {
                Source.Add(item);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en búsqueda: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CargarDatosAsync(bool mostrarCarga)
    {
        if (mostrarCarga) IsLoading = true;

        try
        {
            // Usamos AsNoTracking() porque es una consulta de solo lectura, mejora mucho el rendimiento
            var data = await _db.Oficios
                .AsNoTracking()
                .OrderByDescending(o => o.FechaIngreso)
                .Take(100)
                .ToListAsync();

            // Sincronizar la colección preservando los elementos (evita parpadeos en la UI)
            Source.Clear();
            foreach (var item in data)
            {
                Source.Add(item);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en carga de oficios: {ex.Message}");
        }
        finally
        {
            if (mostrarCarga) IsLoading = false;
        }
    }
}
