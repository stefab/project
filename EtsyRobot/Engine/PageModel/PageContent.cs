using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Collections;
using System.Drawing.Imaging;

using Newtonsoft.Json;
using System.Security.Cryptography;

namespace EtsyRobot.Engine.PageModel
{

    public class NodeOverlappedInfo
    {
        public int overlappedSquare = 0;
        public int ID = -1;
        public bool flMain = false;

        public NodeOverlappedInfo(int id, int square)
        {
            this.ID = id;
            this.overlappedSquare = square;
        }
    }

    public class PageNode
    {
        //public Rectangle rt;
        public Point topLeft = Point.Empty;
        public Point bottomRight = Point.Empty;
        public int ID = 0;
        public int visibleSquare = 0;
        public Node node = null;
        public NodeOverlappedInfo overlappedInfo = null;
        public PageNode() {
            //rt.X = -1;
            //rt.Y = -1;
            //rt.Width = 0;
            //rt.Height = 0;
        }

        public Rectangle getVisibleRect() {
            return new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X /*+ 1*/, bottomRight.Y - topLeft.Y /*+ 1*/);
        }

        public void addVisiblePoint(int x, int y) {
            visibleSquare++;
            if (topLeft.IsEmpty) {
                topLeft.X = x;
                topLeft.Y = y;
            }
            else {
                if (x < topLeft.X)
                    topLeft.X = x;
                if (y < topLeft.Y)
                    topLeft.Y = y;
            }

            if (bottomRight.IsEmpty) {
                bottomRight.X = x;
                bottomRight.Y = y;
            }
            else {
                if (x > bottomRight.X)
                    bottomRight.X = x;
                if (y > bottomRight.Y)
                    bottomRight.Y = y;
            }
        }
    }
	    
    
    [JsonObject]
	public sealed class PageContent
	{
		[JsonIgnore]
		public Image Screenshot { get; set; }

		[JsonProperty("layout")]
		public int[,] Layout { get; set; }

		[JsonProperty("nodes")]
		public IList<Node> Nodes { get; set; }

        [JsonProperty("ad_zones")]
        public IList<AdZoneNode> AdZoneNodes { get; set; }

        private Dictionary<int, PageNode> _pageNodes = null;

        public Dictionary<int, PageNode> pageVisibleNodes
        {
            get { 
                if (_pageNodes == null) {
                    _pageNodes = GetPageVisibleNodes();
                }
                return _pageNodes; 
            }
        }
         
		public int[] GetVisibleNodeIds()
		{
            return (from y in Enumerable.Range(0, this.Layout.GetUpperBound(0))
			        from x in Enumerable.Range(0, this.Layout.GetUpperBound(1))
			        let id = this.Layout[y, x]
			        where id >= 0
			        select id).Distinct().OrderBy(id => id).ToArray();
		}

        public IEnumerable<Point> GetNodeViewportPoints(int nodeID)
        {
            for (int y = 0; y < this.Layout.GetUpperBound(0); y++)
            {
                for (int x = 0; x < this.Layout.GetUpperBound(1); x++)
                {
                    if (this.Layout[y, x] == nodeID)
                    {
                        yield return new Point(x, y);
                    }
                }
            }
        }

        public Dictionary<int, PageNode> GetPageVisibleNodes()
        {
            Dictionary<int, PageNode> nodes = new Dictionary<int, PageNode>();
            int prevId = -1;

            PageNode curNode = null;
            int height = this.Layout.GetUpperBound(0); 
            int width = this.Layout.GetUpperBound(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int id = this.Layout[y, x];
                    if (id < 0) {
                        // ignore
                    }
                    else
                    if (id != prevId)
                    {
                        // create new node
                        if (!nodes.ContainsKey(id))
                        {  
                            PageNode n = new PageNode();
                            n.ID = id;
                            try {
                                n.node = this.Nodes[id];
                            }
                            catch(Exception e) {
                            }
                            curNode = n;
                            nodes[id] = n;
                        }
                        else
                        {
                            curNode = nodes[id];
                        }
                        prevId = id;
                    }
                    curNode.addVisiblePoint(x, y);
                }
            }
            return nodes;
        }

        public Bitmap createBitmap()
        {
            Dictionary<int, int> colorMap = new Dictionary<int, int>();
            Bitmap image = null;
            int prevId = -1;
            int curColor = 0;
            MD5 crypt = MD5.Create();
            try
            {
                // Retrieve the image.
                int height = this.Layout.GetUpperBound(0);
                int width = this.Layout.GetUpperBound(1); 
                image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int id = this.Layout[y, x];
                        if (prevId != id)
                        {
                            if (!colorMap.ContainsKey(id))
                            {
                                byte[] intBytes = crypt.ComputeHash(BitConverter.GetBytes(id));
                                colorMap[id] = BitConverter.ToInt32(intBytes, 0);
                            }
                            prevId = id;
                            curColor = colorMap[id];
                        }
                        Color newColor = Color.FromArgb(curColor); 
                        image.SetPixel(x, y, newColor);
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return image;
        }


		public IEnumerable<int> GetRectVisibleNodes(Rectangle rect)
		{
			for (int y = rect.Y; y <= rect.Bottom; y++)
			{
				for (int x = rect.X; x < rect.Right; x++)
				{
					yield return this.Layout[y, x];
				}
			}
		}

        public IList<Tuple<int, int>> GetRectVisibleNodesSquare(Rectangle rect)
        {
            return new List<Tuple<int, int>>() { };
        }

       
		public Rectangle GetNodeViewport(int nodeID)
		{
			Point topLeft = Point.Empty, bottomRight = Point.Empty;
			foreach (Point point in this.GetNodeViewportPoints(nodeID))
			{
                //if (topLeft.IsEmpty)
                //{
                //    topLeft = point;
                //}
                //else
                //{
                //    if (point.X < topLeft.X || point.Y < topLeft.Y)
                //    {
                //        topLeft = point;
                //    }
                //}
                //
                //if (bottomRight.IsEmpty)
                //{
                //    bottomRight = point;
                //}
                //else
                //{
                //    if (point.X > bottomRight.X || point.Y > bottomRight.Y)
                //    {
                //        bottomRight = point;
                //    }
                //}

				if (topLeft.IsEmpty)
				{
					topLeft = point;
				}
				else
				{
					if (point.X < topLeft.X )
					{
						topLeft.X = point.X;
					}
                    if (point.Y < topLeft.Y) {
                        topLeft.Y = point.Y;
                    }
				}

				if (bottomRight.IsEmpty)
				{
					bottomRight = point;
				}
				else
				{
					if (point.X > bottomRight.X )
					{
						bottomRight.X = point.X;
					}
                    if (point.Y > bottomRight.Y) {
                        bottomRight.Y = point.Y;
                    }
				}
			}

			return new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X + 1, bottomRight.Y - topLeft.Y + 1);
		}

        public AdZoneNode getAdZoneById(int id)
        {
            return this.AdZoneNodes.FirstOrDefault(x => x.data_id == id);
        }
	}
}