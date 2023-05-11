using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewVariant.Models;
using NewVariant.Exceptions;

namespace WebService.Pages
{
    /// <summary>
    /// Класс, описывающий модель покупки.
    /// </summary>
    public class SaleModel : PageModel
    {
        /// <summary>
        /// Автореализуемое свойство, позволяющее получать и редактировать сообщение для пользователя.
        /// </summary>
        public static string MessageToUser { get; set; } = "";
        /// <summary>
        /// Метод, осуществляющий GET-запрос на получение сообщения для пользователя.
        /// </summary>
        /// <param name="message">Сообщение для пользователя</param>
        public void OnGet(string message)
        {
            MessageToUser += message;
        }
        /// <summary>
        /// Метод, осуществляющий POST-запрос на добавление новой продажи в таблицу.
        /// </summary>
        /// <param name="buyerid">Идентификатор покупателя</param>
        /// <param name="shopid">Идентификатор магазина</param>
        /// <param name="goodid">Идентификатор товара</param>
        /// <param name="goodcount">Количество товара</param>
        public void OnPost(int buyerid, int shopid, int goodid, int goodcount)
        {
            try
            {
                bool flag = false;
                if (!Program.dataAccessLayer.GetBuyerWithCurrentIdExisance(Program.dataBase, buyerid))
                {
                    OnGet("Buyer with current Id does not exist!!! ");
                    flag = true;
                }
                if (!Program.dataAccessLayer.GetShopWithCurrentIdExisance(Program.dataBase, shopid))
                {
                    OnGet("Shop with current Id does not exist!!! ");
                    flag = true;
                }
                if (!Program.dataAccessLayer.GetGoodWithCurrentIdExisance(Program.dataBase, goodid))
                {
                    OnGet("Good with current Id does not exist!!! ");
                    flag = true;
                }
                if (flag)
                {
                    return;
                }
                Program.dataBase.InsertInto<Sale>(() => new Sale(buyerid, shopid, goodid, goodcount));
            }
            catch (DataBaseException ex)
            {
                Program.dataBase.CreateTable<Sale>();
                Program.dataBase.InsertInto<Sale>(() => new Sale(buyerid, shopid, goodid, goodcount));
            }
        }
    }
}
