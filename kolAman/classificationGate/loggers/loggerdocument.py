
import datetime
import json

def create_log_geographic_document(geographic_command,massage,level = "INFO" ):
    log = {
        "level" : level,
        "geographic_command" : geographic_command,
        "massage" : massage,
        "@timestamp":  datetime.datetime.now()

    } 

    return json.dumps(log)

def create_regular_log(massage,level = "INFO"):
    log = {
            "level" : level,           
            "massage" : massage,
            "@timestamp":  datetime.datetime.now()
        } 