




using AlertApi.Dtos;

namespace AlertApi
{
    public interface IRepositoryAlert
    {
        public Task<CountAlertsByCommandsRes> GetCountAlertsByCommandsAsync();

        public  Task<ICollection<CountAlertByComanndsPrioriryRes>> GetCountAlertsByCommandsPrioritiesAsync();

        public Task<ICollection<CountAlertByComanndsStatusRes>> GetCountAlertsByCommandsStatusAsync();



    }

}
