using System.Drawing;
using System.Reflection;

namespace SwissPension.Trial.Common;

public static class ResourceAccessor
{
    public enum ResourceName
    {
        abt,
        frau,
        mann,
        stiftung
    }

    public static string GetSvg(this ResourceName resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.{resourceName}.svg") ?? throw new ArgumentException("Resource not found.", nameof(resourceName));
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static Image GetImage(this ResourceName resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        if (assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.{resourceName}.svg") is { } resourceStream)
            return Image.FromStream(resourceStream);
        using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.{resourceName}.png") ?? throw new ArgumentException("Resource not found.", nameof(resourceName));
        return Image.FromStream(stream);
    }

    public static Bitmap GetBitmap(this ResourceName resourceName) => new(resourceName.GetImage());
}