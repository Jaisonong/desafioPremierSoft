using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using static MyApp.Program;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string caminho = (@"C:\Estudos\DesafioPremierSoft\pessoas.txt");


            LeitorDeArquivo leitor = new LeitorDeArquivo();
            List<Candidato> candidatos = leitor.LerArquivo(caminho);

            ProcessadorDeDados processador = new ProcessadorDeDados(candidatos);
            processador.CalcularPorcentagemPorVaga();
            processador.MediaDeIdadePorVaga("QA");
            processador.IdadeCandidatoMaisVelho("Mobile");
            processador.IdadeCandidatoMaisNovo("Web");
            processador.CalcularSomaDeIdadeParaTodasVagas("QA");
            processador.EstadosDistintosEntreOsCandidatos();



            //Novo arquivo CSV com os nomes em ordem
            string caminhoNovoArquivo = @"C:\Estudos\DesafioPremierSoft\\Sorted_Academy_Candidates.csv";
            EscritorDeArquivo escritor = new EscritorDeArquivo();
            escritor.OrdenandoArquivo(candidatos, caminhoNovoArquivo);



            //Verifica número quadrado perfeito e verifica se está entre 18 e 30
            Func<int, bool> criterioIdadeQA = idade => Math.Sqrt(idade) % 1 == 0 && idade >= 18 && idade <= 30;
            //Verifica se o nome é palindromo, separando o sobrenome e deixando apenas o primeiro
            Func<string, bool> criterioNomeQA = nome => Palindromo(nome.Split(' ')[0]);

            //Verifica idade numero par entre 30 e 40
            Func<int, bool> criterioIdadeMobile = idade => idade % 2 == 0 && idade >= 30 && idade <= 40;
            //Verifica se o sobrenome começa com "C"
            Func<string, bool> criterinoNomeMobile = nome => nome.Split(' ')[1].StartsWith("C");

            //Encontrar o instrutor QA
            EncontrarInstrutor(candidatos, "QA", "SC", criterioIdadeQA, criterioNomeQA);
            //Encontrar o instrutor Mobile
            EncontrarInstrutor(candidatos, "Mobile", "PI", criterioIdadeMobile, criterinoNomeMobile);


        }
        public class Candidato
        {
            public string Nome { get; set; }
            public int Idade { get; set; }
            public string Vaga { get; set; }
            public string Estado { get; set; }

            public Candidato(string nome, int idade, string vaga, string estado)
            {
                Nome = nome;
                Idade = idade;
                Vaga = vaga;
                Estado = estado;
            }

            public override string ToString()
            {
                return $"{Nome}, {Idade} anos, {Vaga}, {Estado}";
            }
        }

        class LeitorDeArquivo
        {

            public List<Candidato> LerArquivo(string caminho)
            {

                List<Candidato> candidatos = new List<Candidato>();
                try
                {
                    //Pula o cabeçalho
                    var linhas = File.ReadAllLines(caminho).Skip(1);

                    //Percorre pelo arquivo inserindo os dados na lista
                    foreach (var linha in linhas)
                    {
                        var dados = linha.Split(';');

                        if (dados.Length == 4)
                        {
                            string nome = dados[0];
                            int idade = int.Parse(dados[1].Replace(" anos", ""));
                            string vaga = dados[2];
                            string estado = dados[3];

                            candidatos.Add(new Candidato(nome, idade, vaga, estado));
                        }
                        else
                        {
                            Console.WriteLine("Linha com fomato invalido: " + linha);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao ler o arquivo: {ex.Message}");
                }
                return candidatos;
            }
        }

        class ProcessadorDeDados
        {
            private List<Candidato> _candidatos;

            public ProcessadorDeDados(List<Candidato> candidatos)
            {
                _candidatos = candidatos;
            }

            public void CalcularPorcentagemPorVaga()
            {
                var totalDeCandidatos = _candidatos.Count;

                Console.WriteLine("Proporção de candidatos por vaga:");
                foreach (var vaga in _candidatos.Select(c => c.Vaga).Distinct())
                {
                    int quantidadePorVaga = _candidatos.Count(c => c.Vaga == vaga);
                    double porcentagem = (quantidadePorVaga * 100.0) / totalDeCandidatos;
                    Console.WriteLine($"{vaga}: {porcentagem:F2}%");
                }
            }

            public void MediaDeIdadePorVaga(string vaga)
            {
                var candidatosPorVaga = _candidatos.Where(c => c.Vaga == vaga).ToList();

                if (candidatosPorVaga.Any())
                {
                    double idadeMedia = candidatosPorVaga.Average(c => c.Idade);
                    Console.WriteLine($"\nIdade média dos candidatos de {vaga}: {idadeMedia:F0} anos");
                }
                else
                {
                    Console.WriteLine($"\nNenhum candidato encontrado para a vaga {vaga}.");
                }

            }

            public void IdadeCandidatoMaisVelho(string vaga)
            {
                var candidatosPorVaga = _candidatos.Where(c => c.Vaga == vaga).ToList();

                if (candidatosPorVaga.Any())
                {
                    int idadeMaisNovo = candidatosPorVaga.Max(c => c.Idade);
                    Console.WriteLine($"\nIdade do mais novo {vaga}: {idadeMaisNovo:F0} anos");
                }
                else
                {
                    Console.WriteLine($"\nNenhum candidato encontrado para a vaga {vaga}.");
                }
            }


            public void IdadeCandidatoMaisNovo(string vaga)
            {
                var candidatosPorVaga = _candidatos.Where(c => c.Vaga == vaga).ToList();

                if (candidatosPorVaga.Any())
                {
                    int idadeMaisNovo = candidatosPorVaga.Min(c => c.Idade);
                    Console.WriteLine($"\nIdade do mais novo {vaga}: {idadeMaisNovo:F0} anos");
                }
                else
                {
                    Console.WriteLine($"\nNenhum candidato encontrado para a vaga {vaga}.");
                }

            }

            public void CalcularSomaDeIdadeParaTodasVagas(string vaga)
            {
                var candidatosPorVaga = _candidatos.Where(c => c.Vaga == vaga).ToList();

                if (candidatosPorVaga.Any())
                {
                    int somaDeIdade = candidatosPorVaga.Sum(c => c.Idade);
                    Console.WriteLine($"\nSoma de idade {vaga}: {somaDeIdade:F0} anos");
                }
                else
                {
                    Console.WriteLine($"\nNenhum candidato encontrado para a vaga {vaga}.");
                }

            }

            public void EstadosDistintosEntreOsCandidatos()
            {
                var estadosDistintos = _candidatos.Select(c => c.Estado).Distinct().ToList();

                Console.WriteLine($"\nNumero de estados distindos: {estadosDistintos.Count}");

            }
        }

        public class EscritorDeArquivo
        {
            public void OrdenandoArquivo(List<Candidato> candidatos, string caminho)
            {

                var candidatosOrdenados = candidatos.OrderBy(c => c.Nome).ToList();

                try
                {
                    using (StreamWriter sw = new StreamWriter(caminho))
                    {
                        sw.WriteLine("Nome;Idade;Vaga;Estado");

                        foreach (var candidato in candidatosOrdenados)
                        {
                            sw.WriteLine($"{candidato.Nome};{candidato.Idade} anos;{candidato.Vaga};{candidato.Estado}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao escrever arquivo: {ex.Message}");
                }
            }

        }

        public static void EncontrarInstrutor(List<Candidato> candidatos, string vaga, string estado, Func<int, bool> criterioIdade, Func<string, bool> criterioNome)
        {
            //Filtrar candidatos
            var candidatosFiltrados = candidatos.Where(c => c.Vaga == vaga);

            //Filtrar o estado
            candidatosFiltrados = candidatosFiltrados.Where(c => c.Estado == estado);

            //Aplica critério de idade
            candidatosFiltrados = candidatosFiltrados.Where(c => criterioIdade(c.Idade));

            // Aplicar o critério de nome
            candidatosFiltrados = candidatosFiltrados.Where(c => criterioNome(c.Nome));

            var instrutor = candidatosFiltrados.FirstOrDefault();

            if (instrutor != null)
            {
                Console.WriteLine($"\nO nome do instrutor da vaga de {vaga} é: {instrutor.Nome}");
            }
            else
            {
                Console.WriteLine("\nNenhum candidato encontrado que atenda a todos os critérios.");
            }
        }

        //Verifica se o nome é palindromo
        public static bool Palindromo(string nome)
        {
            string nomeLimpo = nome.ToLower();
            return nomeLimpo == new string(nomeLimpo.Reverse().ToArray());
        }
    }
}

