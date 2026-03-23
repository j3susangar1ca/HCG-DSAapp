using System.Runtime.CompilerServices;
using DSAapp.Core.Contracts.Services;
using DSAapp.Core.Models;
using Windows.Devices.Enumeration;
using Windows.Devices.Scanners;
using Windows.Storage.Streams;

namespace DSAapp.Core.Services;

[System.Runtime.Versioning.SupportedOSPlatform("windows10.0.17763.0")]
public class ScannerService : IScannerService
{
    public async Task<IAsyncEnumerable<ScannedPage>> ScanBatchAsync(
        ScannerConfig config, CancellationToken ct)
    {
        var devices = await DeviceInformation
            .FindAllAsync(ImageScanner.GetDeviceSelector());

        if (devices.Count == 0)
            throw new InvalidOperationException("No se encontró ningún escáner conectado.");

        var scanner = await ImageScanner.FromIdAsync(devices[0].Id);
        return ProcessScannerAsync(scanner, config, ct);
    }

    private async IAsyncEnumerable<ScannedPage> ProcessScannerAsync(
        ImageScanner scanner,
        ScannerConfig config,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var tempFolder = await Windows.Storage.ApplicationData.Current
                              .TemporaryFolder
                              .CreateFolderAsync(
                                  $"scan_{Guid.NewGuid()}",
                                  Windows.Storage.CreationCollisionOption.ReplaceExisting);

        int pageIndex = 0;

        if (scanner.IsScanSourceSupported(ImageScannerScanSource.Feeder))
        {
            // Usamos la resolución óptica como base segura ya que las propiedades de ImageScannerResolution son de solo lectura
            var resolution = scanner.FeederConfiguration.OpticalResolution;
            scanner.FeederConfiguration.DesiredResolution = resolution;

            var result = await scanner.ScanFilesToFolderAsync(
                ImageScannerScanSource.Feeder, tempFolder);

            foreach (var file in result.ScannedFiles)
            {
                ct.ThrowIfCancellationRequested();

                var stream = new InMemoryRandomAccessStream();
                using var fileStream = await file.OpenReadAsync();
                await RandomAccessStream.CopyAsync(fileStream, stream);
                stream.Seek(0);

                yield return new ScannedPage(stream)
                {
                    PageLabel = $"Página {++pageIndex}",
                    IsBlank   = false
                };
            }
        }
        else if (scanner.IsScanSourceSupported(ImageScannerScanSource.Flatbed))
        {
            var resolution = scanner.FlatbedConfiguration.OpticalResolution;
            scanner.FlatbedConfiguration.DesiredResolution = resolution;

            var result = await scanner.ScanFilesToFolderAsync(
                ImageScannerScanSource.Flatbed, tempFolder);

            foreach (var file in result.ScannedFiles)
            {
                ct.ThrowIfCancellationRequested();

                var stream = new InMemoryRandomAccessStream();
                using var fileStream = await file.OpenReadAsync();
                await RandomAccessStream.CopyAsync(fileStream, stream);
                stream.Seek(0);

                yield return new ScannedPage(stream)
                {
                    PageLabel = $"Página {++pageIndex}",
                    IsBlank   = false
                };
            }
        }
    }
}
