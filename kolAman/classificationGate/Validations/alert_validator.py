
from datetime import datetime





def validate_alert(raw_alert):
    if  is_all_fields_exsits(raw_alert) == False:
         return False
    
    if is_fields_empty(raw_alert) == False:
        return False
    
    raw_alert = {'alert_id': 'a09d9b15-3a6b-4a4e-b2c8-fe33884c7fee', 'source': 'aman', 'title': 'זוהו הכנות לשיגור', 'content': 'זוהו הכנות לשיגור באזור בצרה, עיראק. זוהו 7 משגרים בשטח.', 'priority': 'HIGH', 'classification': 'SECRET', 'lat': 30.5062, 'lon': 47.7854, 'timestamp': '2026-10-05T13:04:40.666Z', 'status': 'WAITING'}
    alert_id = raw_alert["alert_id"] 
    source = raw_alert["source"]
    priority = raw_alert["priority"]
    title = raw_alert["title"]
    status = raw_alert["status"]
    lat,lon = raw_alert["lat"],raw_alert["lon"]
    classification = raw_alert["classification"]
    timestamp = raw_alert["timestamp"]
    content = raw_alert["content"]
    
    valild_titles_mossad =["התרעה על כוונה לפגוע ביעד ישראלי בחוץ לארץ","זוהה נתיב הברחת אמצעי לחימה","פעילות חריגה באתר אסטרטגי","ניסיון כניסה של פעיל עוין לישראל","העברת כספים לארגון טרור","תנועת פעיל עוין בין מדינות"]
    vaild_titles_pikud_haoref = ["ירי רקטות וטילים","חדירת כלי טיס עוין","חדירת מחבלים","התרעה מקדימה","רעידת אדמה","האירוע הסתיים"]
    vaild_titles_shabak = ["התרעה חמה לפיגוע","תנועת מחבל מבוקש","חשד לחדירה ליישוב","רכב חשוד","חשד לפעילות ריגול עבור גורם עוין","גניבת אמצעי לחימה"]
    valid_titels_aman = ["זוהו הכנות לשיגור","זוהה שיגור טיל בליסטי","זוהו הכנות לשיגור","זוהה כלי טיס בלתי מאויש עוין","תנועת כוחות חריגה סמוך לגבול","שיבושי ניווט באזור"]

    if type(alert_id) != str:
             return False 
    if type(content) != str:
             return False
    
    is_title_valid_check = None
    if source == "aman":
        is_title_validation_check = is_title_valid(title,valid_titels_aman)
    
    elif source == "mossad":
         is_title_validation_check = is_title_valid(title,valild_titles_mossad)

    elif source == "pikud-haoref":
         is_title_validation_check = is_title_valid(title,vaild_titles_pikud_haoref)

    elif source == "shabak":
         is_title_valid_check = is_title_valid(title,vaild_titles_pikud_haoref)
    
    

    if is_title_valid_check == False:
         return False

    if is_valid_priority(priority) == False:
         return False
    
    if  is_valid_classification(classification) == False:
         return False
    
    if is_valid_lat_lon(lat,lon) == False:
         return False

    if is_valid_timestamp(timestamp) == False:
         return False
    if is_status_valid(status) == False:
         return False
    return True





def is_all_fields_exsits(raw_alert):
    all_fields = ["alert_id","source","title","content","priority","classification","lat","lon","timestamp","status"]
    for field in all_fields:
        if field not in raw_alert.keys():
            return False
    return True

def is_fields_empty(raw_alert):    
        for key in raw_alert.keys():
            value = str(raw_alert[key])
            if value == None:
                 return False
            if value == "" or value.strip(" ") == "":
                 return False
        return True



def is_valid_lat_lon(lat,lon):
    try:
        int(lat)
        int(lon)
        if lat > 90 or lat < -90:
            return False

        if lon > 180 or lon < -180:
            return False

        return True
    except:
         return False

def is_title_valid(title,valid_titels):

    if type(title) != str:
        return False 
    title = title.strip(" ")
    if title not in valid_titels:      
        return False
    return True



def is_valid_priority(priority):
    if type(priority) != str:
            return False 
    priority = priority.strip(" ").upper()
    valid_priorities = ["CRITICAL", "HIGH" ,"MEDIUM" , "LOW"]
    
    if priority not in valid_priorities:
         return False
    
    return True

def is_valid_classification(classification):
    if type(classification) != str:
            return False 
    classification = classification.strip(" ").upper()
    valid_classification = ["UNCLASSIFIED", "RESTRICTED", "SECRET" , "TOP_SECRET"]
    if classification not in valid_classification:
         return False

    return True

def is_valid_timestamp(timestamp):
    try:
        datetime.fromisoformat(timestamp) 
        return True
    except:
         return False


def is_status_valid(status):
    if type(status) != str:
                return False 
    status = status.strip(" ").upper()
    if status != "WAITING":
         return False
    return True
