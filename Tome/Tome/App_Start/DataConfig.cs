using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Tome.App_Start
{
    public class DataConfig
    {

        public static String FolderPath { get; set; } = Assembly.GetExecutingAssembly().Location + "tomes";
        public static void ConfigureTomeDirectory()
        {
            Directory.CreateDirectory(FolderPath);
        }


    }
}