class BRGAnimation(object):
    """Age of Mythology BRG model animation"""
    def __init__(self):
        self.duration = 1.0
        self.time_step = 1.0
        self.mesh_keys = []

    def read_json(self, json):
        self.duration = json["Duration"]
        self.time_step = json["TimeStep"]
        self.mesh_keys = [key for key in json["MeshKeys"]]
