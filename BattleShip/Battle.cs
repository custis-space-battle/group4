using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBatle
{
    class Program
    {
        private bool[,] Map = new bool[9, 9] ;
        private Queue<Cell> ShootQueue=new Queue<Cell>();
        private Queue<Cell> PriorShoot = new Queue<Cell>();

        private struct Cell
        {
            public int XCoord;
            public int YCoord;
            public bool Shooted;
        }
        private void Prepare()
        {
            for(int i=0; i<100; i+=2)
            {
                Cell CurCell = new Cell();
                CurCell.XCoord = i / 10+1;
                CurCell.YCoord = i % 10;
                ShootQueue.Enqueue(CurCell);
            }
            for (int i = 1; i < 100; i += 2)
            {
                Cell CurCell = new Cell();
                CurCell.XCoord = i / 10 + 1;
                CurCell.YCoord = i % 10;
                ShootQueue.Enqueue(CurCell);
            }
        }
        
        private void AnalizaAns(string Ans, Cell Target)
        {
            if(Ans=="HIT")
            {

            }
        }
        private List<Cell> FindNotShoot(Cell Target)
        {
            List<Cell> List = new List<Cell>();
            for(int i=Target.XCoord-1; i<=Target.XCoord+1; i++)
            {
                for (int j = Target.YCoord - 1 ; j <= Target.YCoord + 1; j++)
                {
                    if(i>=0 && j >= 0 && !Map[i,j])
                    {
                        Cell CurCell = new Cell();
                        CurCell.XCoord = i;
                        CurCell.YCoord = j;
                        List.Add(CurCell);
                    }
                }
            }
            return List;
        }
    }
}
