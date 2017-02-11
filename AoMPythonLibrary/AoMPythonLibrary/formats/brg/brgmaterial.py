NAME = "Name"
DIFFUSE_COLOR = "DiffuseColor"
AMBIENT_COLOR = "AmbientColor"

class BRGMaterial(object):
    """Age of Mythology BRG model material"""
    def __init__(self):
        self.name = ""
        self.diffuse_color = [1, 1, 1]
        self.ambient_color = [1, 1, 1]
        self.specular_color = [0, 0, 0]
        self.emissive_color = [0, 0, 0]
        self.opacity = 1.0
        self.specular_exponent = 0.0
        self.face_map = False
        self.two_sided = False

        self.id_num = 0
        self.flags = 0
        self.diffuse_map = ""
        self.bump_map = ""
        self.sfx = []

    def read_json(self, json):
        self.name = json[NAME]
        self.diffuse_color = json[DIFFUSE_COLOR]
        self.ambient_color = json[AMBIENT_COLOR]
        self.specular_color = json["SpecularColor"]
        self.emissive_color = json["EmissiveColor"]
        self.opacity = json["Opacity"]
        self.specular_exponent = json["SpecularExponent"]
        self.face_map = json["FaceMap"]
        self.two_sided = json["TwoSided"]

        self.id_num = json["Id"]
        self.flags = json["Flags"]
        self.diffuse_map = json["DiffuseMap"]
        self.bump_map = json["BumpMap"]

class BRGMaterialFlags(object):
    MATNONE1 = 0x80000000
    MATNONE2 = 0x40000000
    MATNONE3 = 0x20000000
    ILLUMREFLECTION = 0x10000000
    MATNONE10 = 0x08000000
    REFLECTIONTEXTURE = 0x04000000
    PLAYERCOLOR2 = 0x02000000
    LOWPLAYERCOLOR2 = 0x01000000
    Alpha = 0x00800000
    SubtractiveBlend = 0x00400000
    AdditiveBlend = 0x00200000
    FaceMap = 0x00100000
    PixelXForm2 = 0x00080000
    PixelXForm1 = 0x00040000
    SpecularExponent = 0x00020000
    MATNONE16 = 0x00010000
    BumpMap = 0x00008000
    PlayerXFormTx2 = 0x00004000
    PlayerXFormTx1 = 0x00002000
    PlayerXFormColor2 = 0x00001000
    PlayerXFormColor1 = 0x00000800
    TwoSided = 0x00000400
    WrapVTx3 = 0x00000200
    WrapUTx3 = 0x00000100
    WrapVTx2 = 0x00000080
    WrapUTx2 = 0x00000040
    WrapVTx1 = 0x00000020
    WrapUTx1 = 0x00000010
    Specular = 0x00000008
    UseColors = 0x00000004
    Updateable = 0x00000002
    HasTexture = 0x00000001

    def __init__(self):
        self.flags = 0

    def has_flag(self, flag):
        return (self.flags & flag) == flag
