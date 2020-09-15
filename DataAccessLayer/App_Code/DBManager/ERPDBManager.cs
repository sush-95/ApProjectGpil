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
        public List<GP_Becon_Inv_Rcon> gstBeconRconList;

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

            if (gstBeconRconList == null || gstBeconRconList.Count == 0)
            {
                GET_GP_Becon_Inv_Rcon();
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
                        party.AIRLINE_GST_REG_NUM = Convert.ToString(dr["AIRLINE_GST_REG_NUM"]);
                        party.GPI_GST_REGN_NO = Convert.ToString(dr["GPI_GST_REGN_NO"]);
                        party.PARTY_NAME = Convert.ToString(dr["PARTY_NAME"]);
                        party.PARTY_SITE = Convert.ToString(dr["PARTY_SITE"]);
                        gstPartyMapList.Add(party);
                    }
                }
            };
            
        }

        void  GET_GP_Gst_Doc_Type_Map()
        {
            gstDocTypeMapList = new List<GP_Gst_Doc_Type_Map>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.xxgp_gst_doc_type_map_v";
                using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        GP_Gst_Doc_Type_Map docType = new GP_Gst_Doc_Type_Map();
                        docType.DOCUMENT_TYPE_IN_PDF = Convert.ToString(dr["DOCUMENT_TYPE_IN_PDF"]);
                        docType.TRANSACTION_TYPE_IN_ERP = Convert.ToString(dr["TRANSACTION_TYPE_IN_ERP"]);
                        gstDocTypeMapList.Add(docType);
                    }
                }
               
            };
        }

        void  GET_GP_Gpi_Gst_OU_Map()
        {
            gstOUMapList = new List<GP_Gpi_Gst_OU_Map>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.xxgp_gpi_gst_ou_map_v";
                using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        GP_Gpi_Gst_OU_Map ouMap = new GP_Gpi_Gst_OU_Map();
                        ouMap.ADJUSTMENT_ACCOUNT = Convert.ToString(dr["ADJUSTMENT_ACCOUNT"]);
                        ouMap.GPI_GST_REGN_NUMBER = Convert.ToString(dr["GPI_GST_REGN_NUMBER"]);
                        ouMap.LOCATION_NAME = Convert.ToString(dr["LOCATION_NAME"]);
                        ouMap.OPERATING_UNIT = Convert.ToString(dr["OPERATING_UNIT"]);
                        ouMap.ORGANIZATION_NAME = Convert.ToString(dr["ORGANIZATION_NAME"]);
                        ouMap.UTGST_OR_SGST = Convert.ToString(dr["UTGST_OR_SGST"]);
                        ouMap.LOC_EMAIL_ADD = Convert.ToString(dr["LOC_EMAIL_ADD"]);
                        gstOUMapList.Add(ouMap);
                    }
                }
               
            };
        }

        void  GET_GP_Gst_Sac_Code_Map()
        {
            gstSacCodeMapList = new List<GP_Gst_Sac_Code_Map>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.xxgp_gst_sac_code_map_v";
                using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        GP_Gst_Sac_Code_Map sacCode = new GP_Gst_Sac_Code_Map();
                        sacCode.GST_ENTRY = Convert.ToString(dr["GST_ENTRY"]);
                        sacCode.ITEM_CLASS = Convert.ToString(dr["ITEM_CLASS"]);
                        sacCode.ITEM_NAME = Convert.ToString(dr["ITEM_NAME"]);
                        sacCode.SAC_CODE = Convert.ToString(dr["SAC_CODE"]);
                        gstSacCodeMapList.Add(sacCode);
                    }
                }
               
            };
        }

        void  GET_GP_Gst_Tax_Rate_Map()
        {
            gstTaxRateMapList = new List<GP_Gst_Tax_Rate_Map>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.xxgp_gst_tax_rate_map_v";
                using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        GP_Gst_Tax_Rate_Map rateMap = new GP_Gst_Tax_Rate_Map();
                        rateMap.GPI_GST_REG_NUMBER = Convert.ToString(dr["GPI_GST_REG_NUMBER"]);
                        rateMap.TAX_RATE_ID = Convert.ToString(dr["TAX_RATE_ID"]);
                        //rateMap.TAX_RATE_PERCENTAGE = Convert.ToString(dr["TAX_RATE_PERCENTAGE"]);
                        string ratePer = string.Empty;
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(dr["TAX_RATE_PERCENTAGE"])))
                        {
                            ratePer = Convert.ToString(Math.Round(Convert.ToDouble(dr["TAX_RATE_PERCENTAGE"]), 2, MidpointRounding.AwayFromZero));
                            // rateMap.TAX_RATE_PERCENTAGE = Convert.ToString(dr["TAX_RATE_PERCENTAGE"]);
                        }
                        rateMap.TAX_RATE_PERCENTAGE = ratePer.ToString();
                        rateMap.TAX_TYPE_IN_PDF = Convert.ToString(dr["TAX_TYPE_IN_PDF"]);
                        gstTaxRateMapList.Add(rateMap);
                    }
                }
               
            };
        }
        
        void  GET_GP_Gst_Tax_Type_Map()
        {
            taxTypeMapList = new List<GP_Gst_Tax_Type_Map>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.xxgp_gst_tax_type_map_v";
                using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        GP_Gst_Tax_Type_Map taxType = new GP_Gst_Tax_Type_Map();
                        taxType.Tax_Type_in_ERP = Convert.ToString(dr["Tax_Type_in_ERP"]);
                        taxType.Tax_Type_in_PDF = Convert.ToString(dr["Tax_Type_in_PDF"]);
                        taxTypeMapList.Add(taxType);
                    }
                }
                
            };
        }

        void GET_GP_Becon_Inv_Rcon()
        {
            gstBeconRconList = new List<GP_Becon_Inv_Rcon>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.xxgp_beacon_inv_reco_v";
                using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        GP_Becon_Inv_Rcon beconRconType = new GP_Becon_Inv_Rcon();
                        beconRconType.AGENCY_INVNO = Convert.ToString(dr["AGENCY_INVNO"]);
                        beconRconType.AMOUNT_PAID = Convert.ToString(dr["AMOUNT_PAID"]);
                        beconRconType.TICKET_NO = Convert.ToString(dr["TICKET_NO"]);
                        beconRconType.VOUCHER_NO = Convert.ToString(dr["VOUCHER_NO"]);
                        gstBeconRconList.Add(beconRconType);
                    }
                }

            };
        }

        public void Dispose()
        {
            
        }
    }
}
