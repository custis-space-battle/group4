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
        private int ShipsLeft;
        private int CurShipSize;
        private int MissCount;

        public  LetsShoot()
        {
            ShootQueue = new Queue<Cell>();
            PriorShoot = new Queue<Cell>();
            ShipsLeft = 6;
            CurShipSize = 0;
            MissCount = 0;
            //OldIns();
            TryCreate();
        }

        public void AnalizaAns(string Ans, Cell Target)
        {
            Map[Target.X, Target.Y] = true;
            if(ShipsLeft==0)
            {
                Reload();
                ShipsLeft = 10;
            }
            switch (Ans)
            {
                case "MISS":
                    MissCount += 1;
                    if (MissCount > 6) { Reload2(); }
                    break;
                case "HIT":
                    MissCount = 0;
                    CurShipSize += 1;
                    HitMe(Target);
                break;
                case "KILL":
                    MissCount = 0;
                    if (CurShipSize > 0) { ShipsLeft -= 1; }
                    CurShipSize = 0;
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

        private void TryCreate()
        {
            List<Cell> MyTwo = new List<Cell>();
            List<Cell> MyOne = new List<Cell>();
            Cell CurCell;
            

            for (int i=0; i<100; i++)
            {
                CurCell = GetCell(i);
                if ((CurCell.X + CurCell.Y) % 2 == 0) { MyTwo.Add(CurCell); }
                else { MyOne.Add(CurCell); }
            }
            Random MyRnd = new Random();

            List<int> Trying = new List<int>();
            for(int i=0; i<4;i++)
            {
                Trying.Add(i + 1);
            }

            int CheckInt;
            while(Trying.Count>0)
            {
                CheckInt = MyRnd.Next(0, Trying.Count);
                SortNum(Trying[CheckInt], MyOne);
                Trying.RemoveAt(CheckInt);
            }
            
            while (MyOne.Count > 0)
            {
                int ListNum = MyRnd.Next(0, MyOne.Count);
                ShootQueue.Enqueue(MyOne[ListNum]);
                MyOne.RemoveAt(ListNum);
            }
            while (MyTwo.Count > 0)
            {
                int ListNum = MyRnd.Next(0, MyTwo.Count);
                ShootQueue.Enqueue(MyTwo[ListNum]);
                MyTwo.RemoveAt(ListNum);
            }
        }

        private Cell GetCell(int i)
        {
            int x = i / 10;
            int y = i % 10;
            return new Cell(x, y);
        }

        private void OldIns()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
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
        }

        private void Reload()
        {

            List<Cell> NewMass=new List<Cell>();
            while(ShootQueue.Count>0)
            {
                NewMass.Add(ShootQueue.Dequeue());
            }
            Random MyRnd = new Random();

            while (NewMass.Count > 0)
            {
                int ListNum = MyRnd.Next(0, NewMass.Count);
                ShootQueue.Enqueue(NewMass[ListNum]);
                NewMass.RemoveAt(ListNum);
            }
        }
        private void Reload2()
        {
            List<Cell> NewMass = new List<Cell>();
            while (ShootQueue.Count > 0)
            {
                NewMass.Add(ShootQueue.Dequeue());
            }
            List<Cell> MyTwo = new List<Cell>();
            List<Cell> MyOne = new List<Cell>();

            foreach (Cell NewCell in NewMass)
            {
                if ((NewCell.X + NewCell.Y) % 2 == 0) { MyTwo.Add(NewCell); }
                else { MyOne.Add(NewCell); }
            }
            Random MyRnd = new Random();
            while (MyOne.Count > 0)
            {
                int ListNum = MyRnd.Next(0, MyOne.Count);
                ShootQueue.Enqueue(MyOne[ListNum]);
                MyOne.RemoveAt(ListNum);
            }
            while (MyTwo.Count > 0)
            {
                int ListNum = MyRnd.Next(0, MyTwo.Count);
                ShootQueue.Enqueue(MyTwo[ListNum]);
                MyTwo.RemoveAt(ListNum);
            }
        }

        private void SortNum(int Num, List<Cell> MyOne)
        {
            int i;
            int j;
            switch(Num)
            {
                case 1:
                case 3:
                    i = 0;
                    j = 1;
                    break;
                default:
                    i = 8;
                    j = 9;
                    break;
            }

            if (Num == 1 || Num == 2)
            {
                foreach (Cell TryCell in MyOne)
                {
                    if (TryCell.X == i || TryCell.X == j)
                    {
                        ShootQueue.Enqueue(TryCell);
                        //MyOne.Remove(TryCell);
                    }
                }
            }
            else
            {
                foreach (Cell TryCell in MyOne)
                {
                    if (TryCell.Y == i || TryCell.Y == j)
                    {
                        ShootQueue.Enqueue(TryCell);
                        //MyOne.Remove(TryCell);
                    }
                }
            }
        }
    }

    public  class Cell
    {
        public int X;
        public int Y;
        public Cell(int I, int J)
        {
            X = I;
            Y = J;
        }
    }

}
