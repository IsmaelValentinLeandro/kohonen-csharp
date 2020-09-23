using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kohonen
{
    class Cluster
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Letter> letters { get; set; }
        public Double distanciaEuclidiana { get; set; } 

        public Cluster()
        {
            letters = new List<Letter>();
        }

        public string percentualValor()  
        {
            List<List<Letter>> listas = new List<List<Letter>>();
                    
            int qtdLetras = 0;
            string letras = "";
            foreach(Letter letter in letters)
            {
                if(!letras.Contains(letter.name))
                {
                    List<Letter> lista = new List<Letter>();
                    lista.Add(letter);
                    listas.Add(lista);
                    letras += letter.name;
                    qtdLetras++;
                }
                else
                {
                    foreach(List<Letter> list in listas)
                    {
                        foreach (Letter letra in list)
                        {
                            if(letra.name == letter.name)
                            {
                                list.Add(letter);
                                break;
                            }
                        }
                    }
                }
            }

            letras = "";
            Double percentual = 0.0;
            Double vlr1 = 0;
            Double vlr2 = 0;

            foreach (List<Letter> list in listas)
            {
                vlr1 = list.Count();
                vlr2 = letters.Count();
                percentual = vlr1 / vlr2;
                percentual = percentual * 100;
                letras += list[0].name + ": " + percentual.ToString("###,##0.00") + "%\r\n";
            }
            return letras;
        }
    }
}
