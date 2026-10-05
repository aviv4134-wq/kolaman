from Servies.Consumer_Kafka import Consume_raw_alerts_topic
import json
from Validations.alert_validator import validate_alert
import redis
from loggers.logger import logger
from dotenv import load_dotenv,find_dotenv
import os

def main():

    
    while True:
        try:
            raw_alert_bytes = Consume_raw_alerts_topic()
            if raw_alert_bytes is None:
                continue
            raw_alert = raw_alert_bytes.decode("utf-8")
            raw_alert = json.loads(raw_alert)
            if validate_alert(raw_alert) == False:                
                continue
        
        except Exception as err:
            logger.error(err)
            continue
        
    






main()


