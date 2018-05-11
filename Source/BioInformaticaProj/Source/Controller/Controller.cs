﻿using BioInformaticaProj.Source.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioInformaticaProj.Source.Controller
{

    class Controller
    {
        ArquivoFasta arquivoFasta;
        public String tituloMaterial;
        public List<int> listSkew;
        public DadosRegiaoOri Resultado;

        public Controller()
        {
            arquivoFasta = new ArquivoFasta();
            listSkew = new List<int>();
            Resultado = new DadosRegiaoOri();
        }

        public bool carregarArquivoFasta(StreamReader sr, String _diretorio)
        {
            arquivoFasta.diretorio = _diretorio;
            arquivoFasta.organizarEstrutura(sr);
            this.tituloMaterial = arquivoFasta.tituloMaterial;
            return true;
        }

        public void limparFasta()
        {
            arquivoFasta.excluirFasta();
        }

        public void IniciarBuscaRegiaoOri(int tamanhoAglomerado, int tamanhoKMer, int tamanhoRegiaoOri, int distHamming)
        {
            Clump algoritmoBusca = new Clump();
            SkewDiagram skew = new SkewDiagram();

            skew.genoma = arquivoFasta.genoma;
            skew.tamanhoRegiaoOri = tamanhoRegiaoOri;
            skew.GerarSkewDiagram();
            listSkew = skew.listSkew;

            algoritmoBusca.tamanhoAglomerado = tamanhoAglomerado;
            algoritmoBusca.tamanhoRegiaoOri = tamanhoRegiaoOri;
            algoritmoBusca.tamanhoKmer = tamanhoKMer;
            algoritmoBusca.genoma = arquivoFasta.genoma;
            algoritmoBusca.listPosBusca = skew.listValesSkew;
            algoritmoBusca.listRegiaoOri = skew.listRegiaoOri;
            algoritmoBusca.distanciaHamming = distHamming;
            algoritmoBusca.inicioRegiaoOriNoGenoma = skew.inicioRegiaoOriNoGenoma;

            algoritmoBusca.ProcessarGenoma();
            
            Resultado = algoritmoBusca.dadosOri;
        }


    }
}
