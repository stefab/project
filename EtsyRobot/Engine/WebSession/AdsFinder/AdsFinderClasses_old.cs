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


namespace EtsyRobot.Engine.WebSession.AdsFinder
{
    internal class _Ad_old
    {
        // Parse ad content from dom in current scope

        private IWebElement _elem;
        private _ShowContentFrame_old _parent_show_content_frame = null;
        private bool _displayed = false;
        private IWebElement _image_elem = null;
        private int _width = -1;
        private int _height = -1;
        private string _outer_html = "";
        private int _data_id = -1;

        public _Ad_old(IWebElement elem)
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
                // own ads => ./a/img or .//img[contains(@src, "adtrustmedia.com")]
                this._outer_html = _elem.GetAttribute("outerHTML");
                this._data_id =  int.Parse(_elem.GetAttribute("data-id"));
                ReadOnlyCollection<IWebElement> elements = _elem.FindElements(By.XPath("./a/img"));
                if (elements.Count > 0)
                {
                    this._image_elem = elements[0];
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

        public _ShowContentFrame_old parentShowContentFrame
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


    internal class _ShowContentFrame_old
    {
        /**
         *  Ad frame inserted by safecontent script
         *  Parse ad content from dom in current scope
         *  <iframe id="932fef6ad115064ae6b22f8c9c416713" height="250" frameborder="0" width="300" src="http://ads.adtrustmedia.com/show_content.php?id=3_201506021951_d1699490dc4fe28a411f77ee6afe65f0"
         *   scrolling="no" style="background-color: transparent; height: 250px; width: 300px;">
         */

        private IWebElement _elem;
        private IBrowserSession _session;
        private IWebDriver _webDriver;
        private bool _displayed = false;
        private int _width = -1;
        private int _height = -1;
        private List<_Ad_old> _ads;
        // ids of the parents  for current frame 
        private Dictionary<string, string> _parentIds;
        
        // data from safe_script
        private _SafeScript_old _parentSafeScript = null;
        private int _safeWidth = -1;
        private int _safeHeight = -1;
        private string _advert = "";
        private string _ta_divid = "";
        private int _data_id = -1;

        public _ShowContentFrame_old(IWebElement elem, IBrowserSession session, IWebDriver webDriver)
        {
            _elem = elem;
            _session = session;
            _webDriver = webDriver;
            _ads = new List<_Ad_old>();
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
        public int data_id  { get { return this._data_id; } }

        // sets this property from link function
        public _SafeScript_old parentSafeScript
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

        public IList<_Ad_old> getAds()
        {
            return _ads;
        }

        public void extractFrameInfo()
        {
            try
            {
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
                try
                {
                    _webDriver.SwitchTo().Frame(_elem);
                    // find inserted ads
                    foreach (IWebElement adElem in _webDriver.FindElements(By.XPath(".//*[@id=\"banner\"]")))
                    {
                        _Ad_old _ad = new _Ad_old(adElem);
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


    internal class _SafeScript_old
    {
        /**
         *  Parse data from safe content script element
         *  <script type="text/javascript" src="//ads.adtrustmedia.com/safecontent.php?width=300&height=250&
         *     adtype=image&method=js&advert=www.googletagservices.com&ts=1433289017421&tz=GMT%2B0300&cb=11384594198&charset=UTF-8&referer=http%3A%2F%2Fwww.dawn.com%2F&cookie_enabled=1&logonly=&ta_affiliateid=48&ta_insid=d30d5ae2-9970-af1c-ccff-dfa7f49c0201&
         *     ta_divid=div-gpt-ad-1381496974053-0_sub&pv=2.2.0.21&fold=above&ctype=clean&page_id=92062b1be7e723b749694bb004661a51">
         */
        private IWebElement _elem;
        public int width = -1;
        public int height = -1;
        public string advert = "";
        public string ta_divid = "";
        public string src = "";
        public string adtype = "";
        public int data_id = -1;

        public _SafeScript_old(IWebElement elem)
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
                if (src.StartsWith("//"))
                {
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
