using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

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
        _hubConnection.On("OficioActualizado", async () =>
        {
            // Ejecutar en el hilo de la UI para evitar excepciones de concurrencia
            App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
            {
                await CargarDatosAsync(mostrarCarga: false);
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

    public async void OnNavigatedFrom()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
        }
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
            System.Diagnostics.Debug.WriteLine($"Error en carga de oficios: {ex.Message}");
        }
        finally
        {
            if (mostrarCarga) IsLoading = false;
        }
    }
}
