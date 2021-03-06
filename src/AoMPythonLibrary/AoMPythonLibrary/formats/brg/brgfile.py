import os
import subprocess
import json
from .brganimation import BRGAnimation
from .brgmesh import BRGMesh
from .brgmaterial import BRGMaterial

ANIMATION = "Animation"
MESHES = "Meshes"
MATERIALS = "Materials"

class BRGFile(object):
    """Age of Mythology BRG model file"""
    def __init__(self):
        self.animation = BRGAnimation()
        self.meshes = []
        self.materials = []

    def open(self, filename):
        ext = os.path.splitext(os.path.basename(filename))[1]
        if ext == ".brg":
            converter = os.path.abspath(
                os.path.dirname(os.path.realpath(__file__)) +
                "../../../../AoMModelViewer/AoMModelViewer.exe")
            subprocess.run([converter, filename, "-s"])
            self.read(filename + ".json")
            os.remove(filename + ".json")
        else:
            self.read(filename)

    def read(self, filename):
        """
        Read the file at the given filename
        :param filename: The location of the file on the system
        """
        with open(filename, 'r') as file_stream:
            j = json.load(file_stream)

        self.animation.read_json(j[ANIMATION])

        for mesh in j[MESHES]:
            brg_mesh = BRGMesh()
            brg_mesh.read_json(mesh)
            self.meshes.append(brg_mesh)

        for material in j[MATERIALS]:
            brg_mat = BRGMaterial()
            brg_mat.read_json(material)
            self.materials.append(brg_mat)
