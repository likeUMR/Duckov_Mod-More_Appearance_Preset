using UnityEngine;

namespace MoreAppearancePreset
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private bool _hasCopied = false; // 标记是否已经复制，用于停止检测
        private GameObject? _presetObject = null; // 保存Preset对象的引用

        void Awake()
        {
            Debug.Log("[MoreAppearancePreset] Mod Loaded!!!");
        }

        void Update()
        {
            // 如果已经复制过，检查键盘输入和UI点击
            if (_hasCopied)
            {
                // 检测是否按下数字键8
                if (Input.GetKeyDown(KeyCode.Alpha8))
                {
                    PresetViewManager.TogglePresetView(_presetObject);
                }

                // 检测鼠标左键抬起
                if (Input.GetMouseButtonUp(0))
                {
                    PresetViewManager.CheckUIClick(_presetObject);
                }
                return;
            }
            else
            {
                // 检测目标UI是否存在
                CheckAndCopyTargetUI();
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
                }
                else
                {
                    Debug.LogError("[ModBehaviour] ✗ Panel复制失败，无法继续后续操作");
                }
                
                // 标记已复制，停止后续检测
                _hasCopied = true;
                
                Debug.Log("[ModBehaviour] ✓ UI复制流程完成，停止检测");
            }
        }
    }
}
