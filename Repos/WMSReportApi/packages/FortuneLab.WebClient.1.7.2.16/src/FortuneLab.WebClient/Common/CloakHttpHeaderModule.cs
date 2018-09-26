using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FortuneLab.WebClient.Common
{
    /// <summary>
    /// Custom HTTP Module for Cloaking IIS7 Server Settings to allow anonymity
    /// </summary>
    public class CloakHttpHeaderModule : IHttpModule
    {
        /// <summary>
        /// List of Headers to remove
        /// </summary>
        private List<string> headersToCloak;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloakHttpHeaderModule"/> class.
        /// </summary>
        public CloakHttpHeaderModule()
        {
            this.headersToCloak = new List<string>
                                      {
                                              "Server",
                                              "X-AspNet-Version",
                                              "X-AspNetMvc-Version",
                                              "X-Powered-By",
                                      };
        }

        /// <summary>
        /// Dispose the Custom HttpModule.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Handles the current request.
        /// </summary>
        /// <param name="context">
        /// The HttpApplication context.
        /// </param>
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += this.OnPreSendRequestHeaders;
        }

        /// <summary>
        /// Remove all headers from the HTTP Response.
        /// </summary>
        /// <param name="sender">
        /// The object raising the event
        /// </param>
        /// <param name="e">
        /// The event data.
        /// </param>
        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            this.headersToCloak.ForEach(h => HttpContext.Current.Response.Headers.Remove(h));
        }
    }
}
