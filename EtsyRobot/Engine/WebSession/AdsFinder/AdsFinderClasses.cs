using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

using System.Web;
using System.Collections.Specialized;
using System.Diagnostics;


namespace EtsyRobot.Engine.WebSession.AdsFinder
{
    internal class _Ad
    {
        // Parse ad content from dom in current scope

        private IWebElement _elem;
        private _ShowContentFrame _parent_show_content_frame = null;
        private bool _displayed = false;
        private IWebElement _image_elem = null;
        private int _width = -1;
        private int _height = -1;
        private string _outer_html = "";
        private int _data_id = -1;

        public _Ad(IWebElement elem)
        {
            this._elem = elem;
            this.extractAdInfo();
        }

        public IWebElement webElem
        {
            get { return this._elem; }
        }

        protected void extractAdInfo()
        {
            try
            {
                this._displayed = _elem.Displayed;
                // <div id="banner">
                //     <a href="http://ads.adtrustmedia.com/click.php?id=1_201607011200_e5476be5baa0e0b8be80227c553e984c" target="_blank">
                //         <img src="http://ads.adtrustmedia.com/system/files/images/733/74a87568d5948b4cb81148b1a6d30725.jpg" alt="Test" width="728" height="90">
                //     </a>
                // </div>
                // 
                // xpath
                // own ads => ./a/img or .//img[contains(@src, "adtrustmedia.com")]
                this._outer_html = _elem.GetAttribute("outerHTML");
                int number;
                if (int.TryParse(_elem.GetAttribute("data-id"), out number))
                {
                    this._data_id = number;
                }

                ReadOnlyCollection<IWebElement> elements = _elem.FindElements(By.XPath("./a/img"));
                if (elements.Count > 0)
                {
                    this._image_elem = elements[0];
                    new TraceSource("AdsFinder", SourceLevels.All).TraceEvent(TraceEventType.Verbose, 0, "Image ad find." );
                }
                if (this._image_elem != null)
                {
                    this._width = this._image_elem.Size.Width;
                    this._height = this._image_elem.Size.Height;
                }
                else
                {
                    this._width = int.Parse(this._elem.GetAttribute("offsetWidth"));
                    this._height = int.Parse(this._elem.GetAttribute("offsetHeight"));
                }
            }
            catch (Exception e)
            {

            }
        }

        public _ShowContentFrame parentShowContentFrame
        {
            get { return this._parent_show_content_frame; }
            set
            {
                // may be weakref ?
                _parent_show_content_frame = value;
            }
        }

        public override string ToString()
        {
            string _str = String.Format("w=%d, h=%d, displayed=%s, outer_html=%s", _width, _height, _displayed, _outer_html);
            if (_parent_show_content_frame != null)
            {
                _str = _str + "\nFrame:" + _parent_show_content_frame.ToString();
            }

            return _str;
        }

        //public Dictionary<string, string> getAsDict()
        //{
        //    Dictionary<string, string> FD = (from x in this.GetType().GetProperties() select x).ToDictionary(x => x.Name, x => (x.GetGetMethod().Invoke(this, null) == null ? "" : x.GetGetMethod().Invoke(this, null).ToString()));
        //    return FD;
        //}

        public Dictionary<string, string> getAsDict()
        {
            Dictionary<string, string> FD = new Dictionary<string, string>
            {
                {"width", this._width.ToString()},
                {"height", this._height.ToString()},
                {"displayed", this._displayed.ToString()}
            };
            if (this._parent_show_content_frame != null)
            {
                FD.Add("frame_data", this._parent_show_content_frame.ToString());
            }
            return FD;
        }

    }


    internal class _ShowContentFrame
    {
        private IWebElement _elem;
        private IBrowserSession _session;
        private IWebDriver _webDriver;
        private bool _displayed = false;
        private int _width = -1;
        private int _height = -1;
        private List<_Ad> _ads;
        // ids of the parents  for current frame 
        private Dictionary<string, string> _parentIds;
        
        // data from safe_script
        private _SafeScript _parentSafeScript = null;
        private int _safeWidth = -1;
        private int _safeHeight = -1;
        private string _advert = "";
        private string _ta_divid = "";
        private int _data_id = -1;

        public _ShowContentFrame(IWebElement elem, IBrowserSession session, IWebDriver webDriver)
        {
            _elem = elem;
            _session = session;
            _webDriver = webDriver;
            _ads = new List<_Ad>();
            _parentIds = new Dictionary<string, string>();
            this.extractFrameInfo();
        }

        public bool isParent(string id)
        {
            return _parentIds.ContainsKey(id);
        }

        public IWebElement webElem { get { return this._elem; } }
        public bool Displayed { get { return this._displayed; } }
        public int Width { get { return this._width; } } 
        public int Height { get { return this._height; } }
        public int SafeWidth  { get { return this._safeWidth; } }
        public int SafeHeight { get { return this._safeHeight; } }
        public string Advert  { get { return this._advert; } }
        public string ta_divid { get { return this._ta_divid; } }
        // sets by injected script
        public int data_id  { get { return this._data_id; } }

        // sets this property from link function
        public _SafeScript parentSafeScript
        {
            get { return this._parentSafeScript; }
            set
            {
                this._parentSafeScript = value;
                this._safeHeight = _parentSafeScript.height;
                this._safeWidth = _parentSafeScript.width;
                this._advert = _parentSafeScript.advert;
                this._ta_divid = _parentSafeScript.ta_divid;
            }
        }

        public IList<_Ad> getAds()
        {
            return _ads;
        }

        public void extractFrameInfo()
        {
            try
            {
                new TraceSource("AdsFinder", SourceLevels.All).TraceEvent(TraceEventType.Verbose, 0, "In ShowContent frame.");
                int number;
                _displayed = _elem.Displayed;
                if (int.TryParse(_elem.GetAttribute("data-id"), out number)) {
                    this._data_id = number;
                }
                _width = int.Parse(this._elem.GetAttribute("offsetWidth"));
                _height = int.Parse(this._elem.GetAttribute("offsetHeight"));
                // get parents ids
                // //*[@class='logo']/ancestor-or-self::*[@id]
                ReadOnlyCollection<IWebElement> _ids = _elem.FindElements(By.XPath(".//ancestor-or-self::*[@id]"));
                foreach (IWebElement _id in _ids)
                {
                    this._parentIds[_id.GetAttribute("id")] = _id.TagName;
                }

                // switch to current frame dom
                _webDriver.SwitchTo().Frame(_elem);
                try
                {
                    // <iframe src="http://ads.adtrustmedia.com/show_content.php?id=1_201607011200_0655ac19939c1049eea25c2a3a078793&amp;a=p7NdJ0KeDifNJJB5sVk0kK1k8T9K0R8SBRkrOx/GBrTpqTgvugOvRLijhwmhqQP+gsye0s0UNrGCdRJrdum6fg==" 
                    //    width="728" height="90" scrolling="no" allowtransparency="true" frameborder="0" style=";backgroundColor: transparent!important;width: 728px!important;height: 90px!important">
                    //    <html><body>
                    //      <!-- HERE Our ad --> 
                    //      <div id="banner">
                    //        <a href="http://ads.adtrustmedia.com/click.php?id=1_201607011200_e5476be5baa0e0b8be80227c553e984c" target="_blank">
                    //          <img src="http://ads.adtrustmedia.com/system/files/images/733/74a87568d5948b4cb81148b1a6d30725.jpg" alt="Test" width="728" height="90">
                    //         </a>
                    //      </div>
                    //      <script> ... </script>
                    //      <img src="/images/1x1.jpg?id=1_201607011200_e5476be5baa0e0b8be80227c553e984c" alt="" width="1" height="1" style="display:none">
                    //     </body></html>
                    // </iframe>
                    _webDriver.SwitchTo().Frame(_elem);
                    new TraceSource("AdsFinder", SourceLevels.All).TraceEvent(TraceEventType.Verbose, 0, "Parse ShowContent frame.");
                    // find inserted ads
                    foreach (IWebElement adElem in _webDriver.FindElements(By.XPath(".//*[@id=\"banner\"]")))
                    {
                        new TraceSource("AdsFinder", SourceLevels.All).TraceEvent(TraceEventType.Verbose, 0, "Banner element found.");
                        _Ad _ad = new _Ad(adElem);
                        _ad.parentShowContentFrame = this;
                        this._ads.Add(_ad);
                    }
                }
                finally
                {
                    _webDriver.SwitchTo().ParentFrame();
                }
            }
            catch (Exception e)
            {
                new TraceSource("AdsFinder", SourceLevels.All).TraceEvent(TraceEventType.Verbose, 0, "Error while Parse ShowContent frame." + e.Message);
            }
        }

        public Dictionary<string, string> getAsDict()
        {
            Dictionary<string, string> FD = new Dictionary<string, string>
            {
                {"width", this._width.ToString()},
                {"height", this._height.ToString()},
                {"advert", this._advert},
                {"ta_divid", this._ta_divid},
                {"displayed", this._displayed.ToString()},
                {"safe_width", this._safeWidth.ToString()},
                {"safe_height", this._safeHeight.ToString()},
                {"data_id", this._data_id.ToString()}
            };
            return FD;
        }
    }
    
    internal class _SafeScript
    {
        /** 
         * Parse data from safe content script element
         *  <script type="text/javascript" async="" src="//ads.adtrustmedia.com/safecontent.php?width=728&amp;
         *      height=90&amp;adtype=image&amp;method=js&amp;advert=www.googletagservices.com&amp;ts=1467368551211&amp;tz=GMT%2B0300&amp;cb=98619129681&amp;charset=windows-1251&amp;referer=http%3A%2F%2Fobkom.net.ua%2F&amp;cookie_enabled=1&amp;logonly=&amp;
         *      ta_affiliateid=10001005&amp;ta_insid=TRUE_COMPUTER_ID_VALUE&amp;
         *      ta_divid=div-gpt-ad-1456223961795-0&amp;pv=1.5.7.41&amp;fold=above&amp;ctype=clean&amp;
         *      page_id=44c5be2535364920f6469cf491f0029b&amp;refactored=1&amp;
         *      callback=cta_linr.safecontent.deliveryDefault">
         *  </script>
         */                       
        private IWebElement _elem;
        public int width = -1;
        public int height = -1;
        public string advert = "";
        public string ta_divid = "";
        public string src = "";
        public string adtype = "";
        public int data_id = -1;

        public _SafeScript(IWebElement elem)
        {
            int number;
            _elem = elem;
            src = _elem.GetAttribute("src");
            if (int.TryParse(_elem.GetAttribute("data-id"), out number))
            {
                this.data_id = number;
            }
            if (src != null)
            {
                if (src.StartsWith("//")) {
                    src = "http:" + src;
                }
                Uri _uri = new Uri(src);
                NameValueCollection _params = HttpUtility.ParseQueryString(_uri.Query);
                width = _params["width"] != null ? int.Parse(_params["width"]) : -1;
                height = _params["height"] != null ? int.Parse(_params["height"]) : -1;
                adtype = _params["adtype"] != null ? _params["adtype"] : "";
                ta_divid = _params["ta_divid"] != null ? _params["ta_divid"] : "";
                advert = _params["advert"] != null ? _params["advert"] : "";
            }
        }

        public IWebElement webElem
        {
            get { return this._elem; }
        }

        public Dictionary<string, string> getAsDict()
        {
            Dictionary<string, string> FD = new Dictionary<string, string>
            {
                {"width", this.width.ToString()},
                {"height", this.height.ToString()},
                {"advert", this.advert},
                {"ta_divid", this.ta_divid},
                {"adtype", this.adtype}
            };
            return FD;
        }
    }

}
