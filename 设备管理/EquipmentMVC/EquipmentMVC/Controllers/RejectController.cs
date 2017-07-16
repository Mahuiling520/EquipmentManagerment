using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EquipmentMVC.Models;

namespace EquipmentMVC.Controllers
{
    public class RejectController : Controller
    {
        //
        // GET: /Reject/

        EquipmentDBEntities db = new EquipmentDBEntities();
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetList(string EName, string EType1, string RDate)
        {
            //类型表
            var EType = db.EType.ToList();
            //设备表
            var Equipment = db.Equipment.ToList();
            var Reject = db.Reject.ToList();
            //模糊查询
            //根据设备名称查询
            if (!string.IsNullOrWhiteSpace(EName))
            {
                Reject = Reject.Where(r => r.Equipment.EName.Contains(EName)).ToList();
            }
            //根据设备类型查询
            if (!string.IsNullOrWhiteSpace(EType1))
            {
                Reject = Reject.Where(r => r.Equipment.EType.EType1.Contains(EType1)).ToList();
            }
            //根据日期查询
            if (!string.IsNullOrWhiteSpace(RDate))
            {
                DateTime STime = DateTime.Parse(RDate);
                Reject = Reject.Where(r => r.RDate == STime).ToList();
            }

            var obj = new
            {
                total = Reject.Count(),
                rows = (
                    from r in Reject
                    join e in Equipment on r.EID equals e.EID
                    join et in EType on e.ETypeID equals et.ETypeID
                    select new
                    {
                        EName = e.EName,
                        RDate = r.RDate.ToString(),
                        RCause = r.RCause,
                        RCost = r.RCost,
                        EType = et.EType1

                    }).ToArray()
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        //根据设备示相应的设备图片
        public ActionResult DeviceImg(int DID)
        {
            List<Equipment> DeviceList = db.Equipment.Where(d => d.EID == DID).ToList();
            string obj = (from d in DeviceList select d.EPic).First().ToString();
            return Content(obj);
        }

        //设备类型
        public ActionResult selType()
        {

            List<EType> et = db.EType.ToList();
            var obj = (from e in et select new { ETypeID = e.ETypeID, EType1 = e.EType1 }).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);

        }
        //设备名称
        public ActionResult selName()
        {

            List<Equipment> eq = db.Equipment.ToList();
            var obj = (from e in eq select new { EID = e.EID, EName = e.EName }).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);

        }
        //级联
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

        public ActionResult DropName()
        {
            List<Equipment> eqt = db.Equipment.ToList();
            var obj = (from e in eqt select new { EName = e.EName, EID = e.EID }).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        //报废
        public ActionResult LoginReject()
        {
            return View();
        }
        [HttpPost]
        public ActionResult add(Reject rej)
        {
            var id = int.Parse(Request["EID"]);
            Equipment e = db.Equipment.Find(id);
            e.EState = 3;
            var a = Request["RCost"].ToString();
            var b = Request["EType"].ToString();
            var c = Request["RCause"].ToString();
            rej.RCost = decimal.Parse(a);
            rej.RDate = DateTime.Now;
            rej.RCause = c;
            //rej.RID = 3;
            rej.EID = int.Parse(b);
            db.Reject.Add(rej);
            db.SaveChanges();
            var obj = new
            {
                success = "true",
                message = "OK"
            };
            return Json(obj, "text/plain", JsonRequestBehavior.AllowGet);
        }
        //释放数据库上下文对象
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
