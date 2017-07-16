using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EquipmentMVC.Models;
using Newtonsoft.Json;
namespace EquipmentMVC.Controllers
{
    public class BorrowController : Controller
    {
        //
        // GET: /Borrow/
        EquipmentDBEntities db = new EquipmentDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Return()
        {
            var BID = Request["BID"].ToString();
            int id = int.Parse(BID);
            UpReturn(id);
            return View();
        }

        public ActionResult SelBorrow()
        {
            //GetList();
            return View();
        }

        /// <summary>
        /// 查询借还功能
        /// </summary>
        /// <returns></returns>
        public ActionResult DropList1()
        {
            List<EType> et = db.EType.ToList();
            var obj = (from e in et select new { ETypeID = e.ETypeID, EType1 = e.EType1 }).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetList(string name, string type, string BDate)
        {
            //List<BorrowAndReturn> bar = db.BorrowAndReturn.ToList();
            //List<Equipment> ept = db.Equipment.ToList();
            //List<EType> et = db.EType.ToList();
            //List<Brand> brand = db.Brand.ToList();
            //if (name != null && name != "")
            //{
            //    bar = db.BorrowAndReturn.Where(b => b.Equipment.EName.Contains(name)).ToList();
            //}

            //if (type != null && type != "")
            //{
            //    bar = db.BorrowAndReturn.Where(b => b.EType1.EType1.Contains(type)).ToList();
            //}

            //if (EndDate != null && EndDate != "")
            //{
            //    DateTime time = DateTime.Parse(EndDate);
            //    bar = db.BorrowAndReturn.Where(b => b.BDate >= time).ToList();
            //}

            //if (BeginDate != null && BeginDate != "")
            //{
            //    DateTime time = DateTime.Parse(BeginDate);
            //    bar = db.BorrowAndReturn.Where(b => b.BDate > time).ToList();
            //}
           
            var bar = db.BorrowAndReturn.ToList();
            var ept = db.Equipment.ToList();
            var et = db.EType.ToList();
            var brand = db.Brand.ToList();
            //模糊查询
            //根据设备名称查询
            if (!string.IsNullOrWhiteSpace(name))
            {
                bar = db.BorrowAndReturn.Where(b => b.Equipment.EName.Contains(name)).ToList();
            }
            //根据设备类型查询
            if (!string.IsNullOrWhiteSpace(type))
            {
                bar = db.BorrowAndReturn.Where(b => b.Equipment.EType.EType1.Contains(type)).ToList();
            }
            //根据日期查询
            if (!string.IsNullOrWhiteSpace(BDate))
            {
                DateTime STime = DateTime.Parse(BDate);
                bar = db.BorrowAndReturn.Where(b => b.BDate == STime).ToList();
            }
            ////根据日期查询
            //if (!string.IsNullOrWhiteSpace(RDate))
            //{
            //    DateTime STime = DateTime.Parse(RDate);
            //    bar = db.BorrowAndReturn.Where(b => b.RDate < STime).ToList();
            //}
            var obj = new
            {
                total = bar.Count(),
                rows = (from b in bar
                        join ep in ept on b.EID equals ep.EID
                        join e in et on ep.ETypeID equals e.ETypeID
                        join bd in brand on b.BBrandID equals bd.BID
                        where b.EStateID == 1
                        select new
                        {
                            BID = b.BID,
                            EName = ep.EName,
                            EType = e.EType1,
                            BrandName = bd.BrandName,
                            BPMan = b.BPMan,
                            BDate = b.BDate,
                            BHour = b.BHour
                        }).ToArray()
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加借还功能
        /// </summary>
        /// <returns></returns>

        //根据设备示相应的设备图片
        public ActionResult DeviceImg(int DID)
        {
            List<Equipment> DeviceList = db.Equipment.Where(d => d.EID == DID).ToList();
            string obj = (from d in DeviceList select d.EPic).First().ToString();
            return Content(obj);
        }

        public ActionResult DropName()
        {
            List<Equipment> eqt = db.Equipment.ToList();
            var obj = (from e in eqt select new { EName = e.EName, EID = e.EID }).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DropList()
        {
            List<EType> et = db.EType.ToList();
            var obj = (from e in et select new { ETypeID = e.ETypeID, EType1 = e.EType1 }).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DropBrand()
        {
            List<Brand> brand = db.Brand.ToList();
            var obj = (from b in brand select new { BrandID = b.BID, BrandName = b.BrandName }).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DorpType(string Name)
        {
            List<Equipment> eqt = db.Equipment.ToList();
            List<EType> et = db.EType.ToList();
            var obj = (from e in et
                       join eq in eqt on e.ETypeID equals eq.ETypeID
                       where eq.EName == Name
                       select new { ETypeID = e.ETypeID, EType1 = e.EType1 }).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DropNameID(int id)
        {
            List<Equipment> eqt = db.Equipment.ToList();
            List<EType> et = db.EType.ToList();
            var obj = (from ep in eqt
                       join e in et on ep.ETypeID equals e.ETypeID
                       where e.ETypeID == id
                       select new { EName = ep.EName, EID = ep.EID }).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ADDEmq(BorrowAndReturn borrow)
        {
            borrow.EStateID = 1;
            db.BorrowAndReturn.Add(borrow);
            if (db.SaveChanges() > 0)
            {
                Response.Write("<script>alert('添加成功')</script>");
                return RedirectToAction("SelBorrow");
            }
            else
            {
                Response.Write("<script>alert('添加失败')</script>");
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// 归还管理
        /// </summary>
        /// <returns></returns>

        //public ActionResult Load()
        //{
        //    var BID = Request["BID"].ToString();
        //    UpReturn(int.Parse(BID));
        //    return View();
        //}


        public ActionResult UpReturn(int id)
        {
            List<BorrowAndReturn> bar = db.BorrowAndReturn.ToList();
            List<Equipment> ept = db.Equipment.ToList();
            List<EType> et = db.EType.ToList();
            List<Brand> brand = db.Brand.ToList();
            var obj = (from b in bar
                       join ep in ept on b.EID equals ep.EID
                       join e in et on ep.ETypeID equals e.ETypeID
                       join bd in brand on b.BBrandID equals bd.BID
                       where b.BID == id
                       select new
                       {
                           BID = b.BID,
                           EID = ep.EName,
                           EType = e.EType1,
                           BBrandID = bd.BrandName,
                           BPMan = b.BPMan,
                           BDate = b.BDate,
                           BHour = b.BHour,
                           EPic = ep.EPic
                       }).ToList();
            ViewBag.BID = obj.ElementAt(0).BID;
            ViewBag.EID = obj.ElementAt(0).EID;
            ViewBag.EType = obj.ElementAt(0).EType;
            ViewBag.BBrandID = obj.ElementAt(0).BBrandID;
            ViewBag.BPMan = obj.ElementAt(0).BPMan;
            ViewBag.BDate = obj.ElementAt(0).BDate;
            ViewBag.BHour = obj.ElementAt(0).BHour;
            ViewBag.EPic = obj.ElementAt(0).EPic;
            return View();
        }

        public ActionResult UpEmp(DateTime RDate)
        {
            int id = int.Parse(Request["BID"]);
            BorrowAndReturn bt = db.BorrowAndReturn.Find(id);
            bt.EStateID = 2;
            bt.RDate = RDate;
            if (db.SaveChanges() > 0)
            {
                return RedirectToAction("SelBorrow");
            }
            else
            {
                return RedirectToAction("Return");
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
