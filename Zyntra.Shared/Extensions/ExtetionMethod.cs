namespace Zyntra.Shared.Extensions;
/// <summary>
/// Métodos de extensão para manipulação de strings.
/// </summary>
public static class ExtetionMethod
{
    /// <summary>
    /// Remove caracteres especiais como ponto, hífen, barra, parênteses e espaços de uma string.
    /// </summary>
    /// <param name="text">Texto de entrada.</param>
    /// <returns>Texto sem os caracteres especiais especificados.</returns>
    public static string RemoveScore(this string text)
    {
        return !string.IsNullOrEmpty(text) ? text.Replace(".", "").Replace("-", "").Replace("/", "").Replace("(", "").Replace(")", "").Replace(" ", "") : text;
    }

    /// <summary>
    /// Remove espaços em branco do início e do fim da string e converte para minúsculas.
    /// </summary>
    /// <param name="text">Texto de entrada.</param>
    /// <returns>Texto em minúsculas e sem espaços extras.</returns>
    public static string RemoveSpace(this string text)
    {
        return !string.IsNullOrEmpty(text) ? text.ToLower().Trim().TrimStart().TrimEnd() : text;
    }
}
