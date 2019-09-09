----------------------------------------------
            Xffect Editor Pro
 Copyright 2012- Shallway Studio
                Version 3.1.0
    http://shallway.net/forum.php?mod=viewthread&tid=40&page=1&extra=#pid40
            shallwaycn@gmail.com
----------------------------------------------

Thank you for buying Xffect Pro!

If you have any questions, suggestions, comments or feature requests, please
drop by the Xffect forum, found here: http://shallway.net/forum.php?mod=forumdisplay&fid=2

---------------------------------------
documentation, and tutorials
---------------------------------------

All can be found here:
http://shallway.net/forum.php?mod=viewthread&tid=39&extra=page%3D1


--------------------
 How To Update Xffect
--------------------
1. In Unity, File -> New Scene
2. Delete the Xffect folder from the Project View.
3. Import Xffect from the updated Unity Package.


---------------------------------------
Release Notes
---------------------------------------

**********************************************IMPORTANT: Upgrade Note*******************************************************************************************
If you are using version below 3.0.0, you need to patch all your Xffect Objects (including prefabs):
	1,Delete the Xffect folder from the Project View.
	2,Import Xffect from the updated Unity Package.
	3,click menu: "Window/Xffect/Patch/Ver3.0.0/Patch Current Project" to patch all the Xffect prefabs in current project folder. this will take some time.
	or you can click menu: "Window/Xffect/Patch/Ver3.0.0/Patch Selected Object" to patch current select gameobject(including its children).
	recommended: make sure all your old Xffect Object has a prefab link in asset folder. then click "Window/Xffect/Patch/Ver3.0.0/Patch Current Project" to patch.
****************************************************************************************************************************************************************
Version 3.1.0
*-NEW* Added a new Curve Editor "CURVE01" whose range is limited in (0,1), and you only need to specify the max value to indicate the range. This is similar to the Shuriken's curve editor.
*-NEW* added a new module : "Sub Emitters" which is similar to Shuriken particle system's "Sub Emitters" module, please check out "Tutorial/sub emitters.scene" to know more.
*-NEW* added a new Camera Effect "Glitch", you can use it to shake the screen with RGB displacement.
-NEW: added new scale and rotation option : RANDOM, you can use it to change the scale or rotation randomly.
-NEW: added new collision type: Plane Collision.
-NEW: added new option "is fixed circle track" in Vortex Modifier.
-NEW: added new option "random radius" in Circle Emitter.
-NEW: added new Sprite Type: 'BILLBOARD_Y' which will rotate around Y axis to face the camera.
-NEW: Added 'burst' emit option in Emitter Config.
-NEW: Added random angle option in Cone Direction Type.
-FIXED: added back angle option in "Cone" config which was existed in version 2.x.
-OBSOLETE: Abandoned "chance to emit per loop" option in Emitter Config.

Version 3.0.3
-FIXED: FIXED a bug that in unity 4 the scale curve can't be edited.
-FIXED: Collision Config will not be auto disabled if the Collision Goal is null;

Version 3.0.2
-FIXED: fixed the bug that if "Color Config" is open in inspector, the CPU will run to 100%...
-FIXED: Now the Active() API should always work, if the Xffect Object is active, it will be reset. in the lower version if Xffect Objest is already active, Active() will not work.

Version 3.0.1
-FIXED: Fixed a bug that some times the "Color Gradient Editor" window will crash.
-FIXED: now the "Color Gradient Editor" window is always on top of other windows.

Version 3.0.0
-NEW: brand new UI, including a Gradient Color Editor, now the use of Xffect is more convenient.
-NEW: Camera Effect : Glow Per Object Event added, you can use this event to make beautiful glow trails.
-IMPROVED: performance improved!
-FIXED: Xffect's "update delta" time will not be messed up by game pause or delay now.
-FIXED: Cone's UV fixed, you can set the cone's texture wrap mode to "Clamp" to wipe off the opaque edge.
-FIXED: 'Cone and Custom Mesh' UV supports Texture Sheet Animation now.

Version 2.5.3
-NEW: Custom Mesh Type supports UV Configuration now!
-NEW: Added new example "Mesh Fog Volume".
-FIXED: Removed "Xffect" menu and put it under "Window" menu.
-FIXED: Some minor bugs fixed.

Version 2.5.2:
-Fixed: repacked Xffect with unity 3.5.5, no need to upgrade your project to upgrade your project to unity4 now.
-NEW: added 8 mobile examples, open "xft_demo_mobile" scene to know more.
(and please aware that if some prefab works bad on your mobile, you need to reduce the emit count. It has nothing to do with Xffect's performance, don't be annoyed).

Version 2.5.1
-Fixed:rewrote a new radial blur shader to support unity4's DX11 renderer, and now the effect is better.
-DEL: cleaned some unused textures.

Version 2.5.0
-Improved: now it's compatible with Unity4.
-Improved: XffectComponent's mesh will set to non-active when Awake,this will save some performance.
-Improved: Each Xffect can be configured to influence by Time.timeScale now.
-Added: Added a new Image Effect Event : "Camera Color Inverse".
-Added: Added a new Example "Devil Trigger" which has used "Camera Color Inverse" event.
-Added: Added a new Event : "Time Scale", which can be used to make slow motion effects.
-Improved: now each Xffect Object should be culled by camera correctly.
-Improved: added a prefix to SmoothRandom() function's name.


Version 2.4.0
-Improved: "heat_distortion" shader improved, now the distortion effect is better.
-Fixed: fixed a bug that "Sphere" Direction is incorrect when the Client's rotation is not zero.
-Added: Added a new RenderType "Custom Mesh", which is just like the shuriken's mesh renderer.
-Added: Added new examples: cyclone1, cyclone2 and cyclone3, which demonstrated the new feature of "Custom Mesh".

Version 2.3.0
-Added: add a new API SetScale() which can change sprite length during runtime, and use it can make magic chain effects.
-Added: add a new Event: "Camera Radial Blur Mask" which can be used on mobile device.
-Added: add a Tutorial folder, incuding 3 demos to demonstrate the basic use of APIs.

Version 2.2.0
-Fixed: fixed the bug that "Sphere" Collision Type sometime is not working properly.
-Fixed: now each collision will pass a Xft.CollisionParam to the handler function.
-Fixed: fixed a bug when XffectComponent's scale not equals 1, the RibbonTrail and BillboardSelf will not face the camera.
(about this scale issue, i hvae fixed so many bugs... if you find other bug, please feel free to contact me.)
-Improved: improved "Jet Affector", you can use it to change the node's speed dynamicly now.
-Added: add a new shader: "xft_distort_additive".
-Added: add new Prefab effect: blood_sucker, blood_energy, requiem_of_souls.


Version 2.1.1
-Added: Add a new emitter method: emit by curve. abandoned the old option : "is emit by distance".
*UPDATE NOTICE* if you used the "is emit by distance" option of "emitter configuration" in old version, you need to set the new "emit method" to "by distance".
I'm sorry you have to do this update manually.
-Added: add a new option in "Scale Configuration" : "use same scale curve" which is convenient to scale both scaleX and scaleY.
-Fixed: fixed a bug when a new level is loaded , the old Xffect's camera was not updated. Now each XffectComponent will check if its camera was destroyed and update it automatically.
-Added: add a new API : "ResetCamera(Camera cam)" to manually change the Camera of XffectComponent.

Version 2.1.0
-Fixed: fixed a bug that scaled Xffect's position was affected by client's position even though the option "sync to client pos" was un-checked.
-Fixed: fixed a bug that when "UV Configuration"'s "loop count" was not equal -1, the Effect will be very strange in run-game.
-Fixed: fixed a bug that "Cone" Render Type's scale and angle were not performed correctly.
-Added: add "DragAffector" which will perform a drag force in specify area.
-Added: add prefab : cone_explode
-Replaced: add a new shader mask_blend, replaced the old beam_light shader.

Version 2.0.1
-Fixed: Fixed a error message in editor mode:"Destroying assets is not permitted to avoid data loss.", now you can put any Object to Xffect as its child.
-Fixed: Now Xffect Object can be set to any layer, so it can be rendered by diffrent camera now.


Version 2.0
-*Feature Added: Event System, support CameraShake, Sound, Light and ImageEffect Event, which will greatly enhance your game visual.
-Fixed: heat_distortion shader add "ZTest Always" to avoid being clipped by other game object.
-Fixed: "Cone" node's direction can change from inspector now.
-Added: Example RadialBlur.
-Added: Example VolumetricLight.
-Improved: Example explosion1, use CameraShake and CameraGlow Event.

Version 1.3.2
-Added: support "random speed" in "Direction Configuration" now.
-Added: xft_beam_light shader.
-Added: WindowLight Example.

Version 1.3.1
-*Performance Improved* No GC Alloc During Run Time Now!!!
-Removed: Fog Of War Example, since it has nothing to do with xffect editor. but i will release "Fog Of War For RTS" package to asset store soon.
-Added: Dissolve Example.


Version 1.3.0(from this version, i will continue to share my own shaders to Xffect package.)
-Fixed: parent's rotation don't influence XffectComponent's mesh now, you can put XffectComponent to any parent with any rotation(mainly fixed for slash trails).
-Added: each EffectLayer can custom the debug color now.
-Added: fog of war shader(in xft_fog_of_war scene), Best and Fast solution, but currently is not supported with AA and Mobile device.
-Added: Volume Fog shader(in xft_demo scene ),not supported in mobile.
-Added: IceImpact Example, Swing Around Example.
-Added: MobileMaterialSetting.cs, useful to change all the materials in a folder to fit the mobile device(many thanks to kukuСز).  

Version 1.2.2
-Useful function added: now, you can manually adjust the xffect's scale by only one option in XffectComponent��the "scale" edit box, which is very useful for prefabs to reuse in diffrent size's world.
-Example Added: LevelUp.


Version 1.2.1
1,Added: Vortex Affector support random direction now.
2,Added: Ribbon Trail support random width and random length now.
3,Added: "material.renderqueue" can be configured from EffectLayer, so removed "xft_heat_distortion_forward" shader.
4,Example Added: SpreadSlash, PinkSoul,Sakura.

Version 1.2.0
1,fixed: "XZ Plane" sprite's direction is influenced by "Direction" now.
2,Added: "Mesh" Emitter, support emit by vertex and emit by triangle.
3,Added: "Direction Configuration" is more specific now, support "Sphere" "Cone" "Cylindrical" direction.
4,Fixed some xffect component can't sync to parent Transform.

Version 1.1.2
1,bug fixed: BombAffector's force fixed, now it's in units of mass / time^2.
2,add a BombAffector example to show the fixed Bomb Affector feature.
3,fixed: now the parent's scale don't influence XffectComponent's position now, you can put XffectComponent to anyparent with any scale.

Version 1.1.1
1,example improved: "lightning_explode", "water_fall"
2,example added: "transform_spell"
3,fixed: if Gravity Affector is non-accelerate, the velocity is constant.

Version 1.1.0
1,now the box emitter's rotation can inherit from client rotation
2,direction can always sync to client rotation now
3,provide slash trail option, with heat distortion shader, it can render amazing weapon trails.
4,add a phantom_sword prefab which shows all the features above.
