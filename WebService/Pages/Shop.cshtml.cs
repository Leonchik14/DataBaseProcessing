using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewVariant.Models;
using NewVariant.Exceptions;

namespace WebService.Pages
{
    /// <summary>
    /// �����, ����������� ������ ��������.
    /// </summary>
    public class ShopModel : PageModel
    {
        /// <summary>
        /// �����, �������������� POST-������ �� ���������� �������� � �������.
        /// </summary>
        /// <param name="name">��� ��������</param>
        /// <param name="city">����� ��������</param>
        /// <param name="country">������ ��������</param>
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
