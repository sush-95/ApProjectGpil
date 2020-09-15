using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineProcessorConsole
{
    public static class ProcessLogger
    {
       static string FileContent = "==============================================================" + Environment.NewLine;
        public static void WriteLogger(DateTime start,DateTime end)
        {
            FileContent += "Service  Started : " + start.ToString()+Environment.NewLine;
            FileContent += "Duration : " + (end - start).Seconds.ToString()+" Seconds"+Environment.NewLine;
            FileContent += "Service  Ended : " + end.ToString() + Environment.NewLine;
            FileContent += "==============================================================" + Environment.NewLine;

            string Filepath = ConfigurationManager.AppSettings["Logfile"].ToString();
            if (!Directory.Exists(Filepath))
            {
                Directory.CreateDirectory(Filepath);
            }
            Filepath += DateTime.Today.ToString("ddMMyyyy")+"AP_Log.txt";          
            if (File.Exists(Filepath))
            {
                File.AppendAllText(Filepath, FileContent);
            }
            else
            {
                File.Create(Filepath);
                File.Open(Filepath,FileMode.Open);
                File.WriteAllText(Filepath, FileContent);
            }
        }
    }
}
