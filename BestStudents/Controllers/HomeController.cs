using BestStudents.Models;
using BestStudents.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BestStudents.Controllers
{
    public class HomeController : Controller
    {
        Repository_Autentification Autentification = new Repository_Autentification();

        string _UserId;
        string _Name;
        string _Email;
        string _Password;
        string _PhoneNumber;
        string _Money;
        string _RoleId;

        ObservableCollection<Products> listproducts;
        ObservableCollection<Basket> listbasket;
        ObservableCollection<Users> listusers;
        ObservableCollection<Report> listreport;

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

        bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }



        public ActionResult Main()
        {
            ViewBag.menu_item = "menu_item4";
            return View();
        }

        public ActionResult Add(int IdProduct)
        {
            HttpCookie Data = Request.Cookies["Data"];

            if (Data == null)
            {
                ViewBag.menu_item = "menu_item3";
                return View("Login");
            }
            else
            {
                _Name = Data["Name"];
                _Email = Data["Email"];
                _Money = Data["Money"];
                _UserId = Data["UserId"];
            }

            Products products = new Products();

            products = Autentification.GetProdect(IdProduct);
            if (products.Count != 0)
            {
                Autentification.AddToBascet(products.Name, products.Cost, 1, products.Description, Convert.ToInt32(_UserId));

                Autentification.UpdateProduct(products.Id, products.Count - 1);

                ViewBag.menu_item = "menu_item1";

                listproducts = Autentification.GetAllProducts();

                ViewBag.listproducts = listproducts;
                return View("Shop");
            }
            else
            {
                ViewBag.menu_item = "menu_item1";

                listproducts = Autentification.GetAllProducts();

                ViewBag.listproducts = listproducts;
                return View("Shop");
            }
        }




        [HttpGet]
        public ActionResult Account()
        {
            HttpCookie Data = Request.Cookies["Data"];

            if (Data == null)
            {
                ViewBag.menu_item = "menu_item3";
                return View("Login");
            }
            else
            {
                _UserId = Data["UserId"];
                _Name = Data["Name"];
                _Email = Data["Email"];
                _Money = Data["Money"];
                ViewBag.Login = _Name;
                ViewBag.Email = _Email;
                ViewBag.Money = _Money + "руб.";


                int? Cost = Autentification.GetCost(Convert.ToInt32(_UserId));
                int? Count = Autentification.GetCount(Convert.ToInt32(_UserId));

                listbasket = Autentification.GetAllbasket(Convert.ToInt32(_UserId));
                ViewBag.listbasket = listbasket;
                ViewBag.menu_item = "menu_item2";
                ViewBag.Cost = Cost;
                ViewBag.Count = Count;
                return View("Account");
            }
            return View();
        }

        public ActionResult Login()
        {
            ViewBag.menu_item = "menu_item3";

            return View();
        }

        public ActionResult AdminPanel()
        {
            return View();
        }

        public ActionResult Shop()
        {
            ViewBag.menu_item = "menu_item1";

            listproducts = Autentification.GetAllProducts();

            ViewBag.listproducts = listproducts;

            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Buy()
        {
            HttpCookie Data = Request.Cookies["Data"];

            if (Data == null)
            {
                ViewBag.menu_item = "menu_item3";
                return View("Login");
            }
            else
            {
                _Name = Data["Name"];
                _Email = Data["Email"];
                _Money = Data["Money"];
                _UserId = Data["UserId"];
            }

            int? Money2 = Autentification.GetCost(Convert.ToInt32(_UserId));

            if (Convert.ToInt32(_Money) != 0 && Convert.ToInt32(Money2) != 0)
            {

                if (Convert.ToInt32(_Money) > Convert.ToInt32(Money2))//True
                {
                    if (Autentification.Buy(Convert.ToInt32(_Money), Convert.ToInt32(Money2), Convert.ToInt32(_UserId)) == true)
                    {
                        Users user = Autentification.Get(_Name);

                        HttpCookie cookie = new HttpCookie("Data");
                        cookie.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(cookie);

                        //Add Cookies

                        HttpCookie Data1 = new HttpCookie("Data");

                        Data1.Values.Add("UserId", user.UserId.ToString());
                        Data1.Values.Add("Name", user.Name.ToString());
                        Data1.Values.Add("Email", user.Email.ToString());
                        Data1.Values.Add("Password", user.Password.ToString());
                        Data1.Values.Add("PhoneNumber", user.PhoneNumber.ToString());
                        Data1.Values.Add("Money", user.Money.ToString());
                        Data1.Values.Add("RoleId", user.RoleId.ToString());
                        Data1.Expires = DateTime.Now.AddMinutes(30);
                        Response.Cookies.Add(Data1);

                        if (Data1 == null)
                        {
                            return View("Registration");
                        }
                        else
                        {
                            _UserId = Data1["UserId"];
                            _Name = Data1["Name"];
                            _Email = Data1["Email"];
                            _Money = Data1["Money"];
                            ViewBag.Login = _Name;
                            ViewBag.Email = _Email;
                            ViewBag.Money = _Money + "руб.";




                            int? Cost = Autentification.GetCost(Convert.ToInt32(_UserId));
                            int? Count = Autentification.GetCount(Convert.ToInt32(_UserId));

                            ViewBag.Cost = Cost;
                            ViewBag.Count = Count;

                            listbasket = Autentification.GetAllbasket(Convert.ToInt32(_UserId));
                            ViewBag.listbasket = listbasket;
                            return View("Account");
                        }
                    }
                    else
                    {
                        return View("Account");
                    }
                }
                else//False
                {
                    Users user = Autentification.Get(_Name);

                    HttpCookie cookie = new HttpCookie("Data");
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(cookie);

                    //Add Cookies

                    HttpCookie Data1 = new HttpCookie("Data");

                    Data1.Values.Add("UserId", user.UserId.ToString());
                    Data1.Values.Add("Name", user.Name.ToString());
                    Data1.Values.Add("Email", user.Email.ToString());
                    Data1.Values.Add("Password", user.Password.ToString());
                    Data1.Values.Add("PhoneNumber", user.PhoneNumber.ToString());
                    Data1.Values.Add("Money", user.Money.ToString());
                    Data1.Values.Add("RoleId", user.RoleId.ToString());
                    Data1.Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies.Add(Data1);

                    if (Data1 == null)
                    {
                        return View("Registration");
                    }
                    else
                    {
                        _UserId = Data1["UserId"];
                        _Name = Data1["Name"];
                        _Email = Data1["Email"];
                        _Money = Data1["Money"];
                        ViewBag.Login = _Name;
                        ViewBag.Email = _Email;
                        ViewBag.Money = _Money + "руб.";




                        int? Cost = Autentification.GetCost(Convert.ToInt32(_UserId));
                        int? Count = Autentification.GetCount(Convert.ToInt32(_UserId));

                        ViewBag.Cost = Cost;
                        ViewBag.Count = Count;

                        listbasket = Autentification.GetAllbasket(Convert.ToInt32(_UserId));
                        ViewBag.listbasket = listbasket;
                        return View("Account");
                    }
                }
            }
            else
            {
                Users user = Autentification.Get(_Name);

                HttpCookie cookie = new HttpCookie("Data");
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);

                //Add Cookies

                HttpCookie Data1 = new HttpCookie("Data");

                Data1.Values.Add("UserId", user.UserId.ToString());
                Data1.Values.Add("Name", user.Name.ToString());
                Data1.Values.Add("Email", user.Email.ToString());
                Data1.Values.Add("Password", user.Password.ToString());
                Data1.Values.Add("PhoneNumber", user.PhoneNumber.ToString());
                Data1.Values.Add("Money", user.Money.ToString());
                Data1.Values.Add("RoleId", user.RoleId.ToString());
                Data1.Expires = DateTime.Now.AddMinutes(30);
                Response.Cookies.Add(Data1);

                if (Data1 == null)
                {
                    return View("Registration");
                }
                else
                {
                    _UserId = Data1["UserId"];
                    _Name = Data1["Name"];
                    _Email = Data1["Email"];
                    _Money = Data1["Money"];
                    ViewBag.Login = _Name;
                    ViewBag.Email = _Email;
                    ViewBag.Money = _Money + "руб.";




                    int? Cost = Autentification.GetCost(Convert.ToInt32(_UserId));
                    int? Count = Autentification.GetCount(Convert.ToInt32(_UserId));

                    ViewBag.Cost = Cost;
                    ViewBag.Count = Count;

                    listbasket = Autentification.GetAllbasket(Convert.ToInt32(_UserId));
                    ViewBag.listbasket = listbasket;
                    return View("Account");
                }
            }
        }

        [HttpPost]
        public ActionResult Reg(string login, string email, string phone, string password, string passwrodCheck)
        {
            if (login == "" || email == "" || phone == "" || password == "" || passwrodCheck == "")
            {
                ViewBag.Error = "Заполните поля";
                return View("Registration");
            }
            else
            {
                if (IsValidEmail(email) == false)
                {
                    ViewBag.Error = "Неверная почта";
                    return View("Registration");
                }
                else
                {
                    if (password.Length < 6 || passwrodCheck.Length < 6)
                    {
                        ViewBag.Error = "Пароль слишком короткий";
                        return View("Registration");
                    }
                    else
                    {
                        Users user = Autentification.Get(login);
                        if (user == null)
                        {
                            if (GetHash(password) == GetHash(passwrodCheck))
                            {
                                int? UserId = Autentification.Create(login, email, phone, password, 0, 2);
                                if (UserId == null)
                                {
                                    ViewBag.Error = "Пользователь не создан";
                                    return View("Registration");
                                }
                                else
                                {
                                    Users user1 = Autentification.Get(login);

                                    HttpCookie cookie = new HttpCookie("Data");
                                    cookie.Expires = DateTime.Now.AddDays(-1);
                                    Response.Cookies.Add(cookie);

                                    HttpCookie Data = new HttpCookie("Data");

                                    //Add Cookies
                                    Data.Values.Add("UserId", user1.UserId.ToString());
                                    Data.Values.Add("Name", user1.Name.ToString());
                                    Data.Values.Add("Email", user1.Email.ToString());
                                    Data.Values.Add("Password", user1.Password.ToString());
                                    Data.Values.Add("PhoneNumber", user1.PhoneNumber.ToString());
                                    Data.Values.Add("Money", user1.Money.ToString());
                                    Data.Values.Add("RoleId", user1.RoleId.ToString());
                                    Data.Expires = DateTime.Now.AddMinutes(30);
                                    Response.Cookies.Add(Data);

                                    if (Data == null)
                                    {
                                        return View("Registration");
                                    }
                                    else
                                    {
                                        _UserId = Data["UserId"];
                                        _Name = Data["Name"];
                                        _Email = Data["Email"];
                                        _Money = Data["Money"];
                                        ViewBag.Login = _Name;
                                        ViewBag.Email = _Email;
                                        ViewBag.Money = _Money + "руб.";

                                        int? Cost = Autentification.GetCost(Convert.ToInt32(_UserId));
                                        int? Count = Autentification.GetCount(Convert.ToInt32(_UserId));

                                        ViewBag.Cost = Cost;
                                        ViewBag.Count = Count;

                                        listbasket = Autentification.GetAllbasket(Convert.ToInt32(_UserId));
                                        ViewBag.listbasket = listbasket;
                                        return View("Account");
                                    }
                                }
                            }
                            else
                            {
                                ViewBag.Error = "Пароли не совпадают";
                                return View("Registration");
                            }
                        }
                        else
                        {
                            ViewBag.Error = "Такой пользователь уже существует";
                            return View("Registration");
                        }

                    }
                }
            }
        }

        [HttpPost]
        public ActionResult Aut(string login, string password)
        {
            //Очищение cooki
            HttpCookie cookie = new HttpCookie("Data");
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);

            Users user = Autentification.Get(login);

            if (user != null)
            {
                if (Convert.ToString(GetHash(password)) == user.Password)
                {
                    if (user.RoleId == 1)
                    {
                        HttpCookie Data = new HttpCookie("Data");

                        Data.Values.Add("UserId", user.UserId.ToString());
                        Data.Values.Add("Name", user.Name.ToString());
                        Data.Values.Add("Email", user.Email.ToString());
                        Data.Values.Add("Password", user.Password.ToString());
                        Data.Values.Add("PhoneNumber", user.PhoneNumber.ToString());
                        Data.Values.Add("Money", user.Money.ToString());
                        Data.Values.Add("RoleId", user.RoleId.ToString());
                        Data.Expires = DateTime.Now.AddMinutes(30);
                        Response.Cookies.Add(Data);

                        listusers = Autentification.GetAllUsers(user.UserId);
                        ViewBag.listusers = listusers;
                        return View("AdminPanel");//Admin Panell
                    }
                    else
                    {

                        HttpCookie Data = new HttpCookie("Data");

                        Data.Values.Add("UserId", user.UserId.ToString());
                        Data.Values.Add("Name", user.Name.ToString());
                        Data.Values.Add("Email", user.Email.ToString());
                        Data.Values.Add("Password", user.Password.ToString());
                        Data.Values.Add("PhoneNumber", user.PhoneNumber.ToString());
                        Data.Values.Add("Money", user.Money.ToString());
                        Data.Values.Add("RoleId", user.RoleId.ToString());
                        Data.Expires = DateTime.Now.AddMinutes(30);
                        Response.Cookies.Add(Data);

                        _UserId = Data["UserId"];
                        _Name = Data["Name"];
                        _Email = Data["Email"];
                        _Money = Data["Money"];
                        ViewBag.Login = _Name;
                        ViewBag.Email = _Email;
                        ViewBag.Money = _Money + "руб.";

                        int? Cost = Autentification.GetCost(Convert.ToInt32(_UserId));
                        int? Count = Autentification.GetCount(Convert.ToInt32(_UserId));

                        ViewBag.Cost = Cost;
                        ViewBag.Count = Count;


                        listbasket = Autentification.GetAllbasket(Convert.ToInt32(_UserId));
                        ViewBag.listbasket = listbasket;

                        return View("Account");
                    }
                }
                else
                {
                    return View("Login");
                }
            }
            else
            {
                return View("Login");
            }
        }

        public ActionResult SortProducts(string Sort)
        {



            ViewBag.menu_item = "menu_item1";

            listproducts = Autentification.Sort(Sort);
            ViewBag.listproducts = listproducts;
            return View("Shop");
        }



        public ActionResult AddMoney(int money, int addMoney)//addMoney- Id пользователя
        {
            HttpCookie Data = new HttpCookie("Data");

            if (Autentification.AddMoney(money, addMoney) == true)
            {
                _UserId = Data["UserId"];
                _Name = Data["Name"];
                _Email = Data["Email"];
                _Money = Data["Money"];


                listusers = Autentification.GetAllUsers(Convert.ToInt32(_UserId));
                ViewBag.listusers = listusers;
                return View("AdminPanel");//Admin Panell
            }
            else
            {
                _UserId = Data["UserId"];
                _Name = Data["Name"];
                _Email = Data["Email"];
                _Money = Data["Money"];


                listusers = Autentification.GetAllUsers(Convert.ToInt32(_UserId));
                ViewBag.listusers = listusers;
                return View("AdminPanel");//Admin Panell
            }
        }

        public ActionResult Report()
        {
            listreport = Autentification.GetReports();
            ViewBag.listreport = listreport;
            return View("Report");
        }

        public ActionResult TakeReport()
        {
            listreport = Autentification.GetReports();
            ViewBag.listreport = listreport;
            return View("Report");
        }


        public ActionResult Delete(int delSelectedItem)
        {
            HttpCookie Data = new HttpCookie("Data");

            if (Autentification.Delete(delSelectedItem) == true)
            {
                _UserId = Data["UserId"];
                _Name = Data["Name"];
                _Email = Data["Email"];
                _Money = Data["Money"];

                listusers = Autentification.GetAllUsers(Convert.ToInt32(_UserId));
                ViewBag.listusers = listusers;
                return View("AdminPanel");//Admin Panell
            }
            else
            {
                _UserId = Data["UserId"];
                _Name = Data["Name"];
                _Email = Data["Email"];
                _Money = Data["Money"];

                listusers = Autentification.GetAllUsers(Convert.ToInt32(_UserId));
                ViewBag.listusers = listusers;
                return View("AdminPanel");//Admin Panell
            }
        }
    }
}
