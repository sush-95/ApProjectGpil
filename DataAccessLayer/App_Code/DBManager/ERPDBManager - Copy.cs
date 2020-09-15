using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using DataAccessLayer.App_Code.ViewModel;

namespace DataAccessLayer.App_Code.DBManager
{
    public class ERPDBManager:IDisposable
    {
        public List<GP_Airline_Gst_Party_Map> gstPartyMapList;
        public List<GP_Gst_Tax_Type_Map> taxTypeMapList;
        public List<GP_Gpi_Gst_OU_Map> gstOUMapList;
        public List<GP_Gst_Tax_Rate_Map> gstTaxRateMapList;
        public List<GP_Gst_Doc_Type_Map> gstDocTypeMapList;
        public List<GP_Gst_Sac_Code_Map> gstSacCodeMapList;
     

        string connString = System.Configuration.ConfigurationManager.AppSettings["ERPDB"];

        public ERPDBManager()
        {
            if(gstPartyMapList == null)
            {
                GET_GP_Airline_Gst_Party_Map();               
            }

            if (gstDocTypeMapList == null || gstDocTypeMapList.Count == 0)
            {
                GET_GP_Gst_Doc_Type_Map();
            }

            if (gstOUMapList == null || gstOUMapList.Count == 0)
            {
                GET_GP_Gpi_Gst_OU_Map();
            }

            if (gstSacCodeMapList == null || gstSacCodeMapList.Count == 0)
            {
                GET_GP_Gst_Sac_Code_Map();
            }

            if (gstTaxRateMapList == null || gstTaxRateMapList.Count == 0)
            {
                GET_GP_Gst_Tax_Rate_Map();
            }

            if (taxTypeMapList == null || taxTypeMapList.Count == 0)
            {
                GET_GP_Gst_Tax_Type_Map();
            }
        }

        void GET_GP_Airline_Gst_Party_Map()
        {
            gstPartyMapList = new List<GP_Airline_Gst_Party_Map>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.xxgp_airline_gst_party_map_v";
                using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    foreach(DataRow dr in dt.Rows)
                    {
                        GP_Airline_Gst_Party_Map party = new GP_Airline_Gst_Party_Map();
                        party.AIRLINE_GST_REG_NUM = Convert.ToString(reader["AIRLINE_GST_REG_NUM"]);
                        party.GPI_GST_REGN_NO = Convert.ToString(reader["GPI_GST_REGN_NO"]);
                        party.PARTY_NAME = Convert.ToString(reader["PARTY_NAME"]);
                        party.PARTY_SITE = Convert.ToString(reader["PARTY_SITE"]);
                        gstPartyMapList.Add(party);
                    }


                }

                    conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    
                };
            };
            
        }

        void  GET_GP_Gst_Doc_Type_Map()
        {
            gstDocTypeMapList = new List<GP_Gst_Doc_Type_Map>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.xxgp_gst_doc_type_map_v";
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    GP_Gst_Doc_Type_Map docType = new GP_Gst_Doc_Type_Map();
                    docType.DOCUMENT_TYPE_IN_PDF = Convert.ToString(reader["DOCUMENT_TYPE_IN_PDF"]);
                    docType.TRANSACTION_TYPE_IN_ERP = Convert.ToString(reader["TRANSACTION_TYPE_IN_ERP"]);
                    gstDocTypeMapList.Add(docType);
                };
            };
        }

        void  GET_GP_Gpi_Gst_OU_Map()
        {
            gstOUMapList = new List<GP_Gpi_Gst_OU_Map>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.xxgp_gpi_gst_ou_map_v";
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    GP_Gpi_Gst_OU_Map ouMap = new GP_Gpi_Gst_OU_Map();
                    ouMap.ADJUSTMENT_ACCOUNT = Convert.ToString(reader["ADJUSTMENT_ACCOUNT"]);
                    ouMap.GPI_GST_REGN_NUMBER = Convert.ToString(reader["GPI_GST_REGN_NUMBER"]);
                    ouMap.LOCATION_NAME = Convert.ToString(reader["LOCATION_NAME"]);
                    ouMap.OPERATING_UNIT = Convert.ToString(reader["OPERATING_UNIT"]);
                    ouMap.ORGANIZATION_NAME = Convert.ToString(reader["ORGANIZATION_NAME"]);
                    ouMap.UTGST_OR_SGST = Convert.ToString(reader["UTGST_OR_SGST"]);
                    gstOUMapList.Add(ouMap);
                };
            };
        }
        void  GET_GP_Gst_Sac_Code_Map()
        {
            gstSacCodeMapList = new List<GP_Gst_Sac_Code_Map>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.xxgp_gst_sac_code_map_v";
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    GP_Gst_Sac_Code_Map sacCode = new GP_Gst_Sac_Code_Map();
                    sacCode.GST_ENTRY = Convert.ToString(reader["GST_ENTRY"]);
                    sacCode.ITEM_CLASS = Convert.ToString(reader["ITEM_CLASS"]);
                    sacCode.ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]);
                    sacCode.SAC_CODE = Convert.ToString(reader["SAC_CODE"]);
                    gstSacCodeMapList.Add(sacCode);
                };
            };
        }

        void  GET_GP_Gst_Tax_Rate_Map()
        {
            gstTaxRateMapList = new List<GP_Gst_Tax_Rate_Map>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.xxgp_gst_tax_rate_map_v";
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    GP_Gst_Tax_Rate_Map rateMap = new GP_Gst_Tax_Rate_Map();
                    rateMap.GPI_GST_REG_NUMBER = Convert.ToString(reader["GPI_GST_REG_NUMBER"]);
                    rateMap.TAX_RATE_ID = Convert.ToString(reader["TAX_RATE_ID"]);
                    rateMap.TAX_RATE_PERCENTAGE = Convert.ToString(reader["TAX_RATE_PERCENTAGE"]);
                    rateMap.TAX_TYPE_IN_PDF = Convert.ToString(reader["TAX_TYPE_IN_PDF"]);
                    gstTaxRateMapList.Add(rateMap);
                };
            };
        }


        void  GET_GP_Gst_Tax_Type_Map()
        {
            taxTypeMapList = new List<GP_Gst_Tax_Type_Map>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.xxgp_gst_tax_type_map_v";
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    GP_Gst_Tax_Type_Map taxType = new GP_Gst_Tax_Type_Map();
                    taxType.Tax_Type_in_ERP = Convert.ToString(reader["Tax_Type_in_ERP"]);
                    taxType.Tax_Type_in_PDF = Convert.ToString(reader["Tax_Type_in_PDF"]);                    
                    taxTypeMapList.Add(taxType);
                };
            };
        }

        public void Dispose()
        {
            
        }
    }
}
