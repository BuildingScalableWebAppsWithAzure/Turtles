using System.Configuration;
using System.Web.Mvc;

namespace Turtles.Web
{
    public static class CDNHelper
    {
        public static string CDN(this HtmlHelper helper, string imageNameAndPath)
        {
            bool useCDN = bool.Parse(ConfigurationManager.AppSettings["UseCDN"]);
            useCDN = true;
            if (useCDN)
            {
                //we ARE using the CDN. 
                var cdnHostName = ConfigurationManager.AppSettings["CDNHostName"];
                string cdnHostAndPath = cdnHostName + imageNameAndPath;

                //cache bustin'. To generate a unique query string, we'll append the 
                //assembly version number to the image URL.
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName()
                    .Version.ToString();
                cdnHostAndPath += "?v=" + version;
                return cdnHostAndPath;
            }
            else
            {
                //Use the relative path. We're not using the CDN. 
                return imageNameAndPath;
            }
        }
    }
}
