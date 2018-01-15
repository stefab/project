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
    class AdsFinder_old
    {
        protected DefaultBrowserSession _session;
        private IWebDriver _webDriver;
        protected List<_Ad> _lstAds;
        protected List<_ShowContentFrame> _lstShowContentFrames;
        protected List<_SafeScript> _lstSafeScripts;

        public AdsFinder_old(DefaultBrowserSession session, IWebDriver webDriver)
        {
            _session = session;
            _webDriver = webDriver;
            _lstAds = new List<_Ad>();
            _lstShowContentFrames = new List<_ShowContentFrame>();
            _lstSafeScripts = new List<_SafeScript>();
        }

        protected void resetResults()
        {
            _lstAds.Clear();
            _lstShowContentFrames.Clear();
            _lstSafeScripts.Clear();
        }

        public IList<_ShowContentFrame> findReplacedAds()
        {
            // search ad blocks
            this.resetResults();
            this._webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(100);
            this._findAllReplacedAdsRecursion(this._webDriver, 0);
            this._linkAds();
            return _lstShowContentFrames;
        }

        protected void _findAllReplacedAdsRecursion(ISearchContext topWebElement, int lvl = 0)
        {
            // Iterate over iframes tree
            // must not change context (switch_to)

            // switch to current frame context
            if (topWebElement is IWebDriver)
            {
                ((IWebDriver)topWebElement).SwitchTo().DefaultContent();
            }
            else if ((topWebElement is IWebElement) && string.Equals(((IWebElement)topWebElement).TagName, "iframe", StringComparison.OrdinalIgnoreCase))
            {
                this._webDriver.SwitchTo().Frame((IWebElement)topWebElement);
            }
            try
            {  
                // iterate over all frames
                ReadOnlyCollection<IWebElement> _curFrames = this._webDriver.FindElements(By.XPath("//iframe"));
                // find all objects in current scope
                Tuple<List<_Ad>, List<_SafeScript>, List<_ShowContentFrame>> _result = this._findAdsInCurrentScope();
                this._lstAds.AddRange(_result.Item1);
                this._lstSafeScripts.AddRange(_result.Item2);
                this._lstShowContentFrames.AddRange(_result.Item3);
                foreach (IWebElement _curFrame in  _curFrames) {
                    string frmtStr = new string('-', 2 * lvl);
                    //Console.WriteLine(frmtStr + "frame: " + _curFrame.GetAttribute("outerHTML"));
                    this._findAllReplacedAdsRecursion(_curFrame, ++lvl);
                }
            }
            finally
            {
                // restore current parent frame
                this._webDriver.SwitchTo().ParentFrame();
            }

        }

        protected void _linkAds()
        {
            foreach (_SafeScript curSafeScript in this._lstSafeScripts) {
                string divid = curSafeScript.ta_divid;
                if (divid != "") {
                    foreach (_ShowContentFrame curContentFrame in this._lstShowContentFrames) {
                        if (curContentFrame.isParent(divid)) {
                            curContentFrame.parentSafeScript = curSafeScript;
                        }
                    }
                }
            }
        }

        protected Tuple<List<_Ad>, List<_SafeScript>, List<_ShowContentFrame>> _findAdsInCurrentScope()
        {
            // find safe scripts, frames and ads objects in current dom scope
            //
            // <div id="banner"> - ads signature
            // xpath for search ads
            // //*[@id="banner"]
            // /html/body/div/a
            // /html/body/div/a/img
            // ------------------------------------
            // 1. find .//script[contains(@src, '//ads.adtrustmedia.com/safecontent.php?')]
            // 2. parse ta_divid from query
            //
            // 3. find frames with .//iframe[contains(@src, '//ads.adtrustmedia.com/show_content.php?')]
            // 4. in this frame find //*[@id="banner"]
            // 5. find parents ids for show_content frame
            // 6. link show_content frame to safecontent script by ta_divid
            // ------------------------------------
            // dawn.com example :
            // <script type="text/javascript" src="//ads.adtrustmedia.com/safecontent.php?width=300&amp;height=250&amp;adtype=image&amp;method=js&amp;advert=www.googletagservices.com&amp;
            //     ts=1433255481468&amp;tz=GMT%2B0300&amp;cb=28381881810&amp;charset=UTF-8&amp;referer=http%3A%2F%2Fwww.dawn.com%2F&amp;cookie_enabled=1&amp;logonly=&amp;
            //     ta_affiliateid=48&amp;ta_insid=d30d5ae2-9970-af1c-ccff-dfa7f49c0201&amp;
            //     ta_divid=div-gpt-ad-1381496974053-0_sub&amp;pv=2.2.0.21&amp;fold=above&amp;ctype=clean&amp;page_id=0990cba169f9c8f58d280971fe4d4655"></script>
            // <div id="div-gpt-ad-1381496974053-0" class="ad" style="width: 300px; height: 250px; transform-origin: 0px 0px 0px; position: absolute; transform: scale(0.999);" name="cta_divforc"
            //     <div>
            //         <div id="div-gpt-ad-1381496974053-0_sub" >
            //             <iframe height="250" frameborder="0" width="300" src="http://ads.adtrustmedia.com/show_content.php?id=1_201506021115_10e00d976b60f2dd0331bbad35c1a307" id="2c48873eccbb1bd87ffde607d3b421e2"
            //                    scrolling="no" style="background-color: transparent; height: 250px; width: 300px;">
            //                 <html>
            //                     <head>...</head>
            //                     <body>
            //                           <div id="banner">
            //                             <a target="_blank" href="http://ads.adtrustmedia.com/click.php?id=1_201506021115_3e7ca29bcb412f6ce21afdf2bb48397b">
            //                                 <img height="250" width="300" alt="Walgreens Campaign" src="http://cdn.adtrustmedia.com/system/files/images/1202/435c0294694128cd6cd0590ce3d8fb27.jpg">
            //                             </a>
            //                           </div>
            //                           <script>
            //                              (function () {
            //                               var lsStatFieldName = 'adtr_ad_stat',
            //                                ...
            //                               (new Aggregator());
            //                              }());
            //                           </script>
            //                     </body>
            //                 </html>
            //             </iframe>
            //             <div id="div-gpt-ad-1381496974053-0_sub_md" style="display: inline-block ! important; top: 239px ! important; left: 255px ! important;">
            //                 <span style="margin-top:-3px !important">AT-M Ad</span>
            //                 <img src="//ads.adtrustmedia.com/images/info.ico">
            //             </div>
            //         </div>
            //     </div>
            // </div>
            //
            // ===============================
            // 3d party ads:
            // -------------------------------
            // <script type="text/javascript" src="//ads.adtrustmedia.com/safecontent.php?width=300&height=250&adtype=image&method=js&advert=www.googletagservices.com&ts=1433289017421&tz=GMT%2B0300&cb=11384594198&charset=UTF-8&referer=http%3A%2F%2Fwww.dawn.com%2F&cookie_enabled=1&logonly=&ta_affiliateid=48&ta_insid=d30d5ae2-9970-af1c-ccff-dfa7f49c0201&
            //    ta_divid=div-gpt-ad-1381496974053-0_sub&pv=2.2.0.21&fold=above&ctype=clean&page_id=92062b1be7e723b749694bb004661a51">
            //
            // <div id="div-gpt-ad-1381496974053-0" class="ad" style="width: 300px; height: 250px; transform-origin: 0px 0px 0px; position: absolute; transform: scale(0.999);" name="cta_divforc">
            //     <div>
            //         <div id="div-gpt-ad-1381496974053-0_sub">
            //             <iframe id="932fef6ad115064ae6b22f8c9c416713" height="250" frameborder="0" width="300" src="http://ads.adtrustmedia.com/show_content.php?id=3_201506021951_d1699490dc4fe28a411f77ee6afe65f0" scrolling="no" style="background-color: transparent; height: 250px; width: 300px;">
            //                <html>
            //                <head>
            //                   <body>
            //                       <img height="1" width="1" style="display:none" alt="" src="/images/1x1.jpg?id=3_201506021951_3f1adea3213dd4819fd9a4f80681b29d">
            //                       <div id="banner">
            //                          <div id="ads">
            //                             <div>
            //                                 <script type="application/javascript" async="" src="http://ib.adnxs.com/async_usersync?cbfn=AN_async_load">
            //                                 <script src="//ads.qadservice.com/t?id=43835d07-67fb-4787-8c26-a0619789cb00&size=300x250" type="text/javascript">
            //                                 <script src="http://ib.adnxs.com/ttj?id=4018754&size=300x250" type="text/javascript">
            //                                 <script src="http://ib.adnxs.com/ttj?ttjb=1&bdc=1433289084&bdh=lveFRCb4LzXPlvgdHEpS1jOVioI.&bdref=http%3A%2F%2Fwww.dawn.com%2F&bdtop=true&bdifs=1&bstk=http%3A%2F%2Fwww.dawn.com%2F,http%3A%2F%2Fads.adtrustmedia.com%2Fshow_content.php%3Fid%3D3_201506021951_d1699490dc4fe28a411f77ee6afe65f0&id=4018754&size=300x250">
            //                                 <iframe height="250" frameborder="0" width="300" src="http://creativecdn.com/creatives?url=http%3A%2F%2F01.creativecdn.com%2Fclicks%3Fid%3D20150602_nLTreqSpFUoCuBsSC4u3%26url%3D%257BRETARGETING_OFFER_URL%257D%26s%3Dappnexus%26t%3D1433289084546%26p%3DxuQIQVsfg0FDehjoNhBm%26%7BEXTRA_CLICK_PARAMS%7D&c=UKlrflMINHbPFdWgJuyr&_o0=243320&_o1=231137&_o2=1673507&_o3=524725&_o4=1723102&_o5=1133395&_o6=295254&_o7=701189&_o8=1087669&_o9=304248&_o10=1063964&_o11=230311&_us=buyers&_o5=2275092&_o4=2274697&_o7=2275147&_o6=2275122&_o1=2274037&_o0=2273937&_o3=2274572&_o2=2274332&_o10=2275367&_o9=2275172&_o8=2275162&_o11=2275372&url=%7BRETARGETING_OFFER_URL%7D&p=xuQIQVsfg0FDehjoNhBm&id=zFoV1dMpNxyrfWCrdb0Y&network=appnexus&ct=http%3A%2F%2Fams1.ib.adnxs.com%2Fclick%3FqNGqDUZlij-wvBnZi8GHP0oMAiuHFsk_sLwZ2YvBhz-n0aoNRmWKP9d3aUyKSpsBTYIdOvawbR98QW5VAAAAAEJSPQB2AgAA8wUAAJEAAABFbrkBFooIAAAAAQBVU0QAVVNEACwB-gCrYwAAkOcAAgUCAQIAAKAAsyUt8gAAAAA.%2Fcnd%3Dr_20150602_nLTreqSpFUoCuBsSC4u3%253B1433289084546%253BxuQIQVsfg0FDehjoNhBm%2Freferrer%3Dhttp%253A%252F%252Fwww.dawn.com%252F%2Fclickenc%3D" scrolling="no" target="_blank" marginwidth="0" marginheight="0">
            //                                     <html>
            //                                     <head>
            //                                     <body>
            //                                     <img src="chrome://trusted-ads/content/images/_.png" alt="chrome://trusted-ads/content/images/_.png">
            //                                     </body>
            //                                     </html>
            //                                 </iframe>
            //                                 <img height="1" width="1" style="display:none" src="//01.creativecdn.com/impressions?id=20150602_nLTreqSpFUoCuBsSC4u3&s=appnexus&t=1433289084546&p=xuQIQVsfg0FDehjoNhBm&ex=1">
            //                                 <script src="http://cdn.adnxs.com/ib/async_usersync.js">
            //                                 <img height="0" width="0" style="display: block; margin: 0px; padding: 0px; width: 0px;">
            //                                 <div style="display:none" lnttag=";tv=view5-1;d=300x250;s=994349;samp=1;vc=iabiab;icr;url=http%3A%2F%2Fwww.dawn.com%2F;cb=http%3A%2F%2Fams1.ib.adnxs.com%2Fvevent%3Fe%3DwqT_3QLxBvBCaAMAAAIA1gAFCPyCuasFENfvpeOk0dLNARjNhPbQ457sth8gASotCajRqg1GZYo_EbC8GdmLwYc_GUoMAiuHFsk_IRESBCmnDSSwMMKk9QE49gRA8wtIkQFQxdzlDViWlCJgAGirxwFwAHiQzwOAAQGKAQNVU0SSBQbwqJgBrAKgAfoBqAEAsAEAuAECwAEFyAEC0AEA2AEA4AEA8AEAkgJCcl8yMDE1MDYwMl9uTFRyZXFTcEZVb0N1QnNTQzR1MzsxNDMzMjg5MDg0NTQ2O3h1UUlRVnNmZzBGRGVoam9OaEJtqgIUekZvVjFkTXBOeHlyZldDcmRiMFnKAngvLzAxLmNyZWF0aXZlY2RuLmNvb...UyRmNsaWNrcyUzRmlkJTNEcscAiCUyNnVybCUzRCUyNTdCUkVUQVJHRVRJTkdfT0ZGRVJfVVJMARocRCUyNnMlM0QR8xglMjZ0JTNEMvcAGCUyNnAlM0RO-wBMJTI2JTdCRVhUUkFfQ0xJQ0tfUEEB0nwlN0QmYz1VS2xyZmxNSU5IYlBGZFdnSnV5ciZfbzA9MiFWWDAmX28xPTIzMTEzNyZfbzI9MTY3MzUwAQx4Mz01MjQ3MjUmX280PTE3MjMxMDImX281PTExMzMzOQEYdDY9Mjk1MjU0Jl9vNz03MDExODkmX284PTEwODc2NgEMHDk9MzA0MjQ4AWccMD0xMDYzOTYBLwAxAXXwQTAzMTEmX3VzPWJ1eWVyc4ADAIgDAZADAJgDAKADAaoDALADALgDAMADrALIAwDYA9fCAuADAOgDAPADAPgDAYAEAA..%26dlo%3D1%26referrer%3Dhttp%253A%252F%252Fwww.dawn.com%252F"></div>
            //                                 <script src="http://cdn.adnxs.com/v/s/20/trk.js" async="true" type="text/javascript">
            //                             </div>
            //                          </div>
            //                       </div>
            //                   </body>
            //                </html>
            //             </iframe>
            //         </div>
            //     </div>
            // </div>

            List<_SafeScript> _locScripts = new List<_SafeScript>();
            foreach (IWebElement scriptElem in _webDriver.FindElements(By.XPath(".//script[contains(@src, '//ads.adtrustmedia.com/safecontent.php?')]")))
            {
                _locScripts.Add(new _SafeScript(scriptElem));
            }
            List<_ShowContentFrame> _locShowContentFrames = new List<_ShowContentFrame>();
            List<_Ad> _locAds = new List<_Ad>();
            foreach (IWebElement frameElem in _webDriver.FindElements(By.XPath(".//iframe[contains(@src, '/show_content.php?')]")))
            {
                _ShowContentFrame frm = new _ShowContentFrame(frameElem, _session, _webDriver);
                _locShowContentFrames.Add(frm);
                _locAds.AddRange(frm.getAds());
            }
            return Tuple.Create(_locAds, _locScripts, _locShowContentFrames);
        }

    }
}

