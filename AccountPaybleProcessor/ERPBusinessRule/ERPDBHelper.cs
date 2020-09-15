using DataAccessLayer.App_Code.DBManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AccountPaybleProcessor.App_Code.ERPViewModel;

namespace AccountPaybleProcessor.ERPBusinessRule
{
    public class ERPDBHelper
    {
        APERPDBManager eRPDBManager;

        public ERPDBHelper()
        {
            eRPDBManager = APERPDBManager.Get_ERPDBManager;
        }
        public List<XXGP_RPA_PO_VIEW_Model> Get_XXGP_RPA_PO_VIEW_Map(string PONumber)
        {

            List<XXGP_RPA_PO_VIEW_Model> list = eRPDBManager.XXGP_RPA_PO_VIEW_Map_List;
            return list.Where(x => x.PO_Number.Equals(PONumber)).ToList();
        }

        public List<XXGP_RPA_INV_PAY_REF_Model> Get_XXGP_RPA_INV_PAY_REF_Map(string PONumber, string invoiceNo, string date)
        {

            List<XXGP_RPA_INV_PAY_REF_Model> list = eRPDBManager.XXGP_RPA_INV_PAY_REF_Map_List;
            return list.Where(x => x.PO_Number.Equals(PONumber) && x.Invoice_Number.Equals(invoiceNo) && x.Invoice_Date.Equals(date)).ToList();
        }
        public List<XXGP_RPA_PO_VIEW_Model> Get_XXGP_RPA_PO_VIEW_Model_PoAprrovedMap(string PONumber)
        {
            List<XXGP_RPA_PO_VIEW_Model> list = eRPDBManager.XXGP_RPA_PO_VIEW_Map_List;
            return list.Where(x => x.PO_Number.Equals(PONumber)).ToList();
        }
        public List<XXGP_RPA_PO_VIEW_Model> Get_XXGP_RPA_PO_VIEW_Model_CheckGstMap(string PONumber, string gstno)
        {
            List<XXGP_RPA_PO_VIEW_Model> list = eRPDBManager.XXGP_RPA_PO_VIEW_Map_List;
            
            return list.Where(x => x.PO_Number.Equals(PONumber) && x.Supplier_GSTN_number.Equals(gstno.Trim())).ToList();
        }
        public List<XXGP_RPA_2WAY_PO_VIEW_Model> Get_XXGP_RPA_2WAY_PO_VIEW_ModelList(string PONumber)
        {
            List<XXGP_RPA_2WAY_PO_VIEW_Model> list = eRPDBManager.XXGP_RPA_2WAY_PO_VIEW_Map_List;
            return list.Where(x => x.PO_Number.Equals(PONumber)).ToList();
        }
        public List<XXGP_RPA_2WAY_PO_VIEW_Model> Get_XXGP_RPA_2WAY_PO_VIEW_ModelList(string PONumber, string locationid, string gpigst)
        {
            List<XXGP_RPA_2WAY_PO_VIEW_Model> list = eRPDBManager.XXGP_RPA_2WAY_PO_VIEW_Map_List;
            return list.Where(x => x.PO_Number.Equals(PONumber) && x.Line_location_id.Equals(locationid) && x.GPI_GST_Regn_No.Equals(gpigst)).ToList();
        }
        public List<XXGP_RPA_2WAY_PO_VIEW_Model> Get_XXGP_RPA_2WAY_PO_VIEW_ModelList(string PONumber, string locationid)
        {
            List<XXGP_RPA_2WAY_PO_VIEW_Model> list = eRPDBManager.XXGP_RPA_2WAY_PO_VIEW_Map_List;
            return list.Where(x => x.PO_Number.Equals(PONumber) && x.Line_location_id.Equals(locationid)).ToList();
        }
        public List<XXGP_RPA_2WAY_PO_VIEW_Model> Get_XXGP_RPA_2WAY_PO_VIEW_ModelList(string PONumber,string locationid, ref decimal amount)
        {
            List<XXGP_RPA_2WAY_PO_VIEW_Model> list = eRPDBManager.XXGP_RPA_2WAY_PO_VIEW_Map_List;
             list = list.Where(x => x.PO_Number.Equals(PONumber)).ToList();
            foreach (var item in list)
            {
                amount += Convert.ToDecimal(item.Available_Amount);
            }
            return list.Where(x => x.PO_Number.Equals(PONumber) && x.Line_location_id.Equals(locationid)).ToList();
        }
        public List<XXGP_RPA_3WAY_PO_VIEW_Model> Get_XXGP_RPA_3WAY_PO_VIEW_ModelList(string PONumber)
        {
            List<XXGP_RPA_3WAY_PO_VIEW_Model> list = eRPDBManager.XXGP_RPA_3WAY_PO_VIEW_Map_List;
          
            return list.Where(x => x.PO_Number.Equals(PONumber)).ToList();
        }
        public List<XXGP_RPA_3WAY_PO_VIEW_Model> Get_XXGP_RPA_3WAY_PO_VIEW_ModelList(string PONumber, string receiptNo)
        {
            List<XXGP_RPA_3WAY_PO_VIEW_Model> list = eRPDBManager.XXGP_RPA_3WAY_PO_VIEW_Map_List;
            return list.Where(x => x.PO_Number.Equals(PONumber)  && x.Receipt_number.Equals(receiptNo)).ToList();
        }
    }
}
