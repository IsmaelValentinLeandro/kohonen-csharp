using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kohonen 
{
    public class Letter
    {
        public Letter(int x, int y)
        {
            this.x = x;
            this.y = y;
            matriz = new int[x, y];
            vector = new int[x * y];
        }
        public int x { get; set; }
        public int y { get; set; }
        public string path { get; set; }
        public string name { get; set; }
        public int[] vector { get; set; }
        public int[,] matriz { get; set; }

        public string toString()
        {
            string letter = "\r\n";
            string vetor = "";
            int count = 0;
            for (int posY = 0; posY < y; posY++)
            {
                for(int posX=0; posX<x; posX++)
                {
                    vetor += vector[count];
                    letter += (matriz[posX, posY] == 1) ? "+" : "-";
                    count++;
                }
                letter += "\r\n";
            }
            letter += vetor;
            letter += "\r\n";
            return letter;
        }

    }
}
