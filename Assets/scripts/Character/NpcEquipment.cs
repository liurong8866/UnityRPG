
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace MyLib
{
    [RequireComponent(typeof(MyAnimationEvent))]
    public class NpcEquipment : KBEngine.MonoBehaviour
    {
        GameObject defaultChest;
        GameObject Weapon;

        public bool HasWeapon()
        {
            return Weapon != null;
        }

        EquipData WeaponData;
        EquipData ChestData;
        EquipData ShoesData;
        EquipData GlovesData;
        EquipData TrousersData;
        EquipData headData;
        Dictionary<ItemData.EquipPosition, EquipData> partToData = new Dictionary<ItemData.EquipPosition, EquipData>();
        GameObject Scabbard;
        GameObject Chest;
        GameObject Shoes;
        GameObject Gloves;
        GameObject Head = null;
        GameObject Trousers;

        /*
         * Default Equip Part
         */ 
        GameObject defaultBoots;
        GameObject defaultGloves;
        GameObject defaultHead;
        GameObject defaultTrousers;
        Transform rightHand;
        Transform back;
        //MyAnimationEvent myAnimationEvent;
        GameObject weaponTrail;
        NpcAttribute attribute;
        bool IsFakeObject = false;

        public void SetFakeObj()
        {
            IsFakeObject = true;
        }
        //FakeObj 对应的真实对象的ID

        public void SetLocalId(int lid)
        {
            photonView.SetLocalId(lid);
        }
        /*
         * Equipment PartName:
         *   head ---> face  helmet
         *   body ---> chest
         *   gloves ---> gloves
         *   shoes ---> boots
         *   trousers ---> NULL
         */
        void Awake()
        {
            attribute = GetComponent<NpcAttribute>();
            //myAnimationEvent = GetComponent<MyAnimationEvent> ();

            rightHand = Util.FindChildRecursive(transform, "Armature|tag_righthand");
            if (rightHand == null)
            {
                rightHand = Util.FindChildRecursive(transform, "Point001");
            }

            back = Util.FindChildRecursive(transform, "Point002");



            /*
             * first set body shadow then set equipment shadow
             */
        
            regEvt = new List<MyEvent.EventType>() {
                MyEvent.EventType.CharEquipChanged,
                MyEvent.EventType.TryEquip,

            };
            
            regLocalEvt = new List<MyEvent.EventType>() {
                MyEvent.EventType.ShowWeaponTrail,
                MyEvent.EventType.HideWeaponTrail,
            };
            RegEvent();
        }
        public Transform  GetRightHand() {
            return rightHand;
        }

        void SetDefaultEquip(GameObject fullSet, ItemData.EquipPosition pos, out GameObject g)
        {

            g = new GameObject(pos.ToString().ToLower());
            g.transform.parent = transform;
            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
            g.transform.localScale = Vector3.one;

            var render = g.AddComponent<SkinnedMeshRenderer>();
            var copyPart = fullSet.transform.Find(pos.ToString().ToLower()).gameObject;
            var copyRender = copyPart.GetComponent<SkinnedMeshRenderer>();
            Util.SetBones(g, copyPart, gameObject);
            render.sharedMesh = copyRender.sharedMesh;
            render.sharedMaterials = copyRender.sharedMaterials;
            GetComponent<ShadowComponent>().SetShadowPlane(g);
        }

        public void InitPlayerEquipmentFromBackPack()
        {
            Log.Sys("Init Player Equipment From Backpack");
            foreach (ItemData.EquipPosition ep in (ItemData.EquipPosition[])Enum.GetValues(typeof(ItemData.EquipPosition)))
            {
                var ed = BackPack.backpack.EnumAction(ep);
                if (ed != null)
                {

                    WieldEquip(ed);
                }
            }
        }

        public void InitDefaultEquip()
        {
            Log.Sys("Initial default equip " + gameObject.name);
            //return;
            if (defaultChest == null)
            {
                if (attribute.ObjUnitData.GetIsPlayer())
                {

                    var defaultWardrobe = attribute.ObjUnitData.GetDefaultWardrobe();
                    if(string.IsNullOrEmpty(defaultWardrobe)) {
                        return;
                    }
                    var wardrobe = Resources.Load<GameObject>(defaultWardrobe);
                    if (transform.Find("chest") != null)
                    {
                        defaultChest = transform.Find("chest").gameObject;
                    } else
                    {
                        defaultChest = transform.Find("body").gameObject;
                    }

                    if (transform.Find("boots") != null)
                    {
                        defaultBoots = transform.Find("boots").gameObject;
                    } else
                    {
                        defaultBoots = transform.Find("shoes").gameObject;
                    }
                    
                    if (transform.Find("head") != null)
                    {
                        defaultHead = transform.Find("head").gameObject;
                    } else
                    {
                        defaultHead = null;
                    }
                    
                    if (transform.Find("trousers") != null)
                    {
                        defaultTrousers = transform.Find("trousers").gameObject;
                    } else
                    {
                        defaultTrousers = null;
                    }
                    
                    defaultGloves = transform.Find("gloves").gameObject;
                    //原有mesh都Disable了则动画停止，需要设定总是动画
                    GetComponent<Animation>().cullingType = AnimationCullingType.AlwaysAnimate;

                    defaultTrousers.SetActive(false);
                    defaultChest.SetActive(false);
                    defaultHead.SetActive(false);
                    defaultBoots.SetActive(false);
                    defaultGloves.SetActive(false);

                    SetDefaultEquip(wardrobe, ItemData.EquipPosition.BODY, out defaultChest);

                    SetDefaultEquip(wardrobe, ItemData.EquipPosition.HEAD, out defaultHead);

                    SetDefaultEquip(wardrobe, ItemData.EquipPosition.SHOES, out defaultBoots);

                    SetDefaultEquip(wardrobe, ItemData.EquipPosition.GLOVES, out defaultGloves);

                    SetDefaultEquip(wardrobe, ItemData.EquipPosition.TROUSERS, out defaultTrousers);
                
                }
            }
        }

        //不要刀光了
        protected override void OnLocalEvent(MyEvent evt)
        {
            if (evt.type == MyEvent.EventType.ShowWeaponTrail)
            {
                //NpcShowWeaponTrail(evt);
            } else if (evt.type == MyEvent.EventType.HideWeaponTrail)
            {
                //NpcHideWeaponTrail();
            }
        }

        protected override void OnEvent(MyEvent evt)
        {

            //角色自身装备变更  FakeObject 不处理事件
            if (!IsFakeObject && evt.type == MyEvent.EventType.CharEquipChanged)
            {
                if (evt.localID == photonView.GetLocalId())
                {
                    if (evt.boolArg)
                    {
                        WieldEquip(evt.equipData);
                    } else
                    {
                        UnwieldEquip(evt.equipData);
                    }
                }
            }

            Log.GUI("Try Equip Fashion " + photonView.GetLocalId());
            if (IsFakeObject && evt.type == MyEvent.EventType.TryEquip && evt.localID == photonView.GetLocalId())
            {

                if (evt.boolArg)
                {
                    WieldEquip(evt.equipData);
                } else
                {
                    UnwieldEquip(evt.equipData);
                }
            }
        }


        //更新UI角色对象的装备
        void FakeWieldEquip(EquipData ed)
        {
            var fake = ObjectManager.objectManager.GetFakeObj(photonView.GetLocalId());
            if (fake != null)
            {
                fake.GetComponent<NpcEquipment>().WieldEquip(ed);
            }
        }

        //设置UI角色对象所在的层
        void SetFakeObjLayer()
        {
            if (IsFakeObject)
            {
                Util.SetLayer(gameObject, GameLayer.PlayerCamera);
            }
        }

        //根据实际对象初始化Fake对象的装备
        public void InitFakeEquip()
        {
            if (IsFakeObject)
            {
                Log.Sys("Intial Fake Equip " + gameObject.name);
                var obj = ObjectManager.objectManager.GetLocalPlayer(photonView.GetLocalId());
                if (obj != null)
                {
                    var equip = obj.GetComponent<NpcEquipment>();
                    UnwieldEquip(ItemData.EquipPosition.WEAPON);
                    UnwieldEquip(ItemData.EquipPosition.BODY);
                    UnwieldEquip(ItemData.EquipPosition.HEAD);
                    UnwieldEquip(ItemData.EquipPosition.SHOES);
                    UnwieldEquip(ItemData.EquipPosition.GLOVES);
                    UnwieldEquip(ItemData.EquipPosition.TROUSERS);

                    Log.Sys("Player Fake Equip is " + " " + obj.name + " " + equip);
                    WieldEquip(equip.WeaponData);
                    WieldEquip(equip.ChestData);
                    WieldEquip(equip.ShoesData);
                    WieldEquip(equip.GlovesData);
                    WieldEquip(equip.headData);
                    WieldEquip(equip.TrousersData);
                }
            }
        }

        //角色装备变更 同时修正FakeObject的装备，FakeObject是背包UI界面上3d模型
        void WieldEquip(EquipData ed)
        {
            if (ed == null || ed.itemData == null)
            {
                return;
            }
            Log.Sys("Wield Equip is " + ed.itemData.ItemName);

            if (!IsFakeObject)
            {
                FakeWieldEquip(ed);
            }
            var defeq = attribute.ObjUnitData.GetDefaultWardrobe();
            if(string.IsNullOrEmpty(defeq)){
                return;
            }

            partToData [ed.itemData.equipPosition] = ed;
            switch (ed.itemData.equipPosition)
            {

                case ItemData.EquipPosition.WEAPON:
                    if (Weapon != null)
                    {
                        UnwieldEquip(ItemData.EquipPosition.WEAPON);
                    }

                    WeaponData = ed;
                //是否有刀鞘
                    if (WeaponData.itemData.HasScabbard)
                    {
                        var fullWeapon = Resources.Load<GameObject>(WeaponData.itemData.ModelName);
                        Log.Sys("NpcEquipment::WieldEquip " + fullWeapon);
                        Weapon = Instantiate(Util.FindChildRecursive(fullWeapon.transform, "weapon1").gameObject) as GameObject;
                        Scabbard = Instantiate(Util.FindChildRecursive(fullWeapon.transform, "weapon2").gameObject) as GameObject;
                        var Point3 = Util.FindChildRecursive(fullWeapon.transform, "Point3");

                        if (back != null)
                        {
                            Scabbard.transform.parent = back;
                            Scabbard.transform.localPosition = Vector3.zero;
                            Scabbard.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                            foreach (Transform c in Scabbard.transform)
                            {
                                if (c.GetComponent<Renderer>() != null)
                                {
                                    GetComponent<ShadowComponent>().SetShadowPlane(c.gameObject);
                                }
                            }
                        }

                        //PeaceMode Level
                        if (WorldManager.worldManager.IsPeaceLevel())
                        {   
                            Weapon.transform.parent = back;
                            var mat = Matrix4x4.TRS(Point3.localPosition, Point3.localRotation, Point3.localScale);
                            var inv = mat.inverse;
                            Weapon.transform.localPosition = inv.GetColumn(3);
                            Weapon.transform.localRotation = Quaternion.LookRotation(inv.GetColumn(2), inv.GetColumn(1));
                            Weapon.transform.localScale = Vector3.one;

                            foreach (Transform c in Weapon.transform)
                            {
                                if (c.GetComponent<Renderer>() != null)
                                {
                                    GetComponent<ShadowComponent>().SetShadowPlane(c.gameObject);
                                }
                            }

                        } else
                        {
                            //UI上对象 武器在手中
                            if (rightHand != null)
                            {
                                Weapon.transform.parent = rightHand;
                                Weapon.transform.localPosition = Vector3.zero;
                                Weapon.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                                foreach (Transform c in Weapon.transform)
                                {
                                    if (c.GetComponent<Renderer>() != null)
                                    {
                                        GetComponent<ShadowComponent>().SetShadowPlane(c.gameObject);
                                    }
                                }
                            }

                        }

                    } else
                    {
                        Weapon = Instantiate(Resources.Load<GameObject>(WeaponData.itemData.ModelName)) as GameObject;
                        Log.Sys("NpcEquipment::WieldEquip No Scab  " + Weapon);
                        if (WorldManager.worldManager.IsPeaceLevel())
                        {
                            if (back != null)
                            {
                                var Point3 = Util.FindChildRecursive(Weapon.transform, "Point3");
                                Log.Sys("Wield Weapon in Back " + Point3);
                                var mat = Matrix4x4.TRS(Point3.localPosition, Point3.localRotation, Point3.localScale);
                                var inv = mat.inverse;

                                Weapon.transform.parent = back;
                                Weapon.transform.localPosition = inv.GetColumn(3);
                                Weapon.transform.localRotation = Quaternion.LookRotation(inv.GetColumn(2), inv.GetColumn(1));
                                Weapon.transform.localScale = Vector3.one;

                                Log.Sys("Set Weapon Transform " + inv.GetColumn(3));
                                if (Weapon.GetComponent<Renderer>() != null)
                                {
                                    GetComponent<ShadowComponent>().SetShadowPlane(Weapon);
                                }

                                foreach (Transform c in Weapon.transform)
                                {
                                    if (c.GetComponent<Renderer>() != null)
                                    {
                                        GetComponent<ShadowComponent>().SetShadowPlane(c.gameObject);
                                    }
                                }
                            } else
                            {
                                Debug.Log("NpcEquipment:: Not Find Back For equip ");
                            }
                        } else
                        {
                            Log.Sys("Wield Weapon On RightHand");
                            if (rightHand != null)
                            {
                                Weapon.transform.parent = rightHand;
                                Weapon.transform.localPosition = Vector3.zero;
                                Weapon.transform.localRotation = Quaternion.identity;
                                Weapon.transform.localScale = Vector3.one;
                                //3dsmax 
                                if (Weapon.GetComponent<Renderer>() != null)
                                {
                                    GetComponent<ShadowComponent>().SetShadowPlane(Weapon);
                                }

                                foreach (Transform c in Weapon.transform)
                                {
                                    if (c.GetComponent<Renderer>() != null)
                                    {
                                        GetComponent<ShadowComponent>().SetShadowPlane(c.gameObject);
                                    }
                                }
                            } else
                            {
                                Debug.LogError("NpcEquipment:: Not Found RightHand");
                            }
                        }
                    }

                    break;
            
                
                case ItemData.EquipPosition.BODY:
                    SetArmor(ed, out Chest);
                    ChestData = ed;
                    defaultChest.SetActive(false);
                    break;
                case ItemData.EquipPosition.GLOVES:
                    SetArmor(ed, out Gloves);
                    GlovesData = ed;
                    defaultGloves.SetActive(false);
                    break;

                case ItemData.EquipPosition.SHOES:
                    SetArmor(ed, out Shoes);
                    ShoesData = ed;
                    defaultBoots.SetActive(false);
                    break;
                case ItemData.EquipPosition.HEAD:
                    Log.Trivial(Log.Category.System, "Wear Head");

                    SetArmor(ed, out Head);
                    headData = ed;
                    defaultHead.SetActive(false);
                    break;
                case ItemData.EquipPosition.TROUSERS:
                    SetArmor(ed, out Trousers);
                    TrousersData = ed;
                    defaultTrousers.SetActive(false);
                    break;

                default:
                    Debug.LogError("WieldEquip Not Exists " + ed.itemData.equipPosition);
                    break;
            }

            SetFakeObjLayer();
        }

        void SetArmor(EquipData ed, out GameObject g)
        {
            UnwieldEquip(ed.itemData.equipPosition);

            var fullSet = Resources.Load<GameObject>(ed.itemData.ModelName);
            SetDefaultEquip(fullSet, ed.itemData.equipPosition, out g);
        }

        void UnwieldEquip(ItemData.EquipPosition pos)
        {
            if (pos == ItemData.EquipPosition.WEAPON)
            {
                if (Weapon != null)
                {
                    GameObject.Destroy(Weapon);
                    WeaponData = null;
                    if (Scabbard != null)
                    {
                        GameObject.Destroy(Scabbard);
                        Scabbard = null;
                    }
                }
            } else
            {
                switch (pos)
                {
                    case ItemData.EquipPosition.HEAD:
                        if (Head != null)
                        {
                            defaultHead.SetActive(true);
                            GameObject.Destroy(Head);
                        }
                        headData = null;
                        break;
                    case ItemData.EquipPosition.BODY:
                        if (Chest != null)
                        {
                            defaultChest.SetActive(true);
                            GameObject.Destroy(Chest);
                        }
                        ChestData = null;
                        break;
                    case ItemData.EquipPosition.GLOVES:
                        if (Gloves != null)
                        {
                            defaultGloves.SetActive(true);
                            GameObject.Destroy(Gloves);
                        }
                        GlovesData = null;
                        break;
                    case ItemData.EquipPosition.SHOES:
                        if (Shoes != null)
                        {
                            defaultBoots.SetActive(true);
                            GameObject.Destroy(Shoes);
                        }
                        ShoesData = null;
                        break;
                    case ItemData.EquipPosition.TROUSERS:
                        if (Trousers != null)
                        {
                            defaultTrousers.SetActive(true);
                            GameObject.Destroy(Trousers);
                        }
                        TrousersData = null;
                        break;
                //项链戒指
                    default:
                        Debug.LogError("UnwieldEquip NotExist " + pos);
                        break;
                }
            }
        }

        void UnwieldEquip(EquipData ed)
        {
            if (ed != null || ed.itemData != null)
            {
                return;
            }

            if (!IsFakeObject)
            {
                var fake = ObjectManager.objectManager.GetFakeObj(photonView.GetLocalId());
                if (fake != null)
                {
                    fake.GetComponent<NpcEquipment>().UnwieldEquip(ed);
                }
            }

            UnwieldEquip(ed.itemData.equipPosition);
            SetFakeObjLayer();
        }

        /// <summary>
        ///所有装备都可以增加攻击和防御 
        /// </summary>
        /// <returns>The damage.</returns>
        public int GetDamage()
        {
            var dam = 0;
            foreach(var p in partToData.Values) {
                dam += p.itemData.Damage + p.entry.ExtraAttack+p.entry.RndAttack;
            }
            Log.Sys("GetDamage ForEquip "+gameObject+" dam "+dam);
            return dam;
        }

        /*
         * Armor With Gems or Strength Has Defense
         * 
         * Affix  OF_POISONDEFENSES
         */ 
        public int GetPoisonDefense()
        {
            return 0;
        }

        public int GetArmor()
        {
            int arm = 0;
            foreach (var p in partToData.Values)
            {
                arm += p.itemData.RealArmor+p.entry.ExtraDefense+p.entry.RndDefense;
            }
            return arm;
        }

        //刀光
        void NpcShowWeaponTrail(MyEvent evt)
        {
            Log.Ani("Do Show Trail");
            if (weaponTrail == null)
            {
                GameObject trail = Instantiate(Resources.Load<GameObject>("particles/newWeaponTrail")) as GameObject;
                weaponTrail = trail;
                if (rightHand != null)
                {
                    trail.transform.parent = rightHand;
                    trail.transform.localPosition = Vector3.zero;
                    trail.transform.localScale = Vector3.one;
                    //X Rotate 90 For New Game Model
                    //模型的挂点的z轴向上 因此需要调整 weaponTrail 的Y轴和模型z轴一致
                    trail.transform.localRotation = Quaternion.Euler(90, 0, 0);
                }
                GetComponent<AnimationController>().AddTrail(trail.GetComponent<WeaponTrail>());
                var wt = trail.GetComponent<WeaponTrail>();
                float duration = Mathf.Max(1, evt.floatArg);
                wt.SetTime(duration, 0.1f, 1);
                weaponTrail.SetActive(true);

            } else
            {
                var wt = weaponTrail.GetComponent<WeaponTrail>();
                float duration = Mathf.Max(1, evt.floatArg);

                wt.SetTime(duration, 0.1f, 1);
                weaponTrail.SetActive(true);
            }
            GetComponent<AnimationController>().SetAnimationSampleRate(100);
        }

        IEnumerator EnableWeaponTrail()
        {
            yield return null;

        }

        void NpcHideWeaponTrail()
        {
            Log.Ani("Do Hide Trail");
            if (weaponTrail != null)
            {
                //weaponTrail.SetActive (false);
                var wt = weaponTrail.GetComponent<WeaponTrail>();
                wt.SetTime(0f, 0, 1);
                GetComponent<AnimationController>().SetAnimationSampleRate(30);
                //weaponTrail.GetComponent<XffectComponent> ().Reset ();
            }
        }
        

    }
}
