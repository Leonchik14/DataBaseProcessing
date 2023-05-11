using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewVariant.Models;
using NewVariant.Exceptions;

namespace WebService.Pages
{
    /// <summary>
    /// Класс, описывающий модель магазина.
    /// </summary>
    public class ShopModel : PageModel
    {
        /// <summary>
        /// Метод, осуществляющий POST-запрос на добавление магазина в таблицу.
        /// </summary>
        /// <param name="name">Имя магазина</param>
        /// <param name="city">Город магазина</param>
        /// <param name="country">Страна магазина</param>
        public void OnPost(string name, string city, string country)
        {
            try
            {
                Program.dataBase.InsertInto<Shop>(() => new Shop(name, city, country));
            }
            catch (DataBaseException ex)
            {
                Program.dataBase.CreateTable<Shop>();
                Program.dataBase.InsertInto<Shop>(() => new Shop(name, city, country));
            }
        }
    }
}
