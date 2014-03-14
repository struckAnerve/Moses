using System;
using System.Drawing;
using Robocode;
using Robocode.Util;

namespace Santom
{
	/// <summary>
	/// Helper functionality (methods) for Robocode.
	/// version: 1.0
	/// author: Tomas Sandnes - santom@nith.no
	/// </summary>
	public static class RoboHelpers
	{
		/// <summary>
		/// Method to draw half-transparent bullet-line & targeting-box (the size of a robot) on the battlefield. 
		/// The idea is to use this for visual debugging: Set start point to own robot's position, and end point 
		/// to where you mean the bullet to go. Then see if this really is where the bullet is heading: 
		/// 1) If the targeting-box is off the spot you wanted it, you got a bug in your target prediction code.
		/// 2) If the targeting-box is on the spot, but the bullet is off the line (and center of the box), you 
		///    got a bug in your "gun turning and firing" code.
		/// </summary>
		public static void DrawBulletTarget(IGraphics graphics, Color drawColor, Point2D start, Point2D end)
		{
			// Set color to a semi-transparent one.
			Color halfTransparent = Color.FromArgb(128, drawColor);
			// Draw line and rectangle.
			graphics.DrawLine(new Pen(halfTransparent), (int)start.X, (int)start.Y, (int)end.X, (int)end.Y);
			graphics.FillRectangle(new SolidBrush(halfTransparent), (int)end.X - 18, (int)end.Y - 18, 36, 36);
		}

        public static Color NewColor()
        {
            var rand = new Random();
            var red = rand.Next(255);
            var blue = rand.Next(255);
            var green = rand.Next(255);
            var color = Color.FromArgb(red, blue, green);

            return color;
        }

		/// <summary>
		/// Method to find Vector2D from Robot to Target, according to the battlefield coordinate system.
		/// </summary>
		public static Vector2D CalculateTargetVector(double robotHeadingRadians,
													 double bearingToTargetRadians,
													 double distance)
		{
			double battlefieldRelativeTargetAngleRadians = Utils.NormalRelativeAngle(robotHeadingRadians + bearingToTargetRadians);
			Vector2D targetVector = new Vector2D(Math.Sin(battlefieldRelativeTargetAngleRadians) * distance,
												 Math.Cos(battlefieldRelativeTargetAngleRadians) * distance);
			return targetVector;
		}


		/// <summary>
		/// Method to find offset from gun to target, in the range -180 to +180 degrees.
		/// </summary>
		public static double GunToTargetAngleDegrees(double robotHeadingDegrees,
													 double gunHeadingDegrees,
													 double bearingToTargetDegrees)
		{
			return Utils.NormalRelativeAngleDegrees(robotHeadingDegrees + bearingToTargetDegrees - gunHeadingDegrees);
		}


		/// <summary>
		/// Method to find offset from radar to target, in the range -180 to +180 degrees.
		/// </summary>
		public static double RadarToTargetAngleDegrees(double robotHeadingDegrees,
													   double radarHeadingDegrees,
													   double bearingToTargetDegrees)
		{
			return Utils.NormalRelativeAngleDegrees(robotHeadingDegrees + bearingToTargetDegrees - radarHeadingDegrees);
		}
	}
}