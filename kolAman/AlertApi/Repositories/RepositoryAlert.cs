using AlertApi.Dtos;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NotificationGate.Models;

namespace AlertApi
{
    public class RepositoryAlert : IRepositoryAlert
    {

        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<Alert> _alerts;

        public RepositoryAlert(IMongoDatabase db)
        {
            _db = db;
            _alerts = db.GetCollection<Alert>("Alerts");

        }

        public async Task<CountAlertsByCommandsRes> GetCountAlertsByCommandsAsync()
        {
           var alerts = _alerts.AsQueryable();
           var countOverSeasAlert = await alerts.CountAsync(a => a.Geographic_location_command == "OVERSEAS");
           var countnNorthAlert = await alerts.CountAsync(a => a.Geographic_location_command == "NORTH");
           var countnCenterAlert = await alerts.CountAsync(a => a.Geographic_location_command == "CENTER");
           var countnSouthAlert = await alerts.CountAsync(a => a.Geographic_location_command == "SOUTH");

           var resoult  = new CountAlertsByCommandsRes
            {
                CENTER = countnCenterAlert,
                NORTH = countnNorthAlert,
                SOUTH = countnSouthAlert,
                OVERSEAS = countOverSeasAlert
            };

            return resoult;

        }

        public async Task<ICollection<CountAlertByComanndsPrioriryRes>> GetCountAlertsByCommandsPrioritiesAsync()
        {
            var alerts = _alerts.AsQueryable();
            


            var resoult  = await alerts.GroupBy(a => a.Geographic_location_command)
                .Select(a => new CountAlertByComanndsPrioriryRes
            {
                priority = a.Key,

                CRITICAL = a.Count(a => a.Priority == "CRITICAL"),
                
                HIGH = a.Count(a => a.Priority == "HIGH"),

                MEDIUM = a.Count(a => a.Priority == "MEDIUM"),

                LOW = a.Count(a => a.Priority == "LOW")

            }).ToListAsync();

            return resoult;

        }

        public async Task<ICollection<CountAlertByComanndsStatusRes>> GetCountAlertsByCommandsStatusAsync()
        {
            var alerts = _alerts.AsQueryable();



            var resoult = await alerts.GroupBy(a => a.Geographic_location_command)
                .Select(a => new CountAlertByComanndsStatusRes
                {
                    status = a.Key,

                    INPROGRESS = a.Count(a => a.Status == "INPROGRESS"),

                    DONE = a.Count(a => a.Status == "DONE"),

                    WAITING = a.Count(a => a.Status == "WAITING "),

                }).ToListAsync();

            return resoult;

        }


    }
}
