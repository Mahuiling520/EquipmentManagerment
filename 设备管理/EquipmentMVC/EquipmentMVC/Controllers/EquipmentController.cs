using EquipmentMVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EquipmentMVC.Controllers
{
    public class EquipmentController : Controller
    {
        //
        // GET: /Equipment/

        EquipmentDBEntities db = new EquipmentDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        //查询设备
        public ActionResult QueryInfo(string EName, string EType1, string EMFD)
        {
            //IQueryable<EType> em = null;
            //IQueryable<Equipment> eq = null;
            //if (EName != null && ETName != null)
            //{
            //    eq = db.Equipment.Where(e => e.EName.Contains(EName)).AsQueryable(); ;
            //    em = db.EType.Where(t => t.EType1.Contains(ETName)).AsQueryable(); ;
            //}
            //else
            //{
            //    eq = db.Equipment.AsQueryable();
            //    em = db.EType.AsQueryable();
            //}
            //类型表
            var em = db.EType.ToList();
            //设备表
            var eq = db.Equipment.ToList();
            ////维修表
            //IQueryable<Service> se = db.Service.AsQueryable();
            //模糊查询
            //根据设备名称查询
            if (!string.IsNullOrWhiteSpace(EName))
            {
                eq = db.Equipment.Where(e => e.EName.Contains(EName)).ToList();
            }
            //根据设备类型查询
            if (!string.IsNullOrWhiteSpace(EType1))
            {
                em = db.EType.Where(b => b.EType1.Contains(EType1)).ToList();
            }
            //根据日期查询
            if (!string.IsNullOrWhiteSpace(EMFD))
            {
                DateTime STime = DateTime.Parse(EMFD);
                eq = db.Equipment.Where(r => r.EMFD == STime).ToList();
            }
            var obj = (from e in eq
                       join t in em on e.ETypeID equals t.ETypeID
                       select new
                       {
                           EID = e.EID,
                           EName = e.EName,
                           TypeName = t.EType1,
                           EModel = e.EModel,
                           EBrand = e.EBrand,
                           Serial = e.ENum,
                           EFactory = e.EFactory,
                           OutTime = e.EMFD,
                           BuyTime = DateTime.Now,
                           EState = e.EState,
                           ENum = e.ENum,
                           EMFD = e.EMFD,
                           EBD = e.EBD
                       }).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        //上传图片
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            if (file != null)
            {
                if (file.ContentLength == 0)
                {
                    return View();
                }
                else
                {
                    string extName = Path.GetExtension(file.FileName);
                    string imageFilter = ".jpg|.png|.gif|.ico";
                    if (imageFilter.Contains(extName.ToLower()))
                    {
                        string phyFilePath = Server.MapPath("~/images/") + file.FileName;
                        file.SaveAs(phyFilePath);
                        ViewBag.Info = "图片上传成功！";
                        return Content(string.Format("<script type='text/javascript'>alert('上传成功!');</script>", Url.Action("Index", "Equipment")));
                    }
                    else
                    {
                        ViewBag.Message = "抱歉，只能上传图片！";
                    }
                }
            }
            return View();
        }
        //添加
        public ActionResult AddE(Equipment e, string brand)
        {
            e.EState = Convert.ToInt32("1");
            e.EName = Request["EName"];
            e.ETypeID = Convert.ToInt32(Request["EType"]);
            e.EModel = Request["EModel"];
            e.ENum = Request["ENum"];
            e.EMFD = DateTime.Parse(Request["EMFD"]);
            e.EBrand = brand;
            e.EFactory = Request["EFactory"];
            e.EBD = DateTime.Parse(Request["EBD"]);
            db.Equipment.Add(e);
            if (db.SaveChanges() > 0)
            {
                return Content("Y");
            }
            else
                return Content("N");
        }
        public ActionResult UpdateE(int id)
        {
            Equipment e = db.Equipment.Find(id);
            e.EName = Request["EName"];
            e.ETypeID = Convert.ToInt32(Request["EType"]);
            e.EModel = Request["EModel"];
            e.ENum = Request["ENum"];
            e.EMFD = DateTime.Parse(Request["EMFD"]);
            e.EBrand = Request["EBrand"];
            e.EFactory = Request["EFactory"];
            e.EBD = DateTime.Parse(Request["EBD"]);
            if (db.SaveChanges() > 0)
            {
                return Content("Y");
            }
            else
                return Content("N");
        }
        //设备类型的下拉框
        public ActionResult ComTypeList()
        {
            List<EType> typetb = db.EType.ToList();
            var obj = (from d in typetb
                       select new
                       {
                           TypeID = d.ETypeID,
                           TypeName = d.EType1
                       }).ToArray();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TypeIndex()
        {
            return View();
        }

        public ActionResult GetTypeTB()
        {
            var TypeTb = db.EType.ToList();
            var info = from u in TypeTb
                       select new
                       {
                           ETypeID = u.ETypeID,
                           EType = u.EType1
                       };
            return Json(info, JsonRequestBehavior.AllowGet);
        }
        //添加设备类型
        public ActionResult AddType(string type)
        {
            EType etype = new EType();
            etype.EType1 = type;
            db.EType.Add(etype);
            if (db.SaveChanges() > 0)
            {
                return Content("Y");
            }
            else
            {
                return Content("N");
            }
        }
        //修改设备类型
        public ActionResult UpdateType(int? id, string TypeName)
        {
            var type = db.EType.Find(id);
            type.EType1 = TypeName;
            if (db.SaveChanges() > 0)
            {
                return Content("Y");
            }
            else
                return Content("N");
        }
        //删除设备类型
        public ActionResult DeleteType(int? id)
        {
            var type = db.EType.Find(id);
            db.EType.Remove(type);
            db.SaveChanges();
            return Content("success");
        }

    }
}
