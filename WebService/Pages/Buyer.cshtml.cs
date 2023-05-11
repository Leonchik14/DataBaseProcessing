using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewVariant.Models;
using NewVariant.Exceptions;

namespace WebService.Pages
{
    /// <summary>
    /// �����, ����������� ������ ����������.
    /// </summary>
    public class BuyerModel : PageModel
    {

        /// <summary>
        /// �����, ����������� POST-������ � ����������� ������ ���������� � �������.
        /// </summary>
        /// <param name="name">��� ����������</param>
        /// <param name="surname">������� ����������</param>
        /// <param name="city">����� ����������</param>
        /// <param name="country">������ ����������</param>
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
