using AccountPaybleWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static AccountPaybleWeb.Manager.ViewModelClass;

namespace AccountPaybleWeb.Manager
{
    public static class SearchInvoiceClass
    {
        public static void GetSearchList(SearchViewModel obj, ref List<Tbl_AP_PODetail> polist, ref List<Tbl_AP_LineItemDetail> LineItemlist, ref List<Tbl_AP_PODetail> ApprovedList, string Dept)
        {
            try
            {
                using (GPModel db = new GPModel())
                {
                    if (string.IsNullOrEmpty(obj.Search))
                    {
                        polist = db.Tbl_AP_PODetail.Where(x => x.Department.Equals(Dept)).ToList();
                        LineItemlist = GetLineItemList(polist, db);
                        ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept)).ToList();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(obj.Status)) { obj.Status = obj.Status.Contains(",") ? obj.Status.Split(',')[0] : obj.Status; }
                        if (obj.fromdate.Year != 1 && obj.todate.Year != 1)
                        {
                            if (!string.IsNullOrEmpty(obj.Vendor) && !string.IsNullOrEmpty(obj.Status) && !string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) &&
                                x.InvoiceStatus.Equals(obj.Status.Trim()) && x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.InvoiceDate >= obj.fromdate &&
                                 x.InvoiceDate <= obj.todate && x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Vendor) && !string.IsNullOrEmpty(obj.Status))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) &&
                               x.InvoiceStatus.Equals(obj.Status.Trim()) && x.VendorName.Equals(obj.Vendor.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.InvoiceDate >= obj.fromdate &&
                                 x.InvoiceDate <= obj.todate && x.VendorName.Equals(obj.Vendor.Trim())).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Vendor) && !string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) &&
                                x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.InvoiceDate >= obj.fromdate &&
                                 x.InvoiceDate <= obj.todate && x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Status) && !string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) &&
                                x.InvoiceStatus.Equals(obj.Status.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.InvoiceDate >= obj.fromdate &&
                                 x.InvoiceDate <= obj.todate && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                            }                                                                                                                                                         
                            else if (!string.IsNullOrEmpty(obj.Vendor))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) && x.VendorName.Equals(obj.Vendor.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.InvoiceDate >= obj.fromdate &&
                                x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) && x.VendorName.Equals(obj.Vendor.Trim())).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Status))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) && x.InvoiceStatus.Equals(obj.Status.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.InvoiceDate >= obj.fromdate &&
                                x.InvoiceDate <= obj.todate).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.PONumber.Equals(obj.ponumber.Trim()) && x.Department.Equals(Dept) && x.InvoiceDate >= obj.fromdate &&
                                x.InvoiceDate <= obj.todate).ToList();
                            }
                            else
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept)).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.InvoiceDate >= obj.fromdate &&
                                x.InvoiceDate <= obj.todate).ToList();
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(obj.Vendor) && !string.IsNullOrEmpty(obj.Status) && !string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(obj.Status.Trim()) && x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim()) && x.Department.Equals(Dept)).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) &&
                                 x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim()) && x.Department.Equals(Dept)).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Vendor) && !string.IsNullOrEmpty(obj.Status))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.Department.Equals(Dept) && x.InvoiceStatus.Equals(obj.Status.Trim()) && x.VendorName.Equals(obj.Vendor.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) 
                                && x.VendorName.Equals(obj.Vendor.Trim())).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Vendor) && !string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x =>  x.Department.Equals(Dept) &&x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Status) && !string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.Department.Equals(Dept) &&x.InvoiceStatus.Equals(obj.Status.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Vendor))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.VendorName.Equals(obj.Vendor.Trim()) && x.Department.Equals(Dept)).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval)
                                && x.VendorName.Equals(obj.Vendor.Trim()) && x.Department.Equals(Dept)).ToList();

                            }
                            else if (!string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.PONumber.Equals(obj.ponumber.Trim()) && x.Department.Equals(Dept)).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval)
                                && x.PONumber.Equals(obj.ponumber.Trim()) && x.Department.Equals(Dept)).ToList();

                            }
                            else
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(obj.Status.Trim()) && x.Department.Equals(Dept)).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(obj.Status.Trim()) && x.Department.Equals(Dept)).ToList();
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static List<Tbl_AP_LineItemDetail> GetLineItemList(List<Tbl_AP_PODetail> polist, GPModel db)
        {
            List<Tbl_AP_LineItemDetail> linelist = new List<Tbl_AP_LineItemDetail>();
            foreach (var item in polist)
            {
                List<Tbl_AP_LineItemDetail> List = db.Tbl_AP_LineItemDetail.Where(x => x.InvoiceID == item.InvoiceID).ToList();
                if (List[0].MatchOption.ToLower().Equals("3-way"))
                {
                    linelist.Add(List[0]);
                }
                else
                {
                    linelist.AddRange(List);
                }

            }
            return linelist;

        }


        public static void GetSearchListForExcel(SearchViewModel obj, ref List<Tbl_AP_PODetail> polist, ref List<Tbl_AP_LineItemDetail> LineItemlist, ref List<Tbl_AP_PODetail> ApprovedList, string Dept)
        {
            try
            {
                using (GPModel db = new GPModel())
                {
                    if (obj.fromdate.Year==1&&obj.todate.Year==1&&string.IsNullOrEmpty(obj.ponumber)&& string.IsNullOrEmpty(obj.Vendor)&& string.IsNullOrEmpty(obj.Status))
                    {
                        polist = db.Tbl_AP_PODetail.Where(x => x.Department.Equals(Dept)).ToList();
                        LineItemlist = GetLineItemList(polist, db);
                        ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept)).ToList();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(obj.Status)) { obj.Status = obj.Status.Contains(",") ? obj.Status.Split(',')[0] : obj.Status; }
                        if (obj.fromdate.Year != 1 && obj.todate.Year != 1)
                        {
                            if (!string.IsNullOrEmpty(obj.Vendor) && !string.IsNullOrEmpty(obj.Status) && !string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) &&
                                x.InvoiceStatus.Equals(obj.Status.Trim()) && x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.InvoiceDate >= obj.fromdate &&
                                 x.InvoiceDate <= obj.todate && x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Vendor) && !string.IsNullOrEmpty(obj.Status))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) &&
                               x.InvoiceStatus.Equals(obj.Status.Trim()) && x.VendorName.Equals(obj.Vendor.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.InvoiceDate >= obj.fromdate &&
                                 x.InvoiceDate <= obj.todate && x.VendorName.Equals(obj.Vendor.Trim())).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Vendor) && !string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) &&
                                x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.InvoiceDate >= obj.fromdate &&
                                 x.InvoiceDate <= obj.todate && x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Status) && !string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) &&
                                x.InvoiceStatus.Equals(obj.Status.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.InvoiceDate >= obj.fromdate &&
                                 x.InvoiceDate <= obj.todate && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Vendor))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) && x.VendorName.Equals(obj.Vendor.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.InvoiceDate >= obj.fromdate &&
                                x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) && x.VendorName.Equals(obj.Vendor.Trim())).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Status))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) && x.InvoiceStatus.Equals(obj.Status.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.InvoiceDate >= obj.fromdate &&
                                x.InvoiceDate <= obj.todate).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.PONumber.Equals(obj.ponumber.Trim()) && x.Department.Equals(Dept) && x.InvoiceDate >= obj.fromdate &&
                                x.InvoiceDate <= obj.todate).ToList();
                            }
                            else
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceDate >= obj.fromdate && x.InvoiceDate <= obj.todate && x.Department.Equals(Dept)).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.InvoiceDate >= obj.fromdate &&
                                x.InvoiceDate <= obj.todate).ToList();
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(obj.Vendor) && !string.IsNullOrEmpty(obj.Status) && !string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(obj.Status.Trim()) && x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim()) && x.Department.Equals(Dept)).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) &&
                                 x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim()) && x.Department.Equals(Dept)).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Vendor) && !string.IsNullOrEmpty(obj.Status))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.Department.Equals(Dept) && x.InvoiceStatus.Equals(obj.Status.Trim()) && x.VendorName.Equals(obj.Vendor.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept)
                                && x.VendorName.Equals(obj.Vendor.Trim())).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Vendor) && !string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.Department.Equals(Dept) && x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.VendorName.Equals(obj.Vendor.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Status) && !string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.Department.Equals(Dept) && x.InvoiceStatus.Equals(obj.Status.Trim()) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval) && x.Department.Equals(Dept) && x.PONumber.Equals(obj.ponumber.Trim())).ToList();
                            }
                            else if (!string.IsNullOrEmpty(obj.Vendor))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.VendorName.Equals(obj.Vendor.Trim()) && x.Department.Equals(Dept)).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval)
                                && x.VendorName.Equals(obj.Vendor.Trim()) && x.Department.Equals(Dept)).ToList();

                            }
                            else if (!string.IsNullOrEmpty(obj.ponumber))
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.PONumber.Equals(obj.ponumber.Trim()) && x.Department.Equals(Dept)).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(Constants.PODetailInvoiceStatus.PendingForApproval)
                                && x.PONumber.Equals(obj.ponumber.Trim()) && x.Department.Equals(Dept)).ToList();

                            }
                            else
                            {
                                polist = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(obj.Status.Trim()) && x.Department.Equals(Dept)).ToList();
                                LineItemlist = GetLineItemList(polist, db);
                                ApprovedList = db.Tbl_AP_PODetail.Where(x => x.InvoiceStatus.Equals(obj.Status.Trim()) && x.Department.Equals(Dept)).ToList();
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}