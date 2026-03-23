using Windows.Storage.Streams;

namespace DSAapp.Core.Models;

public sealed class ScannedPage : IDisposable
{
    public IRandomAccessStream ImageStream { get; }
    public string PageLabel { get; init; } = string.Empty;
    public bool IsBlank { get; init; }

    public ScannedPage(IRandomAccessStream stream)
    {
        ImageStream = stream;
    }

    public void Dispose() => ImageStream?.Dispose();
}
