
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AccountPaybleProcessor.App_Code.ERPViewModel;

namespace AccountPaybleProcessor.ERPBusinessRule
{
    public class APERPDBManager : IDisposable
    {
        public List<XXGP_RPA_PO_VIEW_Model> XXGP_RPA_PO_VIEW_Map_List;
        public List<XXGP_RPA_INV_PAY_REF_Model> XXGP_RPA_INV_PAY_REF_Map_List;
        public List<XXGP_RPA_2WAY_PO_VIEW_Model> XXGP_RPA_2WAY_PO_VIEW_Map_List;
        public List<XXGP_RPA_3WAY_PO_VIEW_Model> XXGP_RPA_3WAY_PO_VIEW_Map_List;

        string connString = System.Configuration.ConfigurationManager.AppSettings["ERPDB"];

        private static readonly Lazy<APERPDBManager> eRPDBManagers = new Lazy<APERPDBManager>(() => new APERPDBManager());
        public static APERPDBManager Get_ERPDBManager
        {
            get
            {
                return eRPDBManagers.Value;
            }
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        private APERPDBManager()
        {
            if (XXGP_RPA_PO_VIEW_Map_List == null || XXGP_RPA_PO_VIEW_Map_List.Count == 0)
            {
                GET_XXGP_RPA_PO_VIEW_Map_List();
            }

            if (XXGP_RPA_INV_PAY_REF_Map_List == null || XXGP_RPA_INV_PAY_REF_Map_List.Count == 0)
            {
                GET_XXGP_RPA_INV_PAY_REF_Map_List();
            }

            if (XXGP_RPA_2WAY_PO_VIEW_Map_List == null || XXGP_RPA_2WAY_PO_VIEW_Map_List.Count == 0)
            {
                GET_XXGP_RPA_2WAY_PO_VIEW_Map_List();
            }
            if (XXGP_RPA_3WAY_PO_VIEW_Map_List == null || XXGP_RPA_3WAY_PO_VIEW_Map_List.Count == 0)
            {
                GET_XXGP_RPA_3WAY_PO_VIEW_Map_List();
            }

        }


        void GET_XXGP_RPA_PO_VIEW_Map_List()
        {
            XXGP_RPA_PO_VIEW_Map_List = new List<XXGP_RPA_PO_VIEW_Model>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.XXGP_RPA_PO_VIEW";
                using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        XXGP_RPA_PO_VIEW_Model PoObject = new XXGP_RPA_PO_VIEW_Model();
                        PoObject.PO_Number = Convert.ToString(dr["PO_Number"]);
                        PoObject.Match_option = Convert.ToString(dr["Match_option"]);
                        PoObject.Vendor_Name = Convert.ToString(dr["Vendor_Name"]);
                        PoObject.Vendor_Site = Convert.ToString(dr["Vendor_Site"]);
                        PoObject.PO_Status = Convert.ToString(dr["PO_Status"]);
                        PoObject.User_Name = Convert.ToString(dr["User_Name"]);
                        PoObject.Supplier_GSTN_number = Convert.ToString(dr["Supplier_GSTN_number"]);
                        XXGP_RPA_PO_VIEW_Map_List.Add(PoObject);
                    }
                }
            };

        }

        void GET_XXGP_RPA_INV_PAY_REF_Map_List()
        {
            XXGP_RPA_INV_PAY_REF_Map_List = new List<XXGP_RPA_INV_PAY_REF_Model>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.XXGP_RPA_INV_PAY_REF";
                using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        XXGP_RPA_INV_PAY_REF_Model Object = new XXGP_RPA_INV_PAY_REF_Model();
                        Object.PO_Number = Convert.ToString(dr["PO_Number"]);
                        Object.Invoice_Number = Convert.ToString(dr["Invoice_Number"]);
                        Object.Invoice_Date = Convert.ToString(dr["Invoice_Date"]);
                        Object.Payment_reference = Convert.ToString(dr["Payment_reference"]);

                        XXGP_RPA_INV_PAY_REF_Map_List.Add(Object);
                    }
                }
            };

        }

        void GET_XXGP_RPA_2WAY_PO_VIEW_Map_List()
        {
            XXGP_RPA_2WAY_PO_VIEW_Map_List = new List<XXGP_RPA_2WAY_PO_VIEW_Model>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.XXGP_RPA_2WAY_PO_VIEW";
                using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        XXGP_RPA_2WAY_PO_VIEW_Model Object = new XXGP_RPA_2WAY_PO_VIEW_Model();
                        Object.PO_Number = Convert.ToString(dr["PO_Number"]);
                        Object.Line_Description = Convert.ToString(dr["Line_Description"]);
                        Object.Location_code = Convert.ToString(dr["Location_code"]);
                        Object.Line_location_id = Convert.ToString(dr["Line_location_id"]);
                        Object.Line_price = Convert.ToString(dr["Line_price"]);
                        Object.Available_Qty = Convert.ToString(dr["Available_Qty"]);
                        Object.Available_Amount = Convert.ToString(dr["Available_Amount"]);
                        Object.GPI_GST_Regn_No = Convert.ToString(dr["GPI_GST_Regn_No"]);
                        Object.Tax_Rate = Convert.ToString(dr["Tax_Rate"]);
                        XXGP_RPA_2WAY_PO_VIEW_Map_List.Add(Object);
                    }
                }
            };

        }
        void GET_XXGP_RPA_3WAY_PO_VIEW_Map_List()
        {
            XXGP_RPA_3WAY_PO_VIEW_Map_List = new List<XXGP_RPA_3WAY_PO_VIEW_Model>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.XXGP_RPA_3WAY_PO_VIEW";
                using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        XXGP_RPA_3WAY_PO_VIEW_Model Object = new XXGP_RPA_3WAY_PO_VIEW_Model();
                        Object.Invoice_Number = Convert.ToString(dr["Invoice_Number"]);
                        Object.Invoice_Date = Convert.ToString(dr["Invoice_Date"]);
                        Object.Receipt_number = Convert.ToString(dr["Receipt_number"]);
                        Object.Receipt_total = Convert.ToString(dr["Receipt_total"]);
                        Object.GPI_GST_Regn_No = Convert.ToString(dr["GPI_GST_Regn_No"]);
                        Object.PO_Number= Convert.ToString(dr["PO_Number"]);
                        XXGP_RPA_3WAY_PO_VIEW_Map_List.Add(Object);
                    }
                }
            };
        }
    }
}
