using Windows.Storage.Streams;

namespace DSAapp.Core.Contracts.Services;

public interface IOcrService
{
    Task<string> ExtractTextFromImageAsync(IRandomAccessStream imageStream, CancellationToken cancellationToken = default);
}