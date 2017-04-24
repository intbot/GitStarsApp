using Plugin.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApp
{
    public class Util
    {
        public static async Task OpenBrowser(string url)
        {
            await CrossShare.Current.OpenBrowser(url);
        }
    }
}
