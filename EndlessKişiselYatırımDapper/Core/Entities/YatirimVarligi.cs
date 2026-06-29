namespace EndlessKişiselYatırımDapper.Core.Entities
{
    public class YatirimVarligi
    {
        public int Id { get; set; }
        public string Sembol { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
        public decimal Miktar { get; set; }
        public decimal Fiyat { get; set; }
    }
}
