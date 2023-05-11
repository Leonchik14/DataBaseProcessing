using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewVariant.Models;
using NewVariant.Exceptions;

namespace WebService.Pages
{
    /// <summary>
    /// Класс, описывающий модель покупателя.
    /// </summary>
    public class BuyerModel : PageModel
    {

        /// <summary>
        /// Метод, реализующий POST-запрос с добавлением нового покупателя в таблицу.
        /// </summary>
        /// <param name="name">Имя покупателя</param>
        /// <param name="surname">Фамилия покупателя</param>
        /// <param name="city">Город покупателя</param>
        /// <param name="country">Страна покупателя</param>
        public void OnPost(string name, string surname, string city, string country)
        {
            try
            {

                Program.dataBase.InsertInto<Buyer>(() => new Buyer(name, surname, city, country));
            }
            catch (DataBaseException ex)
            {
                Program.dataBase.CreateTable<Buyer>();
                Program.dataBase.InsertInto<Buyer>(() => new Buyer(name, surname, city, country));
            }
        }
    }
}
