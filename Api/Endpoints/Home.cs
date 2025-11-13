namespace MinimalApi.Endpoints
{
    public class Home
    {
        public string Mensagem => "API Minimal rodando com sucesso, com endpoints separados em clasees especificas para cada entidade!";

        public DateTime DataAtual => DateTime.Now;
    }
}
