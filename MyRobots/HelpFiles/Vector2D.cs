using System;

namespace Santom
{
	/// <summary>
	/// Class for storing a 2D vector of type {double, double}. 
	/// Basically a C# conversion of Matt Buckland's Vector2D for C++.
	/// version: 1.0
	/// author: Tomas Sandnes - santom@nith.no
	/// </summary>
	public class Vector2D
	{
		/// <summary>
		/// Creates a new Vector2D with the normalized values of the input vector.
		/// </summary>
		/// <returns>A new Vector2D, being a normalized version of the original vector.</returns>
		public static Vector2D Normalize(Vector2D v) {
			Vector2D vec = new Vector2D(v.X, v.Y);
			if (!vec.IsZero()) {
				double vector_length = vec.Length();
				vec.X /= vector_length;
				vec.Y /= vector_length;
			}

			return vec;
		}

		public Vector2D() {
			Zero();
		}

		public Vector2D(double xVal, double yVal) {
			X = xVal;
			Y = yVal;
		}

		public double X { get; set; }

		public double Y { get; set; }

		/// <summary>
		/// Sets x and y to zero.
		/// </summary>
		public void Zero() {
			X = 0.0;
			Y = 0.0;
		}

		/// <summary>
		/// Returns true if both X and Y are zero.
		/// </summary>
		public bool IsZero() {
			return (((X * X) + (Y * Y)) < Double.MinValue);
		}

		/// <summary>
		/// Returns the SQUARED length of the vector (thereby avoiding the Sqrt).
		/// </summary>
		public double LengthSq() {
			return ((X * X) + (Y * Y));
		}

		/// <summary>
		/// Returns the length of the vector.
		/// </summary>
		public double Length() {
			return Math.Sqrt(LengthSq());
		}

		public void Normalize() {
			if (!IsZero()) {
				double tmpLength = Length();
				X /= tmpLength;
				Y /= tmpLength;
			}
		}

		public double Dot(Vector2D v2) {
			return ((X * v2.X) + (Y * v2.Y));
		}

		/// <summary>
		/// Returns positive if v2 is clockwise of this vector, negative if anticlockwise. 
		/// (Assuming the Y axis is pointing down, X axis to right, like a Window app.)
		/// </summary>
		public int Sign(Vector2D v2) {
			if ((Y * v2.X) > (X * v2.Y)) {
				// Anticlockwise.
				return -1;
			} else {
				// Clockwise.
				return 1;
			}
		}

		/// <summary>
		/// Returns the vector that is perpendicular to this one.
		/// </summary>
		public Vector2D Perp() {
			return new Vector2D(-Y, X);
		}

		/// <summary>
		/// Adjusts X and Y so that the length of the vector does not exceed max.
		/// </summary>
		public void Truncate(double max) {
			if (Length() > max) {
				Normalize();
				X *= max;
				Y *= max;
			}
		}

		/// <summary>
		/// Returns the SQUARED distance between this vector and the one passed as a parameter.
		/// </summary>
		public double DistanceSq(Vector2D v2) {
			var ySeparation = v2.Y - Y;
			var xSeparation = v2.X - X;

			return ((ySeparation * ySeparation) + (xSeparation * xSeparation));
		}

		/// <summary>
		/// Returns the distance between this vector and the one passed as a parameter.
		/// </summary>
		public double Distance(Vector2D v2) {
			return Math.Sqrt(DistanceSq(v2));
		}

		/// <summary>
		/// Given a normalized vector this method reflects the vector it is operating upon. 
		/// (Like the path of a ball bouncing off a wall.)
		/// </summary>
		public void Reflect(Vector2D norm) {
			var modifier = 2.0 * Dot(norm) * norm.GetReverse();
			X += modifier.X;
			Y += modifier.Y;
		}

		/// <summary>
		/// Returns the vector that is the reverse of this vector.
		/// </summary>
		public Vector2D GetReverse() {
			return new Vector2D(-X, -Y);
		}

		public bool Equals(Vector2D v) {
			// If parameter is null return false:
			if ((object)v == null) {
				return false;
			}

			// Return true if the fields match:
			return (X == v.X) && (Y == v.Y);
		}

		public override bool Equals(System.Object obj) {
			// If parameter is null return false.
			if (obj == null)
			{
				return false;
			}

			// If parameter cannot be cast to Vector2D return false.
			Vector2D v = obj as Vector2D;
			if ((System.Object)v == null) {
				return false;
			}

			// Return true if the fields match:
			return (X == v.X) && (Y == v.Y);
		}

		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		// We want some overloaded operators:
		// ==================================

		public static Vector2D operator +(Vector2D lhs, Vector2D rhs) {
			return new Vector2D(lhs.X + rhs.X, lhs.Y + rhs.Y);
		}

		public static Vector2D operator -(Vector2D lhs, Vector2D rhs) {
			return new Vector2D(lhs.X - rhs.X, lhs.Y - rhs.Y);
		}

		public static Vector2D operator *(Vector2D lhs, double rhs) {
			return new Vector2D(lhs.X * rhs, lhs.Y * rhs);
		}

		public static Vector2D operator *(double lhs, Vector2D rhs) {
			return new Vector2D(lhs * rhs.X, lhs * rhs.Y);
		}

		public static Vector2D operator /(Vector2D lhs, double rhs) {
			return new Vector2D(lhs.X / rhs, lhs.Y / rhs);
		}

		public static bool operator ==(Vector2D lhs, Vector2D rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Vector2D lhs, Vector2D rhs) {
			return !(lhs.Equals(rhs));
		}
	}
}
