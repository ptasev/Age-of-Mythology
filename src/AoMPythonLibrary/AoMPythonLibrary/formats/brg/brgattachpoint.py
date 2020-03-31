class BRGAttachpoint(object):
    """Age of Mythology BRG model attachpoint"""
    def __init__(self):
        self.name = ""
        self.xvector = [0, 0, 0]
        self.yvector = [0, 0, 0]
        self.zvector = [0, 0, 0]
        self.position = [0, 0, 0]
        self.bounding_box_min = [0, 0, 0]
        self.bounding_box_max = [0, 0, 0]

    def read_json(self, json):
        self.name = json["Name"]
        self.xvector = json["XVector"]
        self.yvector = json["YVector"]
        self.zvector = json["ZVector"]
        self.position = json["Position"]
        self.bounding_box_min = json["BoundingBoxMin"]
        self.bounding_box_max = json["BoundingBoxMax"]
