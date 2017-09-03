bl_info = {
    "name": "Age of Mythology BRG & GRN format",
    "author": "Petar Tasev",
    "version": (1, 0, 2017, 218),
    "blender": (2, 78, 0),
    "location": "File > Import-Export",
    "description": "Import-Export BRG & GRN mesh, attachpoints, skeleton, and animation",
    "warning": "",
    "wiki_url": "http://www.petartasev.com/modding/rise-of-nations/blender-addon/",
    "tracker_url": "https://github.com/Ryder25/Rise-of-Nations/issues",
    "support": 'COMMUNITY',
    "category": "Import-Export"}

import bpy
from bpy_extras.io_utils import ImportHelper, ExportHelper
from bpy.props import StringProperty, BoolProperty, EnumProperty, IntProperty

class AoMAttachpointPanel(bpy.types.Panel):
    bl_space_type = "VIEW_3D"
    bl_region_type = "TOOLS"
    bl_context = "objectmode"
    bl_category = "Create"
    bl_label = "AoM Attachpoints"
    bpy.types.Scene.attach_name = EnumProperty(
        name="Name",
        description="Choose the attachpoint name",
        items=(("attachpoint", "attachpoint", ""),
               ("smokepoint", "smokepoint", ""),
               ("reserved", "reserved", ""),
               ("lefthand", "lefthand", ""),
               ("righthand", "righthand", ""),
               ("topofhead", "topofhead", ""),
               ("forehead", "forehead", ""),
               ("face", "face", ""),
               ("chin", "chin", ""),
               ("leftear", "leftear", ""),
               ("rightear", "rightear", ""),
               ("neck", "neck", ""),
               ("leftshoulder", "leftshoulder", ""),
               ("rightshoulder", "rightshoulder", ""),
               ("frontchest", "frontchest", ""),
               ("backchest", "backchest", ""),
               ("frontabdomen", "frontabdomen", ""),
               ("backabdomen", "backabdomen", ""),
               ("pelvis", "pelvis", ""),
               ("leftthigh", "leftthigh", ""),
               ("rightthigh", "rightthigh", ""),
               ("leftleg", "leftleg", ""),
               ("rightleg", "rightleg", ""),
               ("leftfoot", "leftfoot", ""),
               ("rightfoot", "rightfoot", ""),
               ("leftforearm", "leftforearm", ""),
               ("rightforearm", "rightforearm", ""),
               ("hitpointbar", "hitpointbar", ""),
               ("garrisonflag", "garrisonflag", ""),
               ("smoke0", "smoke0", ""),
               ("smoke1", "smoke1", ""),
               ("smoke2", "smoke2", ""),
               ("smoke3", "smoke3", ""),
               ("smoke4", "smoke4", ""),
               ("smoke5", "smoke5", ""),
               ("smoke6", "smoke6", ""),
               ("smoke7", "smoke7", ""),
               ("smoke8", "smoke8", ""),
               ("smoke9", "smoke9", ""),
               ("reserved0", "reserved0", ""),
               ("reserved1", "reserved1", ""),
               ("reserved2", "reserved2", ""),
               ("reserved3", "reserved3", ""),
               ("reserved4", "reserved4", ""),
               ("reserved5", "reserved5", ""),
               ("reserved6", "reserved6", ""),
               ("reserved7", "reserved7", ""),
               ("reserved8", "reserved8", ""),
               ("reserved9", "reserved9", ""),
               ("gatherpoint", "gatherpoint", ""),
               ("fire", "fire", ""),
               ("decal", "decal", ""),
               ("corpse", "corpse", ""),
               ("launchpoint", "launchpoint", ""),
               ("targetpoint", "targetpoint", "")),
        default="hitpointbar",
        )

    def draw(self, context):
        scn = context.scene
        col = self.layout.column(align=True)
        col.prop(scn, "attach_name")
        col.separator()
        col.operator("object.brg_attachpoint_add", text="Add Attachpoint")

class AoMAttachpointEmpty(bpy.types.Operator):
    bl_idname = "object.brg_attachpoint_add"
    bl_label = "Add Attachpoint"
    bl_options = {"UNDO"}

    def invoke(self, context, event):
        scn = context.scene
        empty = bpy.data.objects.new("Dummy_" + scn.attach_name, None)
        empty.location = scn.cursor_location
        empty.rotation_mode = "QUATERNION"
        empty.scale = (0.2, 0.2, 0.2)
        scn.objects.link(empty)
        scn.objects.active = empty
        return {"FINISHED"}

class ImportBRG(bpy.types.Operator, ImportHelper):
    """Load an Age of Mythology BRG file"""
    bl_idname = "import_scene.brg"  # important since its how bpy.ops.import_test.some_data is constructed
    bl_label = "Import BRG"

    # ImportHelper mixin class uses this
    filename_ext = ".brg"

    filter_glob = StringProperty(
        default="*.brg;*.brg.json",
        options={'HIDDEN'},
    )

    # List of operator properties, the attributes will be assigned
    # to the class instance from the operator settings before calling.
    import_normals = BoolProperty(
        name="Import Normals",
        description="Import the normals from the file",
        default=True,
    )

    #    type = EnumProperty(
    #            name="Example Enum",
    #            description="Choose between two items",
    #            items=(('OPT_A', "First Option", "Description one"),
    #                   ('OPT_B', "Second Option", "Description two")),
    #            default='OPT_A',
    #            )

    def execute(self, context):
        from .brgfileimporter import BRGFileImporter
        file_importer = BRGFileImporter(context, self.filepath)
        return file_importer.load()


class ExportBRG(bpy.types.Operator, ExportHelper):
    """Save an Age of Mythology BRG file"""
    bl_idname = "export_scene.brg"
    bl_label = "Export BRG"

    # ExportHelper mixin class uses this
    filename_ext = ".brg"

    filter_glob = StringProperty(
        default="*.brg",
        options={'HIDDEN'},
    )

    preserve_uvs = BoolProperty(
        name="Preserve UVs",
        description="Duplicate the mesh vertices so that they have 1:1 correspondence with their UVs",
        default=False,
    )

    def execute(self, context):
        #from .blender.bh3fileexporter import BH3FileExporter
        #file_exporter = BH3FileExporter(self.preserve_uvs)
        #return file_exporter.save(context, self.filepath)
        return


class ImportGRN(bpy.types.Operator, ImportHelper):
    """Load an Age of Mythology GRN file"""
    bl_idname = "import_scene.grn"  # important since its how bpy.ops.import_test.some_data is constructed
    bl_label = "Import GRN"

    # ImportHelper mixin class uses this
    filename_ext = ".grn"

    filter_glob = StringProperty(
        default="*.grn",
        options={'HIDDEN'},
    )

    stabilize_quaternions = BoolProperty(
        name="Stabilize Quaternions",
        description="Import each quaternion as the shortest arc from the previous keyframe",
        default=True,
    )

    def execute(self, context):
        #from .blender.bhafileimporter import BHAFileImporter
        #file_importer = BHAFileImporter(self.stabilize_quaternions)
        #return file_importer.load(context, self.filepath)
        return


class ExportGRN(bpy.types.Operator, ExportHelper):
    """Save an Age of Mythology GRN file"""
    bl_idname = "export_scene.grn"
    bl_label = "Export GRN"

    # ExportHelper mixin class uses this
    filename_ext = ".grn"

    filter_glob = StringProperty(
        default="*.grn",
        options={'HIDDEN'},
    )

    def execute(self, context):
        #from .blender.bhafileexporter import BHAFileExporter
        #file_exporter = BHAFileExporter()
        #return file_exporter.save(context, self.filepath)
        return


# Only needed if you want to add into a dynamic menu
def menu_func_import(self, context):
    self.layout.operator(ImportBRG.bl_idname, text="Age of Mythology (.BRG)")


def menu_func_export(self, context):
    self.layout.operator(ExportBRG.bl_idname, text="Age of Mythology (.BRG)")


def menu_func_import_grn(self, context):
    self.layout.operator(ImportGRN.bl_idname, text="Age of Mythology (.GRN)")


def menu_func_export_grn(self, context):
    self.layout.operator(ExportGRN.bl_idname, text="Age of Mythology (.GRN)")


def register():
    bpy.utils.register_class(ImportBRG)
    bpy.utils.register_class(ExportBRG)
    bpy.utils.register_class(ImportGRN)
    bpy.utils.register_class(ExportGRN)
    bpy.types.INFO_MT_file_import.append(menu_func_import)
    bpy.types.INFO_MT_file_export.append(menu_func_export)
    bpy.types.INFO_MT_file_import.append(menu_func_import_grn)
    bpy.types.INFO_MT_file_export.append(menu_func_export_grn)

    from . import brgmaterialui
    brgmaterialui.register()

    bpy.utils.register_class(AoMAttachpointPanel)
    bpy.utils.register_class(AoMAttachpointEmpty)

def unregister():
    bpy.utils.unregister_class(ImportBRG)
    bpy.utils.unregister_class(ExportBRG)
    bpy.utils.unregister_class(ImportGRN)
    bpy.utils.unregister_class(ExportGRN)
    bpy.types.INFO_MT_file_import.remove(menu_func_import)
    bpy.types.INFO_MT_file_export.remove(menu_func_export)
    bpy.types.INFO_MT_file_import.remove(menu_func_import_grn)
    bpy.types.INFO_MT_file_export.remove(menu_func_export_grn)

    from . import brgmaterialui
    brgmaterialui.unregister()

    bpy.utils.unregister_class(AoMAttachpointPanel)
    bpy.utils.unregister_class(AoMAttachpointEmpty)


if __name__ == "__main__":
    register()

    # from formats.bh3.bh3file import BH3File
    # bh3_file = BH3File()
    # bh3_file.read('C:\Games\Steam\SteamApps\common\Rise of Nations\\art\\riflemanO.BH3')
    # bh3_file.write('C:\Games\Steam\SteamApps\common\Rise of Nations\\art\\riflemanO_t.BH3')

    # test call
    # bpy.ops.import_scene.bh3('INVOKE_DEFAULT')
