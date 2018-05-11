using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioInformaticaProj.Source.Model
{
    public enum enumTipoSequencia { DnaA, ComplementoReverso, Neighbors };
    class Sequencia
    {
        public enumTipoSequencia tipoSequencia;
        public String sequencia;
        public List<int> posicaoSequenciaOri;
        public int numeroOcorrencias;
        public Sequencia()
        {
            posicaoSequenciaOri = new List<int>();
        }
    }
    class SequenciaResultado
    {
        public List<Sequencia> sequencia;
        public SequenciaResultado()
        {
            sequencia = new List<Sequencia>();
        }
    }
    class DadosRegiaoOri
    {
        public List<SequenciaResultado> listSequencia { get; set; }
        public String sequenciaOri { get; set; }
        public String LocalizacaoDna { get; set; }
        public int NumeroTotalOcorrencias { get; set; }
        public DadosRegiaoOri()
        {
            listSequencia = new List<SequenciaResultado>();
        }
    }

    class Clump : Utils
    {
        public String genoma;
        public int tamanhoAglomerado;
        public int tamanhoRegiaoOri;
        public int tamanhoKmer;
        public List<String> listRegiaoOri;
        public List<String> listNeighbors;
        public List<int> listContagemPadrao;
        public List<int> listContagemComplementoReverso;
        public List<int> listContagemNeighbors;
        public List<int> listResultado;
        public List<int> listPosBusca;
        public int distanciaHamming;
        public int inicioRegiaoOriNoGenoma;
        public DadosRegiaoOri dadosOri;
        int maiorFrequencia = 0;

        public Clump()
        {
            listContagemPadrao = new List<int>();
            listResultado = new List<int>();
            listRegiaoOri = new List<string>();
            listContagemComplementoReverso = new List<int>();
            listNeighbors = new List<String>();
            dadosOri = new DadosRegiaoOri();
            listContagemNeighbors = new List<int>();
        }

        private void ValidarSequencia(String sequencia, int L, int K)
        {
            if (L > sequencia.Length)
                throw new System.ArgumentException("O tamanho do aglomerado é maior que a sequencia");

            else if (K > L)
		        throw new System.ArgumentException("O tamanho do K-mer é maior que a sequencia");
                
        }

        public List<int> ContaComplementoReversoFita(List<int> listContPadrao, String sequencia, int tamPalavra)
        {
            String palavra;
            int indicePadrao;
            List<int> resultado = new List<int>();
            resultado.AddRange(Enumerable.Repeat(0, Convert.ToInt32(Math.Pow(4, tamPalavra))));

            for (int i = 0; i <= (sequencia.Length - tamPalavra); i++)
            {
                palavra = sequencia.Substring(i, tamanhoKmer);
                indicePadrao = ConverterFitaParaNumero(palavra);
                resultado[indicePadrao] = QuantidadeComplementoReverso(palavra, listContPadrao);
            }

            return resultado;
        }

        private int QuantidadeComplementoReverso(String kmer, List<int> listContagem)
        {
            String complReverso = ComplementoReverso(kmer);
            int indiceComplementoReverso = ConverterFitaParaNumero(complReverso);
            return listContagem[indiceComplementoReverso];

        }

        public List<int> ContaNeighbors(List<int> listContagemPadrao, String sequencia, int tamanhoKmer, int distHamming)
        {
            String palavra;
            int indiceNeighbor;
            int indicePadrao;
            List<int> resultado = new List<int>();
            resultado.AddRange(Enumerable.Repeat(0, Convert.ToInt32(Math.Pow(4, tamanhoKmer))));
            if (distanciaHamming != 0)
            {
                for (int i = 0; i <= (sequencia.Length - tamanhoKmer); i++)
                {
                    palavra = sequencia.Substring(i, tamanhoKmer);
                    indicePadrao = ConverterFitaParaNumero(palavra);

                    listNeighbors = GeraNeighbors(palavra, distHamming);

                    //Busca a contagem de cada neighbor
                    foreach (String neighbor in listNeighbors)
                    {
                        indiceNeighbor = ConverterFitaParaNumero(neighbor);
                        resultado[indicePadrao] = resultado[indicePadrao] + listContagemPadrao[indiceNeighbor];
                    }

                }
            }

            return resultado;
        }

        public void ProcessarAglomeradoCompleto(String sequencia)
        {

            //Realiza a contagem do padrao
            listContagemPadrao.Clear();
            listContagemPadrao.AddRange(Enumerable.Repeat(0, Convert.ToInt32(Math.Pow(4, tamanhoKmer))));
            listContagemPadrao = ContaPadraoFita(sequencia, tamanhoKmer, listContagemPadrao);

            //Realiza a contagem do complemento reverso
            listContagemComplementoReverso.Clear();
            listContagemComplementoReverso.AddRange(Enumerable.Repeat(0, Convert.ToInt32(Math.Pow(4, tamanhoKmer))));
            listContagemComplementoReverso = ContaComplementoReversoFita(listContagemPadrao, sequencia, tamanhoKmer);

            //Realiza a contagem dos neighbors do padrao e do complemento reverso
            listContagemNeighbors.Clear();
            listContagemNeighbors.AddRange(Enumerable.Repeat(0, Convert.ToInt32(Math.Pow(4, tamanhoKmer))));
            listContagemNeighbors = ContaNeighbors(listContagemPadrao, sequencia, tamanhoKmer, distanciaHamming);

            //Inicializa listResultado
            listResultado.Clear();
            listResultado.AddRange(Enumerable.Repeat(0, Convert.ToInt32(Math.Pow(4, tamanhoKmer))));
            listResultado = PreencherListResultado(listContagemPadrao, listContagemComplementoReverso, listContagemNeighbors, listResultado); //Percorre a sequecia
        
        }

        public void ProcessarResultados(List<int> listResult, String seq, int inicioOriNoGenoma)
        {
            
            if (maiorFrequencia < listResult.Max())
            {
                maiorFrequencia = listResult.Max();
                dadosOri.sequenciaOri = seq;
                dadosOri.LocalizacaoDna = inicioOriNoGenoma.ToString();
                dadosOri.NumeroTotalOcorrencias = maiorFrequencia;
            }
        }

        public void ProcessaAglomerado(String regiaoOri)
        {
            String sequencia;
            String novoKMer;
            int indiceNovoKMer;
            int indiceAntigoKMer;

            //Processa a primeira fita passada
            sequencia = regiaoOri.Substring(0, tamanhoAglomerado); //Retira a fita da sequencia

            //Realiza a contagem por completo na sequencia
            ProcessarAglomeradoCompleto(sequencia);

            //Armazena os dados referentes à contagem
            //1 - Armazena o inicio do Ori
            //2 - Armazena a sequencia Ori
            //3 - Armazena a maior frequencia encontrada
            //4 - Armazena uma lista de kmer com a maior frequencia
            ProcessarResultados(listResultado, sequencia, inicioRegiaoOriNoGenoma);

            //Processa da segunda fita em diante
            for (int i = 0; i < regiaoOri.Length - tamanhoAglomerado + 1; i++)
            {
                if (i == 0)
                    continue;

                //Subtrai na sequencia que perdeu
                indiceAntigoKMer = ConverterFitaParaNumero(sequencia.Substring(0, tamanhoKmer));
                if (listContagemPadrao[indiceAntigoKMer] != 0)
                    listContagemPadrao[indiceAntigoKMer] = listContagemPadrao[indiceAntigoKMer] - 1;

                //Retira a fita da sequecia
                sequencia = regiaoOri.Substring(i, tamanhoAglomerado);

                //Soma na sequencia que ganhou
                novoKMer = sequencia.Substring((sequencia.Length - tamanhoKmer), tamanhoKmer);
                indiceNovoKMer = ConverterFitaParaNumero(novoKMer);
                listContagemPadrao[indiceNovoKMer] = listContagemPadrao[indiceNovoKMer] + 1;


                //Realiza a contagem do complemento reverso
                listContagemComplementoReverso.Clear();
                listContagemComplementoReverso.AddRange(Enumerable.Repeat(0, Convert.ToInt32(Math.Pow(4, tamanhoKmer))));
                listContagemComplementoReverso = ContaComplementoReversoFita(listContagemPadrao, sequencia, tamanhoKmer);

                //Realiza a contagem dos neighbors do padrao e do complemento reverso
                listContagemNeighbors.Clear();
                listContagemNeighbors.AddRange(Enumerable.Repeat(0, Convert.ToInt32(Math.Pow(4, tamanhoKmer))));
                listContagemNeighbors = ContaNeighbors(listContagemPadrao, sequencia, tamanhoKmer, distanciaHamming);

                //Soma os resultados encontrados
                listResultado.Clear();
                listResultado.AddRange(Enumerable.Repeat(0, Convert.ToInt32(Math.Pow(4, tamanhoKmer))));
                listResultado = PreencherListResultado(listContagemPadrao, listContagemComplementoReverso, listContagemNeighbors, listResultado); //Percorre a sequecia

                //Armazena os dados referentes à contagem
                //1 - Armazena o inicio do Ori
                //2 - Armazena a sequencia Ori
                //3 - Armazena a maior frequencia encontrada
                //4 - Armazena uma lista de kmer com a maior frequencia
                ProcessarResultados(listResultado, sequencia, inicioRegiaoOriNoGenoma);

            }

        }

        public void ColetaDadosProcessados()
        {
            List<String> neighbors = new List<string>();
            Sequencia objSequencia = new Sequencia();
            SequenciaResultado listSequenciaResultado = new SequenciaResultado();
            //Realiza a contagem por completo na sequencia
            ProcessarAglomeradoCompleto(dadosOri.sequenciaOri);

            
            int maiorIndice = listResultado.Max();

            for (int i = 0; i < listResultado.Count; i++)
            {
                if (listResultado[i] == maiorIndice)
                {
                    //Armazena os dados do padrão
                    objSequencia = new Sequencia();
                    objSequencia.tipoSequencia = new enumTipoSequencia();
                    objSequencia.tipoSequencia = enumTipoSequencia.DnaA;
                    objSequencia.numeroOcorrencias = listContagemPadrao[i];
                    objSequencia.sequencia = ConverterNumeroParaFita(i, tamanhoKmer);
                    //Adiciona as posições
                    objSequencia.posicaoSequenciaOri = new List<int>();
                    for (int j = 0; j <= dadosOri.sequenciaOri.Length - tamanhoKmer; j++)
                    {
                        if (objSequencia.sequencia == dadosOri.sequenciaOri.Substring(j, tamanhoKmer))
                        {
                            objSequencia.posicaoSequenciaOri.Add(j);
                        }
                    }
                    
                    listSequenciaResultado.sequencia.Add(objSequencia);

                    //-------------------------------------------------

                    //Armazena os dados do complemento reverso
                    objSequencia = new Sequencia();
                    objSequencia.tipoSequencia = new enumTipoSequencia();
                    objSequencia.tipoSequencia = enumTipoSequencia.ComplementoReverso;
                    objSequencia.sequencia = ComplementoReverso(ConverterNumeroParaFita(i, tamanhoKmer));
                    objSequencia.numeroOcorrencias = listContagemPadrao[ConverterFitaParaNumero(objSequencia.sequencia)];
                    //Adiciona as posições
                    objSequencia.posicaoSequenciaOri = new List<int>();
                    for (int j = 0; j <= dadosOri.sequenciaOri.Length - tamanhoKmer; j++)
                    {
                        if (objSequencia.sequencia == dadosOri.sequenciaOri.Substring(j, tamanhoKmer))
                        {
                            objSequencia.posicaoSequenciaOri.Add(j);
                        }
                    }
                    
                    listSequenciaResultado.sequencia.Add(objSequencia);

                    //-------------------------------------------------

                    //Analisa os neighbors do padrão
                    neighbors = GeraNeighbors(ConverterNumeroParaFita(i, tamanhoKmer), distanciaHamming);
                    foreach(String neighbor in neighbors)
                    {
                        if (listContagemPadrao[ConverterFitaParaNumero(neighbor)] > 0)
                        {
                            objSequencia = new Sequencia();
                            objSequencia.tipoSequencia = new enumTipoSequencia();
                            objSequencia.tipoSequencia = enumTipoSequencia.Neighbors;
                            objSequencia.numeroOcorrencias = listContagemPadrao[ConverterFitaParaNumero(neighbor)];
                            objSequencia.sequencia = neighbor;
                            //Adiciona as posições
                            objSequencia.posicaoSequenciaOri = new List<int>();
                            for (int j = 0; j <= dadosOri.sequenciaOri.Length - tamanhoKmer; j++)
                            {
                                if (objSequencia.sequencia == dadosOri.sequenciaOri.Substring(j, tamanhoKmer))
                                {
                                    objSequencia.posicaoSequenciaOri.Add(j);
                                }
                            }
                            
                            listSequenciaResultado.sequencia.Add(objSequencia);
                        }
                    }

                    //-------------------------------------------------

                    //Armazena o conjunto de sequencias já processadas
                    dadosOri.listSequencia.Add(listSequenciaResultado);

                }
            }

        }

        public List<int> RetornarPosicoesPadraoNaSequencia(String sequencia, String palavra, int tam)
        {
            List<int> resultado = new List<int>();

            for(int i = 0; i <= sequencia.Length - tamanhoKmer; i++)
            {
                if (palavra == sequencia.Substring(i,tam))
                {
                    resultado.Add(i);
                }
            }

            return resultado;
        }
        public void ProcessarGenoma()
        {
            foreach (String regiaoOri in listRegiaoOri)
            {

                //Inicia a busca na sequencia
                ValidarSequencia(regiaoOri, tamanhoAglomerado, tamanhoKmer);
                ProcessaAglomerado(regiaoOri);
                ColetaDadosProcessados();                 
            }
        }
    }
}
