using Windows.Media.Ocr;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using DSAapp.Core.Contracts.Services;

namespace DSAapp.Core.Services;

public sealed class OcrService : IOcrService
{
    private readonly OcrEngine _engine;

    public OcrService()
    {
        _engine = OcrEngine.TryCreateFromUserProfileLanguages();
    }

    public async Task<string> ExtractTextFromImageAsync(IRandomAccessStream imageStream, CancellationToken ct = default)
    {
        if (imageStream == null) return string.Empty;

        var decoder = await BitmapDecoder.CreateAsync(imageStream);
        using var bitmap = await decoder.GetSoftwareBitmapAsync();

        // El motor nativo de Windows 11 ya optimiza internamente, 
        // pero podemos pasar el bitmap directamente.
        var result = await _engine.RecognizeAsync(bitmap);
        return result.Text;
    }
}