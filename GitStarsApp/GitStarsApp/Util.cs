using Plugin.Share;
using Plugin.Share.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitStarsApp
{
    public class Util
    {
        public static async Task OpenBrowser(string url)
        {
            var options = new BrowserOptions()
            {
                ChromeToolbarColor = new ShareColor(0, 191, 255)
            };

            await CrossShare.Current.OpenBrowser(url, options);
        }
    }
}
