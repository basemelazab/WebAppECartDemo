using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppECartDemo.Models;
using WebAppECartDemo.ViewModel;

namespace WebAppECartDemo.Controllers
{
    public class ItemController : Controller
    {
        private ECartDBEntities db;
        // GET: Item
        public ItemController()
        {
            db = new ECartDBEntities(); 
        }
        public ActionResult Index()
        {
            ItemViewModel objItemViewModel = new ItemViewModel();
            objItemViewModel.CategorySelectListItems=(from obiCat in db.Categories
                                                      select new SelectListItem()
                                                      {
                                                          Text=obiCat.CategoryName,
                                                          Value=obiCat.CategoryId.ToString(),
                                                          Selected=true
                                                      });
            return View(objItemViewModel);
        }
        [HttpPost]
        public JsonResult Index(ItemViewModel objItemViewModel)
        {
            string newImage = Guid.NewGuid() + Path.GetExtension(objItemViewModel.ImagePath.FileName);
            objItemViewModel.ImagePath.SaveAs(Server.MapPath("~/Images/" + newImage));
            Item item = new Item();
            item.ItemPath = "~/Images/" + newImage;
            item.CategoryId = objItemViewModel.CategoryId;
            item.Description = objItemViewModel.Description;
            item.ItemCode = objItemViewModel.ItemCode;
            item.ItemId=Guid.NewGuid();
            item.ItemName = objItemViewModel.ItemName;
            item.ItemPrice = objItemViewModel.ItemPrice;
            db.Items.Add(item);
            db.SaveChanges();

            return Json(new { success =true,Message="Item is added sucessfully."},JsonRequestBehavior.AllowGet);
        }
    }
}