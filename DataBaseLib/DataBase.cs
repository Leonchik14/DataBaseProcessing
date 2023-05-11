using NewVariant.Exceptions;
using NewVariant.Interfaces;
using NewVariant.Models;
using System.Runtime.Serialization;
using System.Text.Json;

namespace DataBaseLib
{
    /// <summary>
    /// Класс DataBase, реализующий основные операции над базой данных.
    /// </summary>
    public class DataBase : IDataBase
    {
        // Словарь, использующийся в качестве базы данных, хранящий в себе 4 таблицы.
        // Ключ: тип таблицы Значение: сама таблица.
        private Dictionary<Type, IEnumerable<IEntity>> _dataBase = new Dictionary<Type, IEnumerable<IEntity>>();

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public DataBase() { }

        /// <summary>
        /// Метод создающий таблицу по переданному типу.
        /// </summary>
        /// <typeparam name="T">Переданный тип</typeparam>
        /// <exception cref="DataBaseException">Выбрасывается, если таблица переданного типа уже существует.</exception>
        public void CreateTable<T>() where T : IEntity
        {
            if (_dataBase.ContainsKey(typeof(T)))
            {
                throw new DataBaseException("This table is already exists!");
            }

            _dataBase.Add(typeof(T), (new List<T>() as IEnumerable<IEntity>));

        }
        /// <summary>
        /// Метод добавления элемента в таблицу переданного типа.
        /// </summary>
        /// <typeparam name="T">Переданный тип</typeparam>
        /// <param name="getEntity">Делегат, возвращающий элемент переданного типа</param>
        /// <exception cref="DataBaseException">Выбрасывается, если таблица переданного типа не существует.</exception>
        public void InsertInto<T>(Func<T> getEntity) where T : IEntity
        {
            if (!_dataBase.ContainsKey(typeof(T)))
            {
                throw new DataBaseException("This table does not exist!");
            }

            IEnumerable<T> tmp = (_dataBase[typeof(T)] as IEnumerable<T>);
            tmp = tmp.Append(getEntity.Invoke());
            _dataBase[typeof(T)] = (tmp as IEnumerable<IEntity>);
        }

        /// <summary>
        /// Метод, десериализующий таблицу переданного типа из файла.
        /// </summary>
        /// <typeparam name="T">Переданный тип</typeparam>
        /// <param name="path">Путь файла</param>
        /// <exception cref="DataBaseException">Выбрасывется при невозможности открыть файл или н</exception>
        public void Deserialize<T>(string path) where T : IEntity
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
            FileStream fileStream;
            try
            {
                 fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch
            {
                throw new DataBaseException("File access error!");
            }
            IEnumerable<T> tableFromFile = JsonSerializer.Deserialize(fileStream, typeof(IEnumerable<T>), options) as IEnumerable<T> ??
                throw new DataBaseException("Serialization error!", new SerializationException("Bad data!"));
            _dataBase[typeof(T)] = (tableFromFile as IEnumerable<IEntity>);
        }
        /// <summary>
        /// Метод, возвращающий таблицу переданного типа
        /// </summary>
        /// <typeparam name="T">Переданный тип</typeparam>
        /// <returns>Таблица типа "Т"</returns>
        /// <exception cref="DataBaseException">Выбрасывается, если таблица переданного типа не существует</exception>
        public IEnumerable<T> GetTable<T>() where T : IEntity
        {
            if (!_dataBase.ContainsKey(typeof(T)))
            {
                throw new DataBaseException("This table does not exist!");
            }

            return (_dataBase[typeof(T)] as IEnumerable<T>);
        }

        /// <summary>
        /// Метод, осуществляющий сериализацию таблицы переданного типа.
        /// </summary>
        /// <typeparam name="T">Переданный тип</typeparam>
        /// <param name="path">Путь файла</param>
        /// <exception cref="DataBaseException">Выбрасывается при невозможности получить доступ к файлу.</exception>
        public void Serialize<T>(string path) where T : IEntity
        {
            if (!_dataBase.ContainsKey(typeof(T)))
            {
                throw new DataBaseException("This table does not exist!");
            }
            try
            {
                using FileStream fileStream = new FileStream(path, FileMode.Create);
                JsonSerializer.Serialize(fileStream, GetTable<T>(), typeof(IEnumerable<T>));
            }
            catch
            {
                throw new DataBaseException("Failed to serialize data!");
            }
        }
    }
}