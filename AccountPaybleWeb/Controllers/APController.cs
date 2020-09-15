using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using AccountPaybleWeb.Manager;
using AccountPaybleWeb.Models;
using Newtonsoft.Json;
using static AccountPaybleWeb.Manager.ViewModelClass;

namespace AccountPaybleWeb.Controllers
{
    public class APController : Controller
    {
        // GET: AP
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Home(SearchViewModel SearchCriteria,string Excel)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            try
            {
                var user = (LoginModel)Session["User"];
                List<Tbl_AP_PODetail> polist = new List<Tbl_AP_PODetail>();
                List<Tbl_AP_LineItemDetail> LineList = new List<Tbl_AP_LineItemDetail>();
                List<Tbl_AP_PODetail> approvedlist = new List<Tbl_AP_PODetail>();
                SearchInvoiceClass.GetSearchList(SearchCriteria, ref polist, ref LineList, ref approvedlist, user.UserName.Trim());
                ViewBag.vendors = MVCManager.GetVendorList(user.UserName.Trim());
                SearchCriteria.Status = Constants.GetStat(SearchCriteria.Status);
                ViewBag.SearchDetail = SearchCriteria;
                ViewBag.polist = polist;
                ViewBag.LineList = LineList;
                ViewBag.ApproveList = approvedlist;
                if (!string.IsNullOrEmpty(Excel))
                {
                    SearchInvoiceClass.GetSearchListForExcel(SearchCriteria, ref polist, ref LineList, ref approvedlist, user.UserName);
                    ExportExcel(polist,LineList, approvedlist, Convert.ToInt32(SearchCriteria.tab));
                    SearchCriteria.Status= Constants.GetStat(SearchCriteria.Status);
                }

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Someting went wrong.";
            }
            return View();
        }

        public void ExportExcel(List<Tbl_AP_PODetail> po,List<Tbl_AP_LineItemDetail> line,List<Tbl_AP_PODetail> Approve,int tab)
        {
            var sb = new StringBuilder();
            string filename = string.Empty;
            try
            {               
                switch (tab)
                {
                    case 1:
                        filename = "ApHome" + DateTime.Now.ToString("ddmmyyyyhhmmss");
                        FormatExcelForPo(po, filename);
                        break;
                    case 2:
                        filename = "ApError" + DateTime.Now.ToString("ddmmyyyyhhmmss");
                        FormattoExcelLineItem(line, filename);
                        break;
                    default:
                        filename = "ApApprove" + DateTime.Now.ToString("ddmmyyyyhhmmss");
                        FormatExcelForAppprove(Approve, filename);
                        break;
                }
            }
            catch (Exception ex)
            {

            }

        }

        #region---------------------------------------------------------------------------------
        public ActionResult RejectInvoice(string ID)
        {
            MVCManager.RejectInvoice(Convert.ToInt64(ID));
            return RedirectToAction("Home");
        }
        public FileResult DowloadInvoice(string path)
        {
            byte[] FileBytes = System.IO.File.ReadAllBytes(path);
            return File(FileBytes, "application/pdf");
        }
        public JsonResult Get3wayView(string ponumber, string LineID)
        {
            // LineID /  no use of it..
            var data = MVCManager.Get3WayList(ponumber.Trim());
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Get2wayView(string ponumber)
        {
            var data = MVCManager.Get2WayList(ponumber.Trim());
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public void UpdateReceiptNumber(string id, string receipt)
        {
            MVCManager.UpdateReceiptNumber(Convert.ToInt64(id), receipt.Trim());
        }
        public void UpdateLineLocationID(string id, string LocID)
        {
            MVCManager.UpdateLineLocID(Convert.ToInt64(id), LocID.Trim());
        }

        public ActionResult ApproveResolveError(string itemids, string search)
        {
            try
            {
                if (!string.IsNullOrEmpty(itemids))
                {
                   MVCManager.ApproveWarnigMessae(itemids.Trim());                    
                    TempData["Success"] = "Error resolved successfully..!!!";
                }

            }
            catch (Exception Ex)
            {
                TempData["Error"] = "Someting went wrong.";
            }
            if (!string.IsNullOrEmpty(search))
            {
                SearchViewModel models = JsonConvert.DeserializeObject<SearchViewModel>(search);
                models.tab = "2";
                return RedirectToAction("Home", models);
            }
            else
            {
                return RedirectToAction("Home");
            }
        }
        public ActionResult ApprovePO(string poids, string search)
        {
            try
            {
                if (!string.IsNullOrEmpty(poids))
                {
                    MVCManager.ApprovePO(poids.Trim());
                    TempData["Success"] = "Invoice approved successfully..!!!";
                }

            }
            catch (Exception Ex)
            {
                TempData["Error"] = "Someting went wrong.";
            }
            if (!string.IsNullOrEmpty(search))
            {
                SearchViewModel models = JsonConvert.DeserializeObject<SearchViewModel>(search);
                models.tab = "3";
                return RedirectToAction("Home", models);
            }
            else
            {
                return RedirectToAction("Home");
            }


        }
        #endregion------------------------------------------------------------------------------------

        #region------------------------------------------------------Excel-----------------------------------
        public void FormatExcelForPo(List<Tbl_AP_PODetail> p, string filename)
        {
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.ClearHeaders();
            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.ContentType = "application/ms-excel";
            System.Web.HttpContext.Current.Response.Write(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">");
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename + ".xls");

            System.Web.HttpContext.Current.Response.Charset = "utf-8";
            System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1250");
            //sets font
            System.Web.HttpContext.Current.Response.Write("<font style='font-size:10.0pt; font-family:Calibri;'>");
            System.Web.HttpContext.Current.Response.Write("<BR><BR><BR>");
            System.Web.HttpContext.Current.Response.Write("<Table border='1' bgColor='#ffffff' " + "borderColor='#000000' cellSpacing='0' cellPadding='0' " +
              "style='font-size:10.0pt; font-family:Calibri; background:white;'> <TR>");
            //System.Web.HttpContext.Current.Response.Write("<Td>");
            //System.Web.HttpContext.Current.Response.Write("<B>");
            //System.Web.HttpContext.Current.Response.Write("InvoiceID");
            //System.Web.HttpContext.Current.Response.Write("</B>");
            //System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Invoice No");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Invoice Date");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("PO Number");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Vendor Name");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Vedor Site");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Vendor GstNO");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("GPI GstNO");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Invoice Amount");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Invoice Status");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Paymen tReference");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");


            foreach (Tbl_AP_PODetail pdata in p)
            {
                System.Web.HttpContext.Current.Response.Write("<TR>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.InvoiceNo);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.InvoiceDate);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.PONumber);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.VendorName);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.VedorSite);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.VendorGstNO);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.GPIGstNO);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.InvoiceAmount);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(MVCManager.GetInviceStatus(pdata.InvoiceStatus));

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.PaymentReference);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("</TR>");

            }
            System.Web.HttpContext.Current.Response.Write("</Table>");
            System.Web.HttpContext.Current.Response.Write("</font>");
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        public void FormattoExcelLineItem(List<Tbl_AP_LineItemDetail> p, string filename)
        {
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.ClearHeaders();
            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.ContentType = "application/ms-excel";
            System.Web.HttpContext.Current.Response.Write(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">");
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename + ".xls");

            System.Web.HttpContext.Current.Response.Charset = "utf-8";
            System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1250");
            //sets font
            System.Web.HttpContext.Current.Response.Write("<font style='font-size:10.0pt; font-family:Calibri;'>");
            System.Web.HttpContext.Current.Response.Write("<BR><BR><BR>");
            System.Web.HttpContext.Current.Response.Write("<Table border='1' bgColor='#ffffff' " + "borderColor='#000000' cellSpacing='0' cellPadding='0' " +
              "style='font-size:10.0pt; font-family:Calibri; background:white;'> <TR>");
            //System.Web.HttpContext.Current.Response.Write("<Td>");
            //System.Web.HttpContext.Current.Response.Write("<B>");
            //System.Web.HttpContext.Current.Response.Write("InvoiceID");
            //System.Web.HttpContext.Current.Response.Write("</B>");
            //System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Vendor Name");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Invoice No Date");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Invoice Line No");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Invoice Line Detail");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("PO Number");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Line Location ID");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Receipt Number");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Match Option");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Error Descrpion");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");


            foreach (Tbl_AP_LineItemDetail pdata in p)
            {
                System.Web.HttpContext.Current.Response.Write("<TR>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.VendorName);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.InvoiceNoDate);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.LineId);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.InvoiceDescription);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.PONumber);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.LineLocationID);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.ReceiptNumber);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.MatchOption);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.ErrorDescrpion);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("</TR>");

            }
            System.Web.HttpContext.Current.Response.Write("</Table>");
            System.Web.HttpContext.Current.Response.Write("</font>");
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        public void FormatExcelForAppprove(List<Tbl_AP_PODetail> p, string filename)
        {
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.ClearHeaders();
            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.ContentType = "application/ms-excel";
            System.Web.HttpContext.Current.Response.Write(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">");
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename + ".xls");

            System.Web.HttpContext.Current.Response.Charset = "utf-8";
            System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1250");
            //sets font
            System.Web.HttpContext.Current.Response.Write("<font style='font-size:10.0pt; font-family:Calibri;'>");
            System.Web.HttpContext.Current.Response.Write("<BR><BR><BR>");
            System.Web.HttpContext.Current.Response.Write("<Table border='1' bgColor='#ffffff' " + "borderColor='#000000' cellSpacing='0' cellPadding='0' " +
              "style='font-size:10.0pt; font-family:Calibri; background:white;'> <TR>");
            //System.Web.HttpContext.Current.Response.Write("<Td>");
            //System.Web.HttpContext.Current.Response.Write("<B>");
            //System.Web.HttpContext.Current.Response.Write("InvoiceID");
            //System.Web.HttpContext.Current.Response.Write("</B>");
            //System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Invoice No");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Invoice Date");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("PO Number");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Vendor Name");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Vedor Site");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Invoice Amount");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Remarks");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");

            System.Web.HttpContext.Current.Response.Write("<Td>");
            System.Web.HttpContext.Current.Response.Write("<B>");
            System.Web.HttpContext.Current.Response.Write("Approvers");
            System.Web.HttpContext.Current.Response.Write("</B>");
            System.Web.HttpContext.Current.Response.Write("</Td>");


            foreach (Tbl_AP_PODetail pdata in p)
            {
                System.Web.HttpContext.Current.Response.Write("<TR>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.InvoiceNo);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.InvoiceDate);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.PONumber);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.VendorName);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.VedorSite);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.InvoiceAmount);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(" ");

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("<Td>");

                System.Web.HttpContext.Current.Response.Write(pdata.Department);

                System.Web.HttpContext.Current.Response.Write("</Td>");

                System.Web.HttpContext.Current.Response.Write("</TR>");

            }
            System.Web.HttpContext.Current.Response.Write("</Table>");
            System.Web.HttpContext.Current.Response.Write("</font>");
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }


        #endregion-------------------------------------------------------------------------------------------------

        #region-------------------------------DownloadInvoice from Cloud--------------------------------------------
        public FileResult GetcloudInvoice(string id)
        {
            string path = MVCManager.GetInvoicepathById(Convert.ToInt64(id));
            byte[] FileBytes = System.IO.File.ReadAllBytes(path);
            return File(FileBytes, "application/pdf");
        }
        #endregion----------------------------------------------------------------------------------------------------------------------

        #region-----------------------Invoice Tracker----------------------------------------
        public ActionResult InvoiceTrackPage()
        {
            return View();
        }
        

        #endregion-------------------------------------------------------------------------
    }
}