using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip
{
    public class LetsShoot
    {
        private bool[,] Map = new bool[9, 9] ;
        private Queue<Cell> ShootQueue;
        private Queue<Cell> PriorShoot;
        
        public  LetsShoot()
        {
            ShootQueue = new Queue<Cell>();
            PriorShoot = new Queue<Cell>();

            for (int i=0; i<100; i+=2)
            {
                Cell CurCell = new Cell();
                CurCell.X = i / 10;
                CurCell.Y = i % 10;
                ShootQueue.Enqueue(CurCell);
            }
            for (int i = 1; i < 100; i += 2)
            {
                Cell CurCell = new Cell();
                CurCell.X = i / 10 ;
                CurCell.Y = i % 10;
                ShootQueue.Enqueue(CurCell);
            }
        }

        public void AnalizaAns(string Ans, Cell Target)
        {
            Map[Target.X, Target.Y] = true;
            switch (Ans)
            {
                case "HIT":
                    for (int i=1; i<=9; i += 2)
                        {
                            Cell CurCell = Next(Target, i);
                            if (CurCell != null) { Map[Target.X, Target.Y] = true; }
                        }
                        for (int i = 2; i <= 9; i += 2)
                        {
                            Cell CurCell = Next(Target, i);
                            if (CurCell != null) {PriorShoot.Enqueue(CurCell); }
                        }
                    //}
                break;
                case "KILL":
                    while(PriorShoot.Count>0)
                    {
                        Cell CurCell = PriorShoot.Dequeue();
                        Map[CurCell.X, CurCell.Y] = true;
                    }
                    break;
            }
        }

        public Cell Shoot ()
        {
            Cell NewShoot = null;
            NewShoot = GetShoot();
            if (NewShoot == null) { return null; }
            else
            {
                if (!Map[NewShoot.X, NewShoot.Y]) { return NewShoot; }
                else
                {
                    NewShoot = Shoot();
                    return NewShoot;
                }
            }
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
                        NewCel = new Cell();
                        NewCel.X = Target.X - 1;
                        NewCel.Y = Target.Y - 1;
                    }
                    break;
                case 2:
                    if (Target.Y > 0)
                    {
                        NewCel = new Cell();
                        NewCel.X = Target.X;
                        NewCel.Y = Target.Y - 1;
                    }
                    break;
                case 3:
                    if (Target.X < 9 && Target.Y > 0)
                    {
                        NewCel = new Cell();
                        NewCel.X = Target.X + 1;
                        NewCel.Y = Target.Y - 1;
                    }
                    break;
                case 4:
                    if (Target.X > 0)
                    {
                        NewCel = new Cell();
                        NewCel.X = Target.X - 1;
                        NewCel.Y = Target.Y;
                    }
                    break;
                case 6:
                    if (Target.X < 9)
                    {
                        NewCel = new Cell();
                        NewCel.X = Target.X + 1;
                        NewCel.Y = Target.Y;
                    }
                    break;
                case 7:
                    if (Target.X > 0 && Target.Y < 9)
                    {
                        NewCel = new Cell();
                        NewCel.X = Target.X - 1;
                        NewCel.Y = Target.Y + 1;
                    }
                    break;
                case 8:
                    if (Target.Y < 9)
                    {
                        NewCel = new Cell();
                        NewCel.X = Target.X;
                        NewCel.Y = Target.Y + 1;
                    }
                    break;
                case 9:
                    if (Target.X < 9 && Target.Y < 9)
                    {
                        NewCel = new Cell();
                        NewCel.X = Target.X + 1;
                        NewCel.Y = Target.Y + 1;
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
    }

}
