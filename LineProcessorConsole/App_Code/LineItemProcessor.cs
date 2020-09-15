using AccountPaybleProcessor.ERPBusinessRule;
using APLineItemDataLayer.ApLineItem;
using APLineItemDataLayer.Model;
using DataAccessLayer.App_Code.DBManager;
using DataAccessLayer.DBModel;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AccountPaybleProcessor.App_Code.ERPViewModel;
using static LineProcessorConsole.App_Code.ViewModelClass;

namespace LineProcessorConsole.App_Code
{
    class LineItemProcessor : Processor, IDisposable
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        public string SharedLocationPath { get { return System.Configuration.ConfigurationManager.AppSettings["SharedLocationPath"]; } }
        public string ProcessedInvoicePath { get { return System.Configuration.ConfigurationManager.AppSettings["ProcessedLocationPath"]; } }

        #region Private Variable

        TBL_ProcessInstances tblProcessInstance;
        IGSTDBManager dbManager;
        ErrorModel MetaError;
        TBL_ProcessInstanceDetails tblProcessInstanceDetails;
        TBL_ProcessInstanceData tblProcessInstanceData;
        //  IPDFProcessor pdfProcessor;

        #endregion


        public LineItemProcessor(TBL_ProcessInstances tblProcessInstance, IGSTDBManager dbManager)
        {
            this.tblProcessInstance = tblProcessInstance;
            this.dbManager = dbManager;
        }

        public void Dispose()
        {

        }

        public override void ExecuteState()
        {
            tblProcessInstanceDetails = this.dbManager.GetProcessInstanceDetail(tblProcessInstance.ProcessInstanceId);
            switch (tblProcessInstanceDetails.StateId)
            {
                case Common.Constants.Process.States.APLineIetmProcess.InitialSate:
                    ApplyBusinessRules();
                    break;
            }
        }
        private void ApplyBusinessRules()
        {
            try
            {
                logger.Info("Business Rule Started.");
                string ErrorMessage = string.Empty, amounttobematched = string.Empty;
                ERPDBHelper ERpHelper = new ERPDBHelper();
                List<Tbl_AP_LineItemDetail> ItemList = new List<Tbl_AP_LineItemDetail>();
                Tbl_AP_PODetail poitem = new Tbl_AP_PODetail();
                List<Tbl_AP_PODetail> PoList = LineItemManager.GetPoList();
                System.Threading.Thread.Sleep(1000);
                List<Tbl_AP_LineItemDetail> AllLineItems = LineItemManager.GetLineItemList();
                Tbl_AP_LineItemDetail line = new Tbl_AP_LineItemDetail();
                foreach (var item in PoList)
                {
                    poitem = item; amounttobematched = string.Empty;
                    ItemList = AllLineItems.Where(x => x.InvoiceID == poitem.InvoiceID).ToList();
                    try
                    {

                        ErrorMessage = string.Empty;

                        //GetOtherDataFromPonumber(ref poitem, ERpHelper, ref ErrorMessage, ref ItemList);
                        //poitem.ViewMapped = string.Empty;

                        if (string.IsNullOrEmpty(poitem.ViewMapped))
                        {
                            GetOtherDataFromPonumber(ref poitem, ERpHelper, ref ErrorMessage, ref ItemList);
                        }
                        if (!poitem.ViewMapped.Contains(Common.Constants.APViewMApped.PaymentReference))
                        {
                            ChceckPamentRef(ref poitem, ref ErrorMessage, ERpHelper, ref ItemList);
                        }
                        if (!poitem.ViewMapped.Contains(Common.Constants.APViewMApped.PoApproved))
                        {
                            ChceckPoApproved(ref poitem, ref ErrorMessage, ERpHelper, ref ItemList);
                        }
                        if (!poitem.ViewMapped.Contains(Common.Constants.APViewMApped.GStNumber) && string.IsNullOrEmpty(ErrorMessage))
                        {
                            CheckGSTINNumber(ref poitem, ref ErrorMessage, ERpHelper, ref ItemList);
                        }
                        if (string.IsNullOrEmpty(ErrorMessage))
                        {
                            var Linelist = new List<Tbl_AP_LineItemDetail>();
                            foreach (var lineitem in ItemList)
                            {
                                if (lineitem.LineItemID == 5)
                                {

                                }
                                line = lineitem;
                                if (line.MatchOption.ToLower().Equals(Common.Constants.APMatchOption.TwoWay))
                                {
                                    if (string.IsNullOrEmpty(line.ViewMapped))
                                    {
                                        Check2wayPomatch(ref line, ref ErrorMessage, ERpHelper);
                                    }
                                    if (!string.IsNullOrEmpty(line.LineLocationID) && !line.ViewMapped.Contains(Common.Constants.APViewMApped.TwoWayGSTMatch))
                                    {
                                        Check2WAYGSTmatch(ref line, ref ErrorMessage, ERpHelper, poitem.GPIGstNO);
                                    }
                                    if (!string.IsNullOrEmpty(line.LineLocationID) && string.IsNullOrEmpty(ErrorMessage) && !line.ViewMapped.Contains(Common.Constants.APViewMApped.TwoWayManualMatchAmount))
                                    {
                                        Check2wayManualMatchForAmount(ref line, ref ErrorMessage, ERpHelper, item.GPIGstNO);
                                    }
                                    if (!string.IsNullOrEmpty(line.LineLocationID) && string.IsNullOrEmpty(ErrorMessage) && !line.ViewMapped.Contains(Common.Constants.APViewMApped.TwoWayManualMatchTotalAmount))
                                    {
                                        Check2wayManualMatchForTotalAmount(ref line, ref ErrorMessage, ERpHelper, item.GPIGstNO, ref amounttobematched);
                                    }
                                }
                                else if (line.MatchOption.ToLower().Equals(Common.Constants.APMatchOption.Threeway))
                                {
                                    if (string.IsNullOrEmpty(line.ViewMapped))
                                    {
                                        Check3wayPomatch(ref line, ref ErrorMessage, ERpHelper, poitem.InvoiceDate.ToString(), poitem.InvoiceNo);
                                    }
                                    if (!string.IsNullOrEmpty(line.ReceiptNumber) && !line.ViewMapped.Contains(Common.Constants.APViewMApped.ThreeWayAmountMap))
                                    {
                                        Check3wayTotalmatch(ref line, ref ErrorMessage, ERpHelper, poitem.InvoiceAmount);
                                    }
                                    if (!string.IsNullOrEmpty(line.ReceiptNumber) && string.IsNullOrEmpty(ErrorMessage) && !line.ViewMapped.Contains(Common.Constants.APViewMApped.ThreeWayGSTMap))
                                    {
                                        Check3wayGPIGstMatch(ref line, ref ErrorMessage, ERpHelper, item.GPIGstNO);
                                    }
                                }
                                else
                                {
                                    ErrorModel MetaError = new ErrorModel()
                                    {
                                        ErrorMessage = "Invalid match option."
                                    };
                                    line.ErrorDescrpion = JsonConvert.SerializeObject(MetaError);

                                }
                                ErrorMessage = line.ErrorDescrpion;
                                Linelist.Add(line);
                            }
                            ItemList = Linelist;
                        }

                    }
                    catch (Exception Ex)
                    {
                        HandlingException(ref ItemList, Ex.Message);
                        ErrorMessage = Ex.Message;
                    }
                    if (!string.IsNullOrEmpty(ErrorMessage))
                    {
                        poitem.InvoiceStatus = Common.Constants.PODetailInvoiceStatus.Error;
                    }
                    else
                    {
                        poitem.InvoiceStatus = Common.Constants.PODetailInvoiceStatus.PendingForApproval;
                    }
                    LineItemManager.EditPoItem(poitem);
                    LineItemManager.EditLineItems(ItemList);
                }



            }
            catch (Exception ex)
            {

            }

        }

        private void HandlingException(ref List<Tbl_AP_LineItemDetail> ItemList, string msg)
        {
            foreach (var item in ItemList)
            {
                item.ErrorDescrpion = "Somthing went wrong." + Environment.NewLine + msg;
            }
        }
        #region-------------------------Po Item Business Rules-------------------------------------

        private void GetOtherDataFromPonumber(ref Tbl_AP_PODetail poitem, ERPDBHelper DbObj, ref string ErrorMesssage, ref List<Tbl_AP_LineItemDetail> itemlist)
        {

            try
            {
                ErrorMesssage = string.Empty;
                List<XXGP_RPA_PO_VIEW_Model> ViewLsit = DbObj.Get_XXGP_RPA_PO_VIEW_Map(poitem.PONumber.Trim());
                if (ViewLsit.Count > 0)
                {

                    poitem.VendorName = ViewLsit[0].Vendor_Name;
                    poitem.VedorSite = ViewLsit[0].Vendor_Site;
                    poitem.Department = ViewLsit[0].User_Name;
                    foreach (var item in itemlist)
                    {
                        if (!string.IsNullOrEmpty(ViewLsit[0].Match_option))
                        {
                            item.VendorName = ViewLsit[0].Vendor_Name;
                            item.MatchOption = ViewLsit[0].Match_option;
                        }
                        else
                        {
                            MetaError = new ErrorModel()
                            {
                                ErrorMessage = "Match Option Not Found." + Environment.NewLine + "XXGP_RPA_PO_VIEW"
                            };
                            item.ErrorDescrpion = JsonConvert.SerializeObject(MetaError);
                            ErrorMesssage = "Match Option Not Found.";
                        }
                    }
                    if (string.IsNullOrEmpty(ErrorMesssage))
                    {
                        poitem.ViewMapped += Common.Constants.APViewMApped.OtherData + ",";
                    }
                }
                logger.Info("View Extraction Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

        }

        private void ChceckPamentRef(ref Tbl_AP_PODetail item, ref string ErroMessage, ERPDBHelper DbObj, ref List<Tbl_AP_LineItemDetail> itemlist)
        {
            try
            {
                ErroMessage = string.Empty;
                List<XXGP_RPA_INV_PAY_REF_Model> list = DbObj.Get_XXGP_RPA_INV_PAY_REF_Map(item.PONumber, item.InvoiceNo, item.InvoiceDate.ToString());
                if (list.Count != 0)
                {
                    item.PaymentReference = list[0].Payment_reference;
                }
                else
                {
                    foreach (var i in itemlist)
                    {
                        MetaError = new ErrorModel()
                        {
                            ErrorMessage = "Payment Reference:N/A." + Environment.NewLine + "XXGP_RPA_INV_PAY_REF"
                        };
                        i.ErrorDescrpion = JsonConvert.SerializeObject(MetaError);
                        ErroMessage = "Payment Reference Not Found.";
                    }
                }
                item.ViewMapped += Common.Constants.APViewMApped.PaymentReference + ",";
            }
            catch (Exception ex)
            {

            }
        }
        private void ChceckPoApproved(ref Tbl_AP_PODetail item, ref string ErroMessage, ERPDBHelper DbObj, ref List<Tbl_AP_LineItemDetail> itemlist)
        {
            try
            {
                ErroMessage = string.Empty;
                List<XXGP_RPA_PO_VIEW_Model> list = DbObj.Get_XXGP_RPA_PO_VIEW_Model_PoAprrovedMap(item.PONumber);
                if (list.Count != 0 && list[0].PO_Status.ToLower().Equals("approved"))
                {
                    item.ViewMapped += Common.Constants.APViewMApped.PoApproved + ",";
                }
                else
                {
                    foreach (var i in itemlist)
                    {
                        MetaError = new ErrorModel()
                        {
                            ErrorMessage = "PO is not approved. Get the po approved" + Environment.NewLine + "XXGP_RPA_PO_VIEW"
                        };
                        i.ErrorDescrpion = JsonConvert.SerializeObject(MetaError);
                        ErroMessage = "PO is not approved. Get the po approved.";
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
        private void CheckGSTINNumber(ref Tbl_AP_PODetail item, ref string ErroMessage, ERPDBHelper DbObj, ref List<Tbl_AP_LineItemDetail> itemlist)
        {
            try
            {
                ErroMessage = string.Empty;
                List<XXGP_RPA_PO_VIEW_Model> list = DbObj.Get_XXGP_RPA_PO_VIEW_Model_CheckGstMap(item.PONumber.Trim(), item.VendorGstNO.Trim());
                if (list.Count != 0)
                {
                    item.ViewMapped += Common.Constants.APViewMApped.GStNumber + ",";
                }
                else
                {
                    foreach (var i in itemlist)
                    {
                        MetaError = new ErrorModel()
                        {
                            ErrorMessage = "Supplier GSTN is not matching with PO.Get the Same corrected in Supplier master." + Environment.NewLine + "XXGP_RPA_PO_VIEW"
                        };
                        i.ErrorDescrpion = JsonConvert.SerializeObject(MetaError);
                        ErroMessage = "Supplier GSTN is not matching with PO. Get the Same corrected in Supplier master.";
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        #endregion-----------------------------------------------------------------------------------

        #region---------------------Line Item Busines Rule-----------------------------------------
        private void Check2wayPomatch(ref Tbl_AP_LineItemDetail lineitem, ref string ErroMessage, ERPDBHelper dbobj)//Rule 7 and 
        {
            try
            {
                ErroMessage = string.Empty;
                List<XXGP_RPA_2WAY_PO_VIEW_Model> list = dbobj.Get_XXGP_RPA_2WAY_PO_VIEW_ModelList(lineitem.PONumber);
                if (list.Count != 0)
                {
                    if (list.Count < 2)
                    {
                        lineitem.LineLocationID = list[0].Line_location_id;
                        lineitem.ViewMapped += Common.Constants.APViewMApped.TwoWayPoMap + ",";
                        lineitem.ErrorDescrpion = string.Empty;
                    }
                    else
                    {
                        MetaError = new ErrorModel()
                        {
                            WarningMessage = "Manual matching is required for PO as this has multiple lines." + Environment.NewLine + "XXGP_RPA_2WAY_PO_VIEW_045"
                        };
                        lineitem.ErrorDescrpion = JsonConvert.SerializeObject(MetaError);
                        lineitem.ViewMapped += Common.Constants.APViewMApped.TwoWayPoMap + ",";
                        ErroMessage = "Manual matching is required for PO as this has multiple lines." + Environment.NewLine + "XXGP_RPA_2WAY_PO_VIEW";

                    }
                }
                else
                {
                    MetaError = new ErrorModel()
                    {
                        ErrorMessage = "Line item is not available for this po." + Environment.NewLine + "XXGP_RPA_2WAY_PO_VIEW"
                    };
                    lineitem.ErrorDescrpion = JsonConvert.SerializeObject(MetaError);
                    ErroMessage = "Line item is not available for this po.";
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void Check2WAYGSTmatch(ref Tbl_AP_LineItemDetail lineitem, ref string ErroMessage, ERPDBHelper dbobj, string gpigst)//Rule 9 and 
        {
            try
            {
                ErroMessage = string.Empty;
                List<XXGP_RPA_2WAY_PO_VIEW_Model> list = dbobj.Get_XXGP_RPA_2WAY_PO_VIEW_ModelList(lineitem.PONumber.Trim(), lineitem.LineLocationID.Trim(), gpigst.Trim());
                if (list.Count != 0)
                {
                    lineitem.ViewMapped += Common.Constants.APViewMApped.TwoWayGSTMatch + ",";
                    lineitem.ErrorDescrpion = string.Empty;
                }
                else
                {
                    MetaError = new ErrorModel()
                    {
                        ErrorMessage = "PO Line GST registration is not matching with GPI Registration mentioned on Invoice." + Environment.NewLine + "XXGP_RPA_2WAY_PO_VIEW"
                    };
                    lineitem.ErrorDescrpion = JsonConvert.SerializeObject(MetaError);
                    ErroMessage = "PO Line GST registration is not matching with GPI Registration mentioned on Invoice.";
                }
            }
            catch (Exception ex)
            {

            }

        }


        private void Check2wayManualMatchForAmount(ref Tbl_AP_LineItemDetail lineitem, ref string ErroMessage, ERPDBHelper dbobj, string gpigst)//Rule 10 and 
        {
            try
            {
                ErroMessage = string.Empty;
                List<XXGP_RPA_2WAY_PO_VIEW_Model> list = dbobj.Get_XXGP_RPA_2WAY_PO_VIEW_ModelList(lineitem.PONumber, lineitem.LineLocationID.Trim());
                if (Convert.ToDecimal(lineitem.Amount) <= Convert.ToDecimal(list[0].Available_Amount))
                {
                    lineitem.ViewMapped += Common.Constants.APViewMApped.TwoWayManualMatchAmount + ",";
                    lineitem.ErrorDescrpion = string.Empty;
                }
                else
                {
                    MetaError = new ErrorModel()
                    {
                        ErrorMessage = "PO Line does not have available amount for matching. Get the PO checked." + Environment.NewLine + "XXGP_RPA_2WAY_PO_VIEW"
                    };
                    lineitem.ErrorDescrpion = JsonConvert.SerializeObject(MetaError);
                    ErroMessage = "PO Line does not have available amount for matching. Get the PO checked.";
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void Check2wayManualMatchForTotalAmount(ref Tbl_AP_LineItemDetail lineitem, ref string ErroMessage, ERPDBHelper dbobj, string gpigst, ref string amounttobematched)//Rule 10 and 
        {
            try
            {
                ErroMessage = string.Empty; decimal sum = 0, lineamount = Convert.ToDecimal(lineitem.Amount); decimal taxrate = 0;
                decimal AmountTobeChecked = 0;
                List<XXGP_RPA_2WAY_PO_VIEW_Model> list = dbobj.Get_XXGP_RPA_2WAY_PO_VIEW_ModelList(lineitem.PONumber, lineitem.LineLocationID, ref sum);
                taxrate = Convert.ToDecimal(list[0].Tax_Rate) / 100;
                if (string.IsNullOrEmpty(amounttobematched))
                {
                    AmountTobeChecked = lineamount + lineamount * taxrate;
                    if (AmountTobeChecked <= sum)
                    {
                        lineitem.AmountToBeMatched = (sum - AmountTobeChecked).ToString();

                    }
                    else
                    {
                        MetaError = new ErrorModel()
                        {
                            WarningMessage = "Invoice total is not matching with matched PO lines" + Environment.NewLine + "XXGP_RPA_2WAY_PO_VIEW_034"
                        };
                        lineitem.ErrorDescrpion = JsonConvert.SerializeObject(MetaError);

                        ErroMessage = "Invoice total is not matching with matched PO lines";
                    }
                }
                else
                {
                    sum = Convert.ToDecimal(amounttobematched);
                    AmountTobeChecked = lineamount + lineamount * taxrate;
                    if (AmountTobeChecked <= sum)
                    {
                        lineitem.AmountToBeMatched = (sum - AmountTobeChecked).ToString();

                    }
                    else
                    {
                        MetaError = new ErrorModel()
                        {
                            WarningMessage = "Invoice total is not matching with matched PO lines" + Environment.NewLine + "XXGP_RPA_2WAY_PO_VIEW"
                        };
                        lineitem.ErrorDescrpion = JsonConvert.SerializeObject(MetaError);
                        ErroMessage = "Invoice total is not matching with matched PO lines";
                    }
                }
                if (string.IsNullOrEmpty(ErroMessage))
                {
                    lineitem.ViewMapped += Common.Constants.APViewMApped.TwoWayManualMatchTotalAmount + ",";
                    lineitem.ErrorDescrpion = string.Empty;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Check3wayPomatch(ref Tbl_AP_LineItemDetail lineitem, ref string ErroMessage, ERPDBHelper dbobj, string invoicedate, string invoiceno)
        {
            try
            {
                ErroMessage = string.Empty;
                List<XXGP_RPA_3WAY_PO_VIEW_Model> list = dbobj.Get_XXGP_RPA_3WAY_PO_VIEW_ModelList(lineitem.PONumber);
                if (list.Count != 0)
                {
                    list = list.Where(x => x.Invoice_Date.Contains(invoicedate) && x.Invoice_Number.Equals(invoiceno)).ToList();
                    if (list.Count == 1)
                    {
                        lineitem.ReceiptNumber = list[0].Receipt_number;
                        lineitem.ViewMapped += Common.Constants.APViewMApped.ThreeWayPoMap + ",";
                        lineitem.ErrorDescrpion = string.Empty;
                    }
                    else
                    {
                        MetaError = new ErrorModel()
                        {
                            WarningMessage = "Matching Receipt not found for this invoice number.Please match with receipt manually" + Environment.NewLine + "XXGP_RPA_3WAY_PO_VIEW_046"
                        };
                        lineitem.ErrorDescrpion = JsonConvert.SerializeObject(MetaError);
                        lineitem.ViewMapped += Common.Constants.APViewMApped.ThreeWayPoMap + ",";
                        ErroMessage = "Matching Receipt not found for this invoice number.Please match with receipt manually" + Environment.NewLine + "XXGP_RPA_3WAY_PO_VIEW_046";
                    }
                }
                else
                {
                    ErroMessage = "No matching receipt number found for the PO Number." + Environment.NewLine + "XXGP_RPA_3WAY_PO_VIEW";
                    MetaError = new ErrorModel()
                    {
                        ErrorMessage = "No matching receipt number found for the PO Number." + Environment.NewLine + "XXGP_RPA_3WAY_PO_VIEW"
                    };
                    lineitem.ErrorDescrpion = JsonConvert.SerializeObject(MetaError);
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void Check3wayTotalmatch(ref Tbl_AP_LineItemDetail lineitem, ref string ErroMessage, ERPDBHelper dbobj, string InvoiceAmount)
        {
            try
            {
                ErroMessage = string.Empty;
                List<XXGP_RPA_3WAY_PO_VIEW_Model> list = dbobj.Get_XXGP_RPA_3WAY_PO_VIEW_ModelList(lineitem.PONumber, lineitem.ReceiptNumber.Trim());
                if (Convert.ToDecimal(list[0].Receipt_total) != Convert.ToDecimal(InvoiceAmount))
                {
                    MetaError = new ErrorModel()
                    {
                        WarningMessage = "Receipt total(" + list[0].Receipt_total + ") is not matching with Invoice total(" + InvoiceAmount + ")" + Environment.NewLine + "XXGP_RPA_3WAY_PO_VIEW_034"
                    };
                    lineitem.ErrorDescrpion = JsonConvert.SerializeObject(MetaError);

                    ErroMessage = "Receipt total is not matching with Invoice total.";
                }
                else
                {
                    lineitem.ViewMapped += Common.Constants.APViewMApped.ThreeWayAmountMap + ",";
                    lineitem.ErrorDescrpion = string.Empty;
                }

            }
            catch (Exception ex)
            {
                ErroMessage = "Receipt total is not matching with Invoice total.";
            }

        }
        private void Check3wayGPIGstMatch(ref Tbl_AP_LineItemDetail lineitem, ref string ErroMessage, ERPDBHelper dbobj, string gpigst)
        {
            try
            {
                ErroMessage = string.Empty;
                List<XXGP_RPA_3WAY_PO_VIEW_Model> list = dbobj.Get_XXGP_RPA_3WAY_PO_VIEW_ModelList(lineitem.PONumber, lineitem.ReceiptNumber.Trim());
                if (list[0].GPI_GST_Regn_No.Trim().Equals(gpigst.Trim()))
                {
                    lineitem.ViewMapped += Common.Constants.APViewMApped.ThreeWayGSTMap + ",";
                    lineitem.ErrorDescrpion = string.Empty;
                }
                else
                {
                    MetaError = new ErrorModel()
                    {
                        ErrorMessage = "Receipt GST registration is not matching with GPI Registration mentioned on Invoice." + Environment.NewLine + "XXGP_RPA_3WAY_PO_VIEW"
                    };
                    lineitem.ErrorDescrpion = JsonConvert.SerializeObject(MetaError);
                    ErroMessage = "Receipt GST registration is not matching with GPI Registration mentioned on Invoice.";
                }
            }
            catch (Exception ex)
            {

            }

        }
        #endregion---------------------------------------------------------------------------------
    }
}
