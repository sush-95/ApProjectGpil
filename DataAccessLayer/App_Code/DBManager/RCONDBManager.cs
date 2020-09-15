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
    class RCONDBManager : IDisposable
    {
        public List<RCON_Tr_Reconciliation> rconRNList;
        public List<RCON_Rn_Od_Rqst> rconCheckList;
        public List<RCON_Party_List> rconPartyList;
        public List<RCON_VoucherType_List> rconVoucherTypeList;

        //string connString = System.Configuration.ConfigurationManager.AppSettings["ERPDB"];
        string connString = "";
        public RCONDBManager()
        {
            //if (rconRNList == null || rconRNList.Count == 0)
            //{
            //    GET_RCON_Reconciliation_List();
            //}
            //if (rconCheckList == null || rconCheckList.Count == 0)
            //{
            //    GET_RCON_RN_Check_List();
            //}
            //if (rconPartyList == null || rconPartyList.Count == 0)
            //{
            //    GET_RCON_Party_List();
            //}
            //if (rconVoucherTypeList == null || rconVoucherTypeList.Count == 0)
            //{
            //    GET_RCON_VoucherType_List();
            //}
        }

        void GET_RCON_Reconciliation_List()
        {
            rconRNList = new List<RCON_Tr_Reconciliation>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.tr_reconciliations_s";
                using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        RCON_Tr_Reconciliation party = new RCON_Tr_Reconciliation();
                        party.REQUEST_NUMBER = Convert.ToString(dr["REQUEST_NUMBER"]);
                        party.AGENCY_INVNO = Convert.ToString(dr["AGENCY_INVNO"]);
                        party.TICKET_NO = Convert.ToString(dr["TICKET_NO"]);
                        rconRNList.Add(party);
                    }
                }
            };
        }

        void GET_RCON_RN_Check_List()
        {
            rconCheckList = new List<RCON_Rn_Od_Rqst>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.xxgp_ea_rn_od_rqst_v";
                using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        RCON_Rn_Od_Rqst party = new RCON_Rn_Od_Rqst();
                        party.TA_NUMBER = Convert.ToString(dr["TA_NUMBER"]);
                        party.EMPLOYEE_ID = Convert.ToString(dr["EMPLOYEE_ID"]);
                        party.USER_NAME = Convert.ToString(dr["USER_NAME"]);
                        party.STATUS = Convert.ToString(dr["STATUS"]);
                        party.EMPLOYEE_CODE = Convert.ToString(dr["EMPLOYEE_CODE"]);
                        //party.LOCATION = Convert.ToString(dr["LOCATION"]);
                        rconCheckList.Add(party);
                    }
                }
            };
        }

        void GET_RCON_Party_List()
        {
            rconPartyList = new List<RCON_Party_List>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.xxgp_beacon_patry_name_orgs";
                using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        RCON_Party_List party = new RCON_Party_List();
                        party.PARTY_NAME = Convert.ToString(dr["PARTY_NAME"]);
                        party.ORG_ID = Convert.ToString(dr["ORG_ID"]);
                        rconPartyList.Add(party);
                    }
                }
            };
        }

        void GET_RCON_VoucherType_List()
        {
            rconVoucherTypeList = new List<RCON_VoucherType_List>();
            using (var conn = new OracleConnection(connString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from apps.xxgp_beacon_inv_types";
                using (OracleDataAdapter dap = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        RCON_VoucherType_List party = new RCON_VoucherType_List();
                        party.INV_TYPE = Convert.ToString(dr["INV_TYPE"]);
                        party.TAG = Convert.ToString(dr["TAG"]);
                        rconVoucherTypeList.Add(party);
                    }
                }
            };
        }
        
        public void Dispose()
        {

        }
    }
}
