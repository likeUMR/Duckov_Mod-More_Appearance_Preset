using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreAppearancePreset
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private GameObject? _presetObject = null; // 保存Preset对象的引用
        private bool _hasFoundScrollbars = false;

        void Awake()
        {
            Debug.Log("[MoreAppearancePreset] Mod Loaded!!!");
        }

        void Update()
        {

            // 首先检查是否是捏脸场景，如果不是则直接返回，节省性能
            if (SceneManager.GetActiveScene().name!="Prologue_1")
            {
                return;
            }

            // 【新增】首次进入场景时，查找所有Scrollbar
            if (!_hasFoundScrollbars)
            {
                GameObject? panelsObject = GameObject.Find(PresetData.PANELS_PATH);

                if (panelsObject != null && panelsObject.activeInHierarchy)
                {
                    Debug.Log("===== 开始查找游戏中的所有Scrollbar =====");
                    ScrollbarFinder.FindAndLogAllScrollbars();
                    _hasFoundScrollbars = true;
                    Debug.Log("===== Scrollbar查找完成，请查看日志 =====");
                    Debug.Log("提示：找到Scrollbar路径后，请复制到ScrollViewCreator的GAME_SCROLLBAR_PATH常量中");
                }
            }

            // 每帧检查Preset对象是否存在（无论是否active）
            // 如果不存在，说明场景刷新了，需要重新应用修改
            if (!IsPresetExists())
            {
                // Preset不存在，执行完整修改流程
                CheckAndCopyTargetUI();
            }
            else
            {
                // Preset存在，只处理UI点击
                // 确保引用是最新的
                // RefreshPresetReference();

                // 检测鼠标左键抬起
                if (Input.GetMouseButtonUp(0))
                {
                    PresetViewManager.CheckUIClick(_presetObject);
                }
            }
        }

        /// <summary>
        /// 检查是否是捏脸场景（通过检查Panels对象是否active）
        /// </summary>
        private bool IsCustomFaceSceneActive()
        {
            GameObject? panelsObject = GameObject.Find(PresetData.PANELS_PATH);
            
            if (panelsObject == null)
            {
                return false;
            }
            
            // 检查Panels对象是否active
            return panelsObject.activeInHierarchy;
        }

        /// <summary>
        /// 检查Preset对象是否存在（无论是否active）
        /// </summary>
        private bool IsPresetExists()
        {
            // 如果已经保存了引用，检查引用是否有效（对象未被销毁）
            if (_presetObject != null && _presetObject)
            {
                return true;
            }

            // // 引用无效，尝试通过路径查找Preset对象（包括未激活的）
            // GameObject? preset = UIFinder.FindGameObjectByPath($"{PresetData.PANELS_PATH}/Preset");
            // if (preset != null)
            // {
            //     // 更新引用
            //     _presetObject = preset;
            //     return true;
            // }

            // 未找到Preset对象，清除无效引用
            _presetObject = null;
            return false;
        }

        /// <summary>
        /// 刷新Preset对象的引用
        /// </summary>
        private void RefreshPresetReference()
        {
            // 如果引用无效或为空，尝试重新查找
            if (_presetObject == null)
            {
                GameObject? preset = UIFinder.FindGameObjectByPath($"{PresetData.PANELS_PATH}/Preset");
                if (preset != null)
                {
                    _presetObject = preset;
                    Debug.Log("[ModBehaviour] ✓ 已刷新Preset对象引用");
                }
            }
        }

        /// <summary>
        /// 检测目标UI是否存在，如果存在则复制一份（即使对象未激活也能找到）
        /// 只有当Panels对象存在且active时，才进行检测
        /// </summary>
        private void CheckAndCopyTargetUI()
        {
            // 首先检查Panels对象是否存在且active
            GameObject? panelsObject = UIFinder.FindGameObjectByPath(PresetData.PANELS_PATH);
            
            if (panelsObject == null)
            {
                // Panels对象不存在，不进行检测
                return;
            }
            
            // 检查Panels对象是否active
            if (!panelsObject.activeInHierarchy)
            {
                // Panels对象存在但未激活，不进行检测
                return;
            }
            
            // Panels对象存在且active，继续检测目标UI对象
            GameObject? targetObject = UIFinder.FindGameObjectByPath(PresetData.TARGET_UI_PATH);

            if (targetObject != null)
            {
                Debug.Log($"[ModBehaviour] ✓ 检测到目标UI: {PresetData.TARGET_UI_PATH}");
                Debug.Log($"[ModBehaviour]   Panels对象状态: activeInHierarchy={panelsObject.activeInHierarchy}");
                Debug.Log($"[ModBehaviour]   对象名称: {targetObject.name}");
                Debug.Log($"[ModBehaviour]   是否激活: {targetObject.activeSelf}");
                Debug.Log($"[ModBehaviour]   完整路径: {UIFinder.GetFullPath(targetObject.transform)}");
                
                // 复制目标对象
                Debug.Log($"[ModBehaviour] ===== 开始复制Panel对象 =====");
                _presetObject = PresetCopier.CopyGameObject(targetObject);
                
                if (_presetObject != null)
                {
                    Debug.Log($"[ModBehaviour] ✓ Panel复制完成");
                    
                    // 修改原始YellowDuck对象的文字为"我是什么？"
                    Debug.Log($"[ModBehaviour] ===== 开始修改原始YellowDuck文字 =====");
                    YellowDuckHandler.UpdateOriginalYellowDuckText();

                    // 修改原始YellowDuck按钮的逻辑，使其等同于按键8的效果
                    Debug.Log($"[ModBehaviour] ===== 开始修改原始YellowDuck按钮逻辑 =====");
                    YellowDuckHandler.UpdateOriginalYellowDuckButton(_presetObject);

                    // 复制YellowDuck到Preset的子级
                    Debug.Log($"[ModBehaviour] ===== 开始复制YellowDuck到Preset =====");
                    YellowDuckHandler.CopyYellowDuckToPreset(_presetObject, PresetData.PresetDataDict);
                    
                    Debug.Log("[ModBehaviour] ✓ UI复制流程完成");

                    // ===== 添加导出代码 =====
                    // Debug.Log("[ModBehaviour] ===== 开始导出UI信息 =====");

                    // 1. 导出整个Preset面板的结构
                    // UIDebugExporter.ExportToFile(_presetObject, "Preset_Panel_Export.txt");

                    // 2. 导出第一个YellowDuck按钮的结构
                    // if (_presetObject.transform.childCount > 0)
                    // {
                    // GameObject firstButton = _presetObject.transform.GetChild(0).gameObject;
                    // UIDebugExporter.ExportToFile(firstButton, "YellowDuck_Button_Export.txt");
                    // }

                    // 3. 查找并导出游戏中所有的ScrollView
                    // UIDebugExporter.FindAndExportAllScrollViews();

                    // Debug.Log("[ModBehaviour] ✓ UI信息导出完成");
                    // }
                    // else
                    // {
                    // Debug.LogError("[ModBehaviour] ✗ Panel复制失败，无法继续后续操作");
                    }
                }
            }
    }
}
