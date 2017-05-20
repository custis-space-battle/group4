using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip
{
    public class LetsShoot
    {
        private bool[,] Map = new bool[10, 10] ;
        private Queue<Cell> ShootQueue;
        private Queue<Cell> PriorShoot;

        public  LetsShoot()
        {
            ShootQueue = new Queue<Cell>();
            PriorShoot = new Queue<Cell>();
            for(int i=0; i<10;i++)
            {
                for(int j=0;j<10;j++)
                {
                    if ((i + j) % 2 == 0) { ShootQueue.Enqueue(new Cell(i, j)); }
                }
            }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if ((i + j) % 2 != 0) { ShootQueue.Enqueue(new Cell(i, j)); }
                }
            }
            //for (int i=0; i<10; i+=2)
            //{
            //    for(int j=0;j<10; j+=2)
            //    {
            //            CurCell = new Cell(i, j);
            //            ShootQueue.Enqueue(CurCell);
            //    }
            //}
            //for (int i = 1; i < 10; i+=2)
            //{
            //    for (int j = 1; j < 10; j+=2)
            //    {
            //        if (i == j)
            //        {
            //            CurCell = new Cell(i,j);
            //            ShootQueue.Enqueue(CurCell);
            //        }
            //    }
            //}
            //for (int i = 0; i < 10; i += 2)
            //{
            //    for (int j = 1; j < 10; j += 2)
            //    {
            //        CurCell = new Cell(i,j);
            //        ShootQueue.Enqueue(CurCell);
            //    }
            //}
            //for (int i = 1; i < 10; i += 2)
            //{
            //    for (int j = 0; j < 10; j += 2)
            //    {
            //        CurCell = new Cell(i,j);
            //        ShootQueue.Enqueue(CurCell);
            //    }
            //}
        }

        public void AnalizaAns(string Ans, Cell Target)
        {
            Map[Target.X, Target.Y] = true;
            switch (Ans)
            {
                case "HIT":
                    HitMe(Target);
                break;
                case "KILL":
                    HitMe(Target);
                    ShootMe();
                    break;
                default:
                    break;
            }
        }
        private void HitMe(Cell Target)
        {
            Cell CurCell;
            for (int i = 1; i <= 9; i += 2)
            {
                CurCell = Next(Target, i);
                if (CurCell != null) { Map[CurCell.X, CurCell.Y] = true; }
            }
            for (int i = 2; i <= 9; i += 2)
            {
                CurCell = Next(Target, i);
                if (CurCell != null) { PriorShoot.Enqueue(CurCell); }
            }
        }

        private void ShootMe()
        {
            Cell CurCell;
            while (PriorShoot.Count > 0)
            {
                CurCell = PriorShoot.Dequeue();
                Map[CurCell.X, CurCell.Y] = true;
            }
        }

        public Cell Shoot ()
        {
            Cell NewShoot = null;
            NewShoot = GetShoot();
            if (NewShoot == null) { return null; }
            else
            {
                if (Map[NewShoot.X, NewShoot.Y])
                {
                    NewShoot = Shoot();
                }
            }
            return NewShoot;
        }

        private Cell GetShoot()
        {
            Cell NewShoot = null;
            if (PriorShoot.Count == 0)
            {
                if (ShootQueue.Count != 0) { NewShoot = ShootQueue.Dequeue(); }
                else { return null; }
            }
            else { NewShoot = PriorShoot.Dequeue(); }
            return NewShoot;
        }

        private Cell Next(Cell Target, int Num)
        {
            Cell NewCel = null;
            switch (Num)
            {
                case 1:
                    if (Target.X > 0 && Target.Y > 0)
                    {
                        NewCel = new Cell(Target.X-1, Target.Y-1 );
                    }
                    break;
                case 2:
                    if (Target.Y > 0)
                    {
                        NewCel = new Cell(Target.X,Target.Y-1);
                    }
                    break;
                case 3:
                    if (Target.X < 9 && Target.Y > 0)
                    {
                        NewCel = new Cell(Target.X + 1, Target.Y-1);
                    }
                    break;
                case 4:
                    if (Target.X > 0)
                    {
                        NewCel = new Cell(Target.X-1,Target.Y);
                    }
                    break;
                case 6:
                    if (Target.X < 9)
                    {
                        NewCel = new Cell(Target.X+1, Target.Y);
                    }
                    break;
                case 7:
                    if (Target.X > 0 && Target.Y < 9)
                    {
                        NewCel = new Cell(Target.X-1, Target.Y+1);
                    }
                    break;
                case 8:
                    if (Target.Y < 9)
                    {
                        NewCel = new Cell(Target.X, Target.Y+1);
                    }
                    break;
                case 9:
                    if (Target.X < 9 && Target.Y < 9)
                    {
                        NewCel = new Cell(Target.X+1,Target.Y+1);
                    }
                    break;
            }
            return NewCel;
        }
    }

    public  class Cell
    {
        public int X;
        public int Y;
        public string State;
        public Cell(int I, int J)
        {
            X = I;
            Y = J;
        }
    }

}
