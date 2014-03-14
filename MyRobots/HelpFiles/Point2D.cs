using System;

namespace Santom
{
	/// <summary>
	/// Class for storing a 2D point of type {double, double}. 
	/// (Really just a stripped down version of the Vector2D class.)
	/// version: 1.0
	/// author: Tomas Sandnes - santom@nith.no
	/// </summary>
	public class Point2D
	{
		public Point2D()
		{
			Zero();
		}

	    public double Distance(double x1, double y1, double x2, double y2)
	    {
	        return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
	    }
        public static double Distance(Point2D p1, Point2D p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

		public Point2D(double xVal, double yVal)
		{
			X = xVal;
			Y = yVal;
		}

		public double X { get; set; }

		public double Y { get; set; }

		/// <summary>
		/// Sets X and Y to zero.
		/// </summary>
		public void Zero()
		{
			X = 0.0;
			Y = 0.0;
		}

		/// <summary>
		/// Returns true if both X and Y are zero.
		/// </summary>
		public bool IsZero()
		{
			return (((X * X) + (Y * Y)) < Double.MinValue);
		}

		public bool Equals(Point2D point)
		{
			// If parameter is null return false:
			if ((Object)point == null) {
				return false;
			}

			// Return true if the fields match:
			return (X == point.X) && (Y == point.Y);
		}

		public override bool Equals(System.Object obj)
		{
			// If parameter is null return false.
			if (obj == null) {
				return false;
			}

			// If parameter cannot be cast to Point2D return false.
			Point2D point = obj as Point2D;
			if ((System.Object)point == null) {
				return false;
			}

			// Return true if the fields match:
			return (X == point.X) && (Y == point.Y);
		}

        public static double PDouble(double x1, double y1, double x2, double y2)
	    {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
	    }

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		// We want some overloaded operators:
		// ==================================

		public static Point2D operator +(Point2D lhs, Point2D rhs)
		{
			return new Point2D(lhs.X + rhs.X, lhs.Y + rhs.Y);
		}

		public static Point2D operator -(Point2D lhs, Point2D rhs)
		{
			return new Point2D(lhs.X - rhs.X, lhs.Y - rhs.Y);
		}

		public static Point2D operator *(Point2D lhs, double rhs)
		{
			return new Point2D(lhs.X * rhs, lhs.Y * rhs);
		}

		public static Point2D operator *(double lhs, Point2D rhs)
		{
			return new Point2D(lhs * rhs.X, lhs * rhs.Y);
		}

		public static Point2D operator /(Point2D lhs, double rhs)
		{
			return new Point2D(lhs.X / rhs, lhs.Y / rhs);
		}

		public static bool operator ==(Point2D lhs, Point2D rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Point2D lhs, Point2D rhs)
		{
			return !(lhs.Equals(rhs));
		}
	}
}