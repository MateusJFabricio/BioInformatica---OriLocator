using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioInformaticaProj.Source.Model
{
    class Utils
    {
        protected String ComplementoReverso(String palavra)
        {
            Char nucleotideo;
            String resultado = "";
            for (int i = 0; i < palavra.Length; i++)
            {
                nucleotideo = palavra[palavra.Length - i - 1];
                if (nucleotideo == 'A')
                    resultado = resultado + "T";
                else if (nucleotideo == 'C')
                    resultado = resultado + "G";
                else if (nucleotideo == 'T')
                    resultado = resultado + "A";
                else if (nucleotideo == 'G')
                    resultado = resultado + "C";
            }      

            return resultado;
        }

        protected int ConverterFitaParaNumero(String palavra)
        {
            String auxNumero = "";
            int resultado = 0;

            foreach (char letra in palavra)
            {
                if (letra == 'A')
                    auxNumero = auxNumero + "0";
                else if (letra == 'C')
                    auxNumero = auxNumero + "1";
                else if (letra == 'G')
                    auxNumero = auxNumero + "2";
                else if (letra == 'T')
                    auxNumero = auxNumero + "3";
            }
            
            for (int i = 0; i < auxNumero.Length; i++)
            {
                //Pega o caractere da palavra
                Char caractere = auxNumero[auxNumero.Length - i - 1];
                //Converte o caractere da palavra para numero
                int nmro = Convert.ToInt32(Convert.ToString(caractere));
                //Soma com o valor anterior
                resultado = resultado + ((int)Math.Pow(4, i) * nmro);
            }


            return resultado;
        }

        protected List<String> GerarListaPossibilidades(int Hamming, List<String> listaPossibilidade)
        {
            String baseNitrogenada;
            int possibilidades;
            if (Hamming == 1)
            {
                listaPossibilidade.Add("A");
                listaPossibilidade.Add("C");
                listaPossibilidade.Add("G");
                listaPossibilidade.Add("T");
                return listaPossibilidade;
            }

            GerarListaPossibilidades(Hamming - 1, listaPossibilidade);

            possibilidades = listaPossibilidade.Count;
            for (int i = 0; i < possibilidades; i++)
            {
                baseNitrogenada = listaPossibilidade[i];
                listaPossibilidade.Add(baseNitrogenada + "A");
                listaPossibilidade.Add(baseNitrogenada + "C");
                listaPossibilidade.Add(baseNitrogenada + "G");
                listaPossibilidade.Add(baseNitrogenada + "T");
            }

            return listaPossibilidade;
        }

        protected List<String> CompletaPalavras(String palavra, List<String> listaPossibilidades)
        {
            List<String> resultado = new List<string>();

            foreach(String sequencia in listaPossibilidades)
            {
                for (int i = 0; i <= (palavra.Length - sequencia.Length); i++)
                {
                    resultado.Add(palavra.Substring(0, i) + sequencia + palavra.Substring((i + sequencia.Length), (palavra.Length - (i + sequencia.Length))));
                }
            }

            return resultado;
        }
        
        protected List<String> GeraNeighbors(String palavra, int distHamming)
        {
            List<String> resultado = new List<string>();
            List<String> listaPossibilidades = new List<string>();

            if (distHamming == 0)
            {
                return resultado;
            }

            if (palavra.Length == 1)
            {
                resultado.Add("A");
                resultado.Add("C");
                resultado.Add("G");
                resultado.Add("T");
                return resultado;
            }

            listaPossibilidades = GerarListaPossibilidades(distHamming, listaPossibilidades);
            listaPossibilidades = CompletaPalavras(palavra, listaPossibilidades);
            resultado = RetirarPalavrasDuplicadas(listaPossibilidades);
            resultado.Remove(palavra);
            return resultado;
        }

        protected List<String> RetirarPalavrasDuplicadas(List<String> listaPossibilidades)
        {
            return listaPossibilidades.Distinct().ToList();
        }

        protected String ConverterNumeroParaFita(int numero, int K)
        {
            String resultado = "";
            String resultadoTemp = "";
            char letra;
            while (numero >= 4)
            {
                resultado = resultado + Convert.ToString(numero % 4);
                numero = numero / 4;
            }

            resultado = resultado + Convert.ToString(numero);

            for (int i = 0; i < resultado.Length; i++)
            {
                letra = resultado[resultado.Length - i - 1];
                resultadoTemp = resultadoTemp + Convert.ToInt32(Convert.ToString(letra));
            }

            resultado = resultadoTemp;
            resultadoTemp = "";

            foreach (char letter in resultado)
            {
                if (letter == '0')
                    resultadoTemp = resultadoTemp + "A";
                else if (letter == '1')
                    resultadoTemp = resultadoTemp + "C";
                else if (letter == '2')
                    resultadoTemp = resultadoTemp + "G";
                else if (letter == '3')
                    resultadoTemp = resultadoTemp + "T";
            }

            while (resultadoTemp.Length < K)
                resultadoTemp = "A" + resultadoTemp;

            return resultadoTemp;
        }



        //Conta as palavras em uma fita qualquer
        protected List<int> ContaPadraoFita(String fita, int TamPalavra, List<int> listContagem)
        {
            String palavra = "";
            int indice = 0;

            for (int i = 0; i <= fita.Length - TamPalavra; i++)
            {
                palavra = fita.Substring(i, TamPalavra);
                indice = ConverterFitaParaNumero(palavra);
                listContagem[indice] = listContagem[indice] + 1;

            }

            return listContagem;


        }


        // Pega todas as palavras que se repetem em uma frequencia do vetor
        protected List<int> PreencherListResultado(List<int> listContagemPadrao, List<int> listContagemComplementoReverso, List<int> listContagemNeighbors, List<int> listResultado)
        {
            for(int i = 0; i < listContagemPadrao.Count; i++)
            {
                listResultado[i] = listContagemPadrao[i] + listContagemComplementoReverso[i] + listContagemNeighbors[i];
            }
            
            return listResultado;
        }

        //Distância Hamming
        protected int ContarDiferencaSequencia(String seq1, String seq2)
        {
            int contador = 0;

            for (int i = 0; i < seq2.Length; i++)
            {
                if (seq1[i] != seq2[i])
                    contador = contador + 1;
            }

            return contador;

        }


        //Contagem de diferença com distancia Hamming
        protected int ContagemAproximadaPadrao(String sequencia, String palavra, int distHamming)
        {
            int resultado = 0;
            String palavra2;
            for (int i = 0; i < (sequencia.Length - palavra.Length + 1);i++)
            {
                palavra2 = sequencia.Substring(i, palavra.Length);
                if (ContarDiferencaSequencia(palavra, palavra2) <= distHamming)
                {
                    resultado = resultado + 1;
                }
                  
            }

            return resultado;
        }
          
          
    }
}
