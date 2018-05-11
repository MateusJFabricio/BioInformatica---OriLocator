using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioInformaticaProj.Source.Model
{
    class SkewDiagram
    {
        public String genoma;
        public int tamanhoRegiaoOri;
        public int inicioRegiaoOriNoGenoma;
        public List<String> listRegiaoOri;
        public List<int> listSkew;
        public List<int> listValesSkew;

        public SkewDiagram()
        {
            listSkew = new List<int>();
            listValesSkew = new List<int>();
            listRegiaoOri = new List<string>();
        }

        private void gerarlistSkew()
        {
            listSkew.Add(0);
            for (int i = 0; i < genoma.Length; i++)
            {
                listSkew.Add(listSkew[i]);
                if (genoma[i] == 'G')
                    listSkew[i + 1] = listSkew[i] + 1;
                else if (genoma[i] == 'C')
                listSkew[i + 1] = listSkew[i] - 1;
             }
        }

        private void EncontrarValesSkew()
        {
            int memoria = 0;
            int posInitOri = 0;
            int excedentePosInitOri = 0;
            int excedentePosFimOri = 0;
            int posFimOri = 0;
            String tempOri = "";

            if (listValesSkew == null)
            {
                List<int> listValesSkew = new List<int>();
            }

            listValesSkew.Clear();

            //Modificar para pegar vários vales
            foreach (int i in listSkew)
            {
                if (i < memoria)
                    memoria = i;
            }


            for (int i = 0; i < listSkew.Count; i++)
            {
                if (listSkew[i] == memoria)
                {
                    listValesSkew.Add(i);
                    posInitOri = i - (tamanhoRegiaoOri / 2);
                    posFimOri = i + (tamanhoRegiaoOri / 2);

                    if (posFimOri > genoma.Length)
                    {
                        excedentePosFimOri = posFimOri - genoma.Length;
                        posFimOri = genoma.Length;
                    }

                    if (posInitOri < 0)
                    {
                        excedentePosInitOri = Math.Abs(posInitOri);
                        posInitOri = 0;
                    }

                    if (posFimOri <= posInitOri)
                        continue;

                    tempOri = genoma.Substring(posInitOri, (posFimOri - posInitOri));
                    tempOri = genoma.Substring(genoma.Length - excedentePosInitOri, excedentePosInitOri) + tempOri + genoma.Substring(0, excedentePosFimOri);
                    listRegiaoOri.Add(tempOri);
                    if (excedentePosInitOri > 0)
                        inicioRegiaoOriNoGenoma = excedentePosInitOri;
                    else
                        inicioRegiaoOriNoGenoma = posInitOri;

                    break; //Adicionado esta linha para pegar somente o primeiro vale
                }
            }
        }

        public void GerarSkewDiagram()
        {
            gerarlistSkew();
            EncontrarValesSkew();
        }
    }
}
