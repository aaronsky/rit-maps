using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace RITMaps.Models
{
	public class QuadTree<T> where T : RITBuilding
	{
		QuadTree<T> northWest;
		QuadTree<T> northEast;
		QuadTree<T> southWest;
		QuadTree<T> southEast;
		BoundingBox boundingBox;
		int bucketCapacity;

		public T[] Points { get; set; }

		int count;

		public static QuadTree<T> Create (T[] data, BoundingBox boundingBox, int capacity)
		{
			var root = new QuadTree<T> (boundingBox, capacity);
			for (int i = 0; i < data.Length; i++) {
				root.Insert (data [i]);
			}
			return root;
		}

		QuadTree (BoundingBox boundary, int bucketCapacity)
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

		public Type ElementType {
			get { return typeof(T); }
		}

		public void Subdivide ()
		{
			var xMid = (boundingBox.xf + boundingBox.x0) / 2.0;
			var yMid = (boundingBox.yf + boundingBox.y0) / 2.0;

			var nWest = new BoundingBox (boundingBox.x0, boundingBox.y0, xMid, yMid);
			northWest = new QuadTree<T> (nWest, bucketCapacity);

			var nEast = new BoundingBox (xMid, boundingBox.y0, boundingBox.xf, yMid);
			northEast = new QuadTree<T> (nEast, bucketCapacity);

			var sWest = new BoundingBox (boundingBox.x0, yMid, xMid, boundingBox.yf);
			southWest = new QuadTree<T> (sWest, bucketCapacity);

			var sEast = new BoundingBox (xMid, yMid, boundingBox.xf, boundingBox.yf);
			southEast = new QuadTree<T> (sEast, bucketCapacity);
		}

		public bool Insert (T data)
		{
			if (!boundingBox.Contains (data)) {
				return false;
			}
			if (count < this.bucketCapacity) {
				Points [count++] = data;
				return true;
			}
			if (northWest == null) {
				Subdivide ();
			}

			if (northWest.Insert (data))
				return true;
			if (northEast.Insert (data))
				return true;
			if (southWest.Insert (data))
				return true;
			if (southEast.Insert (data))
				return true;
			return false;
		}

		public void Gather (BoundingBox range, Action<T> block)
		{
			if (!boundingBox.Intersects (range)) {
				return;
			}
			for (int i = 0; i < count; i++) {
				if (range.Contains (Points [i])) {
					block (Points [i]);
				}
			}
			if (northWest == null) {
				return;
			}

			northWest.Gather (range, block);
			northEast.Gather (range, block);
			southWest.Gather (range, block);
			southEast.Gather (range, block);
		}

		public IEnumerable<T> Traverse ()
		{
			for (int i = 0; i < Points.Length; i++) {
				if (Points [i] != null)
					yield return Points [i];
			}
			if (northWest == null)
				yield break;
			var nWest = northWest.Traverse ();
			foreach (var nw in nWest) {
				yield return nw;
			}
			var nEast = northEast.Traverse ();
			foreach (var ne in nEast) {
				yield return ne;
			}
			var sWest = southWest.Traverse ();
			foreach (var sw in sWest) {
				yield return sw;
			}
			var sEast = southEast.Traverse ();
			foreach (var se in sEast) {
				yield return se;
			}
		}
	}
}
