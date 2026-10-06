import logging


logger = logging.getLogger('Logger')
logger.setLevel(logging.INFO)
handler = logging.FileHandler("Logs/logs.txt")
formatter = logging.Formatter('%(asctime)s | %(funcName)s |   %(levelname)s | %(message)s',datefmt='%Y%m%d-%H:%M:%S')
handler.setFormatter(formatter)
logger.addHandler(handler)
consoleHandler = logging.StreamHandler()
consoleHandler.setFormatter(formatter)
logger.addHandler(consoleHandler)


