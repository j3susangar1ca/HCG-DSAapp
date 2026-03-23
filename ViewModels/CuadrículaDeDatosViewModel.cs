using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using DSAapp.Contracts.ViewModels;
using DSAapp.Core.Models;
using DSAapp.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace DSAapp.ViewModels;

public partial class CuadrículaDeDatosViewModel : ObservableRecipient, INavigationAware
{
    private readonly AppDbContext _db;

    public ObservableCollection<Oficio> Source { get; } = new ObservableCollection<Oficio>();

    [ObservableProperty]
    private bool _isLoading;

    public CuadrículaDeDatosViewModel(AppDbContext db)
    {
        _db = db;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();
        IsLoading = true;

        try
        {
            var data = await _db.Oficios.ToListAsync();

            foreach (var item in data)
            {
                Source.Add(item);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading oficios: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
