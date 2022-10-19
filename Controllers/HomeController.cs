using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChugenTocc.Models;
using Newtonsoft.Json;

namespace ChugenTocc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            TOCCEntities1 DB = new TOCCEntities1();
            List<EmployeeToccTable> data = DB.EmployeeToccTable.ToList();





            Session.Remove("Account");
            ViewBag.list = data;
            return View();
        }


        public ActionResult ORInformation()
        {

            return View();
        }

        public ActionResult ORInformationClient()
        {

            return View();
        }


        public ActionResult HRMenu()
        {
            Session.Remove("InsertAlert");
            return View();
        }

        public ActionResult EditDeaprt()
        {


            return View();
        }

        public ActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminCheck(String Account, String Password)
        {
            HRSQLEntities DB = new HRSQLEntities();
            int check = DB.HRAdmin
                .Where(x => x.Account == Account && x.Password == Password)
                .OrderBy(x => x.@int).Count();
            if (check > 0)
            {
                String AdimnData = "";
                var Admin = from A in DB.HRAdmin
                            where A.Account == Account
                            select A;
                foreach (var b in Admin)
                {
                    AdimnData = b.Name;
                }

                System.Web.HttpContext.Current.Session["AdminLogin"] = AdimnData;
                return RedirectToAction("HRMenu");
            }
            else
            {


                return RedirectToAction("AdminLogin");
            }
        }
        public ActionResult EdiEmployee()
        {
            if (Session["AdminLogin"] != null)
            {

                return View();
            }
            else

                return RedirectToAction("AdminLogin");
        }
        [HttpPost]
        public ActionResult InsertEmployeeAcount(EmployeeAcount employeeAcount)
        {
            try
            {
                HRSQLEntities DB = new HRSQLEntities();
                var CheckEmpId = DB.EmployeeAcount
                    .Where(x => x.EmpID == employeeAcount.EmpID)
                    .OrderBy(x => x.@int).Count();
                if (CheckEmpId > 0)
                {
                    System.Web.HttpContext.Current.Session["InsertAlert"] = "員工編號以重複!!";
                    return RedirectToAction("EdiEmployee");
                }
                else
                {
                    DB.EmployeeAcount.Add(employeeAcount);
                    DB.SaveChanges();
                    System.Web.HttpContext.Current.Session["InsertAlert"] = "Insert Sucess";



                    var newHouers = new EmpHours
                    {

                        EmpID = employeeAcount.EmpID,
                        EmpName = employeeAcount.EmpName,
                        Rhours = 0,
                    };
                    DB.EmpHours.Add(newHouers);
                    DB.SaveChanges();

                    return RedirectToAction("EdiEmployee");
                }
            }
            catch
            {
                System.Web.HttpContext.Current.Session["InsertAlert"] = "欄位不可為空值";
                return RedirectToAction("EdiEmployee");
            }
        }

        public ActionResult RemovieSession()
        {
            Session.Remove("AdminLogin");
            return RedirectToAction("HRMenu");
        }


        public ActionResult EmployeeDateData()
        {
            if (Session["AdminLogin"] != null)
            {

                return View();
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }

        }


        [HttpPost]
        public ActionResult CheckEmployeeID(String CheckEmployeeID)
        {
            HRSQLEntities db = new HRSQLEntities();
            var SelectEmp = from E in db.EmployeeAcount
                            join H in db.EmpHours on E.EmpID equals H.EmpID
                            where E.EmpID == CheckEmployeeID
                            select new { E.Depart, E.EmpID, E.EmpName, H.Rhours };
            int Check = SelectEmp.Count();
            if (Check > 0)
            {
                return Json(SelectEmp);
            }
            else
            {
                SortedList<string, string> ErrorAlert = new SortedList<string, string>();
                ErrorAlert.Add("EmpName", "Error");
                ErrorAlert.Add("EmpName2", "Error2");
                string jsonStr = JsonConvert.SerializeObject(ErrorAlert);

                return Json(new[] { ErrorAlert }, "text/html", JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public ActionResult PostHours(string PostCrePeople, string PostId, string PostName, string PostDepart, int PostHistorHours, string PostReason, string PostReasonContext, int PostReasonHours)
        {
            String CreateDate = DateTime.Now.ToShortDateString();
            HRSQLEntities DB = new HRSQLEntities();
            if (PostReason == "加班")
            {


                var newHistroy = new EmployeeHistory
                {

                    Depart = PostDepart,
                    EmpID = PostId,
                    EmpName = PostName,
                    CreateDate = CreateDate,
                    CreatePeople = PostCrePeople,
                    Reason = PostReason,
                    ReasonContext = PostReasonContext,
                    ReasonHours = PostReasonHours,
                    HistoryHours = PostHistorHours,
                    TotoalHours = PostHistorHours + PostReasonHours,

                };
                DB.EmployeeHistory.Add(newHistroy);
                DB.SaveChanges();

                var UpHours = from UH in DB.EmpHours
                              where UH.EmpID == PostId
                              select UH;
                foreach (var D in UpHours)
                {
                    D.Rhours = D.Rhours + PostReasonHours;
                }
                DB.SaveChanges();



            }
            else if (PostReason == "公假")
            {
                var newHistroy2 = new EmployeeHistory
                {

                    Depart = PostDepart,
                    EmpID = PostId,
                    EmpName = PostName,
                    CreateDate = CreateDate,
                    CreatePeople = PostCrePeople,
                    Reason = PostReason,
                    ReasonContext = PostReasonContext,
                    ReasonHours = 0,
                    HistoryHours = PostHistorHours,
                    TotoalHours = PostHistorHours + 0,

                };
                DB.EmployeeHistory.Add(newHistroy2);
                DB.SaveChanges();

            }
            else
            {
                var newHistroy3 = new EmployeeHistory
                {

                    Depart = PostDepart,
                    EmpID = PostId,
                    EmpName = PostName,
                    CreateDate = CreateDate,
                    CreatePeople = PostCrePeople,
                    Reason = PostReason,
                    ReasonContext = PostReasonContext,
                    ReasonHours = -PostReasonHours,
                    HistoryHours = PostHistorHours,
                    TotoalHours = PostHistorHours - PostReasonHours,

                };
                DB.EmployeeHistory.Add(newHistroy3);
                DB.SaveChanges();

                var UpHours = from UH in DB.EmpHours
                              where UH.EmpID == PostId
                              select UH;
                foreach (var D in UpHours)
                {
                    D.Rhours = D.Rhours - PostReasonHours;
                }
                DB.SaveChanges();
            }

            var UpdateEmp = from E in DB.EmployeeAcount
                            join H in DB.EmpHours on E.EmpID equals H.EmpID
                            where E.EmpID == PostId
                            select new { E.Depart, E.EmpID, E.EmpName, H.Rhours };

            return Json(UpdateEmp);

        }

        public ActionResult SelectHistory()
        {
            if (Session["AdminLogin"] != null)
            {

                return View();
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }
        }

        [HttpPost]
        public ActionResult HistoryData()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SelectDeaprt(string test)
        {


            HRSQLEntities db = new HRSQLEntities();
            var SelectD = from SD in db.HRDepartment
                          select SD;
            return Json(SelectD);
        }

        [HttpPost]
        public ActionResult SelectOpEmployee(String Dept)
        {
            HRSQLEntities DB = new HRSQLEntities();
            var CallBack = from C in DB.EmployeeAcount
                           where C.Depart == Dept
                           select C;
            return Json(CallBack);
        }

        [HttpPost]
        public ActionResult HistoryDataBack(String Employee)
        {
            HRSQLEntities DB = new HRSQLEntities();
            var CallBackH = from CH in DB.EmployeeHistory
                            where CH.EmpName == Employee
                            select CH;
            return Json(CallBackH);
        }
        [HttpPost]
        public ActionResult HistoryDataBackM(String Employee, String Date)
        {
            HRSQLEntities DB = new HRSQLEntities();
            var CallBackH = from CH in DB.EmployeeHistory
                            where CH.CreateDate.Contains(Date) && CH.EmpName == Employee
                            select CH;
            return Json(CallBackH);
        }



        [HttpPost]
        public ActionResult InsertDeaprt(HRDepartment hR)
        {
            HRSQLEntities DB = new HRSQLEntities();
            DB.HRDepartment.Add(hR);
            DB.SaveChanges();
            return RedirectToAction("EditDeaprt");
        }

        [HttpPost]
        public ActionResult DataDeaprt(String test)
        {
            HRSQLEntities DB = new HRSQLEntities();
            var Data = from D in DB.HRDepartment
                       select D;

            return Json(Data);
        }

        [HttpPost]
        public ActionResult DeDepart(String DeleteId)
        {
            int id = Int32.Parse(DeleteId);
            HRSQLEntities DB = new HRSQLEntities();
            var DePa = DB.HRDepartment.Find(id);
            DB.HRDepartment.Remove(DePa);
            DB.SaveChanges();

            return Json(DePa);
        }


        [HttpPost]
        public ActionResult ORInformationClientPost(string test)
        {
            TOCCEntities1 DB = new TOCCEntities1();
            var Data = from D in DB.ORInformation
                       select D;

            return Json(Data);
        }

        [HttpPost]
        public ActionResult ORPaPost(ORInformation oRInformation)
        {

            TOCCEntities1 DB = new TOCCEntities1();
            DB.ORInformation.Add(oRInformation);
            DB.SaveChanges();


            return RedirectToAction("ORInformation");
        }


        [HttpPost]
        public ActionResult ViewOrPa(string test)
        {

            TOCCEntities1 DB = new TOCCEntities1();
            var OrData = from Firstsucess in DB.ORInformation
                         select Firstsucess;

            return Json(OrData);
        }


        [HttpPost]
        public ActionResult DeOrInfo(string DeleteId)
        {
            int id = Int32.Parse(DeleteId);
            TOCCEntities1 DB = new TOCCEntities1();
            var PatientDe = DB.ORInformation.Find(id);
            DB.ORInformation.Remove(PatientDe);
            DB.SaveChanges();

            return RedirectToAction("ORInformation");
        }


        [HttpPost]
        public ActionResult EditOrInfo(int EditId)
        {
            TOCCEntities1 DB = new TOCCEntities1();
            var select = from patient in DB.ORInformation
                         where patient.id == EditId
                         select patient;
            System.Web.HttpContext.Current.Session["PatientId"] = EditId;
            return Json(select);
        }

        public ActionResult EditOrInfo()
        {
            int id = (int)Session["PatientId"];
            TOCCEntities1 DB = new TOCCEntities1();
            //var editpa = from selpatient in DB.Patient  陣列結果
            //             where selpatient.PatientId == id
            //             select selpatient;

            Session.Remove("PatientId");
            var update = DB.ORInformation.Find(id);
            return View(update);
        }

        [HttpPost]
        public ActionResult UpdatePatient(ORInformation patient)
        {
            TOCCEntities1 DB = new TOCCEntities1();
            var PatientUpdate = DB.ORInformation.Find(patient.id);
            DB.ORInformation.Remove(PatientUpdate);
            DB.ORInformation.Add(patient);
            DB.SaveChanges();
            Session.Remove("PatientId");
            return RedirectToAction("ORInformation");
        }

        [HttpPost]
        public ActionResult getdata()
        {
            TOCCEntities1 DB = new TOCCEntities1();
            var Date = DateTime.Now.ToString("yyyy-MM-dd");
            var query = DB.EmployeeToccTable.Where(x => x.Time.Contains(Date)).OrderBy(x => x.Id);


            object aa = new { a = "4", name = "phone" };
            object bb = new { a = "1", name = "camare" };
            object cc = new { a = "2", name = "computer" };
            object dd = new { a = "5", name = "sdcard" };
            object ee = new { a = "6", name = "tv" };
            object ff = new { a = "3", name = "monnitor" };


            string[] s = new string[6];
            //System.Collections.ArrayList myAL = new ArrayList(); 不同方法的array  arraylist不限型別與長度 [] ：特定型別，固定長度的陣列，長度需事先宣告 List：特定型別，不固定長度的陣列。Array：不特定型別，固定長度的陣列，長度需事先宣告。ArrayList：不特定型別，不固定長度的陣列
            //myAL.Add(dd);
            //s[0] = new object { test3 = "OK", test4 = "OKOK" };
            String test = "123";

            return Json(query);
        }
        public ActionResult login()
        {

            return View();
        }

        public ActionResult loginout()
        {

            return RedirectToAction("index");
        }


        [HttpPost]
        public ActionResult logincheck(String login, String password)
        {
            String Account = "Admin";
            String Password = "2813136";
            if (login == Account && password == Password)
            {
                System.Web.HttpContext.Current.Session["Account"] = Account;
                return RedirectToAction("index2");
            }
            else
            {
                return RedirectToAction("index");
            }

        }

        public ActionResult index2()
        {

            if (Session["Account"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("index");
            }


        }


        [HttpPost]
        public ActionResult Creat(EmployeeToccTable TOCC)
        {
            TOCCEntities1 DB = new TOCCEntities1();
            DB.EmployeeToccTable.Add(TOCC);
            DB.SaveChanges();

            return RedirectToAction("Index");
        }



        public ActionResult BoneView()
        {
            if (Session["loadPa"] != null)
            {
                string LoadPa = (string)Session["loadPa"];

                return View();
            }
            else
            {
                return RedirectToAction("SelectXpa");

            }
        }

        [HttpPost]
        public ActionResult ImgUpload(FormCollection collection, HttpPostedFileBase[] files)
        {
            string PaNumber = collection["PaNumber"];
            string PaXrayer = collection["PaXrayer"];
            string PaDate = collection["PaDate"];
            string PaName = collection["PaName"];

            if (files[0] == null)
            {
                System.Web.HttpContext.Current.Session["ImgAlert"] = "上傳失敗-不可為空";
            }
            else
            {
                if (files.Count() > 0)
                {
                    foreach (var uploadFile in files)
                    {

                        string ImgName = uploadFile.FileName;
                        string Router = (@"C:\Users\5F04\Desktop\ChugenTocc\ChugenTocc\ChugenTocc\img\" + uploadFile.FileName);
                        uploadFile.SaveAs(@"C:\Users\5F04\Desktop\ChugenTocc\ChugenTocc\ChugenTocc\img\" + uploadFile.FileName);
                        System.Web.HttpContext.Current.Session["ImgAlert"] = "上傳成功";

                        BoneXrayEntities DB = new BoneXrayEntities();
                        var newRouter = new Img
                        {
                            PaID = PaNumber,
                            ImagePName = ImgName,
                            ImgCreateDate = PaDate,
                            CreatePeople = PaXrayer,
                            ImgRouter = Router,


                        };
                        DB.Img.Add(newRouter);
                        DB.SaveChanges();


                    }
                    return RedirectToAction("BoneView");
                    //HttpPostedFileBase f = Request.Files["file1"];
                    //string Error = f.FileName;
                    //if (Error == "")
                    //{
                    //    System.Web.HttpContext.Current.Session["ImgAlert"] = "上傳失敗";
                    //    return RedirectToAction("BoneView");
                    //}
                    //else
                    //{
                    //    f.SaveAs(@"C:\Users\5F04\Desktop\ChugenTocc\ChugenTocc\ChugenTocc\img\" + f.FileName);
                    //    System.Web.HttpContext.Current.Session["ImgAlert"] = "上傳成功";
                    //    return RedirectToAction("BoneView");
                    //}


                }
            }

            return RedirectToAction("BoneView");
        }

        public ActionResult SelectXpa()
        {
            Session.Remove("loadPa");

            return View();
        }

        [HttpPost]
        public ActionResult InsertNp(BonePa BonePa)
        {
            BoneXrayEntities DB = new BoneXrayEntities();
            DB.BonePa.Add(BonePa);
            DB.SaveChanges();

            return RedirectToAction("SelectXpa");
        }

        [HttpPost]
        public ActionResult PostPaID(string idcard)
        {

            BoneXrayEntities DB = new BoneXrayEntities();
            var queryCallBackP = from Pa in DB.BonePa
                                 where Pa.PaID.Contains(idcard) || Pa.PaName.Contains(idcard)
                                 select Pa;

            return Json(queryCallBackP);

        }



        [HttpPost]
        public ActionResult EditPatient(string Patient)
        {
            System.Web.HttpContext.Current.Session["PaIDSession"] = Patient;
            BoneXrayEntities DB = new BoneXrayEntities();
            var SelectPa = from SP in DB.BonePa
                           where SP.PaID == Patient
                           select SP;
            return Json(SelectPa);

        }


        public ActionResult EditPatient1()
        {

            if (Session["PaIDSession"] != null)
            {

                string SesID = (string)Session["PaIDSession"];
                Session.Remove("PaIDSession");
                BoneXrayEntities DB = new BoneXrayEntities();


                var test = from test1 in DB.BonePa
                           where test1.PaID == SesID
                           select test1;
                int testid = 0;
                foreach (var test2 in test)
                {
                    testid = test2.@int;
                }
                var update = DB.BonePa.Find(testid);
                return View(update);


            }
            else
            {
                return RedirectToAction("SelectXpa");
            }
        }

        [HttpPost]
        public ActionResult EditPaUp(BonePa bonePa)
        {

            BoneXrayEntities DB = new BoneXrayEntities();
            var UpdatePa = DB.BonePa.Find(bonePa.@int);
            DB.BonePa.Remove(UpdatePa);
            DB.BonePa.Add(bonePa);
            DB.SaveChanges();


            return RedirectToAction("SelectXpa");
        }
        [HttpPost]
        public ActionResult loadPatient(string loadId)
        {
            Session.Remove("loadPa");

            System.Web.HttpContext.Current.Session["loadPa"] = loadId;
            SortedList<string, string> SucessAlert = new SortedList<string, string>();
            SucessAlert.Add("Sucess", "Sucess");

            string jsonStr = JsonConvert.SerializeObject(SucessAlert);

            return Json(new[] { jsonStr }, "text/html", JsonRequestBehavior.AllowGet);


        }
        [HttpPost]
        public ActionResult PostIdBack(string PostId)
        {
            BoneXrayEntities DB = new BoneXrayEntities();
            var CallBackD = from cd in DB.BonePa
                            where cd.PaID == PostId
                            select cd;
            return Json(CallBackD);


        }


        [HttpPost]
        public ActionResult SelectXrPa(string PostId)
        {



            BoneXrayEntities DB = new BoneXrayEntities();
            var CallBackXray = from BX in DB.Img
                               join XP in DB.BonePa on BX.PaID equals XP.PaID
                               select new { BX.@int, BX.PaID, BX.ImagePName, BX.ImgCreateDate, XP.PaName };
            return Json(CallBackXray);


        }

        [HttpPost]
        public ActionResult LogBack(string PostDate)
        {



            BoneXrayEntities DB = new BoneXrayEntities();
            var LogBack = from LB in DB.Img
                          where LB.ImgCreateDate.Contains(PostDate)
                          select LB;
            return Json(LogBack);


        }




        public ActionResult DoctorView(string PostId)
        {

            return View();


        }

        public ActionResult EmployeeHistory(string EmployeePart, string EmployeeId)
        {

            return View();


        }
        [HttpPost]
        public ActionResult EmployeeHistory(string Dept)
        {
            HRSQLEntities DB = new HRSQLEntities();
            var EmpBack = from EmpS in DB.EmployeeHR
                          where EmpS.EmpDepart == Dept
                          select EmpS;
            return Json(EmpBack);



        }


        [HttpPost]
        public ActionResult SelectMEmployee(string Employee)
        {
            HRSQLEntities DB = new HRSQLEntities();
            string LDate = DateTime.Now.ToString("yyyy/MM/dd");
            string TDateY = LDate.Substring(0,5);
            //string TDateM = LDate.Substring(6,7);

            var EmpData = from SEPD in DB.Record
                          where SEPD.Name == Employee
                          select SEPD;
            return Json(EmpData);
        }


        public ActionResult EPaitent()
        {
  
            return View();
        }

        

        //[HttpPost]
        //public ActionResult SelectAllMonth(string Employee)
        //{
        //    HRSQLEntities DB = new HRSQLEntities();
        //    var AllEmp = from EmpALL in DB.
        //                where EmpM.EnrollID == UrId
        //                select EmpM;
        //    return Json(MDate);
        //}


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }



        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}