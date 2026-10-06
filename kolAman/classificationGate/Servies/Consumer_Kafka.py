from confluent_kafka import Consumer
import os
from dotenv import load_dotenv,find_dotenv


load_dotenv(find_dotenv())




kafka_boot_strap = os.getenv("BOOTSTRAP_SERVERS_KAFKA")


config = {
        'bootstrap.servers': kafka_boot_strap,
        'group.id':  'aa',
        'auto.offset.reset': 'earliest',
        'enable.auto.commit': 'false',

        
    }

consumer = Consumer(config)


topic = "rawAlerts"
consumer.subscribe([topic])

def Consume_raw_alerts_topic():
    msg = consumer.poll(2.0)
    if msg is None:       
        print("Waiting...")
        return None
    elif msg.error():
        print("ERROR: %s".format(msg.error()))
        return None
    if msg.value() == None:
        return None
    return  msg.value()

def Commit_consumer():
    consumer.commit()

def Consumer_close():
    consumer.close()
    



