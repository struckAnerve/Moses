using System.Collections.Generic;
using Robocode;
using Robocode.Util;

namespace Santom
{
	public class EnemyData
	{
		public long Time { get; set; }  // Time (turn) of currently stored scan.
		public List<BulletData> Bullets { get; set; }  // Queue of BulletData.

		// Enemy stuff
		public string Name { get; set; }  // Name of enemy.
		public double Bearing { get; set; }  // Bearing from us to enemy.
		public double Distance { get; set; }  // Distance from us to enemy.
		public Vector2D Offset { get; set; }  // Offset vector from us to enemy, in battlefield x y coordinates.
		public double Energy { get; set; }  // Energy on enemy.
		public Point2D Position { get; set; }  // Position of enemy, in battlefield x y coordinates.
		public double Velocity { get; set; }  // Velocity of enemy.
		public double Acceleration { get; set; }  // How fast our enemy changes speed. (Calculated by comparing values over 2 scans.)
		public double Heading { get; set; }  // Heading of enemy.
		public double TurnRate { get; set; }  // How fast our enemy turns (change of heading per turn). (Calculated by comparing values over 2 scans.)
        public double BearingRadians { get; set; }
        public double HeadingRadians { get; set; }

		public EnemyData() 
		{
			Bullets = new List<BulletData>();
			Offset = new Vector2D();
			Position = new Point2D();
		}


		public void Clear() 
		{ 
			Time = 0;
			Bullets.Clear();
			Name = null;
			Bearing = 0.0;
			Distance = 0.0;
			Offset.Zero();
			Energy = 0.0;
			Position.Zero();
			Velocity = 0.0;
			Acceleration = 0.0;
			Heading = 0.0;
			TurnRate = 0.0;
		    BearingRadians = 0.0;
		    HeadingRadians = 0.0;
		}


		public void SetEnemyData(long newTime,
							   ScannedRobotEvent newEnemyData,
							   Vector2D newOffset,
							   Point2D newPosition)
		{
			// First we set the stuff that depends on last updates' values:
			TurnRate = Utils.NormalRelativeAngleDegrees(newEnemyData.Heading - Heading) / (newTime - Time);
			Acceleration = (newEnemyData.Velocity - Velocity) / (newTime - Time);

			// General data:
			Time = newTime;

			// Compared-to-us data:
			Bearing = newEnemyData.Bearing;
			Distance = newEnemyData.Distance;
			Offset = newOffset;

			// Enemy specific data:
			Name = newEnemyData.Name;
			Energy = newEnemyData.Energy;
			Position = newPosition;
			Velocity = newEnemyData.Velocity;
			Heading = newEnemyData.Heading;
		    BearingRadians = newEnemyData.BearingRadians;
		    HeadingRadians = newEnemyData.HeadingRadians;
		}
	}
}