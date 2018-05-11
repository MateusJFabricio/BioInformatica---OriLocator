
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
       
        public void excluirFasta()
        {
            tamanho = 0;
            diretorio = null;
            arquivoPuro = null;
            genoma = null;
            tituloMaterial = null;
        }

        public void  organizarEstrutura(StreamReader arquivoFasta)
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
            while (!arquivoFasta.EndOfStream)
            {
                arquivoPuro.Add(arquivoFasta.ReadLine());
            }
            
            foreach (String temp in arquivoPuro){
                if (!(temp.Contains(">")))
                {
                    genoma = genoma + temp;
                }
                else { tituloMaterial = temp; };
            }

            tamanho = genoma.Length;
            
        }  
    }
}
