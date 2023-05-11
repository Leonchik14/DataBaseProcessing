using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewVariant.Models;
using NewVariant.Exceptions;

namespace WebService.Pages
{
    /// <summary>
    /// Класс, описывющий модель товара.
    /// </summary>
    public class GoodModel : PageModel
    {
        /// <summary>
        /// Автореализуемое свойство, позволяющее получать и менять сообщение для пользователя.
        /// </summary>
        public static string MessageToUser { get; set; } = "";
        /// <summary>
        /// Метод, осуществляющий GET-запрос на получение сообщения для пользователя.
        /// </summary>
        /// <param name="message">Сообщение для пользователя</param>
        public void OnGet(string message)
        {
            MessageToUser = message;
        }
        /// <summary>
        /// Метод, осуществляющий POST-запрос на добавление товара в таблицу.
        /// </summary>
        /// <param name="name">Название товара</param>
        /// <param name="shopid">Идентификатор магазина</param>
        /// <param name="category">Категория товара</param>
        /// <param name="price">Цена товара</param>
        public void OnPost(string name, int shopid, string category, int price)
        {
            try
            {
                if (!Program.dataAccessLayer.GetShopWithCurrentIdExisance(Program.dataBase, shopid))
                {
                    OnGet("Shop with current Id does not exist!!!");
                    return;
                }
                Program.dataBase.InsertInto<Good>(() => new Good(name, shopid, category, price));
            }
            catch (DataBaseException ex)
            {
                Program.dataBase.CreateTable<Good>();
                Program.dataBase.InsertInto<Good>(() => new Good(name, shopid, category, price));
            }
        }
    }
}
