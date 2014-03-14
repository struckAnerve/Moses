// Access to standard .NET System
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Drawing;
using Robocode;
using Robocode.Util;
// The namespace with your initials, in this case FNL is the initials
using Santom;

namespace MyRobots
{


    public enum DriveState
    {
		DesertWander,
        Follow,
		Poke
    }

    public enum GunState
    {
        HasTarget,
        NeedTarget
    }


    class arnadr_wigkar_Moses : AdvancedRobot
    {
        private static DriveState _driveState = DriveState.Follow;
        private static GunState _gunState = GunState.NeedTarget;

        private double _maxEnergy;
	    private string _badGuy = "";

		private Vector2D _forwardFolehorn = new Vector2D();
		private Vector2D _rightFolehorn = new Vector2D();
		private Vector2D _leftFolehorn = new Vector2D();

        private bool _reverseDriving = false, _lockedOnEnemy = false;
        public Random Rand = new Random();
        private EnemyData _enemy = new EnemyData();


        public override void Run()
        {
            _maxEnergy = Energy;

            do
            {

	            UpdateFol();


				if (RadarTurnRemaining == 0.0)
				{
					SetTurnRadarRightRadians(Double.PositiveInfinity);
					_gunState = GunState.NeedTarget;
				}

				if (!_badGuy.Equals(""))
				{


					GunEnemyLock();
					CheckStateAndChange();
				}

				TankMovement();
                Execute();
            } while (true);
        }
		
	    private void UpdateFol()
	    {

		    var behind = 1;

		    if (_reverseDriving)
			    behind = -1;

		    const int length = 150;

			_leftFolehorn.X = (X + length * Math.Sin(HeadingRadians - (Math.PI / 5)) * behind);
			_leftFolehorn.Y = (Y + length * Math.Cos(HeadingRadians - (Math.PI / 5))* behind);

			_forwardFolehorn.X = X + length * Math.Sin(HeadingRadians) * behind;
			_forwardFolehorn.Y = Y + length * Math.Cos(HeadingRadians) * behind;

			_rightFolehorn.X = (X + length * Math.Sin(HeadingRadians + (Math.PI / 5)) * behind);
			_rightFolehorn.Y = (Y + length * Math.Cos(HeadingRadians + (Math.PI / 5)) * behind);
	    }

        // Robot event handler, when the robot sees another robot
		public override void OnScannedRobot(ScannedRobotEvent e)
		{

			if (e.Energy == 0)
			{
				_driveState = DriveState.Poke;
				_badGuy = e.Name;
                SetAllColors(Color.LawnGreen);
			}
			else if (e.Energy < 7)
				_badGuy = "";

			if (!e.Name.Equals(_badGuy))
				return;
			var absBearing = HeadingRadians + e.BearingRadians;
			_enemy.SetEnemyData(Time,
								e,
								new Vector2D(X + e.Distance * Math.Sin(absBearing), Y + e.Distance * Math.Cos(absBearing)),
								new Point2D(0, 0));

			_gunState = GunState.HasTarget;

			var angleToEnemy = HeadingRadians + e.BearingRadians;
			var radarTurn = Utils.NormalRelativeAngle(angleToEnemy - RadarHeadingRadians);
			var extraTurn = Math.Min(Math.Atan(36.0 / e.Distance), Rules.RADAR_TURN_RATE_RADIANS);
			radarTurn += (radarTurn < 0 ? -extraTurn : extraTurn);
			SetTurnRadarRightRadians(radarTurn);
		}

	    public override void OnPaint(IGraphics graphics)
	    {
			graphics.DrawLine(new Pen(Color.Chartreuse, 0.3f), new Point((int)_leftFolehorn.X, (int)_leftFolehorn.Y), new Point((int)X, (int)Y));
			graphics.DrawLine(new Pen(Color.Chocolate, 0.3f), new Point((int)_forwardFolehorn.X, (int)_forwardFolehorn.Y), new Point((int)X, (int)Y));
			graphics.DrawLine(new Pen(Color.Crimson, 0.3f), new Point((int)_rightFolehorn.X, (int)_rightFolehorn.Y), new Point((int)X, (int)Y));
			//graphics.DrawLine(new Pen(Color.Chocolate, 0.3f), new Point((int)_enemy.Offset.X, (int)_enemy.Offset.Y), new Point((int)X, (int)Y)); //FillRectangle(new HatchBrush(new HatchStyle(),Color.BlueViolet, Color.Black), new Rectangle((int)_enemy.Bearing,10,100,100));
	    }

	    public void CheckStateAndChange()
        {
            if (Energy > (_maxEnergy / 3) * 2)
                _driveState = DriveState.Follow;
            else if (Energy > (_maxEnergy / 3))
				_driveState = DriveState.Follow;
            else
                _driveState = DriveState.Follow;
        }

        public double Beregner()
        {
            var bulletTime = 17 / _enemy.Distance;
            var katetLength = (_enemy.Velocity / bulletTime);
            var hyp = (Math.Sqrt(Math.Pow(katetLength, 2) + Math.Pow(_enemy.Distance, 2)));
            var grader = Math.Cos((hyp / _enemy.Distance));

            var altering = (_enemy.Bearing / 100);

            var enemHead = _enemy.Heading;
            //if (enemHead > 180)
            //enemHead = enemHead + (enemHead%180);


//            Console.WriteLine("Bullettime: " + bulletTime + ". katetLength: " + katetLength
//                + ". Hypotenus: " + hyp + ". Grader: " + grader + ". Enemy Bearing: " + _enemy.Bearing + ".\nEnemy heading: " + enemHead);

            //			Console.WriteLine(enemHead + " = " + (-enemHead) + " + " + (enemHead % 180) + " = " + (-enemHead + (enemHead % 180 + enemHead)));
            if (_enemy.Bearing < 1 && _enemy.Bearing > -1)
                return 0;


            if (_enemy.Bearing < 0)
                return -grader + altering;

            return grader - altering;
        }


        public void GunEnemyLock()
        {

            switch (_gunState)
            {
                case (GunState.NeedTarget):
                    break;
                case (GunState.HasTarget):
                    var gunturn = HeadingRadians + _enemy.BearingRadians - GunHeadingRadians + Beregner();
                    IsAdjustRadarForGunTurn = true;
                    SetTurnGunRightRadians(Utils.NormalRelativeAngle(gunturn));

					if(_driveState != DriveState.Poke)
						Fire(1);

                    break;
            }

        }

        public void TankMovement()
        {
            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForRobotTurn = true;

            double turn = 0;



            switch (_driveState)
            {
                case (DriveState.Follow):
                    turn = HeadingRadians + _enemy.BearingRadians - HeadingRadians;
                    break;
                case (DriveState.Poke):
                    turn = HeadingRadians + _enemy.BearingRadians - HeadingRadians;
		            //SetAhead(0.2f);
                    break;
            }


	        if (_enemy.Distance > 200 && _reverseDriving)
		        _reverseDriving = false;

            SetTurnRightRadians(Utils.NormalRelativeAngle(turn));

			if(_driveState != DriveState.Poke){
				if (_leftFolehorn.X < 18 || _leftFolehorn.X > (BattleFieldWidth - 18))
					SetTurnRight(Rules.MAX_TURN_RATE);
				if (_leftFolehorn.Y < 18 || _leftFolehorn.Y > (BattleFieldHeight - 18))
					SetTurnRight(Rules.MAX_TURN_RATE);

				if (_rightFolehorn.X < 18 || _rightFolehorn.X > (BattleFieldWidth - 18))
					SetTurnLeft(Rules.MAX_TURN_RATE);
				if (_rightFolehorn.Y < 18 || _rightFolehorn.Y > (BattleFieldHeight - 18))
					SetTurnLeft(Rules.MAX_TURN_RATE);
			}
            if (!_reverseDriving)
                SetAhead(Rand.Next(1, 30));
            else
                SetAhead(-Rand.Next(1, 30));
            //			CheckForWalls();
        }


	    public override void OnHitByBullet(HitByBulletEvent bullet)
	    {
			_badGuy = bullet.Name;
	    }

	    public override void OnHitWall(HitWallEvent evnt)
	    {
			_reverseDriving = !_reverseDriving;
	    }

	    public override void OnHitRobot(HitRobotEvent evnt)
	    {
			_reverseDriving = !_reverseDriving;
	    }

	    public override void OnBulletHit(BulletHitEvent evnt)
	    {

	    }
    }
}