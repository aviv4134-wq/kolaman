namespace AlertApi.Dtos
{
    public class CountAlertByComanndsPrioriryRes
    {
        
        public string priority { get; set; }
        public int CRITICAL { get; set; }

        public int HIGH { get; set; }

        public int MEDIUM { get; set; }

        public int LOW { get; set; }
        
    }
}
