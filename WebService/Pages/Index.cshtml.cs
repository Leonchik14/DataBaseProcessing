using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewVariant.Models;
using NewVariant.Exceptions;
using System.Diagnostics;
using System.Runtime.Serialization;
using DataBaseLib;

namespace WebService.Pages
{
    /// <summary>
    /// Класс, описывающий модель домашней страницы.
    /// </summary>
    public class IndexModel : PageModel
    {
        /// <summary>
        /// Поле, содержащее логгер.
        /// </summary>
        private readonly ILogger<IndexModel> _logger;
        /// <summary>
        /// Конструктор, создающий модель на основе логгера.
        /// </summary>
        /// <param name="logger">Логгер</param>
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// Поле, содержащее сообщение для пользователя.
        /// </summary>
        public static (string, char) MessageToUser { get; set; } = ( "",'n');
        public static string ErrorMessage { get; set; } = "";
        /// <summary>
        /// Метод, осуществляющий GET-запрос на получение сообщения для пользователя.
        /// </summary>
        /// <param name="message"></param>
        public void OnGet(string message, char type)
        {
            MessageToUser = (message, type);
        }
        public void OnGetError(string error)
        {
            ErrorMessage = error;
        }
        /// <summary>
        /// Метод, осуществляющий POST-запрос на сериализацию таблицы, выбранного пользователем типа в файл.
        /// </summary>
        /// <param name="path">Путь файла</param>
        /// <param name="switchie">Переключатель типа таблицы</param>
        public void OnPostSerialize(string path, string switchie)
        {
            try
            {
                switch (switchie)
                {
                    case "shop":
                        Program.dataBase.Serialize<Shop>(path);
                        break;
                    case "buyer":
                        Program.dataBase.Serialize<Buyer>(path);
                        break;
                    case "good":
                        Program.dataBase.Serialize<Good>(path);
                        break;
                    case "sale":
                        Program.dataBase.Serialize<Sale>(path);
                        break;
                }
                OnGet("Successfully serialized!",'g');
            }
            catch
            {
                OnGet("Something went wrong with serialization!",'e');
            }
        }
        /// <summary>
        /// Метод, осуществляющий POST-запрос на десериализацию таблицу из файла в базу данных.
        /// </summary>
        /// <param name="path">Путь файла</param>
        /// <param name="switchie">Переключатеь типа таблицы.</param>
        public void OnPostDeserialize(string path, string switchie)
        {
            try
            {
                switch (switchie)
                {
                    case "shop":
                        Program.dataBase.Deserialize<Shop>(path);
                        foreach(Shop item in Program.dataBase.GetTable<Shop>())
                        {
                            if (item.Name == null || item.Country == null || item.City == null)
                            {
                                throw new IndexOutOfRangeException();
                            }
                        }
                        break;
                    case "buyer":
                        Program.dataBase.Deserialize<Buyer>(path);
                        foreach (Buyer item in Program.dataBase.GetTable<Buyer>())
                        {
                            if (item.Name == null || item.Country == null || item.City == null || item.Surname == null)
                                throw new IndexOutOfRangeException();
                        }
                        break;
                    case "good":
                        Program.dataBase.Deserialize<Good>(path);
                        foreach (Good item in Program.dataBase.GetTable<Good>())
                        {
                            if (item.Name == null || item.ShopId == null || item.Price == null || item.Category == null)
                                throw new IndexOutOfRangeException();
                        }
                        break;
                    case "sale":
                        Program.dataBase.Deserialize<Sale>(path);
                        foreach (Sale item in Program.dataBase.GetTable<Sale>())
                        {
                            if (item.BuyerId == null || item.ShopId == null || item.GoodId == null)
                                throw new IndexOutOfRangeException();
                        }
                        break;
                }
                 OnGet("Successfully deserialized!", 'g');
            }
            catch (DataBaseException ex)
            {
                
                OnGet("Something went wrong with deserialization!", 'e');
            }
            catch (IndexOutOfRangeException ex)
            {
                Program.dataBase = new DataBase();
                OnGet("Data in file is really bad! Database was deleted!", 'e');
            }
        }
        /// <summary>
        /// Метод, осуществляющий POST-запрос на получение из базы данных всех товаров,
        /// купленных покупателем с самым длинным именем.
        /// </summary>
        public void OnPostGetAllGoodsOfLongestNameBuyer()
        {
            try
            {
                Program.dataList = (Program.dataAccessLayer.GetAllGoodsOfLongestNameBuyer(Program.dataBase));
                   
            }
            catch
            {
                OnGetError("There is no goods of longest name buyer!");
                return;
            }
        }
        /// <summary>
        /// Метод, осуществляющий POST-запрос на получение из базы данных категории самого дорогого товара.
        /// </summary>
        public void OnPostGetMostExpensiveGoodCategory()
        {
            try
            {
                Program.dataList = new List<object>() {Program.dataAccessLayer.GetMostExpensiveGoodCategory(Program.dataBase)};
            }
            catch
            {
                OnGetError("There is no most expensive good category!");
                return;
            }
        }

        /// <summary>
        /// Метод, осуществляющий POST-запрос на получение из базы данных города с минимальным количеством потраченных денег.
        /// </summary>
        public void OnPostGetMinimumSalesCity()
        {
            try
            {
                Program.dataList = new List<object>() { Program.dataAccessLayer.GetMinimumSalesCity(Program.dataBase) };
            }
            catch
            {
                OnGetError("There is no minimum sales city!");
                return;
            }
        }
        /// <summary>
        /// Метод, осуществляющий POST-запрос на получение из базы данных покупателей, купивших самый популярный товар.
        /// </summary>
        public void OnPostGetMostPopularGoodBuyers()
        {
            try
            {
                Program.dataList = (Program.dataAccessLayer.GetMostPopularGoodBuyers(Program.dataBase));

            }
            catch
            {
                OnGetError("There is no most popular goods!");
                return;
            }
        }
        /// <summary>
        /// Метод, осуществляющий POST-запрос на получение из базы данных минимального количества магазинов в стране.
        /// </summary>
        public void OnPostGetMinimumNumberOfShopsInCountry()
        {
            try
            {
                Program.dataList = new List<object>() { Program.dataAccessLayer.GetMinimumNumberOfShopsInCountry(Program.dataBase) };
            }
            catch
            {
                OnGetError("There is no minimum number shops!");
                return;
            }
        }
        /// <summary>
        /// Метод, осуществляющий POST-запрос на получение из базы данных все покупки, совершенные покупателями вне своих городов.
        /// </summary>
        public void OnPostGetOtherCitySales()
        {
            try
            {
                Program.dataList = (Program.dataAccessLayer.GetOtherCitySales(Program.dataBase));

            }
            catch
            {
                OnGetError("There is no other city sales!");
                return;
            }
        }
        /// <summary>
        /// Метод, осуществляющий POST-запрос на получение из базы данных сумму всех попкупок.
        /// </summary>
        public void OnPostGetTotalSalesValue()
        {
            try
            {
                Program.dataList = new List<object>(){Program.dataAccessLayer.GetTotalSalesValue(Program.dataBase)};
            }
            catch
            {
                OnGetError("There is no total sales value!");
                return;
            }
        }
        
    }
}