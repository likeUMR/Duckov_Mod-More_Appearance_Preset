using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAppearancePreset
{
    /// <summary>
    /// Preset对象复制和修改模块
    /// </summary>
    public static class PresetCopier
    {
        /// <summary>
        /// 完整复制GameObject及其所有子对象，改名为Preset
        /// 复制后先设置为显示，执行所有操作后再隐藏
        /// </summary>
        public static GameObject? CopyGameObject(GameObject source)
        {
            if (source == null)
            {
                Debug.LogError("[MoreAppearancePreset] 源对象为空，无法复制");
                return null;
            }

            try
            {
                Debug.Log($"[PresetCopier] 开始复制Panel对象: {source.name}");
                Debug.Log($"[PresetCopier] 源对象路径: {UIFinder.GetFullPath(source.transform)}");
                Debug.Log($"[PresetCopier] 源对象激活状态: {source.activeSelf}");

                // 创建副本，使用Instantiate会完整复制GameObject及其所有组件和子对象
                GameObject copy = UnityEngine.Object.Instantiate(source);
                Debug.Log($"[PresetCopier] ✓ 已创建副本，原始名称: {copy.name}");
                
                // 改名为Preset
                copy.name = "Preset";
                string actualName = copy.name;
                Debug.Log($"[PresetCopier] ✓ 已重命名为: {actualName}");
                
                // 获取源对象的父对象，如果存在则设置为同一父对象
                if (source.transform.parent != null)
                {
                    copy.transform.SetParent(source.transform.parent, false);
                    Debug.Log($"[PresetCopier] ✓ 已设置父对象: {source.transform.parent.name}");
                }
                else
                {
                    // 如果源对象没有父对象，创建一个根对象来管理副本
                    GameObject copyRoot = new GameObject("MoreAppearancePreset_CopyRoot");
                    copy.transform.SetParent(copyRoot.transform, false);
                    Debug.Log($"[PresetCopier] ✓ 创建了新的根对象: {copyRoot.name}");
                }

                // 设置副本的位置、旋转和缩放与源对象相同
                copy.transform.localPosition = source.transform.localPosition;
                copy.transform.localRotation = source.transform.localRotation;
                copy.transform.localScale = source.transform.localScale;
                Debug.Log($"[PresetCopier] ✓ 已设置Transform属性 (位置: {copy.transform.localPosition}, 旋转: {copy.transform.localRotation}, 缩放: {copy.transform.localScale})");

                // 设置副本在层级中的顺序（放在源对象之后）
                copy.transform.SetSiblingIndex(source.transform.GetSiblingIndex() + 1);
                Debug.Log($"[PresetCopier] ✓ 已设置层级顺序: {copy.transform.GetSiblingIndex()}");

                // 先设置为显示，确保所有操作能正确执行
                copy.SetActive(true);
                bool actualActiveState = copy.activeSelf;
                Debug.Log($"[PresetCopier] ✓ Preset对象已设置为显示（验证: activeSelf={actualActiveState}），准备执行后续操作");

                // 删除除了名为"Title"的所有其他子级
                Debug.Log($"[PresetCopier] 开始删除子对象（保留Title）...");
                RemoveChildrenExceptTitle(copy);

                // 配置VerticalLayoutGroup为左对齐
                Debug.Log($"[PresetCopier] 开始配置VerticalLayoutGroup对齐方式...");
                ConfigureVerticalLayoutGroup(copy);

                // 修改Title下的TMP组件文字
                Debug.Log($"[PresetCopier] 开始修改Title文字...");
                UpdateTitleText(copy);

                // 操作完成后，隐藏Preset对象
                copy.SetActive(false);
                bool actualHiddenState = copy.activeSelf;
                Debug.Log($"[PresetCopier] ✓ Preset对象已设置为隐藏（验证: activeSelf={actualHiddenState}）");

                string finalCopyName = copy.name;
                Debug.Log($"[PresetCopier] ✓ 成功复制UI对象: {source.name} -> {finalCopyName}（验证）");
                Debug.Log($"[PresetCopier] ✓ 副本层级路径: {UIFinder.GetFullPath(copy.transform)}");
                Debug.Log($"[PresetCopier] ✓ 复制操作完成");

                return copy;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 复制UI对象时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// 删除除了名为"Title"的所有其他子级
        /// </summary>
        private static void RemoveChildrenExceptTitle(GameObject parent)
        {
            if (parent == null)
            {
                Debug.LogWarning("[MoreAppearancePreset] RemoveChildrenExceptTitle: 父对象为空");
                return;
            }

            try
            {
                Transform parentTransform = parent.transform;
                Debug.Log($"[PresetCopier] 当前子对象数量: {parentTransform.childCount}");
                
                List<Transform> childrenToRemove = new List<Transform>();

                // 收集所有需要删除的子对象（除了名为"Title"的）
                for (int i = 0; i < parentTransform.childCount; i++)
                {
                    Transform child = parentTransform.GetChild(i);
                    if (child.name != "Title")
                    {
                        childrenToRemove.Add(child);
                        Debug.Log($"[PresetCopier] 标记待删除子对象: {child.name}");
                    }
                    else
                    {
                        Debug.Log($"[PresetCopier] 保留子对象: {child.name}");
                    }
                }

                Debug.Log($"[PresetCopier] 准备删除 {childrenToRemove.Count} 个子对象");

                // 删除收集到的子对象
                foreach (Transform child in childrenToRemove)
                {
                    if (child != null)
                    {
                        Debug.Log($"[PresetCopier] 正在删除子对象: {child.name}");
                        UnityEngine.Object.Destroy(child.gameObject);
                        Debug.Log($"[PresetCopier] ✓ 已删除子对象: {child.name}");
                    }
                }

                Debug.Log($"[PresetCopier] ✓ 删除操作完成，已删除 {childrenToRemove.Count} 个子对象，保留Title子对象");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 删除子对象时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 配置VerticalLayoutGroup为左对齐
        /// </summary>
        private static void ConfigureVerticalLayoutGroup(GameObject parent)
        {
            if (parent == null)
            {
                Debug.LogWarning("[MoreAppearancePreset] ConfigureVerticalLayoutGroup: 父对象为空");
                return;
            }

            try
            {
                // 获取VerticalLayoutGroup组件
                VerticalLayoutGroup? layoutGroup = parent.GetComponent<VerticalLayoutGroup>();
                
                if (layoutGroup != null)
                {
                    // 设置子对象对齐方式为左对齐（上部左对齐）
                    layoutGroup.childAlignment = TextAnchor.UpperLeft;
                    
                    Debug.Log($"[PresetCopier] ✓ 已配置VerticalLayoutGroup为左对齐");
                    Debug.Log($"[PresetCopier]   childAlignment: {layoutGroup.childAlignment}");
                }
                else
                {
                    Debug.LogWarning("[PresetCopier] ✗ 未找到VerticalLayoutGroup组件");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 配置VerticalLayoutGroup时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 修改Title下的TMP组件文字为"我是什么？"
        /// </summary>
        private static void UpdateTitleText(GameObject parent)
        {
            if (parent == null)
            {
                Debug.LogWarning("[MoreAppearancePreset] UpdateTitleText: 父对象为空");
                return;
            }

            try
            {
                Debug.Log($"[PresetCopier] 开始查找Title子对象...");
                
                // 查找Title子对象
                Transform? titleTransform = null;
                Transform parentTransform = parent.transform;
                
                Debug.Log($"[PresetCopier] 父对象 {parent.name} 的子对象数量: {parentTransform.childCount}");
                
                for (int i = 0; i < parentTransform.childCount; i++)
                {
                    Transform child = parentTransform.GetChild(i);
                    Debug.Log($"[PresetCopier] 检查子对象 [{i}]: {child.name}");
                    if (child.name == "Title")
                    {
                        titleTransform = child;
                        Debug.Log($"[PresetCopier] ✓ 找到Title子对象: {child.name}");
                        break;
                    }
                }

                if (titleTransform == null)
                {
                    Debug.LogWarning("[PresetCopier] ✗ 未找到Title子对象");
                    return;
                }

                Debug.Log($"[PresetCopier] 开始移除TextLocalizer组件...");
                
                // 在修改文字前，先移除TextLocalizer组件
                TextLocalizerRemover.RemoveTextLocalizers(titleTransform.gameObject);

                Debug.Log($"[PresetCopier] 开始查找Title下的TextMeshProUGUI组件...");
                
                // 从Title对象及其子对象中查找TextMeshProUGUI组件
                TextMeshProUGUI? tmp = titleTransform.GetComponentInChildren<TextMeshProUGUI>(true);
                
                if (tmp != null)
                {
                    string oldText = tmp.text ?? "(空)";
                    // 使用富文本语法实现渐变色：最左边是蓝色，最右边是红色
                    // 为"我是什么？"的每个字符设置渐变色
                    string newText = "<color=#0022FF>我</color><color=#CC00FF>是</color><color=#FF00AA>什</color><color=#FF4400>么</color><color=#FF0000>？</color>";
                    tmp.text = newText;
                    string actualText = tmp.text ?? "(空)";
                    Debug.Log($"[PresetCopier] ✓ 成功修改Title下的TMP文字（渐变色）");
                    Debug.Log($"[PresetCopier]   旧文字: \"{oldText}\"");
                    Debug.Log($"[PresetCopier]   新文字（验证）: \"{actualText}\"");
                    Debug.Log($"[PresetCopier]   TMP组件位置: {UIFinder.GetFullPath(tmp.transform)}");
                }
                else
                {
                    Debug.LogWarning("[PresetCopier] ✗ Title及其子对象上未找到TextMeshProUGUI组件");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 修改Title文字时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
            }
        }
    }
}

