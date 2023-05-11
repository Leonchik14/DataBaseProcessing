using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewVariant.Models;
using NewVariant.Exceptions;

namespace WebService.Pages
{
    /// <summary>
    /// �����, ���������� ������ ������.
    /// </summary>
    public class GoodModel : PageModel
    {
        /// <summary>
        /// ��������������� ��������, ����������� �������� � ������ ��������� ��� ������������.
        /// </summary>
        public static string MessageToUser { get; set; } = "";
        /// <summary>
        /// �����, �������������� GET-������ �� ��������� ��������� ��� ������������.
        /// </summary>
        /// <param name="message">��������� ��� ������������</param>
        public void OnGet(string message)
        {
            MessageToUser = message;
        }
        /// <summary>
        /// �����, �������������� POST-������ �� ���������� ������ � �������.
        /// </summary>
        /// <param name="name">�������� ������</param>
        /// <param name="shopid">������������� ��������</param>
        /// <param name="category">��������� ������</param>
        /// <param name="price">���� ������</param>
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
