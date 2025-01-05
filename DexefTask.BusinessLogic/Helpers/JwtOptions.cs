namespace DexefTask.BusinessLogic.Helpers
{
    public class JwtOptions
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double Lifetime { get; set; }
    }
}
