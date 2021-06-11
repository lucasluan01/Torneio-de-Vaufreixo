class Cavalheiro
{
    public int Esforco { get; set; }
    public int Pontos { get; set; }
    public string Nome { get; set; }
    public string Resultado { get; set; }

    public Cavalheiro(int pontos, int esforco, string nome, string resultado = "N/A")
    {
        Pontos = pontos;
        Esforco = esforco;
        Nome = nome;
        Resultado = resultado;
    }
}