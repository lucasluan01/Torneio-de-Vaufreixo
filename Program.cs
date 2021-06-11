using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Torneio_de_Vaufreixo
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Cavalheiro> cavalheiro = new List<Cavalheiro>();
            int quantClassificados = 0;

            LerArquivo(ref quantClassificados, cavalheiro);

            ImprimeTabela(cavalheiro, quantClassificados - 1);
        }

        static void LerArquivo(ref int quantClassificados, List<Cavalheiro> cavalheiro, string nomeArq = "")
        {
            StreamReader arquivo;
            string linha;
            string[] split;
            int cont = 1;

            // solicita o nome do arquivo até que o nome seja válido
            while (!File.Exists(nomeArq))
            {
                Console.Write("\nInforme o nome do arquivo: ");
                nomeArq = Console.ReadLine() + ".txt";

                if (!File.Exists(nomeArq))
                {
                    MensagemErro($"O arquivo {nomeArq} não foi encontrado!");
                    Console.WriteLine("Pressione qualquer tecla para tentar novamente.");
                    Console.ReadKey();
                }
            }
            arquivo = new StreamReader(nomeArq); // abre arquivo para leitura
            linha = arquivo.ReadLine(); // lê a primeira linha (informações do torneio)

            try
            {
                // armazena o número de classificados no torneio
                quantClassificados = int.Parse(linha.Split(' ')[1]);
                linha = arquivo.ReadLine(); // lê a próxima linha (dados dos cavaleiros)

                while (linha != null)
                {
                    // cria novo cavalheiro e adiciona na lista
                    cavalheiro.Add(new Cavalheiro(int.Parse(linha.Split(' ')[0]), int.Parse(linha.Split(' ')[1]), "Cav. " + cont.ToString()));

                    cont++;
                    linha = arquivo.ReadLine(); // lê próxima linha (dados dos cavaleiros)
                }
                arquivo.Close(); // fecha arquivo
            }
            catch (Exception ex)
            {
                MensagemErro("Erro na leitura do arquivo. Linha: " + (cont + 1));
                MensagemErro(ex.Message);
            }
        }

        static void MensagemErro(string mensagem)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{mensagem}\n");
            Console.ResetColor();
        }

        static void ImprimeTabela(List<Cavalheiro> lista, int ultimoClas)
        {
            // ordena a lista de cavalheiros por pontos e usa como critério de desempate o esforço
            lista = lista.OrderByDescending(p => p.Pontos).ThenByDescending(e => e.Esforco).ToList();

            Console.WriteLine("\n\n{0, -12}{1, -12}{2, -12}{3, -12}{4, -12}\n", "Nome", "Pos", "Pontos", "Esforço", "Resultado");

            for (int i = 0; i < lista.Count; i++)
            {
                Console.Write("{0, -12}", lista[i].Nome);

                Console.WriteLine("{0, -12}{1, -12}{2, -12}{3, -12}", (i + 1), lista[i].Pontos, lista[i].Esforco, lista[i].Resultado);

                if (i == ultimoClas)
                    Console.WriteLine("---------------------------------------------------------");
            }
        }
    }
}