using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using e_commerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Hosting;

namespace e_commerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment he;
        private Context dbContext;
        public HomeController(Context context, IHostingEnvironment e)
        {
            dbContext = context;
            he = e;
        }
        [HttpGet("/")]
        public ViewResult Index()
        {
            Login login = new Login();
            ViewBag.login = login;
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.User.Any(a => a.email == user.email))
                {
                    ModelState.AddModelError("email", "Email already exists!");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.password = Hasher.HashPassword(user, user.password);
                    dbContext.Add(user);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("UserID", user.UserId);
                    HttpContext.Session.SetString("Name", user.name);
                    return RedirectToAction ("Dashboard");
                }
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login_User(Login user)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.User.FirstOrDefault(u => u.email == user.Login_Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("email", "Invalid Email");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserID", userInDb.UserId);
                HttpContext.Session.SetString("Name", userInDb.name);
                var hasher = new PasswordHasher<Login>();
                var result = hasher.VerifyHashedPassword(user, userInDb.password, user.Login_Password);
                
                if(result == 0)
                {
                    ModelState.AddModelError("Login_Password", "Invalid Password");
                    return View("Index");
                }
                 
                else
                {
                    return RedirectToAction ("Dashboard");
                }
            }
            else
            {
                return View("Index");
            }
        }
        [HttpGet("logout")]
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Name = HttpContext.Session.GetString("Name");
            List<Product> products = dbContext.Product.OrderByDescending(p => p.ProductId).Take(5).ToList();
            ViewBag.products = products;
            List<Order> orders = dbContext.Order.OrderByDescending(o => o.OrderId).Include(c => c.user).Include(p => p.product).Take(5).ToList();
            ViewBag.orders = orders;
            List<User> users = dbContext.User.OrderByDescending(u => u.UserId).Take(5).ToList();
            ViewBag.users = users;
            return View("Dashboard");
        }

        [HttpGet("products")]
        public IActionResult Products()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            Product product = new Product();
            ViewBag.product = product;
            List<Product> allproducts = dbContext.Product.ToList();
            ViewBag.allproducts = allproducts;
            ViewBag.Id = HttpContext.Session.GetInt32("UserID");
            return View();
        }

        private bool isValidContentType(string contentType)
        {
            return contentType.Equals("image/png") || contentType.Equals("image/jpg") || contentType.Equals("image/jpeg");
        }

        private bool isValidContentLength(int contentLength)
        {
            return ((contentLength / 1024)/ 1024) < 1; // 1MB
        }

        [HttpPost("create_product")]
        public IActionResult Create_Product(Product product)
        {
            if(ModelState.IsValid)
            {
                if(!isValidContentType(product.ImageFile.ContentType))
                {
                    Product product1 = new Product();
                    ViewBag.product = product1;
                    List<Product> allproducts1 = dbContext.Product.ToList();
                    ViewBag.allproducts = allproducts1;
                    ViewBag.Id = HttpContext.Session.GetString("UserId");
                    ViewBag.Error = "Only JPG, JPEG or PNG";
                    return View("Products");
                }
                string filename = Path.Combine(he.WebRootPath,product.ImageFile.FileName);
                product.ImageFile.CopyTo(new FileStream(filename, FileMode.Create));
                string filepath = "/" + Path.GetFileName(product.ImageFile.FileName);
                Product newproduct = new Product 
                {
                    Name = product.Name,
                    Desc = product.Desc,
                    Quantity = product.Quantity,
                    UserId = product.UserId,
                    Image = filepath
                };
                dbContext.Add(newproduct);
                dbContext.SaveChanges();
                return RedirectToAction("Products");
            }
            Product product2 = new Product();
            ViewBag.product = product2;
            List<Product> allproducts = dbContext.Product.ToList();
            ViewBag.allproducts = allproducts;
            ViewBag.Id = HttpContext.Session.GetInt32("UserId");
            return View("Products");
        }


        // [HttpPost("create_product")]
        // public async Task<IActionResult> Create_Product(Product product)
        // {
        //     if(ModelState.IsValid)
        //     {
        //         if(!isValidContentType(product.ImageFile.ContentType))
        //         {
        //             Product product1 = new Product();
        //             ViewBag.product = product1;
        //             List<Product> allproducts1 = dbContext.Product.ToList();
        //             ViewBag.allproducts = allproducts1;
        //             ViewBag.Id = HttpContext.Session.GetString("UserId");
        //             ViewBag.Error = "Only JPG, JPEG or PNG";
        //             return View("Products");
        //         }
        //         Product newproduct = new Product 
        //         {
        //             Name = product.Name,
        //             Desc = product.Desc,
        //             Quantity = product.Quantity,
        //             UserId = product.UserId
        //         };

        //         var filePath = Path.GetTempFileName();
        //         using (var stream = new FileStream(filePath, FileMode.Create))
        //         {
        //             await product.ImageFile.CopyToAsync(stream);
        //             newproduct.Image = stream.ToString();
        //         }
        //         string ImageFileName = Path.GetFileName(product.ImageFile.FileName);
        //         // string FolderPath = Path.Combine(Server.MapPath("~/wwwroot/images"), ImageFileName);

        //         dbContext.Add(newproduct);
        //         dbContext.SaveChanges();
        //         return RedirectToAction("Products");
        //     }
        //     Product product2 = new Product();
        //     ViewBag.product = product2;
        //     List<Product> allproducts = dbContext.Product.ToList();
        //     ViewBag.allproducts = allproducts;
        //     ViewBag.Id = HttpContext.Session.GetInt32("UserID");
        //     return View("Products");
        // }

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            Order order = new Order();
            ViewBag.order = order;
            List<User> users = dbContext.User.ToList();
            ViewBag.users = users;
            List<Product> products = dbContext.Product.ToList();
            ViewBag.products = products;
            List<Order> orders = dbContext.Order.Include(c => c.user).Include(p => p.product).ToList();
            return View("Orders", orders);
        }

        [HttpPost("create_order")]
        public IActionResult Create_Order(Order order)
        {
            if(ModelState.IsValid)
            {
                dbContext.Order.Add(order);
                dbContext.SaveChanges();
                return RedirectToAction("Orders");
            }
            Order order1 = new Order();
            ViewBag.order = order1;
            List<User> users = dbContext.User.ToList();
            ViewBag.users = users;
            List<Product> products = dbContext.Product.ToList();
            ViewBag.products = products;
            List<Order> orders = dbContext.Order.Include(c => c.user).Include(p => p.product).ToList();
            return View("Orders", orders);
        }

        [HttpGet("customers")]
        public IActionResult Customers()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            List<User> customers = dbContext.User.ToList();
            return View(customers);
        }

        [HttpGet("settings")]
        public IActionResult Settings()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Id = HttpContext.Session.GetInt32("UserID");
            return View();
        }

        [HttpPost("update_settings")]
        public IActionResult Update_Settings(User user)
        {
            if(ModelState.IsValid)
            {
                User update_user = dbContext.User.SingleOrDefault(u => u.UserId == user.UserId);
                update_user.name = user.name;
                update_user.alias = user.alias;
                update_user.email = user.email;
                update_user.updated_at = DateTime.Now;
                if(dbContext.User.Any(a => a.email == user.email))
                {
                    ModelState.AddModelError("email", "Email already exists!");
                    return View("Settings");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    update_user.password = Hasher.HashPassword(update_user, update_user.password);
                }
                dbContext.SaveChanges();
                return RedirectToAction("Settings");
            }
            return View("Settings");
        }

        [HttpGet("display_product/{id}")]
        public IActionResult Display_Product(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            Product product = dbContext.Product.SingleOrDefault(p => p.ProductId == id);
            ViewBag.product = product;
            return View("Display_Product", product);
        }
    }
}
