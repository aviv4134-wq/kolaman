import redis
import os
from loggers.logger import logger
from dotenv import load_dotenv,find_dotenv
import json

load_dotenv(find_dotenv())


redis_host = os.getenv("REDIS_HOST","localhost")
redis_port = os.getenv("REDIS_PORT",6379)

logger.info("connect to reddis")
redis_client = redis.Redis(host=redis_host, port=int(redis_port), decode_responses=True)


def check_if_already_exstis(raw_alert):
    if redis_client.get(raw_alert["alert_id"]) != None:
        logger.warning(f"alert id {raw_alert["alert_id"]} is alrady in the system (duplicate alert) ")
        return True
    return False

def save_raw_alert(raw_alert):
    redis_client.set( raw_alert["alert_id"],json.dumps(raw_alert),ex= 43423)


def close_client():
    redis_client.close()
    

    
    
