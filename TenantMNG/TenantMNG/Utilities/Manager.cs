namespace TenantMNG
{
    using Managment;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    public class Manager
    {
        private const string LicenseFN = "app.dll";
        AppManager appManager = new AppManager();
        

        public Manager()
        {            

            appManager.Server = ConfigurationManager.AppSettings["server"]; 
            appManager.AppPath = GetLicenseFilePath();           
        }


        public bool ExitIfNotSatisfied()
        {
            return appManager.IsValid();            
        }




        private static string GetLicenseFilePath()
        {
            // 1st attempt is license file located in application folder
            var executablePath = System.Web.Hosting.HostingEnvironment.MapPath("~/bin");
            if (String.IsNullOrWhiteSpace(executablePath))
                return null;

            var path = Path.Combine(executablePath, LicenseFN);
            if (File.Exists(path))
                return path;
            else
            {
                using (File.Create(path))

                    return path;
            }



            //try
            //{
            //    // 2nd attempt is license file located in shared documents folder
            //    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
            //        LicenseFN);
            //}
            //catch (PlatformNotSupportedException)
            //{
            //    // 3rd attempt...something weird, I can't find SpecialFolder.CommonDocuments
            //    return Path.Combine(Environment.CurrentDirectory, LicenseFN);
            //}
        }


    }
}