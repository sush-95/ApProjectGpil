using APLineItemDataLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APLineItemDataLayer.ApLineItem
{
    public static class LineItemManager
    {
        public static List<Tbl_AP_PODetail> GetPoList()
        {
            using (GPILAPModel db = new GPILAPModel())
            {
                return db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Contains(Common.Constants.PODetailInvoiceStatus.Extracted) ||
                   x.InvoiceStatus.Contains(Common.Constants.PODetailInvoiceStatus.Error)).ToList();
            }
        }
        public static List<Tbl_AP_LineItemDetail> GetLineItemList(string Ponumber)
        {
            using (GPILAPModel db = new GPILAPModel())
            {
                return db.Tbl_AP_LineItemDetail.Where(x => x.PONumber.Equals(Ponumber)).ToList();
            }
        }
        public static List<Tbl_AP_LineItemDetail> GetLineItemList()
        {
            using (GPILAPModel db = new GPILAPModel())
            {
                return db.Tbl_AP_LineItemDetail.ToList();
            }
        }
        public static bool CheckPOAvailable(string po, string invoice, DateTime dt)
        {
            bool check = false;
            using (GPILAPModel db = new GPILAPModel())
            {
                var list = db.Tbl_AP_PODetail.Where(x => x.PONumber.Contains(po) && x.InvoiceNo.Contains(invoice) && x.InvoiceDate == dt && !x.InvoiceStatus.Equals(Common.Constants.PODetailInvoiceStatus.Rejected)).ToList();
                if (list.Count == 0)
                {
                    check = true;
                }
            }
            return check;
        }

        public static long SaveInvoice(Tbl_AP_PODetail poobj)
        {
            long ID = 0;
            try
            {
                using (GPILAPModel db = new GPILAPModel())
                {

                    db.Tbl_AP_PODetail.Add(poobj);
                    db.SaveChanges();
                    ID = db.Tbl_AP_PODetail.OrderByDescending(x => x.InvoiceID).FirstOrDefault().InvoiceID;
                }
            }
            catch (Exception ex)
            {

            }

            return ID;
        }
        public static void SavePoItem(Tbl_AP_PODetail poobj)
        {
            using (GPILAPModel db = new GPILAPModel())
            {

                db.Tbl_AP_PODetail.Add(poobj);
                db.SaveChanges();
            }
        }
        public static void SaveLineIetm(List<Tbl_AP_LineItemDetail> LineItem)
        {
            using (GPILAPModel db = new GPILAPModel())
            {
                db.Tbl_AP_LineItemDetail.AddRange(LineItem);
                db.SaveChanges();
            }
        }

        public static void EditPoItem(Tbl_AP_PODetail poobj)
        {
            using (GPILAPModel db = new GPILAPModel())
            {
                Tbl_AP_PODetail editobj = db.Tbl_AP_PODetail.Where(x => x.InvoiceID.Equals(poobj.InvoiceID)).FirstOrDefault();
                editobj.InvoiceNo = poobj.InvoiceNo;
                editobj.InvoiceDate = poobj.InvoiceDate;
                editobj.VendorName = poobj.VendorName;
                editobj.VedorSite = poobj.VedorSite;
                editobj.VendorGstNO = poobj.VendorGstNO;
                editobj.GPIGstNO = poobj.GPIGstNO;
                editobj.InvoiceAmount = poobj.InvoiceAmount;
                editobj.InvoiceStatus = poobj.InvoiceStatus;
                editobj.PaymentReference = poobj.PaymentReference;
                editobj.ViewMapped = poobj.ViewMapped;
                editobj.Department = poobj.Department;
                db.SaveChanges();
            }
        }
        public static void EditLineItems(List<Tbl_AP_LineItemDetail> LineItem)
        {
            Tbl_AP_LineItemDetail obj = new Tbl_AP_LineItemDetail();
            using (GPILAPModel db = new GPILAPModel())
            {
                foreach (var item in LineItem)
                {
                    obj = db.Tbl_AP_LineItemDetail.Where(x => x.LineItemID == item.LineItemID).FirstOrDefault();
                    obj.LineLocationID = item.LineLocationID;
                    obj.Amount = item.Amount;
                    obj.ReferenceNo = item.ReferenceNo;
                    obj.VendorName = item.VendorName;
                    obj.ReceiptNumber = item.ReceiptNumber;
                    obj.MatchOption = item.MatchOption;
                    obj.ErrorDescrpion = item.ErrorDescrpion;
                    obj.ViewMapped = item.ViewMapped;
                    obj.AmountToBeMatched = item.AmountToBeMatched;
                    obj.Rate = item.Rate;
                    db.SaveChanges();
                }


            }
        }
    }

}
