using System.Web;
using System.Web.Optimization;
using System.Configuration; 

namespace Turtles.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //we have to go ahead create and add our Bundles as if there is no CDN involved.
            //this is because the bundle has to already exist in the BundleCollection
            //in order to get the hash that the MVC framework will generate for the
            //querystring. 
            Bundle jsBundle = new ScriptBundle("~/scripts/js").Include(
               "~/Scripts/jquery-{version}.js",
               "~/Scripts/modernizr-*",
               "~/Scripts/bootstrap.js",
               "~/Scripts/respond.js");


            Bundle cssBundle = new StyleBundle("~/content/css").Include(
               "~/Content/bootstrap.css",
               "~/Content/site.css");

            bundles.Add(jsBundle);
            bundles.Add(cssBundle);

            bool useCDN = bool.Parse(ConfigurationManager.AppSettings["UseCDN"]);
            if (useCDN)
            {
                //only execute this code if we are NOT in debug configuration. 
                bundles.UseCdn = true;
                //grab our base CDN hostname from web.config...
                string cdnHost = ConfigurationManager.AppSettings["CDNHostName"];

                //get the hashes that the MVC framework will use per bundle for 
                //the querystring. 
                string jsHash = GetBundleHash(bundles, "~/scripts/js");
                string cssHash = GetBundleHash(bundles, "~/content/css");

                //set up our querystring per bundle for the CDN path. 
                jsBundle.CdnPath = cdnHost + "/scripts/js?v=" + jsHash;
                cssBundle.CdnPath = cdnHost + "/content/css?v=" + cssHash;
            }
            else
            {
                bundles.UseCdn = false;
            }
        }

        /// <summary>
        /// This method calculates the bundle hash. The hash is what the MVC framework
        /// appends to the bundle querystring for cache busting. 
        /// Based on the code by Frison B Alexander as shared on Stackoverflow.com.
        /// (http://stackoverflow.com/questions/31540121/get-mvc-bundle-querystring)
        /// Licensed under the Creative Commons license. 
        /// </summary>
        private static string GetBundleHash(BundleCollection bundles, string bundlePath)
        {
            //Need the context to generate response
            var bundleContext = new BundleContext(new
               HttpContextWrapper(HttpContext.Current), BundleTable.Bundles, bundlePath);

            //Bundle class has the method we need to get a BundleResponse
            Bundle bundle = BundleTable.Bundles.GetBundleFor(bundlePath);
            var bundleResponse = bundle.GenerateBundleResponse(bundleContext);

            //BundleResponse has the method we need to call, but its marked as
            //internal and therefor is not available for public consumption.
            //To bypass this, reflect on it and manually invoke the method
            var bundleReflection = bundleResponse.GetType();

            var method = bundleReflection.GetMethod("GetContentHashCode",
               System.Reflection.BindingFlags.NonPublic |
               System.Reflection.BindingFlags.Instance);

            //contentHash is whats appended to your url (url?###-###...)
            var contentHash = method.Invoke(bundleResponse, null);
            return contentHash.ToString();
        }

    }
}
