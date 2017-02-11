from .brgattachpoint import BRGAttachpoint

ATTACHPOINTS = "Attachpoints"
VERTICES = "Vertices"
NORMALS = "Normals"
TEXTURE_COORDINATES = "TextureCoordinates"
COLORS = "Colors"
FACES = "Faces"
FACE_MATERIALS = "FaceMaterials"
MESH_ANIMATIONS = "MeshAnimations"

class BRGMesh(object):
    """Age of Mythology BRG model mesh"""
    def __init__(self):
        self.attachpoints = []
        self.vertices = []
        self.normals = []
        self.uvs = []
        self.colors = []
        self.faces = []
        self.face_materials = []
        self.mesh_animations = []

    def read_json(self, json):
        for attachpoint in json[ATTACHPOINTS]:
            attach = BRGAttachpoint()
            attach.read_json(attachpoint)
            self.attachpoints.append(attach)

        self.vertices = json[VERTICES]

        self.normals = json[NORMALS]

        self.uvs = json[TEXTURE_COORDINATES]

        self.colors = json[COLORS]

        self.faces = json[FACES]

        self.face_materials = json[FACE_MATERIALS]

        for mesh in json[MESH_ANIMATIONS]:
            brg_mesh = BRGMesh()
            brg_mesh.read_json(mesh)
            self.mesh_animations.append(brg_mesh)
