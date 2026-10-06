import os
from loggers.logger import logger
from dotenv import load_dotenv,find_dotenv
import json
import pika

load_dotenv(find_dotenv())

rabbit_host = os.getenv("RABBIT_HOST","localhost")

logger.info("connect to rabbit")

connection = pika.BlockingConnection(pika.ConnectionParameters(rabbit_host))
logger.info("create a channel to rabbit")    
channel = connection.channel()

logger.info("craete classified_alerts queue")
channel.queue_declare(queue='classified_alerts', durable=True, arguments={'x-queue-type': 'quorum'})


def save_classification_alert(classified_alert):

    channel.basic_publish(exchange='',
                        routing_key='classified_alerts',
                        body=classified_alert)

def close_client():
    connection.close()
    channel.close()