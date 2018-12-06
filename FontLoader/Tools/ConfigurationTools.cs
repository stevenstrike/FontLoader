using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace ToolsProject.Tools
{
    public class ConfigurationTools
    {
        /// <summary>
        /// The font full path
        /// </summary>
        private string _fontFullPath = "";
        public string fontFullPath
        {
            get { return _fontFullPath; }
            set { _fontFullPath = value; }
        }

        /// <summary>
        /// The font file name
        /// </summary>
        public string fontFileName
        {
            get { return fontFullPath.Split('\\').Last(); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks></remarks>
        public ConfigurationTools()
        {
            loadConfiguration();
        }

        /// <summary>
        /// Load configuration data from app.config
        /// </summary>
        /// <remarks></remarks>

        private void loadConfiguration()
        {
            try
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["fontFullPath"]))
                {
                    fontFullPath = ConfigurationManager.AppSettings["fontFullPath"];
                }
                else
                {
                    throw new ArgumentNullException("'fontFullPath' application settings shouldn't be empty.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}