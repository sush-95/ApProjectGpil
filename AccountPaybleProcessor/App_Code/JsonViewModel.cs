using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountPaybleProcessor.App_Code
{
    public class JsonViewModel
    {
        public JsonViewModel()
        {
            Header = new Header();
            Detail = new ViewPdfModel();
            Status = new Status();
        }
        public Header Header{ get; set; }
        public ViewPdfModel Detail { get; set; }
        public string Error { get; set; }
        public Status Status { get; set; }


    }
    public class Header
    {
        public string ProcessInstanceId { get; set; }
        public string ProcessName { get; set; }
        public string CreatedTs { get; set; }
        public string StateId { get; set; }
    }
    public class Status
    {
        public string Value { get; set; }
        public string Description { get; set; }
      
    }

    public class MailDownloadViewModel
    {
        public string FromMailId { get; set; }
        public string FileName { get; set; }
    }
}
