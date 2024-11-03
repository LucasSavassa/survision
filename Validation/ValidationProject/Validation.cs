using Comuns.Classes;
using Comuns.Interfaces;
using CustomVisionPredictionService;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using ValidationProject;
using YoloPredictionService;

internal class Validation
{
    static string[] _labels = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "HelperFiles", "names.txt"));
    private static void Main(string[] args)
    {     
        Console.WriteLine("Iniciando validação...");

        string pastaRaiz = @"D:\Faculdade\TCC\Projeto\survision\Validation\Images";
        string[] dificuldades = Directory.GetDirectories(pastaRaiz);

        foreach (string dificuldade in dificuldades)
        {
            Console.WriteLine("__Validando dificuldade: " + Path.GetFileName(dificuldade));

            string[] rodadas = Directory.GetDirectories(dificuldade);

            foreach (string rodada in rodadas)
            {
                Console.WriteLine("____Validando rodada: " + Path.GetFileName(rodada));
                var resultadosRodada = new RodadaResults();
                resultadosRodada.RodadaName = Path.GetFileName(rodada);
                resultadosRodada.ImageResults = new List<ImageResults>();

                List<string> imagens = BuscaArquivosDaRodada(rodada);

                foreach (string imagem in imagens)
                {
                    Console.WriteLine("______Validando imagem: " + Path.GetFileName(imagem));
                    var resultadoIA = ResultadoImagemIA(imagem + ".jpg");
                    var resultadoReal = ResultadoImagemReal(imagem + ".txt");
                    resultadosRodada.ImageResults.Add(ComparaResultados(resultadoIA, resultadoReal, Path.GetFileName(imagem)));
                    Console.WriteLine("______Validade concluida da imagem: " + Path.GetFileName(imagem));
                }

                MontaTxtResultadoRodada(resultadosRodada, dificuldade);
                Console.WriteLine("____Validade concluida da rodada: " + Path.GetFileName(rodada));
            }

            Console.WriteLine("__Validade concluida da dificuldade: " + Path.GetFileName(dificuldade));
        }      
    }

    private static List<string> BuscaArquivosDaRodada(string caminho)
    {
        List<string> arquivosSemExtensao = new List<string>();

        foreach (string arquivo in Directory.GetFiles(caminho, "*.jpg"))
        {
            string caminhoSemExtensao = Path.Combine(Path.GetDirectoryName(arquivo), Path.GetFileNameWithoutExtension(arquivo));
            arquivosSemExtensao.Add(caminhoSemExtensao);
        }

        return arquivosSemExtensao;
    }

    private static IPredictionResult ResultadoImagemIA(string path)
    {
        var imagePrediction = new ImagePredictionYolo();

        using (var imagem = new Bitmap(path))
        {
            return imagePrediction.GetImageResults(imagem, 50);
        }
    }

    private static IPredictionResult ResultadoImagemReal(string path)
    {
        var resultadosReais = new ResultadosReais();
        resultadosReais.Predictions = new List<Prediction>();

        using (var arquivo = new StreamReader(path))
        {
            string linha;
            while ((linha = arquivo.ReadLine()) != null)
            {
                string[] valores = linha.Split(' ');

                string name = _labels[Convert.ToInt32(valores[0])];
                double probability = 100;
                double width = Convert.ToDouble(valores[3], CultureInfo.InvariantCulture);
                double height = Convert.ToDouble(valores[4], CultureInfo.InvariantCulture);
                double left = Convert.ToDouble(valores[1], CultureInfo.InvariantCulture) - (Convert.ToDouble(valores[3], CultureInfo.InvariantCulture) / 2);
                double top = Convert.ToDouble(valores[2], CultureInfo.InvariantCulture) - (Convert.ToDouble(valores[4], CultureInfo.InvariantCulture) / 2); ;


                var predicao = new Prediction(
                    name: name,
                    probability: probability,
                    left: left,
                    top: top,
                    height: height,
                    width: width
                );

                resultadosReais.Predictions.Add(predicao);
            }
        }   
        return resultadosReais;
    }

    private static ImageResults ComparaResultados(IPredictionResult resultadoIA, IPredictionResult resultadoReal, string nomeImagem)
    {
        var imageResults = new ImageResults();
        imageResults.ImageName = nomeImagem;
        imageResults.Resultados = new List<ObjectResults>();

        var labelsComparadas = new string[] {
            "Parafuso sem Bloqueio",
            "Arruela", 
            "Alfinete cirúrgico",
            "Porta Agulhas", 
            "Allis", 
            "Anâtomica" 
        };

        foreach (var label in labelsComparadas)
        {
            var resultadosObjeto = new ObjectResults();
            resultadosObjeto.ObjectName = label;

            List<Prediction> objetosReconhecidos = resultadoIA.Predictions.Where(p => p.Name == label).ToList();
            List<Prediction> objetosReais = resultadoReal.Predictions.Where(p => p.Name == label).ToList();

            foreach(var objetoReconhecido in objetosReconhecidos)
            {

                var objetoReal = objetosReais.Where(p => p.Left >= (objetoReconhecido.Left - 0.1) && p.Left <= (objetoReconhecido.Left + 0.1)
                                                      && p.Top >= (objetoReconhecido.Top - 0.1) && p.Top <= (objetoReconhecido.Top + 0.1))
                                             .FirstOrDefault();


                if (objetoReal.Probability == 0)
                {
                    resultadosObjeto.FP++;
                }
                else
                {
                    resultadosObjeto.VP++;
                    objetosReais.Remove(objetoReal);
                }
            }
            resultadosObjeto.FN = objetosReais.Count;
            imageResults.Resultados.Add(resultadosObjeto);
        }

        return imageResults;
    }

    private static void MontaTxtResultadoRodada(RodadaResults resultadosRodada, string caminho)
    {
        StringBuilder sb = new StringBuilder();

        using (StreamWriter writer = new StreamWriter(Path.Combine(caminho, resultadosRodada.RodadaName + "_Yolo.txt")))
        {
            foreach (var image in resultadosRodada.ImageResults)
            {
                writer.Write(image.ImageName + "|");
                foreach (var objeto in image.Resultados)
                {
                    writer.Write(objeto.VP.ToString() + "|");
                    writer.Write(objeto.FP.ToString() + "|");
                    writer.Write(objeto.FN.ToString() + "|");
                }
                writer.WriteLine();
            }
        }
    }
}