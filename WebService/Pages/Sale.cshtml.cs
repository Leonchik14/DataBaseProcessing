using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewVariant.Models;
using NewVariant.Exceptions;

namespace WebService.Pages
{
    /// <summary>
    /// �����, ����������� ������ �������.
    /// </summary>
    public class SaleModel : PageModel
    {
        /// <summary>
        /// ��������������� ��������, ����������� �������� � ������������� ��������� ��� ������������.
        /// </summary>
        public static string MessageToUser { get; set; } = "";
        /// <summary>
        /// �����, �������������� GET-������ �� ��������� ��������� ��� ������������.
        /// </summary>
        /// <param name="message">��������� ��� ������������</param>
        public void OnGet(string message)
        {
            MessageToUser += message;
        }
        /// <summary>
        /// �����, �������������� POST-������ �� ���������� ����� ������� � �������.
        /// </summary>
        /// <param name="buyerid">������������� ����������</param>
        /// <param name="shopid">������������� ��������</param>
        /// <param name="goodid">������������� ������</param>
        /// <param name="goodcount">���������� ������</param>
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
