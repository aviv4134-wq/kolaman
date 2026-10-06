using NotificationGate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertMangment.Serveis
{
    public class ManageAlert
    {



        public bool is_importent_alert(Alert alert)
        {
            if (alert.Title == "האירוע הסתיים")
                return true;

            if (alert.Priority == "CRITICAL" || alert.Priority == "HIGH"
               || alert.Classification == "TOP_SECRET" || alert.Classification == "SECRET")
                return true;
            return false;


        }
    }
}
