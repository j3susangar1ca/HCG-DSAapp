using DSAapp.Core.Models;

namespace DSAapp.Core.Contracts.Services;

public interface IScannerService
{
    Task<IAsyncEnumerable<ScannedPage>> ScanBatchAsync(ScannerConfig config, CancellationToken cancellationToken = default);
}