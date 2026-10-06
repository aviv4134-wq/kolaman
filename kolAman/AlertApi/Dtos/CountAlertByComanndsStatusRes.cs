namespace AlertApi.Dtos
{
    public class CountAlertByComanndsStatusRes
    {

        public string status { get; set; }

        public int DONE { get; set; }

        public int INPROGRESS { get; set; }

        public int WAITING { get; set; }
    }
}
