class_name playerClass

extends CharacterBody3D


@onready var gunRay = $Head/playerCam/gunRay as RayCast3D
@onready var Cam = $Head/playerCam as Camera3D
@onready var MeshInstance = $Head/mesh
@onready var PlayerBody = self as CharacterBody3D
@export var _bullet_scene : PackedScene
var mouseSensitivity = 300
var mouse_relative_x = 0
var mouse_relative_y = 0
var SPEED = 10.0
var JUMP_VELOCITY = 7

@onready var camTransX = Cam.transform.basis.x
@onready var camTransZ = Cam.transform.basis.z
@onready var camTransY = Cam.transform.basis.y

var menuOpen:bool = false

var wile_e_time = 3

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = 10

@onready var skel = get_node("Head/mesh/eclectic_male_rigged_texturing_backuip/Armature/Skeleton3D")
@onready var spine_1 = skel.find_bone("spine_1")
@onready var spine_rot = skel.get_bone_pose(spine_1)


func _ready():
	#skel = get_node("Head/mesh/eclectic_male_rigged_texturing_backuip/Armature/Skeleton3D")
	var count = skel.get_bone_count()
	print("bone count:", count)
	
	#Captures mouse and stops rgun from hitting yourself
	gunRay.add_exception(self)
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED

func rotate_player():
	camTransZ = Cam.transform.basis.z
	camTransX = Cam.transform.basis.x
	camTransY = Cam.transform.basis.y
	MeshInstance.transform.basis.x = Cam.transform.basis.x
	MeshInstance.transform.basis.z = Cam.transform.basis.z
	MeshInstance.transform.basis.y = Cam.transform.basis.y
	#Cam.transform.basis.x = CamTransX
	#Cam.transform.basis.z = CamTransZ
	#Cam.transform.basis.y = CamTransY
	
func _physics_process(delta):
	
	#rotate spine_1 bone to reflect camera rotation
	spine_rot = skel.get_bone_pose(spine_1)
	spine_rot = spine_rot.rotated(Vector3(0.0, 1.0, 0.0), 0.1 * delta)
	
	# Add the gravity.
	if not is_on_floor():
		velocity.y -= gravity * delta
		rotate_player()
	else:
		MeshInstance.rotation.x = clamp(MeshInstance.rotation.x, deg_to_rad(0), deg_to_rad(0) )
	
	# Handle Jump.
	if Input.is_action_just_pressed("Jump") and is_on_floor()==true:
		velocity.y = JUMP_VELOCITY
	# Handle Shooting
	if Input.is_action_just_pressed("Shoot"):
		shoot()
	# Get the input direction and handle the movement/deceleration.
	var input_dir = Input.get_vector("moveLeft", "moveRight", "moveUp", "moveDown")
	var direction = (transform.basis * Vector3(input_dir.x, 0, input_dir.y))
	if direction:
		velocity.x = direction.x * SPEED
		velocity.z = direction.z * SPEED
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)
		velocity.z = move_toward(velocity.z, 0, SPEED)

	move_and_slide()

func _input(event):
	
	if event is InputEventMouseMotion:
		rotation.y -= event.relative.x / mouseSensitivity
		Cam.rotation.x -= event.relative.y / mouseSensitivity
		if is_on_floor() == true:
			Cam.rotation.x = clamp(Cam.rotation.x, deg_to_rad(-90), deg_to_rad(90) )
		mouse_relative_x = clamp(event.relative.x, -50, 50)
		mouse_relative_y = clamp(event.relative.y, -50, 10)

func shoot():
	if not gunRay.is_colliding():
		return
	var bulletInst = _bullet_scene.instantiate() as Node3D
	bulletInst.set_as_top_level(true)
	get_parent().add_child(bulletInst)
	bulletInst.global_transform.origin = gunRay.get_collision_point() as Vector3
	bulletInst.look_at((gunRay.get_collision_point()+gunRay.get_collision_normal()),Vector3.BACK)
	print(gunRay.get_collision_point())
	print(gunRay.get_collision_point()+gunRay.get_collision_normal())

