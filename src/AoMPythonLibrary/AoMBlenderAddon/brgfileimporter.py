from time import process_time
import os
import bpy
from mathutils import Vector, Matrix
from AoMPythonLibrary.formats.brg.brgfile import BRGFile

class BRGFileImporter(object):
    """Import BRG files into Blender"""
    def __init__(self, ctx, filename):
        self._ctx = ctx
        self._filename = filename
        self._file = BRGFile()
        self._model = None
        self._attachpoints = []
        self._mat_id_mapping = {}

    def load(self):
        start_time = process_time()

        self._file.open(self._filename)

        self._create_mesh()

        for attachpoint in self._file.meshes[0].attachpoints:
            self._create_attachpoint(attachpoint)

        for material in self._file.materials:
            self._create_material(material)

        for poly_index, poly in enumerate(self._model.data.polygons):
            face_mat = self._file.meshes[0].face_materials[poly_index]
            poly.material_index = self._mat_id_mapping[face_mat]

        self._create_animation()

        print("BRG import took {:f} seconds".format(process_time() - start_time))
        return {'FINISHED'}

    def _create_mesh(self):
         # create the mesh
        #bpy.ops.object.mode_set(mode='OBJECT')
        model_name = self._filename_no_ext(self._filename)

        mesh = bpy.data.meshes.new(model_name + "_Mesh")
        self._model = bpy.data.objects.new(model_name, mesh)
        self._model.location = [0, 0, 0]
        self._ctx.scene.objects.link(self._model)
        self._ctx.scene.objects.active = self._model
        self._ctx.scene.update()

        brg_mesh = self._file.meshes[0]
        vertices = [[-vert[0], -vert[2], vert[1]] for vert in brg_mesh.vertices]
        normals = [[-norm[0], -norm[2], norm[1]] for norm in brg_mesh.normals]
        uvs = [[uv[0], uv[1]] for uv in brg_mesh.uvs]
        colors = [[color[0], color[1], color[2], color[3]] for color in brg_mesh.colors]
        faces = [[face[0], face[2], face[1]] for face in brg_mesh.faces]

        mesh.from_pydata(vertices, [], faces)
        mesh.update(calc_edges=True, calc_tessface=True)
        mesh.normals_split_custom_set_from_vertices(normals)
        mesh.use_auto_smooth = True

        uv_layer = mesh.uv_textures.new()
        uv_layer.name = model_name + "_UV"
        uv_loops = mesh.uv_layers[-1].data

        for loop in mesh.loops:
            uv_loops[loop.index].uv = uvs[loop.vertex_index]

    def _create_attachpoint(self, attachpoint):
        empty = bpy.data.objects.new("Dummy_" + attachpoint.name, None)

        pos, rot, sca = self._attachpoint_prs_blender(attachpoint)

        empty.rotation_mode = "QUATERNION"
        empty.location = pos
        empty.rotation_quaternion = rot
        empty.scale = sca

        self._ctx.scene.objects.link(empty)
        empty.parent = self._model
        self._attachpoints.append(empty)

    def _create_material(self, material):
        self._mat_id_mapping[material.id_num] = len(self._mat_id_mapping)
        mat_name = self._filename_no_ext(material.diffuse_map)
        mat = bpy.data.materials.new(mat_name + "_Mat")
        mat.diffuse_color = material.diffuse_color
        mat.specular_color = material.specular_color
        mat.specular_hardness = material.specular_exponent
        mat.volume.emission_color = material.emissive_color
        mat.alpha = material.opacity
        mat.aom_data.flags = material.flags

        if material.diffuse_map:
            texture = bpy.data.textures.new(mat_name, type='IMAGE')
            if os.path.isfile(material.diffuse_map):
                texture.image = bpy.data.images.load(material.diffuse_map)
            texture.use_alpha = True
            mtex = mat.texture_slots.add()
            mtex.texture = texture
            mtex.texture_coords = 'UV'

        self._model.data.materials.append(mat)

    def _create_animation(self):
        model_name = self._filename_no_ext(self._filename)
        animation = self._file.animation

        if animation.duration == 0.0:
            return

        mesh = self._model.data
        if not mesh.animation_data:
            mesh.animation_data_create()

        anim_name = model_name + "_Anim"
        action = bpy.data.actions.new(name=anim_name)
        mesh.animation_data.action = action

        fps = 30
        data_path = "vertices[%d].co"
        for vert in mesh.vertices:
            fcurves = [action.fcurves.new(data_path % vert.index, i) for i in range(3)]
            fcurves[0].keyframe_points.insert(0, vert.co[0])
            fcurves[1].keyframe_points.insert(0, vert.co[1])
            fcurves[2].keyframe_points.insert(0, vert.co[2])

            for time, mesh_anim in \
            zip(animation.mesh_keys[1:], self._file.meshes[0].mesh_animations):
                frame = time * fps
                vert_anim = self._vec_blender(mesh_anim.vertices[vert.index])

                fcurves[0].keyframe_points.insert(frame, vert_anim[0])
                fcurves[1].keyframe_points.insert(frame, vert_anim[1])
                fcurves[2].keyframe_points.insert(frame, vert_anim[2])

        data_path = "location"
        data_path_rot = "rotation_quaternion"
        data_path_sca = "scale"
        for attach_index, attach in enumerate(self._attachpoints):
            if not attach.animation_data:
                attach.animation_data_create()
            attach_act = bpy.data.actions.new(name=attach.name + "_Anim")
            attach.animation_data.action = attach_act

            fcurves = [attach_act.fcurves.new(data_path, i) for i in range(3)]
            fcurves_rot = [attach_act.fcurves.new(data_path_rot, i) for i in range(4)]
            fcurves_sca = [attach_act.fcurves.new(data_path_sca, i) for i in range(3)]

            fcurves[0].keyframe_points.insert(0, attach.location[0])
            fcurves[1].keyframe_points.insert(0, attach.location[1])
            fcurves[2].keyframe_points.insert(0, attach.location[2])
            fcurves_rot[0].keyframe_points.insert(0, attach.rotation_quaternion[0])
            fcurves_rot[1].keyframe_points.insert(0, attach.rotation_quaternion[1])
            fcurves_rot[2].keyframe_points.insert(0, attach.rotation_quaternion[2])
            fcurves_rot[3].keyframe_points.insert(0, attach.rotation_quaternion[3])
            fcurves_sca[0].keyframe_points.insert(0, attach.scale[0])
            fcurves_sca[1].keyframe_points.insert(0, attach.scale[1])
            fcurves_sca[2].keyframe_points.insert(0, attach.scale[2])

            for time, mesh_anim in \
            zip(animation.mesh_keys[1:], self._file.meshes[0].mesh_animations):
                frame = time * fps
                attach_anim = mesh_anim.attachpoints[attach_index]
                pos, rot, sca = self._attachpoint_prs_blender(attach_anim)

                fcurves[0].keyframe_points.insert(frame, pos[0])
                fcurves[1].keyframe_points.insert(frame, pos[1])
                fcurves[2].keyframe_points.insert(frame, pos[2])
                fcurves_rot[0].keyframe_points.insert(frame, rot[0])
                fcurves_rot[1].keyframe_points.insert(frame, rot[1])
                fcurves_rot[2].keyframe_points.insert(frame, rot[2])
                fcurves_rot[3].keyframe_points.insert(frame, rot[3])
                fcurves_sca[0].keyframe_points.insert(frame, sca[0])
                fcurves_sca[1].keyframe_points.insert(frame, sca[1])
                fcurves_sca[2].keyframe_points.insert(frame, sca[2])

        self._ctx.scene.render.fps = fps
        self._ctx.scene.frame_end = action.frame_range[1]
        self._ctx.scene.frame_start = 0
        self._ctx.scene.frame_current = 0

    def _vec_blender(self, vector):
        return Vector([-vector[0], -vector[2], vector[1]])
    def _vec_blender2(self, vector):
        return Vector([vector[0], -vector[1], -vector[2]])

    def _attachpoint_prs_blender(self, attachpoint):
        pos = self._vec_blender(attachpoint.position)

        rot = Matrix()
        x = self._vec_blender2(attachpoint.zvector)
        y = self._vec_blender2(attachpoint.xvector)
        z = self._vec_blender2(attachpoint.yvector)
        rot[0].xyz = (x[0], y[0], z[0])
        rot[1].xyz = (x[1], y[1], z[1])
        rot[2].xyz = (x[2], y[2], z[2])
        rot = rot.to_quaternion()

        bbmax = Vector(attachpoint.bounding_box_max)
        bbmin = Vector(attachpoint.bounding_box_min)
        sca = bbmax - bbmin
        sca = [sca[0], sca[2], sca[1]]

        return pos, rot, sca

    def _filename_no_ext(self, filename):
        return os.path.splitext(os.path.basename(filename))[0]
