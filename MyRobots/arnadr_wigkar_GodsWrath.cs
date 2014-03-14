/*     ___        _______       _______      _______       _______         ___                 ___    _____         ________    ______________   __       __       ___     *\
|   __|  ||__    /  _____|     /  ___  \    |   __  \     /  ___  \        |  |               |  |   |   _  \      |   __   |  |_____    _____| |  |     |  |   __||  |__   |
|  |__::::__||  /  /          /  /   \  \   |  |  \  \   |  /   \__|       |  |     _____     |  |   |  | \  |     |  |  |  |        |  |       |  |     |  |  ||__::::__|  |
|     |  ||    |  |   ____   |  |     |  |  |  |   |  |  |  \______         \  \   /  _  \   /  /    |  |_/  |    /  /____\  \       |  |       |  |_____|  |     ||  |     |
|     |  ||    |  |  |__  |  |  |     |  |  |  |   |  |   \_______  \        |  | |  | |  | |  |     |      /    |  _______   |      |  |       |   _____   |     ||  |     |
|     |  ||     \  \____| |   \  \___/  /   |  |__/  /    _______/  |        |  |_|  | |  |_|  |     |  |\  \    |  |      |  |      |  |       |  |     |  |     ||  |     |
|     |__||      \________|    \_______/    |_______/     \________/          \_____/   \_____/      |__| \__|   |__|      |__|      |__|       |__|     |__|     ||__|     |
\*________________________________________________________________________________________________________________________________________________________________________*/


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
    public enum PilegrimageState
    {
		DesertWander,
        Retribution,
        Temptation,
		Salvation
    }


    class arnadr_wigkar_GodsWrath : AdvancedRobot
    {
        private static PilegrimageState _pilgrimageState = PilegrimageState.DesertWander;

        private int _timesPassed = 0;
        private string _badGuy = "";
        private bool _hitThisFrame = false;

		private Vector2D _leftAntennae = new Vector2D();
		private Vector2D _frontAntennae = new Vector2D();
		private Vector2D _rightAntennae = new Vector2D();

        private bool _reverseDriving = false, _lockedOnEnemy = false;
        public Random Rand = new Random();
        private EnemyData _enemy = new EnemyData();

        private int _combo;


        public override void Run()
        {
            SetAllColors(Color.Goldenrod);

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

			if (_pilgrimageState == PilegrimageState.Temptation)
			{
				length = 50;

				_frontAntennae.X = (X + length * Math.Sin(HeadingRadians - (Math.PI)) * -behind);
				_frontAntennae.Y = (Y + length * Math.Cos(HeadingRadians - (Math.PI)) * -behind);
				length = 0;
			}

		    if (_pilgrimageState == PilegrimageState.Salvation || _pilgrimageState == PilegrimageState.Retribution)
		    {
				length = 0;
		    }

			_leftAntennae.X = (X + length * Math.Sin(HeadingRadians - (Math.PI / 5)) * behind);
			_leftAntennae.Y = (Y + length * Math.Cos(HeadingRadians - (Math.PI / 5))* behind);

			_rightAntennae.X = (X + length * Math.Sin(HeadingRadians + (Math.PI / 5)) * behind);
			_rightAntennae.Y = (Y + length * Math.Cos(HeadingRadians + (Math.PI / 5)) * behind);
	    }

        // Robot event handler, when the robot sees another robot
		public override void OnScannedRobot(ScannedRobotEvent e)
		{
			if (e.Energy == 0)
			{
				_pilgrimageState = PilegrimageState.Salvation;
				_badGuy = e.Name;
                SetAllColors(Color.White);
			}
			else if (e.Energy < 10 && e.Name.Equals(_badGuy))
			{
                _pilgrimageState = PilegrimageState.Temptation;
                SetAllColors(Color.HotPink);
			}

		    if (!e.Name.Equals(_badGuy))
		    {
                if (_timesPassed >= 20)
                {
                    _badGuy = "";
                    _timesPassed = 0;
                    Console.WriteLine("Wander");
                }
                if(!_badGuy.Equals(""))
		            _timesPassed++;
                return;
		    }
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

	        if (_pilgrimageState == PilegrimageState.Salvation)
	        {
                graphics.DrawLine(new Pen(Color.GhostWhite, 1f), new Point((int)X, (int)Y + 50), new Point((int)X, (int)Y + 100));
                graphics.DrawLine(new Pen(Color.GhostWhite, 1f), new Point((int)X - 15, (int)Y + 85), new Point((int)X + 15, (int)Y + 85));
            
	        }

            graphics.DrawLine(new Pen(Color.Chartreuse, 0.3f), new Point((int)_leftAntennae.X, (int)_leftAntennae.Y), new Point((int)X, (int)Y));
            graphics.DrawLine(new Pen(Color.Crimson, 0.3f), new Point((int)_rightAntennae.X, (int)_rightAntennae.Y), new Point((int)X, (int)Y));
			graphics.DrawLine(new Pen(Color.DarkBlue, 0.3f), new Point((int)_frontAntennae.X, (int)_frontAntennae.Y), new Point((int)X, (int)Y));

	    }

	    public void CheckStateAndChange()
        {
	        if (_badGuy.Equals("") && _pilgrimageState != PilegrimageState.DesertWander)
	        {
                _pilgrimageState = PilegrimageState.DesertWander;
                SetAllColors(Color.Goldenrod);
	        }
            if (_hitThisFrame && _pilgrimageState != PilegrimageState.Retribution){
                _pilgrimageState = PilegrimageState.Retribution;
                SetAllColors(Color.Red);
                _hitThisFrame = false;
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
            if (_pilgrimageState == PilegrimageState.DesertWander)
                return;

            var gunturn = HeadingRadians + _enemy.BearingRadians - GunHeadingRadians + Beregner();
            IsAdjustRadarForGunTurn = true;
            SetTurnGunRightRadians(Utils.NormalRelativeAngle(gunturn));


            if (_pilgrimageState == PilegrimageState.Retribution)
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

            switch (_pilgrimageState)
            {
                case (PilegrimageState.Retribution):
                    turn = HeadingRadians + _enemy.BearingRadians - HeadingRadians;
                    if (_enemy.Distance > 200 && _reverseDriving)
                        _reverseDriving = false;
                    speed = 100;
                    break;
                case (PilegrimageState.Salvation):
                    turn = HeadingRadians + _enemy.BearingRadians - HeadingRadians;
		            SetAhead((_enemy.Distance/20) + 2);
                    break;
                case (PilegrimageState.Temptation):
                    turn = HeadingRadians + _enemy.BearingRadians - HeadingRadians + Math.PI/2;
                    speed = Rand.Next(10, 40);
                    break;
            }

            SetTurnRightRadians(Utils.NormalRelativeAngle(turn));

			if (_leftAntennae.X < 18 || _leftAntennae.X > (BattleFieldWidth - 18))
				SetTurnRight(Rules.MAX_TURN_RATE);
			if (_leftAntennae.Y < 18 || _leftAntennae.Y > (BattleFieldHeight - 18))
				SetTurnRight(Rules.MAX_TURN_RATE);

			if (_rightAntennae.X < 18 || _rightAntennae.X > (BattleFieldWidth - 18))
				SetTurnLeft(Rules.MAX_TURN_RATE);
			if (_rightAntennae.Y < 18 || _rightAntennae.Y > (BattleFieldHeight - 18))
				SetTurnLeft(Rules.MAX_TURN_RATE);

			if (_frontAntennae.X < 18 || _frontAntennae.X > (BattleFieldWidth - 18))
				_reverseDriving = !_reverseDriving;
			if (_frontAntennae.Y < 18 || _frontAntennae.Y > (BattleFieldHeight - 18))
				_reverseDriving = !_reverseDriving;

            if (_pilgrimageState == PilegrimageState.Salvation)
                return;

            if (!_reverseDriving)
                SetAhead(speed);
            else
                SetAhead(-speed);
        }

	    public override void OnHitByBullet(HitByBulletEvent bullet)
	    {
	        _hitThisFrame = true;
			_badGuy = bullet.Name;
            _timesPassed = 0;
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
	        _combo++;
	    }

        public override void OnBulletMissed(BulletMissedEvent evnt)
        {
            _combo = 1;
        }

        public override void OnRoundEnded(RoundEndedEvent evnt)
        {
            _badGuy = "";
            _pilgrimageState = PilegrimageState.DesertWander;
            _hitThisFrame = false;
            _timesPassed = 0;
        }
    }
}