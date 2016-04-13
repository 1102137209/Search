using Resarch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public ActionResult update(String orderID)
        {
            //填入資料
            String CustomerName = "";
            String Employee = "";
            String ShipperName = "";
            String OrderMonth = "";
            String OrderDay = "";
            String OrderDate = "";
            String NeedMonth = "";
            String NeedDay = "";
            String NeedDate = "";
            String ShipMonth = "";
            String ShipDay = "";
            String ShipDate = "";
            String CustNameID = "";
            String EmpNameID = "";
            String ConpNameID = "";
            List<Orders> Request = db.Orders.Where(x =>x.OrderID.ToString() == orderID).ToList();
           
            Orders data = new Orders();

            foreach(var tmp in Request)
            {
                data.OrderID = tmp.OrderID;
                data.Freight = tmp.Freight;
                data.ShipCountry = tmp.ShipCountry;
                data.ShipCity = tmp.ShipCity;
                data.ShipRegion = tmp.ShipRegion;
                data.ShipPostalCode = tmp.ShipPostalCode;
                data.ShipAddress = tmp.ShipAddress;
                data.ShipName = tmp.ShipName;
                EmpNameID = tmp.EmployeeID.ToString();
                CustNameID = tmp.CustomerID.ToString();
                ConpNameID = tmp.ShipperID.ToString();
                CustomerName =tmp.Customers.CompanyName;
                Employee = tmp.Employees.FirstName;
                ShipperName = tmp.Shippers.CompanyName;


                OrderMonth = DateAddZero(tmp.OrderDate.Month.ToString());
                OrderDay = DateAddZero(tmp.OrderDate.Day.ToString());
                OrderDate = String.Format("{0}-{1}-{2}", tmp.OrderDate.Year, OrderMonth, OrderDay);

                NeedMonth = DateAddZero(tmp.RequiredDate.Month.ToString());
                NeedDay = DateAddZero(tmp.RequiredDate.Day.ToString());
                NeedDate = String.Format("{0}-{1}-{2}", tmp.RequiredDate.Year, NeedMonth, NeedDay);

                ShipMonth = DateAddZero(Convert.ToDateTime(tmp.ShippedDate).Month.ToString());
                ShipDay = DateAddZero(Convert.ToDateTime(tmp.ShippedDate).Day.ToString());
                ShipDate = String.Format("{0}-{1}-{2}", Convert.ToDateTime(tmp.ShippedDate).Year, ShipMonth, ShipDay);

            }
            ViewBag.orderID = data;
            ViewBag.CustomerName = CustomerName;
            ViewBag.Employee = Employee;
            ViewBag.ShipperName = ShipperName;
            ViewBag.OrderDate = OrderDate;
            ViewBag.NeedDate = NeedDate;
            ViewBag.ShipDate = ShipDate;
            ViewBag.CustNameID = CustNameID;
            ViewBag.EmpNameID = EmpNameID;
            ViewBag.ConpNameID = ConpNameID;



            List<Customers> CustName = db.Customers.Where(x=>x.CompanyName != CustomerName ).ToList();
            ViewBag.Custname = CustName;

            List<Employees> EmpName = db.Employees.Where(x=>x.FirstName != Employee).ToList();
            ViewBag.EmpName = EmpName;

            List<Shippers> ShipName = db.Shippers.Where(x=>x.CompanyName != ShipperName).ToList();
            ViewBag.ShipName = ShipName;

            //找到明細資料

            List<OrderDetails> OrderDetails = db.OrderDetails.Where(x => x.OrderID.ToString() == orderID).ToList();
            ViewBag.OrderDetails = OrderDetails;

            List<Products> Product = db.Products.ToList();
            ViewBag.Product = Product;

            //計算總金額
            double total = 0;
            foreach (var item in OrderDetails) {

                total +=total+Convert.ToDouble(item.UnitPrice) * item.Qty;

            }
            total = Math.Round(total + Convert.ToDouble(data.Freight));
            ViewBag.total = total;

            return View();
        }
        /// <summary>
        /// 日期加0
        /// </summary>
        /// <param name="OrderMonth"></param>
        /// <returns></returns>
        public String DateAddZero(String OrderMonth)
        {
            StringBuilder a = new StringBuilder();

            if(OrderMonth.Length < 2)
            {
                a.Append("0");
            }
            a.Append(OrderMonth);

            return a.ToString();
        }

   

        ///傳價錢過去
        public String Price(int priceID)
        {
            String changePrice = db.Products.Find(priceID).UnitPrice.ToString();
            return changePrice;
        }

        
        [HttpPost]
        public ActionResult update(FormCollection input)
        {
            String OrderID = input["OrderID"];
            String CustID = input["CustName"];
            String EmpID = input["EmpName"];
            String OrderDate = input["OrderDate"];
            String NeedDate = input["NeedDate"];
            String ShipDate = input["ShipDate"];
            String ConpID = input["ComName"];
            String Money = input["Money"];
            String ShipCountry = input["ShipCountry"];
            String ShipCity = input["ShipCity"];
            String ShipArea = input["ShipArea"];
            String AddressNum = input["AddressNum"];
            String ShipAddress = input["ShipAddress"];
            String ShipDesc = input["ShipDesc"];

            Orders data = db.Orders.Find(Convert.ToInt32(OrderID));

            data.CustomerID = Convert.ToInt32(CustID);
            data.EmployeeID = Convert.ToInt32(EmpID);
            data.OrderDate = Convert.ToDateTime(OrderDate);
            data.RequiredDate = Convert.ToDateTime(NeedDate);
            data.ShippedDate = Convert.ToDateTime(ShipDate);
            data.ShipperID = Convert.ToInt32(ConpID);
            data.Freight = Convert.ToDecimal(Money);
            data.ShipCountry = ShipCountry;
            data.ShipCity = ShipCity;
            data.ShipRegion = ShipArea;
            data.ShipPostalCode = AddressNum;
            data.ShipAddress = ShipAddress;
            data.ShipName = ShipDesc;
            
            //知道商品新增幾條
            int count = 0;



            for (int i = 1; i < input.Count; i++)
            {

                if (input.AllKeys.Contains("productName[" + i + "]"))
                {

                    count++;

                }
            }

            //列出OrderID所有的資料
            List<OrderDetails> IDdata = db.OrderDetails.Where(x => x.OrderID == data.OrderID).ToList();


            int j = 0;
            foreach(var tmp in IDdata)
            {

                j++;
                //修改資料庫裡面的資料
                tmp.ProductID = Convert.ToInt32(input["productName[" + j + "]"]);
                tmp.UnitPrice = Convert.ToDecimal(input["price[" + j + "]"]);
                tmp.Qty = Convert.ToInt16(input["quanty[" + j + "]"]);

                data.OrderDetails.Add(tmp);
            }


            for (int i = j+1; i <= count; i++)
            {

                OrderDetails ProductDetails = new OrderDetails();

                ProductDetails.OrderID = data.OrderID;
                ProductDetails.ProductID = Convert.ToInt32(input["productName[" + i + "]"]);
                ProductDetails.UnitPrice = Convert.ToDecimal(input["price[" + i + "]"]);
                ProductDetails.Qty = Convert.ToInt16(input["quanty[" + i + "]"]);

                data.OrderDetails.Add(ProductDetails);

            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }


        public ActionResult add()
        {
            int OrderID = db.Orders.Select(x => x.OrderID).Max() + 1;
            ViewBag.OrderID = OrderID;

            List<Customers> CustName = db.Customers.ToList();
            ViewBag.Custname = CustName;

            List<Employees> EmpName = db.Employees.ToList();
            ViewBag.EmpName = EmpName;

            List<Shippers> ShipName = db.Shippers.ToList();
            ViewBag.ShipName = ShipName;

            List<Products> ProdName = db.Products.ToList();
            ViewBag.ProdName = ProdName;



            return View();
        }


      

        [HttpPost]
        public ActionResult add(FormCollection input)
        {
            //把修改的資料放進資料庫
            String OrderID = input["OrderID"];
            String CustID = input["CustName"];
            String EmpID = input["EmpName"];
            String OrderDate = input["OrderDate"];
            String NeedDate = input["NeedDate"];
            String ShipDate = input["ShipDate"];
            String ConpID = input["ComName"];
            String Money = input["Money"];
            String ShipCountry = input["ShipCountry"];
            String ShipCity = input["ShipCity"];
            String ShipArea = input["ShipArea"];
            String AddressNum = input["AddressNum"];
            String ShipAddress = input["ShipAddress"];
            String ShipDesc = input["ShipDesc"];

            Orders data = new Orders();


            //轉成資料庫的型態
            data.CustomerID = Convert.ToInt32(CustID);
            data.EmployeeID = Convert.ToInt32(EmpID);
            data.OrderDate = Convert.ToDateTime(OrderDate);
            data.RequiredDate = Convert.ToDateTime(NeedDate);
            data.ShippedDate = Convert.ToDateTime(ShipDate);
            data.ShipperID = Convert.ToInt32(ConpID);
            data.Freight = Convert.ToDecimal(Money);
            data.ShipCountry = ShipCountry;
            data.ShipCity = ShipCity;
            data.ShipRegion = ShipArea;
            data.ShipPostalCode = AddressNum;
            data.ShipAddress = ShipAddress;
            data.ShipName = ShipDesc;


            //把新增的資料傳進去
            db.Orders.Add(data);

            //找到商品資料有幾筆
            int count = 0;

            for(int i=1;i< input.Count; i++){

                if (input.AllKeys.Contains("productName[" + i + "]")) {

                                count++;

                }
            }

            for(int i = 1; i <= count; i++) {

                OrderDetails ProductDetails = new OrderDetails();

                ProductDetails.OrderID = data.OrderID;
                ProductDetails.ProductID = Convert.ToInt32(input["productName[" + i + "]"]);
                ProductDetails.UnitPrice = Convert.ToDecimal(input["price[" + i + "]"]);
                ProductDetails.Qty = Convert.ToInt16(input["quanty[" + i + "]"]);

                data.OrderDetails.Add(ProductDetails);

            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }


        //刪除
        public void Delete(int OrderID)
        {
            //找到OrderID
            Orders order = db.Orders.Find(OrderID);

            List<OrderDetails> orderDetail = db.OrderDetails.Where(x => x.OrderID == OrderID).ToList();


            db.Orders.Remove(order);
            db.OrderDetails.RemoveRange(orderDetail);
            db.SaveChanges();


        }
        
    }
}