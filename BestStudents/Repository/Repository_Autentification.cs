using BestStudents.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BestStudents.Repository
{
    public class Repository_Autentification
    {
        //Строка подключение к бд
        string connectionString = "Server=93.125.10.36;Database=KubickRubick;Integrated Security=false;User Id=User;Password=2001ksu2001;";

        //Хеширование
        public string GetHash(string input)
        {
            MD5 md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public int? Create(
            string Name,
            string Email,
            string PhoneNumber,
            string Password,
            int Money,
            int RoleId)
        {

            int? userId = null;
            var queryString =
"INSERT INTO [Users] ([Name] ,[Email] ,[PhoneNumber] ,[Password], [Money], [RoleId]) OUTPUT Inserted.UserId VALUES (@Name ,@Email,@PhoneNumber,@Password,@Money,@RoleId)";//sql запрос

            // Создание и открытие соединения в блоке using.
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                //Создание объеков команд и параметров.
                using (var command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Password", GetHash(Password));
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                    command.Parameters.AddWithValue("@Money", Money);
                    command.Parameters.AddWithValue("@RoleId", RoleId);
                    //Открытие подключения
                    connection.Open();
                    userId = (int?)command.ExecuteScalar();
                }
            }
            return userId;
        }


        public Users Get(string Name)
        {
            Users user = null;
            var queryString = "select top 1 UserId,Name,Email,PhoneNumber,Password,Money,RoleId from Users where Name=@Name";//sql запрос

            // Создание и открытие соединения в блоке using.
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@Name", Name);
                // Создание объеков команд и параметров.
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        object dbVal = null;

                        user = new Users();
                        user.UserId = (int)reader.GetValue(0);

                        dbVal = reader.GetValue(1);
                        if (!(dbVal is DBNull))
                        {
                            user.Name = (dbVal as string).Trim();
                        }

                        dbVal = reader.GetValue(2);
                        if (!(dbVal is DBNull))
                        {
                            user.Email = (dbVal as string).Trim();
                        }

                        dbVal = reader.GetValue(3);
                        if (!(dbVal is DBNull))
                        {
                            user.PhoneNumber = (dbVal as string).Trim();
                        }

                        dbVal = reader.GetValue(4);
                        if (!(dbVal is DBNull))
                        {
                            user.Password = (dbVal as string).Trim();
                        }

                        user.Money = (int)reader.GetValue(5);
                        user.RoleId = (int)reader.GetValue(6);
                    }
                }
            }
            return user;
        }


        public Products GetProdect(int Id)
        {
            Products Product = null;
            var queryString = "select * from Products where Id=@Id";//sql запрос

            // Создание и открытие соединения в блоке using.
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@Id", Id);
                // Создание объеков команд и параметров.
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        object dbVal = null;

                        Product = new Products();
                        Product.Id = (int)reader.GetValue(0);

                        dbVal = reader.GetValue(1);
                        if (!(dbVal is DBNull))
                        {
                            Product.Name = (dbVal as string).Trim();
                        }

                        Product.Cost = (int)reader.GetValue(2);

                        Product.Count = (int)reader.GetValue(3);

                        dbVal = reader.GetValue(4);
                        if (!(dbVal is DBNull))
                        {
                            Product.Description = (dbVal as string).Trim();
                        }

                        Product.AddedBy = (int)reader.GetValue(5);
                    }
                }
            }
            return Product;
        }

        public bool UpdateProduct(int Id,int Count)
        {
            try
            {
                var queryString = "Update Products Set Count=@Count where Id = @id";//sql запрос
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Create the Command and Parameter objects.
                    var command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@id", Id);
                    command.Parameters.AddWithValue("@Count", Count);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    reader.Read();
                }


                return true;
            }
            catch(Exception exception)
            {
                return false;
            }
        }

        public bool AddToBascet(string Name, int Cost, int Count, string Description, int UserId)
        {
            try
            {
                var queryString =
                    "INSERT INTO [basket] ([Name] ,[Cost] ,[Count] ,[Description], [UserId]) OUTPUT Inserted.Id VALUES (@Name ,@Cost , @Count, @Description, @UserId)";//sql запрос
                using (SqlConnection connection =
                new SqlConnection(connectionString))
                {
                    //Создание объеков команд и параметров.
                    using (var command = new SqlCommand(queryString, connection))
                    {
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@Cost", Cost);
                        command.Parameters.AddWithValue("@Count", Count);
                        command.Parameters.AddWithValue("@Description", Description);
                        command.Parameters.AddWithValue("@UserId", UserId);
                        //Открытие подключения
                        connection.Open();

                        int? Id = (int?)command.ExecuteScalar();
                    }
                }
                return true;
            }
            catch(Exception exception)
            {
                return false;
            }
        }


        public ObservableCollection<Products> GetAllProducts()
        {
            Products products = null;

            var list = new ObservableCollection<Products>();

            var queryString = "select * from Products";//sql запрос

            // Создание и открытие соединения в блоке using.
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                // Создание объеков команд и параметров.
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object dbVal = null;

                        products = new Products();


                        products.Id = (int)reader.GetValue(0);

                        dbVal = reader.GetValue(1);
                        if (!(dbVal is DBNull))
                        {
                            products.Name = (dbVal as string).Trim();
                        }

                        products.Cost = (int)reader.GetValue(2);

                        products.Count = (int)reader.GetValue(3);

                        dbVal = reader.GetValue(4);
                        if (!(dbVal is DBNull))
                        {
                            products.Description = (dbVal as string).Trim();
                        }

                        products.AddedBy = (int)reader.GetValue(5);

                        list.Add(products);
                    }
                }
            }
            return list;
        }


        public ObservableCollection<Products> Sort(string sort)
        {
            Products products = null;

            var list = new ObservableCollection<Products>();

            var queryString = "select * from Products where Name like @sort ";//sql запрос
            // Создание и открытие соединения в блоке using.
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                // Создание объеков команд и параметров.
                command.Parameters.AddWithValue("@sort", sort+"%");
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object dbVal = null;

                        products = new Products();


                        products.Id = (int)reader.GetValue(0);

                        dbVal = reader.GetValue(1);
                        if (!(dbVal is DBNull))
                        {
                            products.Name = (dbVal as string).Trim();
                        }

                        products.Cost = (int)reader.GetValue(2);

                        products.Count = (int)reader.GetValue(3);

                        dbVal = reader.GetValue(4);
                        if (!(dbVal is DBNull))
                        {
                            products.Description = (dbVal as string).Trim();
                        }

                        products.AddedBy = (int)reader.GetValue(5);

                        list.Add(products);
                    }
                }
            }
            return list;
        }


        public int? GetCost(int userId)
        {
            try
            {
                int? cost = 0;

                var queryString = "select Sum(basket.Cost) from basket where UserId=@userId";//sql запрос

                // Создание и открытие соединения в блоке using.
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand(queryString, connection);
                    // Создание объеков команд и параметров.
                    command.Parameters.AddWithValue("@userId", userId);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cost = (int)reader.GetValue(0);
                        }
                    }
                    if (cost == null)
                    {
                        return 0;
                    }
                    return cost;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int? GetCount(int userId)
        {
            try
            {
                int? count = 0;

                var queryString = "select Count(basket.Count) from basket where UserId=@userId";//sql запрос

                // Создание и открытие соединения в блоке using.
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand(queryString, connection);
                    // Создание объеков команд и параметров.
                    command.Parameters.AddWithValue("@userId", userId);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            count = (int)reader.GetValue(0);
                        }
                    }
                    if (count == null)
                    {
                        return 0;
                    }
                    return count;
                }
            }
            catch(Exception ex)
            {
                return 0;
            }
        }


        public ObservableCollection<Basket> GetAllbasket(int userId)
        {
            Basket Basket = null;

            var list = new ObservableCollection<Basket>();

            var queryString = "select * from basket where UserId=@userId";//sql запрос

            // Создание и открытие соединения в блоке using.
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                // Создание объеков команд и параметров.
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object dbVal = null;

                        Basket = new Basket();


                        Basket.Id = (int)reader.GetValue(0);

                        dbVal = reader.GetValue(1);
                        if (!(dbVal is DBNull))
                        {
                            Basket.Name = (dbVal as string).Trim();
                        }

                        Basket.Cost = (int)reader.GetValue(2);

                        Basket.Count = (int)reader.GetValue(3);

                        dbVal = reader.GetValue(4);
                        if (!(dbVal is DBNull))
                        {
                            Basket.Description = (dbVal as string).Trim();
                        }

                        Basket.UserId = (int)reader.GetValue(5);

                        list.Add(Basket);
                    }
                }
            }
            return list;
        }


        public bool Buy(int Money1, int Money2, int UserId)//Money1 - acc money, Money2 - cost.
        {
            try
            {
                int sum = Money1 - Money2;



                //Sql
                var queryString1 = "Update Users Set Money=@Money where UserId = @UserId";//sql запрос
                var queryString2 = "Delete from basket where UserId=@UserId";//sql запрос
                var queryString3 ="INSERT INTO [Report] ([Date] ,[Money] ,[Description]) OUTPUT Inserted.Id VALUES (@Date ,@Money,@Description)";//sql запрос

                //Connections
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Create the Command and Parameter objects.
                    var command = new SqlCommand(queryString1, connection);
                    command.Parameters.AddWithValue("@Money", sum);
                    command.Parameters.AddWithValue("@UserId", UserId);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    reader.Read();
                }


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Create the Command and Parameter objects.
                    var command = new SqlCommand(queryString2, connection);
                    command.Parameters.AddWithValue("@UserId", UserId);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    reader.Read();
                }

                using (SqlConnection connection =new SqlConnection(connectionString))
                {
                    //Создание объеков команд и параметров.
                    using (var command = new SqlCommand(queryString3, connection))
                    {
                        command.Parameters.AddWithValue("@Date", DateTime.Now);
                        command.Parameters.AddWithValue("@Money", Money2);
                        command.Parameters.AddWithValue("@Description", "User was buy products");
                        //Открытие подключения
                        connection.Open();
                        int? Id = (int?)command.ExecuteScalar();
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }




        public ObservableCollection<Users> GetAllUsers(int Id)
        {
            Users user = null;

            var list = new ObservableCollection<Users>();


            var queryString = "select * from Users where UserId!=@id";//sql запрос

            // Создание и открытие соединения в блоке using.
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                // Создание объеков команд и параметров.
                command.Parameters.AddWithValue("@id", Id);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object dbVal = null;

                        user = new Users();

                        user.UserId = (int)reader.GetValue(0);

                        dbVal = reader.GetValue(1);
                        if (!(dbVal is DBNull))
                        {
                            user.Name = (dbVal as string).Trim();
                        }

                        dbVal = reader.GetValue(2);
                        if (!(dbVal is DBNull))
                        {
                            user.Email = (dbVal as string).Trim();
                        }

                        dbVal = reader.GetValue(3);
                        if (!(dbVal is DBNull))
                        {
                            user.PhoneNumber = (dbVal as string).Trim();
                        }

                        dbVal = reader.GetValue(4);
                        if (!(dbVal is DBNull))
                        {
                            user.Password = (dbVal as string).Trim();
                        }

                        user.Money = (int)reader.GetValue(5);
                        user.RoleId = (int)reader.GetValue(6);

                        list.Add(user);
                    }
                }
            }
            return list;
        }



        public bool AddMoney(int money, int Id)
        {
            try
            {
                var queryString = "select Money from Users where UserId = @UserId ";//sql запрос
                var queryString1 = "Update Users Set Money=@Money where UserId = @UserId";//sql запрос

                int? finallymoney = 0;
                int? oldmoney = 0;

                // Создание и открытие соединения в блоке using.
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@UserId", Id);
                    // Создание объеков команд и параметров.
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            oldmoney = (int)reader.GetValue(0);
                        }
                    }
                }

                finallymoney = oldmoney + money;

                //Connections
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Create the Command and Parameter objects.
                    var command = new SqlCommand(queryString1, connection);
                    command.Parameters.AddWithValue("@Money", finallymoney);
                    command.Parameters.AddWithValue("@UserId", Id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    reader.Read();
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }


        public ObservableCollection<Report> GetReports()
        {

            Report report = null;

            var list = new ObservableCollection<Report>();


            var queryString = "select * from Report";//sql запрос

            // Создание и открытие соединения в блоке using.
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object dbVal = null;

                        report = new Report();

                        report.Id = (int)reader.GetValue(0);

                        dbVal = reader.GetValue(1);
                        if (!(dbVal is DBNull))
                        {
                            report.Date = (dbVal as string).Trim();
                        }

                        report.Money = (int)reader.GetValue(2);

                        dbVal = reader.GetValue(3);
                        if (!(dbVal is DBNull))
                        {
                            report.Description = (dbVal as string).Trim();
                        }

                        list.Add(report);
                    }
                }
            }
            return list;
        }




        public bool Delete(int Id)
        {
            try
            {
                var queryString = "Delete from Products where AddedBy=@id;" +
                    "Delete from basket where UserId=@id;" +
                    "Delete from Users where UserId=@id;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Create the Command and Parameter objects.
                    var command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@id", Id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    reader.Read();
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}