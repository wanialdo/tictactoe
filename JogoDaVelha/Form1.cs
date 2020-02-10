using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JogoDaVelha
{
    public partial class Form1 : Form
    {
        #region campos globais
        private int numPlayers = 1;
        private int activePlayer = 0;
        private int[,] game = new int[3, 3];
        #endregion

        #region métodos dos objetos
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            statusPictureBox(false);
        }

        /// <summary>
        /// Realiza uma jogada.
        /// </summary>
        private void pictureBox_Click(object sender, EventArgs e)
        {
            //Essa linha não cria um objeto. Ela faz referência ao objeto.
            PictureBox obj = (PictureBox)sender;

            //Carrega a imagem do jogador.
            if (activePlayer == 1)
                obj.Image = System.Drawing.Image.FromFile("Circle.png");
            else
                obj.Image = System.Drawing.Image.FromFile("Xis.png");

            //Faz com que a imagem tome todo o espaço do picturebox
            obj.BackgroundImageLayout = ImageLayout.Zoom;

            //Desliga o picturebox para não ser clicado novamente.
            obj.Enabled = false;

            //Substrig retorna parte do texto. Parametro padrão e o local de início.
            //Length me retorna a quantidade de caracteres de um texto.
            int pos = Convert.ToInt32(obj.Name.Substring(obj.Name.Length - 1));

            game[Convert.ToInt32((pos - 1) / 3), ((pos - 1) % 3)] = activePlayer;

            if (isVictory(activePlayer))
            {
                MessageBox.Show("Vitória do Jogador " + activePlayer.ToString());
                statusPictureBox(false);
            }
            else
            {
                if (activePlayer == 1)
                    activePlayer = 2;
                else
                    activePlayer = 1;

                if ((numPlayers == 1) && (activePlayer == 2))
                {
                    ComputerPlay();
                }
            }
        }

        private void novoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startGame(1);
        }

        private void start2PlayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startGame(2);
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region métodos gerais do jogo
        /// <summary>
        /// Verifica se há um vencedor no jogo.
        /// </summary>
        private bool isVictory(int player)
        {
            int lin = 0; int col = 0; int dg1; int dg2;
            dg1 = 0;
            dg2 = 0;

            for (int i = 0; i <= 2; i++)
            {
                lin = 0;
                col = 0;
                for (int j = 0; j <= 2; j++)
                {
                    if (game[i, j] == player) lin += 1;
                    if (game[j, i] == player) col += 1;
                    if ((game[i, j] == player) && (i + j == 2)) dg1 += 1;
                    if ((game[i, j] == player) && (i == j)) dg2 += 1;
                }
                if ((lin == 3) || (col == 3) || (dg1 == 3) || (dg2 == 3)) return true;
            }

            return false;
        }

        /// <summary>
        /// Configura o início de uma partida
        /// </summary>
        private void startGame(int players)
        {
            numPlayers = players;
            activePlayer = 1;
            statusPictureBox(true);
            game = new int[3, 3];
        }

        /// <summary>
        /// Habilita ou desabilita as casas do jogo
        /// </summary>
        private void statusPictureBox(bool status)
        {
            foreach (Object control in this.Controls)
            {
                if (control.GetType() == new PictureBox().GetType())
                {
                    ((PictureBox)control).Enabled = status;
                    ((PictureBox)control).Image = null;
                }
            }
        }
        #endregion

        #region jogador computador
        /// <summary>
        /// Jogada realizada pelo computador.
        /// </summary>
        private void ComputerPlay()
        {
            //A pior parte, ensinar o computador a jogar.
            bool played = false;
            string[] jogadas = { "0,0;0,1;0,2", "1,0;1,1;1,2", "2,0;2,1;2,2", 
                                  "0,0;1,0;2,0", "0,1;1,1;2,1", "0,2;1,2;2,2", 
                                  "0,0;1,1;2,2", "0,2;1,1;2,0" };

            //Anular a possibilidade de ganho do oponente.
            for (int i = 0; i <= 7; i++)
            {
                if (WinRisk(jogadas[i], 1))
                {
                    ComputerMove(ChooseMove(jogadas[i]));
                    played = true;
                }
            }

            //Vencer se possível
            if (!played)
            {
                for (int i = 0; i <= 7; i++)
                {
                    if (WinRisk(jogadas[i], 2))
                    {
                        ComputerMove(ChooseMove(jogadas[i]));
                        played = true;
                    }
                }
            }

            //Nos outros casos a jogada é aleatória para evitar que o jogo sempre termine em empate.
            if (!played)
                ComputerMove(ChooseMove());
        }

        /// <summary>
        /// Seleciona uma casa aleatória para realizar a jogada.
        /// </summary>
        private int ChooseMove()
        {
            //Gerar números aleatórios.
            Random rnd = new Random();
            int spot = 0;
            try
            {
                string value = rnd.Next(1,10).ToString();
                spot = Convert.ToInt32(value.Substring(0, 1));
            }
                //Coloquei o throw
            catch (Exception ex)
            {

            }

            if (spot == 0)
                spot = 1;

            if (game[Convert.ToInt32((spot - 1) / 3), ((spot - 1) % 3)] != 0)
                spot = ChooseMove();

            return spot;
        }

        /// <summary>
        /// Seleciona uma casa em uma linha informada para jogar.
        /// </summary>
        private int ChooseMove(string boardLine)
        {
            string[] spots = { "0,0", "0,1", "0,2", "1,0", "1,1", "1,2", "2,0", "2,1", "2,2" };
            int cont = 0;
            foreach (string item in boardLine.Split(';'))
            {
                if (game[Convert.ToInt32(item.Split(',')[0]), Convert.ToInt32(item.Split(',')[1])] == 0) {
                    for (int i = 0; i <= 8; i++)
                    {
                        if (spots[i] == item)
                            cont = i + 1;
                    }
                }
            }

            return cont;
        }

        /// <summary>
        /// Verifica se o jogador informado já preencheu duas casas na linha informada.
        /// </summary>
        private bool WinRisk(string boardLine, int player)
        {
            int cont = 0;
            bool canplay = false;
            foreach (string item in boardLine.Split(';'))
            {
                if (game[Convert.ToInt32(item.Split(',')[0]), Convert.ToInt32(item.Split(',')[1])] == player) cont++;
                if (game[Convert.ToInt32(item.Split(',')[0]), Convert.ToInt32(item.Split(',')[1])] == 0) canplay = true;
            }

            if ((cont == 2) && (canplay))
                return true;

            return false;
        }

        /// <summary>
        /// Realiza a jogada do computador.
        /// </summary>
        private void ComputerMove(int numPictBox)
        {
            PictureBox obj = (PictureBox)this.Controls.Find("PictureBox" + numPictBox.ToString(), false)[0];
            pictureBox_Click(obj, null);
        }
        #endregion

        private void sobreToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
