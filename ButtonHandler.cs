using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAppearancePreset
{
    /// <summary>
    /// Button组件处理模块
    /// </summary>
    public static class ButtonHandler
    {
        /// <summary>
        /// 删除GameObject上的Button组件并重新创建，添加点击监听器用于复制预设代码到剪贴板
        /// </summary>
        public static void RecreateButtonComponent(GameObject obj, string presetData)
        {
            if (obj == null)
            {
                return;
            }

            try
            {
                Debug.Log($"[ButtonHandler] 开始处理Button组件，对象: {obj.name}");

                bool interactable = true;
                ColorBlock colors = ColorBlock.defaultColorBlock;
                SpriteState spriteState = new SpriteState();
                AnimationTriggers animationTriggers = new AnimationTriggers();
                Graphic? targetGraphic = null;
                Selectable.Transition transition = Selectable.Transition.ColorTint;
                Navigation navigation = Navigation.defaultNavigation;

                // 查找并删除所有Button组件（可能存在多个）
                Button[] existingButtons = obj.GetComponents<Button>();
                if (existingButtons != null && existingButtons.Length > 0)
                {
                    Debug.Log($"[ButtonHandler] ✓ 找到 {existingButtons.Length} 个现有Button组件: {obj.name}");
                    
                    // 保存第一个Button的属性（如果需要在重新创建时恢复）
                    Button firstButton = existingButtons[0];
                    interactable = firstButton.interactable;
                    colors = firstButton.colors;
                    spriteState = firstButton.spriteState;
                    animationTriggers = firstButton.animationTriggers;
                    targetGraphic = firstButton.targetGraphic;
                    transition = firstButton.transition;
                    navigation = firstButton.navigation;

                    Debug.Log($"[ButtonHandler]   已保存Button属性: interactable={interactable}, transition={transition}");

                    // 删除所有Button组件（使用DestroyImmediate立即销毁）
                    foreach (Button btn in existingButtons)
                    {
                        if (btn != null)
                        {
                            UnityEngine.Object.DestroyImmediate(btn);
                            Debug.Log($"[ButtonHandler] ✓ 已立即删除Button组件: {obj.name}");
                        }
                    }
                }
                else
                {
                    Debug.Log($"[ButtonHandler] 未找到现有Button组件，将创建新的Button组件");
                }

                // 再次检查确保所有Button组件都已删除
                Button[] remainingButtons = obj.GetComponents<Button>();
                if (remainingButtons != null && remainingButtons.Length > 0)
                {
                    Debug.LogWarning($"[ButtonHandler] 警告: 仍有 {remainingButtons.Length} 个Button组件残留，强制删除...");
                    foreach (Button btn in remainingButtons)
                    {
                        if (btn != null)
                        {
                            UnityEngine.Object.DestroyImmediate(btn);
                        }
                    }
                }

                // 重新创建Button组件（即使对象处于隐藏状态也能添加）
                Debug.Log($"[ButtonHandler] 开始创建新的Button组件...");
                Button newButton = obj.AddComponent<Button>();
                Debug.Log($"[ButtonHandler] ✓ 已创建新Button组件");

                // 恢复属性
                newButton.interactable = interactable;
                newButton.colors = colors;
                newButton.spriteState = spriteState;
                newButton.animationTriggers = animationTriggers;
                newButton.targetGraphic = targetGraphic;
                newButton.transition = transition;
                newButton.navigation = navigation;
                
                // 验证属性
                bool actualInteractable = newButton.interactable;
                Selectable.Transition actualTransition = newButton.transition;
                Debug.Log($"[ButtonHandler] ✓ 已恢复Button属性（验证: interactable={actualInteractable}, transition={actualTransition}）");

                // 查找Paste对象的Button组件
                GameObject? pasteObject = UIFinder.FindGameObjectByPath(PresetData.PASTE_PATH);
                Button? pasteButton = null;
                
                if (pasteObject != null)
                {
                    pasteButton = pasteObject.GetComponent<Button>();
                    if (pasteButton != null)
                    {
                        Debug.Log($"[ButtonHandler] ✓ 找到Paste按钮: {pasteObject.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"[ButtonHandler] Paste对象上未找到Button组件");
                    }
                }
                else
                {
                    Debug.LogWarning($"[ButtonHandler] 未找到Paste对象: {PresetData.PASTE_PATH}");
                }

                // 添加点击监听器：先复制到剪贴板，然后调用Paste按钮的onClick
                Debug.Log($"[ButtonHandler] 添加点击监听器（预设代码长度: {presetData.Length}字符）...");
                newButton.onClick.AddListener(() =>
                {
                    // 第一步：复制预设代码到剪贴板
                    ClipboardUtility.CopyToClipboard(presetData);
                    
                    // 第二步：调用Paste按钮的onClick
                    if (pasteButton != null)
                    {
                        Debug.Log($"[ButtonHandler] 调用Paste按钮的onClick...");
                        pasteButton.onClick.Invoke();
                        Debug.Log($"[ButtonHandler] ✓ 已调用Paste按钮的onClick");
                    }
                    else
                    {
                        Debug.LogWarning($"[ButtonHandler] Paste按钮不存在，无法调用onClick");
                    }
                });
                
                // 验证按钮组件存在且可交互
                bool buttonExists = newButton != null;
                bool isInteractable = newButton != null && newButton.interactable;
                Debug.Log($"[ButtonHandler] ✓ 已添加点击监听器（验证: 按钮存在={buttonExists}, interactable={isInteractable}）");

                Debug.Log($"[ButtonHandler] ✓ 完成Button组件处理: {obj.name}（已添加复制到剪贴板的监听器）");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 重新创建Button组件时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 重新创建Button组件，添加点击监听器用于应用随机preset
        /// </summary>
        public static void RecreateButtonComponentForRandomPreset(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            try
            {
                Debug.Log($"[ButtonHandler] 开始处理随机preset按钮组件，对象: {obj.name}");

                bool interactable = true;
                ColorBlock colors = ColorBlock.defaultColorBlock;
                SpriteState spriteState = new SpriteState();
                AnimationTriggers animationTriggers = new AnimationTriggers();
                Graphic? targetGraphic = null;
                Selectable.Transition transition = Selectable.Transition.ColorTint;
                Navigation navigation = Navigation.defaultNavigation;

                // 查找并删除所有Button组件（可能存在多个）
                Button[] existingButtons = obj.GetComponents<Button>();
                if (existingButtons != null && existingButtons.Length > 0)
                {
                    Debug.Log($"[ButtonHandler] ✓ 找到 {existingButtons.Length} 个现有Button组件: {obj.name}");
                    
                    // 保存第一个Button的属性
                    Button firstButton = existingButtons[0];
                    interactable = firstButton.interactable;
                    colors = firstButton.colors;
                    spriteState = firstButton.spriteState;
                    animationTriggers = firstButton.animationTriggers;
                    targetGraphic = firstButton.targetGraphic;
                    transition = firstButton.transition;
                    navigation = firstButton.navigation;

                    // 删除所有Button组件
                    foreach (Button btn in existingButtons)
                    {
                        if (btn != null)
                        {
                            UnityEngine.Object.DestroyImmediate(btn);
                            Debug.Log($"[ButtonHandler] ✓ 已立即删除Button组件: {obj.name}");
                        }
                    }
                }

                // 重新创建Button组件
                Debug.Log($"[ButtonHandler] 开始创建新的Button组件...");
                Button newButton = obj.AddComponent<Button>();
                Debug.Log($"[ButtonHandler] ✓ 已创建新Button组件");

                // 恢复属性
                newButton.interactable = interactable;
                newButton.colors = colors;
                newButton.spriteState = spriteState;
                newButton.animationTriggers = animationTriggers;
                newButton.targetGraphic = targetGraphic;
                newButton.transition = transition;
                newButton.navigation = navigation;

                // 查找Paste对象的Button组件
                GameObject? pasteObject = UIFinder.FindGameObjectByPath(PresetData.PASTE_PATH);
                Button? pasteButton = null;
                
                if (pasteObject != null)
                {
                    pasteButton = pasteObject.GetComponent<Button>();
                    if (pasteButton != null)
                    {
                        Debug.Log($"[ButtonHandler] ✓ 找到Paste按钮: {pasteObject.name}");
                    }
                }

                // 添加点击监听器：每次点击时生成新的随机preset
                Debug.Log($"[ButtonHandler] 添加随机preset按钮点击监听器...");
                newButton.onClick.AddListener(() =>
                {
                    // 每次点击时生成新的随机preset
                    string randomPresetData = RandomPresetGenerator.GenerateRandomPreset();
                    Debug.Log($"[ButtonHandler] 生成了随机preset，长度: {randomPresetData.Length}字符");
                    
                    // 复制到剪贴板
                    ClipboardUtility.CopyToClipboard(randomPresetData);
                    
                    // 调用Paste按钮的onClick
                    if (pasteButton != null)
                    {
                        Debug.Log($"[ButtonHandler] 调用Paste按钮的onClick...");
                        pasteButton.onClick.Invoke();
                        Debug.Log($"[ButtonHandler] ✓ 已调用Paste按钮的onClick");
                    }
                    else
                    {
                        Debug.LogWarning($"[ButtonHandler] Paste按钮不存在，无法调用onClick");
                    }
                });
                
                Debug.Log($"[ButtonHandler] ✓ 完成随机preset按钮组件处理: {obj.name}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 重新创建随机preset按钮组件时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
            }
        }
    }

    /// <summary>
    /// 剪贴板工具类
    /// </summary>
    public static class ClipboardUtility
    {
        /// <summary>
        /// 将文本复制到剪贴板
        /// </summary>
        public static void CopyToClipboard(string text)
        {
            try
            {
                GUIUtility.systemCopyBuffer = text;
                string actualClipboard = GUIUtility.systemCopyBuffer ?? "(空)";
                Debug.Log($"[ClipboardUtility] 已将预设代码复制到剪贴板（验证: 剪贴板长度={actualClipboard.Length}字符）");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 复制到剪贴板时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
            }
        }
    }
}

