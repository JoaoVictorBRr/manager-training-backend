using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Globalization;

namespace Zyntra.Shared.Helpers;
public static class EnumFunction
{
    /// <summary>
    /// Método genérico para buscar as informações do enum.
    /// </summary>
    /// <typeparam name="TEnum">Enum</typeparam>
    /// <returns> Retorna uma SelectListItem</returns>
    public static List<SelectListItem> GetEnumList<TEnum>() where TEnum : Enum
    {
        var results = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        var list = from result in results
                   select new SelectListItem { Text = ReturnsEnumDescription(result), Value = ((int)(object)result).ToString(CultureInfo.InvariantCulture) };
        return list.ToList();
    }

    #region Métodos 
    /// <summary>
    ///  Pega a description dos enums
    /// </summary>
    /// <param name="value"> Recebe o enum</param>
    /// <returns>retorna a descrição do enumerador</returns>
    public static string ReturnsEnumDescription(Enum value)
    {
        var description = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (description.Length > 0)
            return description[0].Description.ToUpper();

        return value.ToString();
    }
    #endregion
}
