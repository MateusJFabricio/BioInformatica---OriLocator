using BioInformaticaProj.Source.Controller;
using BioInformaticaProj.Source.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BioInformaticaProj
{
    public partial class Form1 : Form
    {
        Controller controller;

        Stopwatch stopwatch = new Stopwatch();
        int TamanhoAglomerado;
        int QuantidadeMers;
        int TamanhoOri;
        int DistHamming;

        public Form1()
        {
            controller = new Controller();   
            InitializeComponent();
            btnCancelar.Visible = false;
        }

        private void btnBuscarArquivo_Click(object sender, EventArgs e)
        {

            List<String> arquivoFasta = new List<string>();
            openFileDialog1.Filter = "Arquivo Fasta|*.fasta";

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            
            if (String.IsNullOrEmpty(openFileDialog1.FileName))
            {
                MessageBox.Show("Arquivo Inválido", "Erro de validação", MessageBoxButtons.OK);
                return;
            }

            System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog1.FileName);
            if (controller.carregarArquivoFasta(sr, openFileDialog1.FileName))
                statusStrip1.Items[0].Text = "Arquivo carregado.";

            textTituloGenoma.Text = controller.tituloMaterial;
   
        }
       
        private void btnLimpar_Click(object sender, EventArgs e)
        {
            controller.listSkew.Clear();
            
            gcSkew.Series["desaminacao_gc"].Points.Clear();
            textInicioExecucao.Text = "";
            textFimExecucao.Text = "";
            controller.limparFasta();
            textTituloGenoma.Text = "";
            textTempoTotalExecucao.Text = "";
            textInicioOri.Text = "";
            textFimOri.Text = "";
            textNumeroOcorrencias.Text = "";
            richSequenciaOri.Clear();
            richSequenciasEncontradas.Clear();
            statusStrip1.Items[0].Text = "Sem arquivo carregado. Selecione um genoma a ser carregado";

        }

        private void textQuantidadeMers_Validating(object sender, CancelEventArgs e)
        {
            int temp;
            if (textQuantidadeMers.Text != "")
            {
                if (!Int32.TryParse(textQuantidadeMers.Text.Trim(), out temp))
                {
                    MessageBox.Show("Valor invalido");
                    textQuantidadeMers.Text = "";
                }
            }
        }

        private void textTamanhoAglomerado_Validating(object sender, CancelEventArgs e)
        {
            int temp;
            if (textTamanhoAglomerado.Text != "")
            {
                if (!Int32.TryParse(textTamanhoAglomerado.Text.Trim(), out temp))
                {
                    MessageBox.Show("Valor invalido");
                    textTamanhoAglomerado.Text = "";
                }
            }
        }

        private void ValidarCamposBusca()
        {
            int i = 0;
            if (textQuantidadeMers.Text == "")
                throw new System.ArgumentException("O campo de quantidade de Mers é obrigatorio o preenchimento");

            if (! Int32.TryParse(textQuantidadeMers.Text, out i))
                throw new System.ArgumentException("O campo de quantidade de Mers Número inválido. ");

            if (i > 13)
                throw new System.ArgumentException("O campo de quantidade de Mers é inválido. Número máximo permitido é 13");

            if (textTamanhoAglomerado.Text == "")
                throw new System.ArgumentException("O campo de tamanho do aglomerado é obrigtorio o preenchimento");

            if (textTituloGenoma.Text == "")
                throw new System.ArgumentException("Carregue um genoma para iniciar a busca");


            if (textTamanhoOri.Text == "")
                throw new System.ArgumentException("Preencha o tamanho da região Ori");
            
            if ((Convert.ToInt32(cbDistHamming.Text) != 0) && (Convert.ToInt32(cbDistHamming.Text) != 1) && (Convert.ToInt32(cbDistHamming.Text) != 2))
                throw new System.ArgumentException("Valor invalido para a distância Hamming");

            if (Convert.ToInt32(textTamanhoAglomerado.Text) > Convert.ToInt32(textTamanhoOri.Text))
                throw new System.ArgumentException("O Aglomerado esta maior que o Ori");
        }

        private void PreencherDadosProcessados()
        {
            PreencherCamposRegiaoOri();
            PreencherDadosRichSequenciasEncontradas();
            PreencherDadosRichSequenciaOri();
            PreencherDadosGraficoGCSkew();      
        }

        private void PreencherCamposRegiaoOri()
        {
            textInicioOri.Text = controller.Resultado.LocalizacaoDna;
            textFimOri.Text = Convert.ToString(Convert.ToInt32(controller.Resultado.LocalizacaoDna) + Convert.ToInt32(textTamanhoAglomerado.Text));
            textNumeroOcorrencias.Text = Convert.ToString(controller.Resultado.NumeroTotalOcorrencias);
        }

        private void PreencherDadosRichSequenciasEncontradas()
        {
            String DnaA = "";
            String ComplementoReverso = "";
            String Neighbors = "";
            foreach (SequenciaResultado dados in controller.Resultado.listSequencia)
            {
                richSequenciasEncontradas.AppendText("Padrão encontrado na sequência ori: \n\n");
                //Pega o primeiro o padrão da lista
                foreach (Sequencia seq in dados.sequencia)
                {
                    if (seq.tipoSequencia == enumTipoSequencia.DnaA)
                    {
                        DnaA = DnaA + "\tDnaA encontrado\n";
                        DnaA = DnaA + "\t\tSequência: " + seq.sequencia + "\n";
                        DnaA = DnaA + "\t\tPosições no Ori: \n";
                        foreach(int pos in seq.posicaoSequenciaOri)
                        {
                            DnaA = DnaA + "\t\t\tNumero: " + Convert.ToString(pos) + "\n";
                        }
                        DnaA = DnaA + "\t\tNumero de ocorrências: " + seq.numeroOcorrencias + "\n\n";
                    }
                     
                    if ((seq.tipoSequencia == enumTipoSequencia.ComplementoReverso) && (seq.posicaoSequenciaOri.Count > 0))
                    {
                        ComplementoReverso = ComplementoReverso + "\tComplemento reverso encontrado\n";
                        ComplementoReverso = ComplementoReverso + "\t\tSequência: " + seq.sequencia + "\n";
                        ComplementoReverso = ComplementoReverso + "\t\tPosições no Ori: \n";
                        foreach (int pos in seq.posicaoSequenciaOri)
                        {
                            ComplementoReverso = ComplementoReverso + "\t\t\tNumero: " + Convert.ToString(pos) + "\n";
                        }
                        ComplementoReverso = ComplementoReverso + "\t\tNumero de ocorrências: " + seq.numeroOcorrencias + "\n\n";
                    }

                    if (seq.tipoSequencia == enumTipoSequencia.Neighbors)
                    {
                        Neighbors = Neighbors + "\tNeighbor encontrado\n";
                        Neighbors = Neighbors + "\t\tSequência: " + seq.sequencia + "\n";
                        Neighbors = Neighbors + "\t\tPosições no Ori: \n";
                        foreach (int pos in seq.posicaoSequenciaOri)
                        {
                            Neighbors = Neighbors + "\t\t\tNumero: " + Convert.ToString(pos) + "\n";
                        }
                        Neighbors = Neighbors + "\t\tNumero de ocorrências: " + seq.numeroOcorrencias + "\n\n";

                    }
                }
                richSequenciasEncontradas.AppendText(DnaA + ComplementoReverso + Neighbors);
                
            }
        }

        private void PreencherDadosRichSequenciaOri()
        {
            richSequenciaOri.AppendText(controller.Resultado.sequenciaOri);
            foreach (SequenciaResultado dados in controller.Resultado.listSequencia)
            {
                foreach (Sequencia seq in dados.sequencia)
                {
                    foreach (int pos in seq.posicaoSequenciaOri)
                    {
                        richSequenciaOri.Select(Convert.ToInt32(pos), Convert.ToInt32(textQuantidadeMers.Text));
                        richSequenciaOri.SelectionColor = Color.Blue;
                    }
                }
            }
        }

        private void PreencherDadosGraficoGCSkew()
        {
            int incremento = 0;
            statusStrip1.Items[0].Text = "Gerando gráfico Skew";
            gcSkew.Hide();
            gcSkew.Series["desaminacao_gc"].Points.AddXY(0, controller.listSkew[0]);

            for (int i = 0; i < controller.listSkew.Count; i++)
            {
                if (incremento == 1000)
                {
                    gcSkew.Series["desaminacao_gc"].Points.AddXY(i * 10, controller.listSkew[i]);
                    incremento = 0;
                }
                incremento = incremento + 1;
            }

        }

        private void btnIniciarBusca_Click(object sender, EventArgs e)
        {

            try
             {
                btnIniciarBusca.Enabled = false;
                btnBuscarArquivo.Enabled = false;
                btnLimpar.Enabled = false;
                btnCancelar.Visible = true;

                TamanhoAglomerado = Convert.ToInt32(textTamanhoAglomerado.Text);
                QuantidadeMers = Convert.ToInt32(textQuantidadeMers.Text);
                TamanhoOri = Convert.ToInt32(textTamanhoOri.Text);
                DistHamming = Convert.ToInt32(cbDistHamming.Text);

                textInicioExecucao.Text = DateTime.Now.ToString("HH:mm:ss");
                stopwatch.Start();
                //Limpa o gráfico
                gcSkew.Series["desaminacao_gc"].Points.Clear();

                ValidarCamposBusca();
                statusStrip1.Items[0].Text = "Buscando a região ORI. Aguarde o termino do processo....";


                backgroundWorker1.RunWorkerAsync();
                //define a progressBar para Marquee
                progressBar.Style = ProgressBarStyle.Marquee;
                progressBar.MarqueeAnimationSpeed = 5;

                lblBarraProgressoDescricao.Text = "Processando genoma. Aguarde a finalização";
               }
             catch (ArgumentException es)
             {
                 MessageBox.Show(es.Message);

             } 

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void gcSkew_PrePaint(object sender, ChartPaintEventArgs e)
        {
            //MessageBox.Show("Carregando");
        }

        private void gcSkew_PostPaint(object sender, ChartPaintEventArgs e)
        {
            //MessageBox.Show("COncluido");
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (tabControl1.SelectedIndex == 1)
                gcSkew.Show();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            controller.IniciarBuscaRegiaoOri(
                    TamanhoAglomerado,
                    QuantidadeMers,
                    TamanhoOri,
                    DistHamming
                    );

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // reconfigura a progressbar para o padrao.
                progressBar.MarqueeAnimationSpeed = 0;
                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.Value = 0;

                //caso a operação seja cancelada, informa ao usuario.
                lblBarraProgressoDescricao.Text = "Operação Cancelada pelo Usuário!";

            }
            else if (e.Error != null)
            {
                //informa ao usuario do acontecimento de algum erro.
                lblBarraProgressoDescricao.Text = "Aconteceu um erro durante a execução do processo!";

                // reconfigura a progressbar para o padrao.
                progressBar.MarqueeAnimationSpeed = 0;
                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.Value = 0;
            }
            else
            {
                PreencherDadosProcessados();
                statusStrip1.Items[0].Text = "Concluido";

                System.GC.Collect();
                textFimExecucao.Text = DateTime.Now.ToString("HH:mm:ss");
                stopwatch.Stop();
                textTempoTotalExecucao.Text = Convert.ToDateTime(stopwatch.Elapsed.ToString()).ToString("mm:ss");

                //informa que a tarefa foi concluida com sucesso.
                lblBarraProgressoDescricao.Text = "Tarefa Concluida com sucesso!";

                //Carrega todo progressbar.
                progressBar.MarqueeAnimationSpeed = 0;
                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.Value = 100;
                lblBarraProgressoDescricao.Text = progressBar.Value.ToString() + "%";
            }

            //habilita o botao 
            btnIniciarBusca.Enabled = true;
            btnBuscarArquivo.Enabled = true;
            btnLimpar.Enabled = true;
            btnCancelar.Visible = false;

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            //Cancelamento da tarefa com fim indeterminado [bgWorkerIndeterminada]
            if (backgroundWorker1.IsBusy)
            {
                // notifica a thread que o cancelamento foi solicitado.
                // Cancela a tarefa DoWork 
                backgroundWorker1.CancelAsync();
            }
            lblBarraProgressoDescricao.Text = "Cancelando busca";
            btnCancelar.Visible = false;
        }
    }
}
