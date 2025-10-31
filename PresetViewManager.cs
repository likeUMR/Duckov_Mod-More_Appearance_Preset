using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MoreAppearancePreset
{
    /// <summary>
    /// Preset视图管理模块
    /// </summary>
    public static class PresetViewManager
    {
        /// <summary>
        /// 切换Preset视图：隐藏Panels下的所有子对象，显示Preset
        /// </summary>
        public static void TogglePresetView(GameObject? presetObject)
        {
            try
            {
                // 查找Panels对象
                GameObject panelsObject = GameObject.Find(PresetData.PANELS_PATH);
                
                if (panelsObject == null)
                {
                    Debug.LogWarning($"[MoreAppearancePreset] 未找到Panels对象: {PresetData.PANELS_PATH}");
                    return;
                }

                // 隐藏Panels下的所有子对象
                Transform panelsTransform = panelsObject.transform;
                for (int i = 0; i < panelsTransform.childCount; i++)
                {
                    Transform child = panelsTransform.GetChild(i);
                    if (child.gameObject != presetObject) // 不隐藏Preset本身
                    {
                        child.gameObject.SetActive(false);
                        Debug.Log($"[MoreAppearancePreset] 隐藏子对象: {child.name}");
                    }
                }

                // 显示Preset对象
                if (presetObject != null)
                {
                    presetObject.SetActive(true);
                    Debug.Log("[MoreAppearancePreset] 显示Preset对象");
                }
                else
                {
                    Debug.LogWarning("[MoreAppearancePreset] Preset对象引用为空，尝试通过路径查找");
                    // 尝试通过路径查找Preset对象
                    GameObject presetFromPath = GameObject.Find($"{PresetData.PANELS_PATH}/Preset");
                    if (presetFromPath != null)
                    {
                        presetFromPath.SetActive(true);
                        Debug.Log("[MoreAppearancePreset] 通过路径找到并显示Preset对象");
                    }
                    else
                    {
                        Debug.LogError("[MoreAppearancePreset] 无法找到Preset对象");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 切换Preset视图时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 检测UI点击，如果点击的UI不属于Preset，则隐藏Preset
        /// </summary>
        public static void CheckUIClick(GameObject? presetObject)
        {
            // 如果Preset对象不存在或未激活，不需要处理
            if (presetObject == null || !presetObject.activeSelf)
            {
                return;
            }
            
            try
            {
                // 使用EventSystem检测UI点击
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                if (results.Count > 0)
                {
                    // 获取第一个命中的UI对象
                    GameObject clickedObject = results[0].gameObject;
                    
                    // 检查点击的对象是否属于Preset或其子对象
                    bool isPresetOrChild = IsPresetOrChild(clickedObject, presetObject);

                    // 检查点击的对象是否是YellowDuck或Paste（或其子对象）
                    bool isYellowDuckOrPaste = IsYellowDuckOrPaste(clickedObject);

                    if (!isPresetOrChild && !isYellowDuckOrPaste)
                    {
                        // 点击的对象不属于Preset，也不是YellowDuck或Paste，隐藏Preset
                        presetObject.SetActive(false);
                        Debug.Log($"[PresetViewManager] 检测到点击非Preset UI: {clickedObject.name}，已隐藏Preset");
                    }
                    else if (isYellowDuckOrPaste)
                    {
                        Debug.Log($"[PresetViewManager] 点击的是YellowDuck或Paste UI: {clickedObject.name}，保持Preset显示");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 检测UI点击时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 检查指定的GameObject是否属于Preset或其子对象
        /// </summary>
        private static bool IsPresetOrChild(GameObject obj, GameObject? presetObject)
        {
            if (obj == null || presetObject == null)
            {
                return false;
            }

            // 检查是否为Preset本身
            if (obj == presetObject)
            {
                return true;
            }

            // 向上遍历Transform层级，检查是否是Preset的子对象
            Transform current = obj.transform;
            while (current != null)
            {
                if (current.gameObject == presetObject)
                {
                    return true;
                }
                current = current.parent;
            }

            return false;
        }

        /// <summary>
        /// 检查指定的GameObject是否属于YellowDuck或Paste（或其子对象）
        /// </summary>
        private static bool IsYellowDuckOrPaste(GameObject obj)
        {
            if (obj == null)
            {
                return false;
            }

            // 直接使用GameObject.Find查找，避免遍历
            GameObject? yellowDuckObject = GameObject.Find(PresetData.YELLOW_DUCK_PATH);
            GameObject? pasteObject = GameObject.Find(PresetData.PASTE_PATH);

            // 检查是否为YellowDuck本身或其子对象
            if (yellowDuckObject != null)
            {
                if (obj == yellowDuckObject)
                {
                    return true;
                }

                // 向上遍历Transform层级，检查是否是YellowDuck的子对象
                Transform current = obj.transform;
                while (current != null)
                {
                    if (current.gameObject == yellowDuckObject)
                    {
                        return true;
                    }
                    current = current.parent;
                }
            }

            // 检查是否为Paste本身或其子对象
            if (pasteObject != null)
            {
                if (obj == pasteObject)
                {
                    return true;
                }

                // 向上遍历Transform层级，检查是否是Paste的子对象
                Transform current = obj.transform;
                while (current != null)
                {
                    if (current.gameObject == pasteObject)
                    {
                        return true;
                    }
                    current = current.parent;
                }
            }

            return false;
        }
    }
}

