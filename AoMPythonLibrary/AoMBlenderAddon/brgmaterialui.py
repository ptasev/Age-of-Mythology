import bpy
from bpy.props import StringProperty, BoolProperty, EnumProperty, IntProperty
from AoMPythonLibrary.formats.brg.brgmaterial import BRGMaterialFlags

class AoMMaterialProperties(bpy.types.PropertyGroup):
    is_updating_bools = BoolProperty(
        name="is_updating_bools",
        description="control for UI draw",
        default=False
        )

    def update_bools(self, context):
        self.is_updating_bools = True

        self.PixelXForm1 = self.has_flag(BRGMaterialFlags.PixelXForm1)
        self.PlayerXFormColor1 = self.has_flag(BRGMaterialFlags.PlayerXFormColor1)

        self.is_updating_bools = False
    flags = IntProperty(
        name="Flags",
        description="Material Flags",
        default=0,
        update=update_bools
        )
    def update_flags(self, flag_bool, flag):
        if self.is_updating_bools:
            return
        if flag_bool:
            self.flags |= flag
        else:
            self.flags &= ~flag

    def has_flag(self, flag):
        return (self.flags & flag) == flag

    def update_PixelXForm1(self, context):
        self.update_flags(self.PixelXForm1, BRGMaterialFlags.PixelXForm1)
    PixelXForm1 = BoolProperty(
        name="PixelXForm1",
        description="",
        default=False,
        update=update_PixelXForm1
        )
    def update_PlayerXFormColor1(self, context):
        self.update_flags(self.PlayerXFormColor1, BRGMaterialFlags.PlayerXFormColor1)
    PlayerXFormColor1 = BoolProperty(
        name="PlayerXFormColor1",
        description="Typical player color",
        default=False,
        update=update_PlayerXFormColor1
        )

class AoMMaterialPanel(bpy.types.Panel):
    bl_idname = "MATERIAL_PT_aom_mat"
    bl_space_type = "PROPERTIES"
    bl_region_type = "WINDOW"
    bl_context = "material"
    bl_label = "AoM Material"

    @classmethod
    def poll(cls, context):
        return (context.object is not None) and \
            (context.object.active_material is not None)

    def draw(self, context):
        mat = context.object.active_material
        col = self.layout.column(align=True)
        col.operator("material.aom_flags_default_set")
        col.separator()
        col.prop(mat.aom_data, "PixelXForm1")
        col.prop(mat.aom_data, "PlayerXFormColor1")
        col.separator()

class AoMMaterialFlagsDefaultSet(bpy.types.Operator):
    bl_idname = "material.aom_flags_default_set"
    bl_label = "Set Default Flags"

    def invoke(self, context, event):
        mat = context.object.active_material
        mat.aom_data.flags = 2048
        return {"FINISHED"}

def register():
    bpy.utils.register_class(AoMMaterialProperties)
    bpy.types.Material.aom_data = bpy.props.PointerProperty(type=AoMMaterialProperties)

    bpy.utils.register_class(AoMMaterialPanel)
    bpy.utils.register_class(AoMMaterialFlagsDefaultSet)

def unregister():
    bpy.utils.unregister_class(AoMMaterialProperties)
    bpy.utils.unregister_class(AoMMaterialPanel)
    bpy.utils.unregister_class(AoMMaterialFlagsDefaultSet)
