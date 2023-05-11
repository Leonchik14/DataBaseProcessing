using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NewVariant.Exceptions;
using NewVariant.Interfaces;
using NewVariant.Models;

namespace DataBaseLib
{
    /// <summary>
    /// Класс, реализующий запросы к базе данных.
    /// </summary>
    public class DataAccessLayer : IDataAccessLayer
    {
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public DataAccessLayer() { }

        /// <summary>
        /// Метод, осуществляющий возврат коллекции товаров, купленных покупателем с самым длиным именем, из базы данных.
        /// </summary>
        /// <param name="dataBase">База данных</param>
        /// <returns>Коллекция товаров, купленных покупателем с самым длиным именем</returns>
        public IEnumerable<Good> GetAllGoodsOfLongestNameBuyer(IDataBase dataBase)
        {
            var longestNameBuyerGoods = from sale in dataBase.GetTable<Sale>()
                                        where sale.BuyerId == (from b in dataBase.GetTable<Buyer>()
                                                               orderby b.Name.Length, b.Name, b.Id ascending
                                                               select b).LastOrDefault()?.Id
                                        from good in dataBase.GetTable<Good>()
                                        where good.Id == sale.GoodId
                                        select good;

            return longestNameBuyerGoods;
        }
        /// <summary>
        /// Метод, осуществляющий возврат минимального количества магазинов в стране из базы данных.
        /// </summary>
        /// <param name="dataBase">База данных</param>
        /// <returns> Минимальное количество магазинов в стране.</returns>
        public int GetMinimumNumberOfShopsInCountry(IDataBase dataBase)
        {
            var minimumNumberOfShopsInCountry = (from shops in dataBase.GetTable<Shop>()
                                                let id = shops.Id
                                                let country = shops.Country
                                                let countryShopAmount = (from countryShop in dataBase.GetTable<Shop>()
                                                                         where countryShop.Country == country
                                                                         select countryShop).Count()
                                                orderby countryShopAmount ascending
                                                select countryShopAmount).FirstOrDefault();
            return minimumNumberOfShopsInCountry;
            
        }
        /// <summary>
        /// Метод, осуществляющий возврат имени города, в котором было потрачено меньше всего денег, из базы данных.
        /// </summary>
        /// <param name="dataBase">База данных</param>
        /// <returns>Имя города, в котором было потрачено меньше всего денег</returns>
        public string? GetMinimumSalesCity(IDataBase dataBase)
        {
            var minimumSalesCity = (from city in dataBase.GetTable<Shop>()
                                    let name = city.City
                                    let citySum = (from cityShops in (from shop in dataBase.GetTable<Shop>()
                                                                     let city = shop.City
                                                                     let shopSum = (from sale in dataBase.GetTable<Sale>()
                                                                                    where sale.ShopId == shop.Id
                                                                                    from good in dataBase.GetTable<Good>()
                                                                                    where good.Id == sale.GoodId
                                                                                    let saleAmount = good.Price * sale.GoodCount
                                                                                    select saleAmount).Sum()
                                                                     select new { city, shopSum })
                                                   where name == cityShops.city
                                                   select cityShops.shopSum).Sum()
                                    orderby citySum ascending
                                    select name).FirstOrDefault();
            return minimumSalesCity;
        }
        /// <summary>
        /// Метод, возвращающий самую дорогую категорию товаров из базы данных.
        /// </summary>
        /// <param name="dataBase">База данных</param>
        /// <returns>Самая дорогая категория товаров</returns>
        public string? GetMostExpensiveGoodCategory(IDataBase dataBase)
        {
            var mostExpensiveGoodCategory = (from good in dataBase.GetTable<Good>()
                                             orderby good.Price
                                             select good).LastOrDefault()?.Category;
            return mostExpensiveGoodCategory;
        }
        /// <summary>
        /// Метод, возвращающий коллекцию покупателей, купивших самый популярный товар из базы данных.
        /// </summary>
        /// <param name="dataBase">База данных</param>
        /// <returns>Колекция покупателей, купивших самый популярный товар</returns>
        public IEnumerable<Buyer> GetMostPopularGoodBuyers(IDataBase dataBase)
        {
            var mostPopularGoodBuyers = from buyer in dataBase.GetTable<Buyer>()
                                        let mostPopularGoodId = (from sale in dataBase.GetTable<Sale>()
                                                                 let goodID = sale.GoodId
                                                                 let goodCount = (from goodSale in dataBase.GetTable<Sale>()
                                                                                  where goodSale.GoodId == goodID
                                                                                  select goodSale.GoodCount).Sum()
                                                                 orderby goodCount ascending
                                                                 select goodID).Last()
                                        where (from sale in dataBase.GetTable<Sale>()
                                               where sale.GoodId == mostPopularGoodId
                                               select sale.BuyerId).Contains(buyer.Id)
                                        select buyer;
            return mostPopularGoodBuyers;
        }
        /// <summary>
        /// Метод, возвращающий коллекцию покупок, совершенных вне города покупателя из базы данных.
        /// </summary>
        /// <param name="dataBase"></param>
        /// <returns>Коллекция покупок, совершенных вне города покупателя.</returns>
        public IEnumerable<Sale> GetOtherCitySales(IDataBase dataBase)
        {
            var otherCitySales = from buyer in dataBase.GetTable<Buyer>()                                 
                                 from sale in dataBase.GetTable<Sale>()
                                 where sale.BuyerId == buyer.Id
                                 let Id = sale.Id
                                 let ShopId = sale.ShopId
                                 let SaleCity = (from shop in dataBase.GetTable<Shop>()
                                                    where shop.Id == ShopId
                                                    select shop.City).First()
                                 where SaleCity != buyer.City
                                 select sale;
            return otherCitySales;
        }

        /// <summary>
        /// Метод, возвращающий сумму всех совершенных покупок из базы данных.
        /// </summary>
        /// <param name="dataBase">База данных</param>
        /// <returns>Сумма всех совершенных покупок</returns>
        public long GetTotalSalesValue(IDataBase dataBase)
        {
            long totalSalesValue = (from sale in dataBase.GetTable<Sale>()
                                    from good in dataBase.GetTable<Good>()
                                    where good.Id == sale.GoodId
                                    let sum = good.Price * sale.GoodCount
                                    select sum).Sum();

            return totalSalesValue;

        }

        // Дополнительные методы для WebService

        /// <summary>
        /// Метод, определяющий существует ли в базе данных магазин с переданным идентификатором.
        /// </summary>
        /// <param name="dataBase">База данных</param>
        /// <param name="id">Идентификатор</param>
        /// <returns>Существует/не существует</returns>
        public bool GetShopWithCurrentIdExisance(IDataBase dataBase, int id)
        {
            try
            {
                bool shopWithCurrentIdExisance = (from shop in dataBase.GetTable<Shop>()
                                                  select shop.Id).Contains(id);
                return shopWithCurrentIdExisance;
            }
            catch (DataBaseException ex)
            {
                return false;
            }
        }
        /// <summary>
        /// Метод, определяющий существует ли в базе данных покупатель с переданным идентификатором.
        /// </summary>
        /// <param name="dataBase">База данных</param>
        /// <param name="id">Идентификатор</param>
        /// <returns>Существует/не существует</returns>
        public bool GetBuyerWithCurrentIdExisance(IDataBase dataBase, int id)
        {
            try
            {
                bool buyerWithCurrentIdExisance = (from buyer in dataBase.GetTable<Buyer>()
                                                   select buyer.Id).Contains(id);
                return buyerWithCurrentIdExisance;
            }
            catch (DataBaseException ex)
            {
                return false;
            }
        }
        /// <summary>
        /// Метод, определяющий существует ли в базе данных товар с переданным идентификатором.
        /// </summary>
        /// <param name="dataBase">База данных</param>
        /// <param name="id">Идентификатор</param>
        /// <returns>Существует/не существует</returns>
        public bool GetGoodWithCurrentIdExisance(IDataBase dataBase, int id)
        {
            try
            {
                bool goodWithCurrentIdExisance = (from good in dataBase.GetTable<Good>()
                                                  select good.Id).Contains(id);
                return goodWithCurrentIdExisance;
            }
            catch (DataBaseException ex)
            {
                return false;
            }
        }
    }

}
