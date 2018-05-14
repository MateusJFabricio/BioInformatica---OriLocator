
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioInformaticaProj.Source.Model
{
    class ArquivoFasta
    {
        public int tamanho = 0;
        public String diretorio = null;
        public List<String> arquivoPuro = null;
        public String genoma;
        public String tituloMaterial = null;
        public StreamReader arquivoFasta;

        public void excluirFasta()
        {
            tamanho = 0;
            diretorio = null;
            arquivoPuro = null;
            genoma = null;
            tituloMaterial = null;
        }

        public void  organizarEstrutura()
        {
            if (arquivoPuro == null)
            {
                arquivoPuro = new List<string>();
            }

            if (arquivoFasta.EndOfStream)
            {
                throw new System.InvalidOperationException("Nao ha genoma a ser organizado");
            }

            arquivoPuro.Add(arquivoFasta.ReadLine());
            arquivoPuro.Add("");
            while (!arquivoFasta.EndOfStream)
            {
                arquivoPuro[1]  = arquivoPuro[1] + arquivoFasta.ReadLine();
            }
            /*
            foreach (String temp in arquivoPuro){
                if (!(temp.Contains(">")))
                {
                    genoma = genoma + temp;
                }
                else { tituloMaterial = temp; };
            }
            */
            tituloMaterial = arquivoPuro[0];
            genoma = arquivoPuro[1];
            tamanho = genoma.Length;
            
        }  
    }
}
