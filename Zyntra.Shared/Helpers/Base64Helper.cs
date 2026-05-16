using System.Text;

namespace Zyntra.Shared.Helpers;

public static class Base64Helper
{
    public static string Encode(string texto)
    {
        if (string.IsNullOrEmpty(texto))
            return string.Empty;

        var bytes = Encoding.UTF8.GetBytes(texto);
        return Convert.ToBase64String(bytes);
    }

    public static string Decode(string base64)
    {
        if (string.IsNullOrEmpty(base64))
            return string.Empty;

        var bytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(bytes);
    }
}