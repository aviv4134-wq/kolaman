from Servies.Consumer_Kafka import Consume_raw_alerts_topic,Commit_consumer,Consumer_close
import json
from Validations.alert_validator import validate_alert
from loggers.logger import logger
from loggers.loggerdocument import create_log_geographic_document,create_regular_log
import os
from Servies.location_identefy import get_region_with_geopandas
import pika
from elasticsearch import Elasticsearch
import Servies.reddis_client_servies 
import Servies.rabbit_client_servies as rabbit_client_servies


def main():

    logger.info("start classification servies")
  
    regions_file_path = "../../alert-simulator/regions.geojson"

    try:
        logger.info("start loop consume alerts from kafka raw_alerts topic")
        while True:
            try:
                raw_alert_bytes = Consume_raw_alerts_topic()
                if raw_alert_bytes is None:
                    continue
                raw_alert = raw_alert_bytes.decode("utf-8")
                raw_alert = json.loads(raw_alert)
                
                if Servies.reddis_client_servies.check_if_already_exstis(raw_alert) == True:
                    continue

                if validate_alert(raw_alert) == False:                
                    continue
                
                Servies.reddis_client_servies.save_raw_alert(raw_alert)

                
                
                geographic_location = get_region_with_geopandas(regions_file_path,raw_alert["lon"],raw_alert["lat"])
                raw_alert["geographic_command"] = geographic_location

                
                classified_alert = json.dumps(raw_alert)

                rabbit_client_servies.save_classification_alert(classified_alert)
                logger.debug(f"sent to rabbit  {classified_alert}")
                
                
                Commit_consumer()
            except Exception as err:
                        logger.error(err)
                        logger.info("start again consume loop")
                        continue
    
    except Exception as err:
        logger.error(err)
        rabbit_client_servies.close_client()
        Consumer_close()
        Servies.reddis_client_servies.close_client()
        logger.info("close reddis client and rabbit channel and kafka consumer")
        
        
    






main()


