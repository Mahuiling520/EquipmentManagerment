using EquipmentMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EquipmentMVC.Controllers
{
    public class ServiceController : Controller
    {
        //
        // GET: /Service/

        EquipmentDBEntities db = new EquipmentDBEntities();
        public ActionResult Index()
        {
            return View();
        }

        //显示列表（显示页面）
        public ActionResult GetList(string EQName, string TypeName, string STDate, int page = 1, int rows = 10)
        {
            //类型表
            var et = db.EType.ToList();
            //设备表
            var eq = db.Equipment.ToList();
            //维修表
            var se = db.Service.ToList();
            //模糊查询
            //根据设备名称查询
            if (!string.IsNullOrWhiteSpace(EQName))
            {
                se = se.Where(r => r.Equipment.EName.Contains(EQName)).ToList();
            }
            //根据设备类型查询
            if (!string.IsNullOrWhiteSpace(TypeName))
            {
                se = se.Where(r => r.Equipment.EType.EType1.Contains(TypeName)).ToList();
            }
            //根据日期查询
            if (!string.IsNullOrWhiteSpace(STDate))
            {
                DateTime STime = DateTime.Parse(STDate);
                se = se.Where(r => r.SCauseDate == STime).ToList();
            }
            //分页
            int total = se.Count();
            if (total > 0)
            {
                if (page <= 1)
                {
                    se = se.Take(rows).ToList();
                }
                else
                {
                    se = se.Skip((page - 1) * rows).Take(rows).ToList();
                }
            }
            var obj = new
            {
                total = total,
                rows = (
                        from s in se
                        join e in eq on s.EID equals e.EID
                        join et1 in et on e.ETypeID equals et1.ETypeID
                        select new
                        {
                            SID = s.SID,
                            EName = e.EName,
                            EType = et1.EType1,
                            SAddress = s.SAddress,
                            SCauseDate = s.SCauseDate,
                            SPMan = s.SPMan,
                            SCause = s.SCause,
                            SCost = s.SCost,
                            SHour = s.SHour,
                            SDesc = s.SDesc,
                            Img = e.EPic,
                        }).ToArray()
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        //设备类型（显示/添加页面）
        public ActionResult selType()
        {
            List<EType> et = db.EType.ToList();
            var obj = (from e in et select new { ETypeID = e.ETypeID, EType1 = e.EType1 }).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        //设备名称（添加页面）
        public ActionResult selName(int? ETypeID)
        {
            List<Equipment> eq = db.Equipment.Where(e => e.ETypeID == ETypeID).ToList();
            var obj = (from e in eq select new { EID = e.EID, EName = e.EName }).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        //显示图片（添加页面）
        public ActionResult TypeImage(int? Eid)
        {
            List<Equipment> eq = db.Equipment.Where(e => e.EID == Eid).ToList();
            var obj = (from dp in eq select dp.EPic).FirstOrDefault();
            if (obj != null)
            {
                return Content(obj.ToString());
            }
            else
                return Content(null);
        }

        //添加（添加页面）
        public ActionResult AddRepair()
        {
            return View();
        }
        [HttpPost]
        public ActionResult add(Service se)
        {
            db.Service.Add(se);
            var id = int.Parse(Request["EID"]);
            Equipment e = db.Equipment.Find(id);
            e.EState = 2;
            if (db.SaveChanges() > 0)
            {
                return Content("Y");
            }
            else
            {
                return Content("N");
            }
        }

        //修改（显示页面）
        public ActionResult Update(int? SID, string SAddress, string SPMan, string SCause, DateTime SCauseDate, string SCost, decimal SHour, string SDesc)
        {
            Service se = db.Service.Find(SID);
            se.SAddress = SAddress;
            se.SPMan = SPMan;
            se.SCause = SCause;
            se.SCauseDate = SCauseDate;
            se.SCost = SCost;
            se.SHour = SHour;
            se.SDesc = SDesc;
            db.SaveChanges();
            var obj = new
            {
                success = "true",
                message = "OK"
            };
            return Json(obj, "text/plain", JsonRequestBehavior.AllowGet);
        }

        //删除（显示页面）
        //public ActionResult Delete(List<int?> SID)
        //{
        //    foreach (int i in SID)
        //    {
        //        Service se = db.Service.Find(SID);
        //        db.Service.Remove(se);
        //    }
        //    db.SaveChanges();
        //    return Content("success");
        //}

        //释放数据库上下文对象
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
