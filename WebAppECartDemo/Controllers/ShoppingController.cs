using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppECartDemo.Models;
using WebAppECartDemo.ViewModel;

namespace WebAppECartDemo.Controllers
{
    public class ShoppingController : Controller
    {
        private ECartDBEntities db;
        private List<ShoppingCartModel> listOfShoppingCartModel;
        public ShoppingController()
        {
            db = new ECartDBEntities();
            listOfShoppingCartModel=new List<ShoppingCartModel>();
        }
        // GET: Shopping
        public ActionResult Index()
        {
            IEnumerable<ShoppingViewModel> listOfShoppingViewModel = (from objItem in db.Items
                                                                      join
                                                                      objcat in db.Categories
                                                                      on objItem.CategoryId equals objcat.CategoryId
                                                                      select new ShoppingViewModel()
                                                                      {
                                                                          ItemId = objItem.ItemId,
                                                                          ImagePath=objItem.ItemPath,
                                                                          ItemName=objItem.ItemName,
                                                                          Description=objItem.Description,
                                                                          ItemPrice= (decimal)objItem.ItemPrice,
                                                                          Category=objcat.CategoryName,
                                                                          ItemCode=objItem.ItemCode,
                                                                      }
                                                                      ).ToList();
            return View(listOfShoppingViewModel);
        }
        [HttpPost]
        public JsonResult Index(string ItemId)
        {
            
            ShoppingCartModel shoppingCartModel = new ShoppingCartModel();
            Item item = db.Items.Single(model => model.ItemId.ToString() == ItemId);
            if (Session["CartCounter"] != null)
            {
                listOfShoppingCartModel = Session["CartItem"] as List<ShoppingCartModel>;
            }
            if (listOfShoppingCartModel.Any(model => model.ItemId == ItemId))
            {
                shoppingCartModel = listOfShoppingCartModel.Single(model => model.ItemId.ToString() == ItemId);
                shoppingCartModel.Quantity = shoppingCartModel.Quantity+1;
                shoppingCartModel.Total = shoppingCartModel.Quantity * shoppingCartModel.UintPrice;
            }
            else
            {
                shoppingCartModel.ItemId= ItemId;
                shoppingCartModel.ImagePath = item.ItemPath;
                shoppingCartModel.ImageName= item.ItemName;
                shoppingCartModel.Quantity = 1;
                shoppingCartModel.Total= (decimal)item.ItemPrice;
                shoppingCartModel.UintPrice= (decimal)item.ItemPrice;
                listOfShoppingCartModel.Add(shoppingCartModel);

            }
            Session["CartCounter"] = listOfShoppingCartModel.Count;
            Session["CartItem"] = listOfShoppingCartModel;
            return Json(new { Success = true, Counter = listOfShoppingCartModel.Count }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ShoppingCart()
        {
            listOfShoppingCartModel= Session["CartItem"] as List<ShoppingCartModel>;
            return View(listOfShoppingCartModel);
        }
        [HttpPost]
        public ActionResult AddOrder()
        {
            int OrderId = 0;
            listOfShoppingCartModel = Session["CartItem"] as List<ShoppingCartModel>;
            Order order = new Order()
            {
                OrderDate = DateTime.Now,
                OrderNumber = string.Format("{0:ddmmyyyyHHmmsss}",DateTime.Now)
            };
            db.Orders.Add(order);
            OrderId = order.OrderId;
            foreach(var item in listOfShoppingCartModel)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Total = item.Total;
                orderDetail.ItemId = item.ItemId;
                orderDetail.OrderId=OrderId;
                orderDetail.Quantity = item.Quantity;
                orderDetail.UnitPrice = item.UintPrice;
                db.OrderDetails.Add(orderDetail);
                db.SaveChanges();
            }
            Session["CartItem"] = null;
            Session["CartCounter"] = null;
            return RedirectToAction("Index");
        }
    }
}