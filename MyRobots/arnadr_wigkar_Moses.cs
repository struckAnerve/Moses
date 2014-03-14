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
        Follow,
        Dodge,
        Escape
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

        private bool _reverseDriving = false, _lockedOnEnemy = false;
        public Random Rand = new Random();
        private EnemyData _enemy = new EnemyData();


        public override void Run()
        {
            _maxEnergy = Energy;

            do
            {

                if (RadarTurnRemaining == 0.0)
                {
                    SetTurnRadarRightRadians(Double.PositiveInfinity);
                    _gunState = GunState.NeedTarget;
                }

                GunEnemyLock();
                TankMovement();
                CheckStateAndChange();

                Execute();
            } while (true);
        }

        // Robot event handler, when the robot sees another robot
        public override void OnScannedRobot(ScannedRobotEvent e)
        {
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
            graphics.DrawLine(new Pen(Color.Chocolate, 0.3f), new Point((int)_enemy.Offset.X, (int)_enemy.Offset.Y), new Point((int)X, (int)Y)); //FillRectangle(new HatchBrush(new HatchStyle(),Color.BlueViolet, Color.Black), new Rectangle((int)_enemy.Bearing,10,100,100));
        }

        public void CheckStateAndChange()
        {
            if (Energy > (_maxEnergy / 3) * 2)
                _driveState = DriveState.Follow;
            else if (Energy > (_maxEnergy / 3))
                _driveState = DriveState.Dodge;
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


            Console.WriteLine("Bullettime: " + bulletTime + ". katetLength: " + katetLength
                + ". Hypotenus: " + hyp + ". Grader: " + grader + ". Enemy Bearing: " + _enemy.Bearing + ".\nEnemy heading: " + enemHead);

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
                case (DriveState.Dodge):
                    turn = HeadingRadians + _enemy.BearingRadians - HeadingRadians - (Math.PI / 1.8);
                    break;
            }

            SetTurnRightRadians(Utils.NormalRelativeAngle(turn));
            if (!_reverseDriving)
                SetAhead(Rand.Next(1, 30));
            else
                SetAhead(-Rand.Next(1, 30));
            //			CheckForWalls();
        }


        public override void OnHitWall(HitWallEvent evnt)
        {
            _reverseDriving = !_reverseDriving;
        }

        public override void OnHitRobot(HitRobotEvent evnt)
        {
            _reverseDriving = !_reverseDriving;
        }
    }
}