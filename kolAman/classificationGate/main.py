from Servies.Consumer_Kafka import Consume_raw_alerts_topic
import json
from Validations.alert_validator import validate_alert
import redis
from loggers.logger import logger
from dotenv import load_dotenv,find_dotenv
import os
from Servies.location_identefy import get_region_with_geopandas

def main():
    load_dotenv(find_dotenv())
    redis_host = os.getenv("REDIS_HOST","localhost")
    redis_port = os.getenv("REDIS_PORT",6379)
    regions_file_path = "../../alert-simulator/regions.geojson"
    
    redis_client = redis.Redis(host=redis_host, port=int(redis_port), decode_responses=True)
    
    
    while True:
        try:
            raw_alert_bytes = Consume_raw_alerts_topic()
            if raw_alert_bytes is None:
                continue
            raw_alert = raw_alert_bytes.decode("utf-8")
            raw_alert = json.loads(raw_alert)
            if validate_alert(raw_alert) == False:                
                continue
            
            if redis_client.get(raw_alert["alert_id"]) != None:
                continue
            redis_client.set( raw_alert["alert_id"],json.dumps(raw_alert))
            
            geographic_location = get_region_with_geopandas(regions_file_path,raw_alert["lon"],raw_alert["lat"])
            
               
               
            
        
        except Exception as err:
            logger.error(err)
            continue
        
    






main()


