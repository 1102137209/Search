using Resarch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Resarch.Controllers
{
    public class ResarchController : Controller
    {
        DB db = new DB();
        // GET: Resarch
        public ActionResult Index()
        {
            List<String> name=db.Employees.Select(x => x.FirstName).ToList();
            List<String> company = db.Shippers.Select(x => x.CompanyName).ToList();

            ViewBag.name = name;
            ViewBag.company = company;
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection search)
        {
            String OrderID = search["OrderID"];
            String CustName = search["CustName"];
            String emp = search["emp"];
            String company = search["company"];
            String OrderDate = search["OrderDate"];
            String ShipperDate = search["ShipperDate"];
            String NeedDate = search["NeedDate"];

            return RedirectToAction("search", new { OrderID, CustName, emp, company, OrderDate, ShipperDate, NeedDate });
        }
        public ActionResult search(String OrderID, String CustName, String emp, String company, String OrderDate, String ShipperDate, String NeedDate)
        {
            DateTime orderdate = new DateTime();
            orderdate = Convert.ToDateTime(OrderDate);
            DateTime shipdate = new DateTime();
            shipdate = Convert.ToDateTime(ShipperDate);
            DateTime requestdate = new DateTime();
            requestdate = Convert.ToDateTime(NeedDate);

            

            List<Orders> ID = db.Orders.Where(x =>
                (String.IsNullOrEmpty(OrderID) ? true : x.OrderID.ToString() == OrderID) && 
                (String.IsNullOrEmpty(CustName) ? true : x.Customers.CompanyName == CustName) &&
                (String.IsNullOrEmpty(emp) ? true : x.Employees.FirstName ==emp) &&
                (String.IsNullOrEmpty(company) ? true : x.Shippers.CompanyName == company) &&
                (String.IsNullOrEmpty(OrderDate) ? true : x.OrderDate == orderdate) &&
                (String.IsNullOrEmpty(ShipperDate) ? true : x.ShippedDate == shipdate) &&
                (String.IsNullOrEmpty(NeedDate) ? true : x.RequiredDate ==requestdate)
                ).ToList();
            ViewBag.ID = ID;

            return View();
        }

    }
}