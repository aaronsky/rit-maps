using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RITMaps.Models
{
    public class QuadTree<T> where T : class, IRITBuilding
    {
        QuadTree<T> northWest;
        QuadTree<T> northEast;
        QuadTree<T> southWest;
        QuadTree<T> southEast;
        BoundingBox boundingBox;
        int bucketCapacity;
        public T[] Points {get;set;}
        int count;

        public QuadTree (T[] data, BoundingBox boundingBox, int capacity) : this(boundingBox, capacity)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Insert(data[i]);
            }
        }

        public QuadTree(BoundingBox boundary, int bucketCapacity)
        {
            boundingBox = boundary;
            this.bucketCapacity = bucketCapacity;
            northWest = null;
            northEast = null;
            southWest = null;
            southEast = null;
            count = 0;
            Points = new T[bucketCapacity];
        }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public void Subdivide()
        {
            var box = this.boundingBox;
            var xMid = (box.xf + box.x0) / 2.0;
            var yMid = (box.yf + box.y0) / 2.0;
            var northWest = new BoundingBox(box.x0, box.y0, xMid, yMid);
            this.northWest = new QuadTree<T>(northWest, this.bucketCapacity);
            var northEast = new BoundingBox(xMid, box.y0, box.xf, yMid);
            this.northEast = new QuadTree<T>(northEast, this.bucketCapacity);
            var southWest = new BoundingBox(box.x0, yMid, xMid, box.yf);
            this.southWest = new QuadTree<T>(southWest, this.bucketCapacity);
            var southEast = new BoundingBox(xMid, yMid, box.xf, box.yf);
            this.southEast = new QuadTree<T>(southEast, this.bucketCapacity);
        }

        public bool Insert(T data)
        {
            if (!this.boundingBox.Contains(data))
            {
                return false;
            }
            if (this.count < this.bucketCapacity)
            {
                this.Points[count++] = data;
                return true;
            }
            if (this.northWest == null)
            {
                this.Subdivide();
            }

            if (this.northWest.Insert(data)) return true;
            if (this.northEast.Insert(data)) return true;
            if (this.southWest.Insert(data)) return true;
            if (this.southEast.Insert(data)) return true;
            return false;
        }

        public void Gather(BoundingBox range, Action<T> block)
        {
            if (!boundingBox.Intersects(range))
            {
                return;
            }
            for (int i = 0; i < count; i++)
            {
                if (range.Contains(Points[i]))
                {
                    block(Points[i]);
                }
            }
            if (northWest == null)
            {
                return;
            }

            northWest.Gather(range, block);
            northEast.Gather(range, block);
            southWest.Gather(range, block);
            southEast.Gather(range, block);
        }

        public void Traverse(Func<QuadTree<T>, bool> block)
        {
            if (!block(this))
                return;
            if (northWest == null)
                return;
            northWest.Traverse(block);
            northEast.Traverse(block);
            southWest.Traverse(block);
            southEast.Traverse(block);
        }

        public IEnumerable<T> Find (T data)
        {
            var results = new List<T>();
            Traverse((currentNode) => {
                results.AddRange(currentNode.Points.Where(c => c.Equals(data)));
                return true;
            });
            return results;
        }
    }
}
