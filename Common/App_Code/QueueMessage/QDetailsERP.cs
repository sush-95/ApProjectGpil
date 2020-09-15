using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Constants;

namespace Common.App_Code.QueueMessage
{
    public class QDetailsERP : QDetails
    {
        public string Url { get; set; }
        public string OperatingUnit { get; set; }
        public string OrganisationName { get; set; }
        public string TransactionType { get; set; }
        public string PartyName { get; set; }
        public string PartySite { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string ItemClass { get; set; }
        public string ItemName { get; set; }
        public string Remarks { get; set; }
        public string CGSTTaxType { get; set; }
        public string SGSTTaxType { get; set; }
        public string IGSTTaxType { get; set; }
        public string TaxableAmount { get; set; }
        public string AccessibleValue { get; set; }
        public string CGSTTaxRateCode { get; set; }
        public string SGSTTaxRateCode { get; set; }
        public string IGSTTaxRateCode { get; set; }
        public string AdjustmentAmount { get; set; }
        public string Location { get; set; }

        protected override string DetailString()
        {

            string json = "{";
            json += Utils.JElement(JSON.Tags.ERP.Url, Url) + ",";
            json += Utils.JElement(JSON.Tags.ERP.OperatingUnit, OperatingUnit) + ",";
            json += Utils.JElement(JSON.Tags.ERP.OrganisationName, OrganisationName) + ",";
            json += Utils.JElement(JSON.Tags.ERP.TransactionType, TransactionType) + ",";
            json += Utils.JElement(JSON.Tags.ERP.PartyName, PartyName) + ",";
            json += Utils.JElement(JSON.Tags.ERP.PartySite, PartySite) + ",";
            json += Utils.JElement(JSON.Tags.ERP.InvoiceNumber, InvoiceNumber) + ",";
            json += Utils.JElement(JSON.Tags.ERP.InvoiceDate, InvoiceDate) + ",";
            json += Utils.JElement(JSON.Tags.ERP.ItemClass, ItemClass) + ",";
            json += Utils.JElement(JSON.Tags.ERP.ItemName, ItemName) + ",";
            json += Utils.JElement(JSON.Tags.ERP.Remarks, Remarks) + ",";
            json += Utils.JElement(JSON.Tags.ERP.CGSTTaxType, CGSTTaxType) + ",";
            json += Utils.JElement(JSON.Tags.ERP.SGSTTaxType, SGSTTaxType) + ",";
            json += Utils.JElement(JSON.Tags.ERP.IGSTTaxType, IGSTTaxType) + ",";
            json += Utils.JElement(JSON.Tags.ERP.TaxableAmount, TaxableAmount) + ",";
            json += Utils.JElement(JSON.Tags.ERP.AccessibleValue, AccessibleValue) + ",";
            json += Utils.JElement(JSON.Tags.ERP.CGSTTaxRateCode, CGSTTaxRateCode) + ",";
            json += Utils.JElement(JSON.Tags.ERP.SGSTTaxRateCode, SGSTTaxRateCode) + ",";
            json += Utils.JElement(JSON.Tags.ERP.IGSTTaxRateCode, IGSTTaxRateCode) + ",";
            json += Utils.JElement(JSON.Tags.ERP.Location, Location) + ",";
            json += Utils.JElement(JSON.Tags.ERP.AdjustmentAmount, AdjustmentAmount);



            json += "}";
            return json;
        }

        protected override void CreateFromJSON(string json)
        {

        }

        protected override string DetailStringForBeat()
        {
            return "";
        }


    }
}
