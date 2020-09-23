using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kohonen
{
    public partial class FrmKohonen : Form
    {
        private int x = 7;
        private int y = 9;
        private const int QTYPADLEFT = 8;
        private Double[,] matrizValores;
        private int[,] matrizTeste; 
        private Int32 qtdClusters = 25;
        private Int32 qtdEpocas = 220;
        private Double vlrDelta = 0.6;
        private Double vlrDecrecimo = 0.01;
        private List<Letter> letters = new List<Letter>();
        private List<Cluster> clusters = new List<Cluster>();
        private Double[] menorIndiceMatriz = new Double[3];
        
        public FrmKohonen()
        {
            InitializeComponent();
        }

        private void print(string msg)
        {
            txtLog.Text += msg;
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private void btnTreinamento_Click(object sender, EventArgs e)
        {
            try
            {
                // Atribui valores inseridos pelo usuário
                qtdClusters = Convert.ToInt32((txtCluster.Text.ToString().Trim() == "") ? "0" : txtCluster.Text.ToString());
                qtdEpocas = Convert.ToInt32((txtEpocas.Text.ToString().Trim() == "") ? "0" : txtEpocas.Text.ToString());
                vlrDelta = Convert.ToDouble((txtDelta.Text.ToString().Trim() == "") ? "0.0" : txtDelta.Text.ToString());
            }
            catch (Exception exc)
            {
                print("ERRO: Todos os campos devem ser numéricos. > ");
                return;
            }

            txtLog.Text = "Treinando MATRIZ";
            txtLog.Text += "\r\n";
            menorIndiceMatriz = new Double[3];
            carregaLetras();
            kohonen();
            pnlTeste.Enabled = true;
            pnlTestes.Enabled = false;
            
            txtLog.Text += "\r\n";
            print("Matriz TREINADA .. Aguardando entrada de dados para classificação .. ");
        }

        public void kohonen()
        {

            /*----------------------------ALGORITMO KOHONEN----------------------------
                Passo 1.Inicializar pesos Wij (valores randômicos devem ser utilizados)
                        - Setar os parâmetros da vizinhança (Raio)
                        - Setar a taxa de aprendizado (Delta)
                Passo 2. Enquanto a condição de parada for falsa, faça
                         Epocas
                    Passo 3. Para cada vetor de treinamento faça
                             Loop letras
	                    Passo 4. Para cada j calcule:
                                 Distancia euclidiana
                                 D(1) = (.08 – 0)2 + (.24 – 0)2 + (.2 – 1)2 + (.96 – 1)2 = 0.7056
                                 D(2) = (.968 – 0)2 + (.304 – 0)2 + (.112 – 1)2 + (.048 – 1)2 = 2.724
                        Passo 5. Encontre o índice j onde D(j) seja mínimo.
                                 Pega o menor cluster ex D(1)
	                    Passo 6. Para todas as unidades j em uma vizinhança   especificada e para todos os i atualize:
                                 wi2(new) = wi2(old) + 0.6[xi – wi2(old)]
                Passo 7. Alterar taxa de aprendizado
                         Decrementa 0.01 no delta
                Passo 8. Reduzir raio 
                         Não faz nada
                Passo 9. Testar condição de Parada	
            ---------------------------------------------------------------------------*/

            // Passo 1.Inicializar pesos Wij (valores randômicos devem ser utilizados)
            print("Criando matriz de valores iniciais");
            matrizValores = createMatriz();
            // Fim Passo 1
            print("\r\n");
            // Passo 2. Enquanto a condição de parada for falsa, faça
            print("Iniciando KOHONEN");
            for(int epoca=0; epoca < qtdEpocas; epoca++)
            {                
                // Passo 3. Para cada vetor de treinamento faça
                for (int letter = 0; letter < letters.Count(); letter++)
                {                    
                    // Passo 4. Para cada j calcule:
                    // Passo 5. Encontre o índice j onde D(j) seja mínimo.
                    Cluster menorCluster = calculaDistanciaEuclidiana(letters[letter]);
                    // Fim Passo 5
                    // Fim Passo 4

                    clusters[menorCluster.id].letters.Add(letters[letter]);

                    // Passo 6. Para todas as unidades j em uma vizinhança   especificada e para todos os i atualize:
                    ajustaPesosNeuroniosVizinhos(letter);
                    ajustaPesosNeuroniosVencedor(letter, menorCluster.id);
                    // Fim Passo 6
                }
                // Fim Passo 3     
                vlrDelta = vlrDelta - vlrDecrecimo;
            }
            // Fim Passo 2

            print(toStringMatriz());
        }

        // Passo 6 - KOHONEN
        private void ajustaPesosNeuroniosVizinhos(int letter)
        {
            int i = Convert.ToInt32(menorIndiceMatriz[1]);
            int j = Convert.ToInt32(menorIndiceMatriz[0]);

            // RAIO = 1
            matrizValores[i, j] = matrizValores[i, j] + (vlrDelta * (letters[letter].vector[i] - matrizValores[i, j]));
            matrizValores[i, j] = verificaValor(i, j);

            
            // ESQUERDA
            if (j == 0)
            {
                matrizValores[i, qtdClusters - 1] = matrizValores[i, qtdClusters - 1] + (vlrDelta * (letters[letter].vector[i] - matrizValores[i, qtdClusters - 1]));
                matrizValores[i, qtdClusters - 1] = verificaValor(i, j);
            }
            // DIREITA
            if (j == (qtdClusters-1))
            {
                matrizValores[i, 0] = matrizValores[i, 0] + (vlrDelta * (letters[letter].vector[i] - matrizValores[i, 0]));
                matrizValores[i, 0] = verificaValor(i, j);
            }
            // CIMA
            if (i == 0)
            {
                matrizValores[(x * y) - 1, j] = matrizValores[(x * y) - 1, j] + (vlrDelta * (letters[letter].vector[(x * y) - 1] - matrizValores[(x * y) - 1, j]));
                matrizValores[(x * y) - 1, j] = verificaValor(i, j);
            }
            // BAIXO
            if (j == x*y)
            {    
                matrizValores[0, j] = matrizValores[0, j] + (vlrDelta * (letters[letter].vector[0] - matrizValores[0, j]));
                matrizValores[0, j] = verificaValor(i, j);
            }
            
        }
        private void ajustaPesosNeuroniosVencedor(int letter, int j)
        {
            for (int i=0; i < letters[letter].vector.Count(); i++)
            {                // wi2(new) = wi2(old) + 0.6[xi – wi2(old)]
                matrizValores[i, j] = matrizValores[i, j] + (vlrDelta * (letters[letter].vector[i] - matrizValores[i, j]));
                matrizValores[i, j] = verificaValor(i, j);
            }
        }
        // Fim Passo 6

        private Double verificaValor(int i, int j)
        {
            if (matrizValores[i, j] > 1)
               return 1;
            if (matrizValores[i, j] < -1)
               return -1;
            return matrizValores[i, j];
        }

        // Passo 4 - KOHONEN
        private Cluster calculaDistanciaEuclidiana(Letter letter)
        {
            Double somaPonderada = 0;
            Double somaPonderadaAux = 1000;
            Cluster menorCluster = new Cluster();

            for (int cluster = 0; cluster < qtdClusters; cluster++)
            {
                somaPonderada = 0;
                for (int posL = 0; posL < letter.vector.Count(); posL++)
                {
                    somaPonderada += Math.Pow((matrizValores[posL, cluster] - letter.vector[posL]), 2);
                    if (menorIndiceMatriz[2] > matrizValores[posL, cluster])
                    {
                        menorIndiceMatriz[0] = cluster;
                        menorIndiceMatriz[1] = posL;
                        menorIndiceMatriz[2] = matrizValores[posL, cluster];
                    }
                }

                // Passo 5. Encontre o índice j onde D(j) seja mínimo.
                if (somaPonderada < somaPonderadaAux)
                {
                    menorCluster.id = cluster;
                    menorCluster.name = cluster.ToString();
                    menorCluster.distanciaEuclidiana = somaPonderada;
                    somaPonderadaAux = somaPonderada;
                }
                // Fim Passo 5
            }
            return menorCluster;
        }

        private Double[,] createMatriz()
        {
            Random random = new Random();
            clusters = new List<Cluster>();

            // Cria a matriz de clusterização            
            Double[,] matriz = new Double[ ( x * y ), qtdClusters];
            for (int i = 0; i < qtdClusters; i++) // 25 colunas
            {
                // Adiciona quantos cluster vai ter o sistema 
                Cluster cluster = new Cluster();
                cluster.id = i;
                cluster.name = i.ToString();
                clusters.Add(cluster);
                for (int j = 0; j < (x * y); j++)
                    matriz[j, i] = (random.NextDouble() * 2) - 1; // ((double * val max - val min) + val min) Varia cada valor de -1 até 1
            }
            return matriz;
        }

        private void carregaLetras()
        {
            try
            {
                string line;
                int posY = 0;
                int count = 0;
                DirectoryInfo directory = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\letters");
                foreach (FileInfo file in directory.GetFiles())
                {
                    posY = 0;
                    count = 0;
                    Letter letter = new Letter(x, y);
                    StreamReader reader = new StreamReader(file.FullName);
                    letter.name = file.Name.Substring(0,1).ToUpper();
                    letter.path = file.FullName;
                    while ((line = reader.ReadLine()) != null)
                    {
                        for (int posX = 0; posX < x; posX++)
                        {
                            letter.vector[count] = (line[posX] == '#') ? 1 : -1;
                            letter.matriz[posX, posY] = (line[posX] == '#') ? 1 : -1;
                            count++;
                        }
                        posY++;
                    }
                    letters.Add(letter);
                    //print(letter.toString());
                    //break;
                }
            }
            catch (DirectoryNotFoundException dnf)
            {
                print( "["+DateTime.Now.ToString("dd/MM/yyyy")+"] - Diretório não encontrado!" );
            }
            catch (Exception e)
            {
                print( "[" + DateTime.Now.ToString("dd/MM/yyyy") + "] - "+e.Message );
            }
        }

        private string toStringMatriz()
        {
            string matriz = "";
            string cabecalho = "";
            for (int j = 0; j < (x * y); j++)
            {
                for (int i = 0; i < qtdClusters; i++)
                    matriz += matrizValores[j, i].ToString("###,##0.000").PadLeft(QTYPADLEFT, ' ');
                matriz += "\r\n";
            }
            for (int i = 1; i < qtdClusters + 1; i++)
            {
                cabecalho += i.ToString().PadLeft(QTYPADLEFT, ' ');
            }
            matriz = "\r\n" + cabecalho + "\r\n" + matriz;

            matriz += "\r\n"; 
            matriz += "\r\n";

            foreach(Cluster c in clusters)
            {
                matriz += "CLASSE " + (c.id);
                matriz += "\r\n";
                matriz += c.percentualValor();
                matriz += "\r\n";
            }

            return matriz;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            matrizTeste = new int[x, y];
            int[] vetorEntrada = new int[x * y];
            try
            {
                foreach (Control c in pnlLetter.Controls)
                {
                    if (c is CheckBox)
                    {
                        vetorEntrada[((CheckBox)(c)).TabIndex] = (((CheckBox)(c)).Checked) ? 1 : -1;
                    }
                }

                int l = 0;

                Letter letraTeste = new Letter(x, y);
                print("\r\n");
                for(int j=0; j<y; j++)
                {
                    for (int i = 0; i < x; i++)
                    {
                        letraTeste.vector[l] = vetorEntrada[l];
                        letraTeste.matriz[i, j] = vetorEntrada[l];
                        l++;
                    }
                }
                print(letraTeste.toString());

                Cluster cluster = calculaDistanciaEuclidiana(letraTeste);
                print("Cluster selecionado : Cluster " + cluster.name + " \r\n" + clusters[cluster.id].percentualValor());


            }
            catch(Exception exc)
            {
                print("Erro ao iniciar teste. > " + exc.Message);
            }
        }

        public string toStringMatriz(int[,] matriz)
        {
            string letter = "\r\n";
            letter += "Letra Inserida";
            letter += "\r\n";
            for (int posY = 0; posY < y; posY++)
            {
                for (int posX = 0; posX < x; posX++)
                {
                    letter += (matriz[posX, posY] == 1) ? "+" : "-";
                }
                letter += "\r\n";
            }
            letter += "\r\n";
            return letter;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (Control c in pnlLetter.Controls)
            {
                if (c is CheckBox)
                {
                    ((CheckBox)(c)).Checked = false;
                }
            }
        }
    }
}
