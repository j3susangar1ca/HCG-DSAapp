namespace DSAapp.Core.Models;

/// <summary>
/// Configuración técnica para la sesión de escaneo.
/// </summary>
public record ScannerConfig
{
    public int ResolutionDpi { get; init; } = 300;
    public bool IsDuplexEnabled { get; init; } = false;
    public bool IsColorEnabled { get; init; } = true;
}