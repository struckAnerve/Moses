// Access to standard .NET System
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
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
		Salvation
    }


    class arnadr_wigkar_Moses : AdvancedRobot
    {
        private static DriveState _driveState = DriveState.DesertWander;

        private double _maxEnergy;
	    private string _badGuy = "";

		private Vector2D _forwardFolehorn = new Vector2D();
		private Vector2D _rightFolehorn = new Vector2D();
		private Vector2D _leftFolehorn = new Vector2D();

        private bool _reverseDriving = false, _lockedOnEnemy = false;
        public Random Rand = new Random();
        private EnemyData _enemy = new EnemyData();

        private int _combo;


        public override void Run()
        {
            _maxEnergy = Energy;

            do
            {

	            UpdateFol();


				if (RadarTurnRemaining == 0.0)
					SetTurnRadarRightRadians(Double.PositiveInfinity);
				
                CheckStateAndChange();
				GunEnemyLock();

				TankMovement();
                Execute();
            } while (true);
        }
		
	    private void UpdateFol()
	    {

		    var behind = 1;

		    if (_reverseDriving)
			    behind = -1;

	        var length = 150;
	        if (_driveState == DriveState.Salvation || _driveState == DriveState.Follow)
	            length = 0;

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
				_driveState = DriveState.Salvation;
				_badGuy = e.Name;
                SetAllColors(Color.White);
			}
			else if (e.Energy < 5)
				_badGuy = "";

			if (!e.Name.Equals(_badGuy))
				return;
			var absBearing = HeadingRadians + e.BearingRadians;
			_enemy.SetEnemyData(Time,
								e,
								new Vector2D(X + e.Distance * Math.Sin(absBearing), Y + e.Distance * Math.Cos(absBearing)),
								new Point2D(0, 0));

			var angleToEnemy = HeadingRadians + e.BearingRadians;
			var radarTurn = Utils.NormalRelativeAngle(angleToEnemy - RadarHeadingRadians);
			var extraTurn = Math.Min(Math.Atan(36.0 / e.Distance), Rules.RADAR_TURN_RATE_RADIANS);
			radarTurn += (radarTurn < 0 ? -extraTurn : extraTurn);
			SetTurnRadarRightRadians(radarTurn);
		}

	    public override void OnPaint(IGraphics graphics)
	    {

	        if (_driveState == DriveState.Salvation)
	        {
                graphics.DrawLine(new Pen(Color.GhostWhite, 1f), new Point((int)X, (int)Y + 50), new Point((int)X, (int)Y + 100));
                graphics.DrawLine(new Pen(Color.GhostWhite, 1f), new Point((int)X - 15, (int)Y + 85), new Point((int)X + 15, (int)Y + 85));
            
	        }

            //graphics.DrawLine(new Pen(Color.Chartreuse, 0.3f), new Point((int)_leftFolehorn.X, (int)_leftFolehorn.Y), new Point((int)X, (int)Y));
            //graphics.DrawLine(new Pen(Color.Chocolate, 0.3f), new Point((int)_forwardFolehorn.X, (int)_forwardFolehorn.Y), new Point((int)X, (int)Y));
            //graphics.DrawLine(new Pen(Color.Crimson, 0.3f), new Point((int)_rightFolehorn.X, (int)_rightFolehorn.Y), new Point((int)X, (int)Y));
			//graphics.DrawLine(new Pen(Color.Chocolate, 0.3f), new Point((int)_enemy.Offset.X, (int)_enemy.Offset.Y), new Point((int)X, (int)Y)); //FillRectangle(new HatchBrush(new HatchStyle(),Color.BlueViolet, Color.Black), new Rectangle((int)_enemy.Bearing,10,100,100));
	    }

	    public void CheckStateAndChange()
        {
	        if (_badGuy.Equals(""))
	        {
                _driveState = DriveState.DesertWander;
                SetAllColors(Color.Goldenrod);
	        }
            if(!_badGuy.Equals("") && _driveState != DriveState.Salvation){
                _driveState = DriveState.Follow;
                SetAllColors(Color.Red);
            }
        }

        public double Beregner()
        {
            var bulletTime = 17 / _enemy.Distance;
            var katetLength = (_enemy.Velocity / bulletTime);
            var hyp = (Math.Sqrt(Math.Pow(katetLength, 2) + Math.Pow(_enemy.Distance, 2)));
            var grader = Math.Cos((hyp / _enemy.Distance));

            var altering = (_enemy.Bearing / 100);

            var enemHead = _enemy.Heading;
            if (_enemy.Bearing < 1 && _enemy.Bearing > -1)
                return 0;


            if (_enemy.Bearing < 0)
                return -grader + altering;

            return grader - altering;
        }


        public void GunEnemyLock()
        {
            if (_driveState == DriveState.DesertWander)
                return;

            var gunturn = HeadingRadians + _enemy.BearingRadians - GunHeadingRadians + Beregner();
            IsAdjustRadarForGunTurn = true;
            SetTurnGunRightRadians(Utils.NormalRelativeAngle(gunturn));


            if (_driveState == DriveState.Follow)
            {
                int firePower = 1;
                if (_enemy.Energy > 15)
                {
                    if (_enemy.Distance < 55)
                        firePower = 3;
                    else
                    {
                        firePower = _combo;
                        if (_combo >= 3)
                            _combo = 1;
                    }
                }
                Fire(firePower);
            }
        }

        public void TankMovement()
        {
            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForRobotTurn = true;

            double turn = 0;
            double speed = Rand.Next(1, 20);

            switch (_driveState)
            {
                case (DriveState.Follow):
                    turn = HeadingRadians + _enemy.BearingRadians - HeadingRadians;
                    if (_enemy.Distance > 200 && _reverseDriving)
                        _reverseDriving = false;
                    speed = 100;
                    break;
                case (DriveState.Salvation):
                    turn = HeadingRadians + _enemy.BearingRadians - HeadingRadians;
		            SetAhead((_enemy.Distance/20) + 2);
                    break;
            }

            SetTurnRightRadians(Utils.NormalRelativeAngle(turn));

			if (_leftFolehorn.X < 18 || _leftFolehorn.X > (BattleFieldWidth - 18))
				SetTurnRight(Rules.MAX_TURN_RATE);
			if (_leftFolehorn.Y < 18 || _leftFolehorn.Y > (BattleFieldHeight - 18))
				SetTurnRight(Rules.MAX_TURN_RATE);

			if (_rightFolehorn.X < 18 || _rightFolehorn.X > (BattleFieldWidth - 18))
				SetTurnLeft(Rules.MAX_TURN_RATE);
			if (_rightFolehorn.Y < 18 || _rightFolehorn.Y > (BattleFieldHeight - 18))
				SetTurnLeft(Rules.MAX_TURN_RATE);

            if (_driveState == DriveState.Salvation)
                return;

            Console.WriteLine(speed);
            if (!_reverseDriving)
                SetAhead(speed);
            else
                SetAhead(-speed);
        }

	    public override void OnHitByBullet(HitByBulletEvent bullet)
	    {
			_badGuy = bullet.Name;
	    }

        public override void OnHitWall(HitWallEvent evnt)
        {_reverseDriving = !_reverseDriving;
        }

        public override void OnHitRobot(HitRobotEvent evnt)
	    {
			_reverseDriving = !_reverseDriving;
	    }

	    public override void OnBulletHit(BulletHitEvent evnt)
	    {
	        _combo++;
	    }

        public override void OnBulletMissed(BulletMissedEvent evnt)
        {
            _combo = 1;
        }

        public override void OnRoundEnded(RoundEndedEvent evnt)
        {
            _badGuy = "";
            _driveState = DriveState.DesertWander;
        }
    }
}