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

            ForcaBruta(cavalheiro, quantClassificados - 1);
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

        static void ImprimeTabela(List<Cavalheiro> tabela, int ultimoClas)
        {
            // ordena a lista de cavalheiros por pontos e usa como critério de desempate o esforço
            tabela = tabela.OrderByDescending(p => p.Pontos).ThenByDescending(e => e.Esforco).ToList();

            Console.WriteLine($"\n{"Nome",-12}{"Pos",-12}{"Pontos",-12}{"Esforço",-12}{"Resultado",-12}\n");

            for (int i = 0; i < tabela.Count; i++)
            {
                if (tabela[i].Nome == "Ducan")
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                else
                    Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine($"{tabela[i].Nome,-12}{(i + 1),-12}{tabela[i].Pontos,-12}{tabela[i].Esforco,-12}{tabela[i].Resultado,-12}");

                Console.ResetColor();
                if (i == ultimoClas)
                    Console.WriteLine("---------------------------------------------------------");
            }
        }

        static void ForcaBruta(List<Cavalheiro> cavalheiro, int ultimoClas)
        {
            Cavalheiro Ducan, melhorResultado;
            List<Cavalheiro> simulacao;
            string binario;
            int combinacoes, melhorSimulacao = 0;

            Ducan = new Cavalheiro(0, 0, "Ducan", ""); // cria cavalheiro Sr. Ducan

            // objeto para armazenar o caso mais favorável para o Sr. Ducan
            melhorResultado = new Cavalheiro(int.MaxValue, int.MaxValue, "Ducan", "");

            combinacoes = (int)Math.Pow(2, cavalheiro.Count); // identifica o número de combinações possíveis

            for (int i = 0; i < combinacoes; i++) // executa todas as combinações
            {
                Console.WriteLine($"\n\n\nSIMULAÇÃO: {i + 1}");

                simulacao = new List<Cavalheiro>(); // zera os resultados da simulação anterior

                // cria cópia da lista de cavalheiros para simular os campeonatos
                CopiaLista(cavalheiro, simulacao);

                // zera os valores do Ducan indicando a sua chegada no torneio
                Ducan = new Cavalheiro(0, 0, "Ducan", "");

                binario = Convert.ToString(i, 2); // converte o valor i em binário

                // acrescenta 0's à esquerda, se necessário, para completar a quantidade de bits
                binario = binario.PadLeft(cavalheiro.Count, '0');

                for (int j = 0; j < binario.Length; j++) // percorre cada cavalheiro da simulação atual
                {
                    // realiza uma justa com Ducan x Cavalheiro Atual
                    Justa(Ducan, simulacao, binario[j], j);
                }
                ResultadoSimulacao(Ducan, simulacao, ultimoClas, ref melhorResultado, ref melhorSimulacao, i);
            }

            if (melhorResultado.Esforco != int.MaxValue)
            {
                Console.WriteLine("\n\nTotal de Simulações: " + combinacoes);
                Console.WriteLine($"\nMelhor Simulação: {melhorSimulacao,-5}Pontos: {melhorResultado.Pontos,-5} Esforço: {melhorResultado.Esforco,-5}\n");
            }
            else
                Console.WriteLine("\n\n-1");
        }

        static void CopiaLista(List<Cavalheiro> cavalheiro, List<Cavalheiro> simulacao)
        {
            foreach (var item in cavalheiro)
                simulacao.Add(new Cavalheiro(item.Pontos, item.Esforco, item.Nome, item.Resultado));
        }

        static void Justa(Cavalheiro Ducan, List<Cavalheiro> simulacao, char oponente, int j)
        {
            // se cavaleiro na posição j da simulação for igual a zero, então Ducan vence
            if (oponente == '0')
            {
                Ducan.Pontos += 1;
                Ducan.Esforco += simulacao[j].Esforco;
                simulacao[j].Resultado = "D";
            }
            else // senão, o oponente vence
            {
                simulacao[j].Esforco += Ducan.Esforco;
                simulacao[j].Pontos += 1;
                simulacao[j].Resultado = "V";
            }
        }

        static void ResultadoSimulacao(Cavalheiro Ducan, List<Cavalheiro> simulacao, int ultimoClas, ref Cavalheiro melhorResultado, ref int melhorSimulacao, int i)
        {
            int indexDucan;

            // adiciona a pontuação de Ducan para gerar uma tabela com os resultados
            simulacao.Add(Ducan);

            // ordena a tabela para identificar os resultados da simulação
            simulacao = simulacao.OrderByDescending(p => p.Pontos).ThenByDescending(e => e.Esforco).ToList();

            ImprimeTabela(simulacao, ultimoClas);

            // identifica a posição do Ducan na tabela
            indexDucan = simulacao.FindIndex(n => n.Nome == "Ducan");

            // verifica se Ducan está entre os classificados e se é o melhor resultado
            if (indexDucan <= ultimoClas && simulacao[indexDucan].Esforco < melhorResultado.Esforco)
            {
                melhorResultado = simulacao[indexDucan];
                melhorSimulacao = i + 1;
            }
        }
    }
}