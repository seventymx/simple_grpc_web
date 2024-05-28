using System.Drawing;
using System.Reflection;

namespace SwissPension.Trial.Common;

public static class ResourceAccessor
{
    public enum ResourceName
    {
        ag_a,
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
}