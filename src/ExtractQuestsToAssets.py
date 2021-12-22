#import ETree
import xml.etree.ElementTree as ET
import os

def split_xml(path: str):
    if path.endswith('/') or path.endswith('\\'):
        path = path[:-1]
    tree = ET.parse(path)
    path.replace("\\", "/")
    path = path[0:path.rfind('/')]
    root = tree.getroot()

    if not os.path.exists(f"{path}/Assets"):
        os.mkdir(f"{path}/Assets")

    if not os.path.exists(f"{path}/Assets/Quests"):
        os.mkdir(f"{path}/Assets/Quests")

    for child in root:
        if child.tag == "Quests":
            quest_id = child.attrib['Title']
            if ":" in quest_id:
                quest_id = quest_id.replace(": ", "_")
            quest_file = open(f"{path}/Assets/Quests/{quest_id}.xml", "w")
            quest_file.write(ET.tostring(child, encoding='unicode', method='xml'))
            quest_file.close()

split_xml("D:/Users/callu/source/repos/RS3QuestFilter/QuestLog.xml")