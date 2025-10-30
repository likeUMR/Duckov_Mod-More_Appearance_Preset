using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAppearancePreset
{
    /// <summary>
    /// YellowDuck处理模块
    /// </summary>
    public static class YellowDuckHandler
    {
        /// <summary>
        /// 根据预设代码字典数量复制YellowDuck对象到Preset的子级，并重新创建Button组件
        /// </summary>
        public static void CopyYellowDuckToPreset(GameObject preset, Dictionary<string, string> presetDataDict)
        {
            if (preset == null)
            {
                Debug.LogWarning("[MoreAppearancePreset] CopyYellowDuckToPreset: Preset对象为空");
                return;
            }

            try
            {
                Debug.Log($"[YellowDuckHandler] 开始复制YellowDuck对象到Preset...");
                
                // 查找YellowDuck对象（包括未激活的）
                GameObject? yellowDuckSource = UIFinder.FindGameObjectByPath(PresetData.YELLOW_DUCK_PATH);

                if (yellowDuckSource == null)
                {
                    Debug.LogWarning($"[YellowDuckHandler] ✗ 未找到YellowDuck对象: {PresetData.YELLOW_DUCK_PATH}");
                    return;
                }

                Debug.Log($"[YellowDuckHandler] ✓ 找到YellowDuck源对象: {yellowDuckSource.name}");
                Debug.Log($"[YellowDuckHandler]   路径: {UIFinder.GetFullPath(yellowDuckSource.transform)}");
                Debug.Log($"[YellowDuckHandler]   激活状态: {yellowDuckSource.activeSelf}");

                // 首先在最前面创建随机preset按钮
                Debug.Log($"[YellowDuckHandler] ===== 创建随机preset按钮 =====");
                GameObject randomPresetButton = UnityEngine.Object.Instantiate(yellowDuckSource);
                randomPresetButton.name = yellowDuckSource.name;
                randomPresetButton.transform.SetParent(preset.transform, false);
                randomPresetButton.transform.localPosition = yellowDuckSource.transform.localPosition;
                randomPresetButton.transform.localRotation = yellowDuckSource.transform.localRotation;
                randomPresetButton.transform.localScale = yellowDuckSource.transform.localScale;
                
                // 设置随机preset按钮的点击逻辑
                ButtonHandler.RecreateButtonComponentForRandomPreset(randomPresetButton);
                
                // 设置随机preset按钮的文字
                UpdateYellowDuckText(randomPresetButton, "随机预设");
                
                // 添加彩虹色循环效果组件
                Debug.Log($"[YellowDuckHandler] 添加彩虹色循环效果组件...");
                if (randomPresetButton.GetComponent<RainbowColorEffect>() == null)
                {
                    randomPresetButton.AddComponent<RainbowColorEffect>();
                    Debug.Log($"[YellowDuckHandler] ✓ 已添加彩虹色循环效果组件");
                }
                
                // 确保随机按钮在最前面
                randomPresetButton.transform.SetSiblingIndex(0);
                
                Debug.Log($"[YellowDuckHandler] ✓ 随机preset按钮创建完成");

                // 根据预设代码字典的数量复制YellowDuck对象
                int presetCount = presetDataDict.Count;
                Debug.Log($"[YellowDuckHandler] 预设数量: {presetCount}，开始复制...");
                
                int index = 0;
                foreach (var presetEntry in presetDataDict)
                {
                    string presetName = presetEntry.Key;
                    string presetData = presetEntry.Value;

                    Debug.Log($"[YellowDuckHandler] ===== 复制预设 [{index + 1}/{presetCount}]: {presetName} =====");

                    // 复制YellowDuck对象
                    GameObject yellowDuckCopy = UnityEngine.Object.Instantiate(yellowDuckSource);
                    yellowDuckCopy.name = yellowDuckSource.name;
                    Debug.Log($"[YellowDuckHandler] ✓ 已创建YellowDuck副本: {yellowDuckCopy.name}");

                    // 设置为Preset的子对象
                    yellowDuckCopy.transform.SetParent(preset.transform, false);
                    Transform? actualParent = yellowDuckCopy.transform.parent;
                    string parentName = actualParent != null ? actualParent.name : "(无父对象)";
                    Debug.Log($"[YellowDuckHandler] ✓ 已设置为Preset的子对象（验证: 父对象={parentName}）");

                    // 保持位置、旋转和缩放
                    yellowDuckCopy.transform.localPosition = yellowDuckSource.transform.localPosition;
                    yellowDuckCopy.transform.localRotation = yellowDuckSource.transform.localRotation;
                    yellowDuckCopy.transform.localScale = yellowDuckSource.transform.localScale;
                    
                    // 验证Transform属性
                    Vector3 actualPosition = yellowDuckCopy.transform.localPosition;
                    Vector3 actualScale = yellowDuckCopy.transform.localScale;
                    Debug.Log($"[YellowDuckHandler] ✓ 已设置Transform属性（验证: 位置={actualPosition}, 缩放={actualScale}）");

                    // 删除并重新创建Button组件，并添加点击监听器
                    Debug.Log($"[YellowDuckHandler] 开始处理Button组件...");
                    ButtonHandler.RecreateButtonComponent(yellowDuckCopy, presetData);

                    // 更新Text子对象的文字
                    Debug.Log($"[YellowDuckHandler] 开始修改文字为: 我是{presetName}...");
                    UpdateYellowDuckText(yellowDuckCopy, presetName);

                    // 设置Image组件的颜色为预设的mainColor（排除"小黄鸭"）
                    if (presetName != "小黄鸭")
                    {
                        Debug.Log($"[YellowDuckHandler] 开始设置Image颜色为预设mainColor...");
                        SetImageColorFromPreset(yellowDuckCopy, presetData);
                    }

                    index++;
                    Debug.Log($"[YellowDuckHandler] ✓ 完成复制预设 [{index}/{presetCount}]: {presetName}");
                }
                
                Debug.Log($"[YellowDuckHandler] ✓ 所有YellowDuck复制操作完成，共复制 {presetCount} 个");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 复制YellowDuck时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 从预设数据中解析mainColor并设置Image组件的颜色
        /// </summary>
        private static void SetImageColorFromPreset(GameObject buttonObject, string presetDataJson)
        {
            if (buttonObject == null || string.IsNullOrEmpty(presetDataJson))
            {
                return;
            }

            try
            {
                // 从JSON中解析mainColor
                Color? mainColor = ParseMainColorFromPreset(presetDataJson);
                
                if (mainColor.HasValue)
                {
                    // 获取Image组件
                    Image? image = buttonObject.GetComponent<Image>();
                    
                    if (image != null)
                    {
                        image.color = mainColor.Value;
                        Debug.Log($"[YellowDuckHandler] ✓ 已设置Image颜色: R={mainColor.Value.r:F3}, G={mainColor.Value.g:F3}, B={mainColor.Value.b:F3}, A={mainColor.Value.a:F3}");
                    }
                    else
                    {
                        Debug.LogWarning("[YellowDuckHandler] ✗ 按钮对象上未找到Image组件");
                    }
                }
                else
                {
                    Debug.LogWarning("[YellowDuckHandler] ✗ 无法从预设数据中解析mainColor");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 设置Image颜色时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 从预设JSON字符串中解析mainColor
        /// </summary>
        private static Color? ParseMainColorFromPreset(string presetDataJson)
        {
            try
            {
                // 使用正则表达式提取mainColor的r, g, b, a值
                // 查找模式: "mainColor":{"r":value,"g":value,"b":value,"a":value}
                // 支持字段顺序不同，但通常顺序是r, g, b, a
                string pattern = @"""mainColor"":\s*\{[^}]*""r"":\s*([0-9.]+)[^}]*""g"":\s*([0-9.]+)[^}]*""b"":\s*([0-9.]+)[^}]*""a"":\s*([0-9.]+)";
                var match = System.Text.RegularExpressions.Regex.Match(presetDataJson, pattern);
                
                if (match.Success && match.Groups.Count >= 5)
                {
                    if (float.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float r) &&
                        float.TryParse(match.Groups[2].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float g) &&
                        float.TryParse(match.Groups[3].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float b) &&
                        float.TryParse(match.Groups[4].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float a))
                    {
                        return new Color(r, g, b, a);
                    }
                }
                
                // 如果上面的正则表达式失败，尝试更宽松的匹配（分别查找每个字段）
                string rPattern = @"""mainColor"":\s*\{[^}]*""r"":\s*([0-9.]+)";
                string gPattern = @"""mainColor"":\s*\{[^}]*""g"":\s*([0-9.]+)";
                string bPattern = @"""mainColor"":\s*\{[^}]*""b"":\s*([0-9.]+)";
                string aPattern = @"""mainColor"":\s*\{[^}]*""a"":\s*([0-9.]+)";
                
                var rMatch = System.Text.RegularExpressions.Regex.Match(presetDataJson, rPattern);
                var gMatch = System.Text.RegularExpressions.Regex.Match(presetDataJson, gPattern);
                var bMatch = System.Text.RegularExpressions.Regex.Match(presetDataJson, bPattern);
                var aMatch = System.Text.RegularExpressions.Regex.Match(presetDataJson, aPattern);
                
                if (rMatch.Success && gMatch.Success && bMatch.Success && aMatch.Success)
                {
                    if (float.TryParse(rMatch.Groups[1].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float r2) &&
                        float.TryParse(gMatch.Groups[1].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float g2) &&
                        float.TryParse(bMatch.Groups[1].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float b2) &&
                        float.TryParse(aMatch.Groups[1].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float a2))
                    {
                        return new Color(r2, g2, b2, a2);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 解析mainColor时发生错误: {ex.Message}");
            }
            
            return null;
        }

        /// <summary>
        /// 修改原始YellowDuck对象的文字为"我是什么？"
        /// </summary>
        public static void UpdateOriginalYellowDuckText()
        {
            try
            {
                Debug.Log($"[YellowDuckHandler] 开始修改原始YellowDuck对象的文字...");
                
                // 查找原始YellowDuck对象（包括未激活的）
                GameObject? yellowDuckSource = UIFinder.FindGameObjectByPath(PresetData.YELLOW_DUCK_PATH);

                if (yellowDuckSource == null)
                {
                    Debug.LogWarning($"[YellowDuckHandler] ✗ 未找到原始YellowDuck对象: {PresetData.YELLOW_DUCK_PATH}");
                    return;
                }

                Debug.Log($"[YellowDuckHandler] ✓ 找到原始YellowDuck对象: {yellowDuckSource.name}");
                Debug.Log($"[YellowDuckHandler]   路径: {UIFinder.GetFullPath(yellowDuckSource.transform)}");

                // 查找名为"Text (TMP)"的子对象
                Transform? textTransform = null;
                Transform yellowDuckTransform = yellowDuckSource.transform;

                Debug.Log($"[YellowDuckHandler] YellowDuck对象 {yellowDuckSource.name} 的子对象数量: {yellowDuckTransform.childCount}");

                for (int i = 0; i < yellowDuckTransform.childCount; i++)
                {
                    Transform child = yellowDuckTransform.GetChild(i);
                    Debug.Log($"[YellowDuckHandler] 检查子对象 [{i}]: {child.name}");
                    if (child.name == "Text (TMP)")
                    {
                        textTransform = child;
                        Debug.Log($"[YellowDuckHandler] ✓ 找到Text (TMP)子对象: {child.name}");
                        break;
                    }
                }

                if (textTransform == null)
                {
                    Debug.LogWarning("[YellowDuckHandler] ✗ 未找到Text (TMP)子对象");
                    return;
                }

                Debug.Log($"[YellowDuckHandler] 开始移除TextLocalizor组件...");
                
                // 在修改文字前，先移除TextLocalizor组件
                TextLocalizerRemover.RemoveTextLocalizers(textTransform.gameObject);

                Debug.Log($"[YellowDuckHandler] 开始获取TextMeshProUGUI组件...");
                
                // 从Text对象获取TextMeshProUGUI组件
                TextMeshProUGUI? tmp = textTransform.GetComponent<TextMeshProUGUI>();

                if (tmp != null)
                {
                    string oldText = tmp.text ?? "(空)";
                    // 使用富文本语法："什"字为蓝色，"么"字为红色
                    string newText = "我是<color=#0000FF>什</color><color=#FF0000>么</color>？";
                    tmp.text = newText;
                    string actualText = tmp.text ?? "(空)";
                    
                    Debug.Log($"[YellowDuckHandler] ✓ 成功修改原始YellowDuck对象的文字");
                    Debug.Log($"[YellowDuckHandler]   旧文字: \"{oldText}\"");
                    Debug.Log($"[YellowDuckHandler]   新文字（验证）: \"{actualText}\"");
                    Debug.Log($"[YellowDuckHandler]   TMP组件位置: {UIFinder.GetFullPath(tmp.transform)}");
                }
                else
                {
                    Debug.LogWarning("[YellowDuckHandler] ✗ Text子对象上未找到TextMeshProUGUI组件");
                }
                
                Debug.Log($"[YellowDuckHandler] ✓ 原始YellowDuck文字修改完成");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 修改原始YellowDuck文字时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 修改原始YellowDuck按钮的逻辑，使其等同于按键8的效果（切换Preset视图）
        /// </summary>
        public static void UpdateOriginalYellowDuckButton(GameObject? presetObject)
        {
            try
            {
                Debug.Log($"[YellowDuckHandler] 开始修改原始YellowDuck按钮的逻辑...");
                
                // 查找原始YellowDuck对象（包括未激活的）
                GameObject? yellowDuckSource = UIFinder.FindGameObjectByPath(PresetData.YELLOW_DUCK_PATH);

                if (yellowDuckSource == null)
                {
                    Debug.LogWarning($"[YellowDuckHandler] ✗ 未找到原始YellowDuck对象: {PresetData.YELLOW_DUCK_PATH}");
                    return;
                }

                Debug.Log($"[YellowDuckHandler] ✓ 找到原始YellowDuck对象: {yellowDuckSource.name}");

                // 获取Button组件
                Button? button = yellowDuckSource.GetComponent<Button>();
                
                if (button != null)
                {
                    // 清除所有现有的监听器
                    button.onClick.RemoveAllListeners();
                    Debug.Log($"[YellowDuckHandler] ✓ 已清除原有监听器");

                    // 添加新的监听器，等同于按键8的效果
                    button.onClick.AddListener(() =>
                    {
                        Debug.Log($"[YellowDuckHandler] 原始YellowDuck按钮被点击，触发TogglePresetView");
                        PresetViewManager.TogglePresetView(presetObject);
                    });

                    Debug.Log($"[YellowDuckHandler] ✓ 已修改原始YellowDuck按钮逻辑（等同于按键8）");
                }
                else
                {
                    Debug.LogWarning($"[YellowDuckHandler] ✗ 原始YellowDuck对象上未找到Button组件");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 修改原始YellowDuck按钮逻辑时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 修改Paste对象的TMP组件文字为"我是什么"
        /// </summary>
        public static void UpdatePasteText()
        {
            try
            {
                Debug.Log($"[YellowDuckHandler] 开始修改Paste对象的文字...");
                
                // 查找Paste对象（包括未激活的）
                GameObject? pasteObject = UIFinder.FindGameObjectByPath(PresetData.PASTE_PATH);

                if (pasteObject == null)
                {
                    Debug.LogWarning($"[YellowDuckHandler] ✗ 未找到Paste对象: {PresetData.PASTE_PATH}");
                    return;
                }

                Debug.Log($"[YellowDuckHandler] ✓ 找到Paste对象: {pasteObject.name}");
                Debug.Log($"[YellowDuckHandler]   路径: {UIFinder.GetFullPath(pasteObject.transform)}");

                Debug.Log($"[YellowDuckHandler] 开始移除TextLocalizer组件...");
                
                // 在修改文字前，先移除TextLocalizer组件
                TextLocalizerRemover.RemoveTextLocalizers(pasteObject);

                // 首先尝试从对象本身获取TMP组件
                TextMeshProUGUI? tmp = pasteObject.GetComponent<TextMeshProUGUI>();
                
                // 如果对象本身没有，则从子对象中查找（包括未激活的）
                if (tmp == null)
                {
                    Debug.Log($"[YellowDuckHandler] Paste对象本身没有TMP组件，从子对象中查找...");
                    tmp = pasteObject.GetComponentInChildren<TextMeshProUGUI>(true);
                }

                if (tmp != null)
                {
                    string oldText = tmp.text ?? "(空)";
                    string newText = "我是什么";
                    tmp.text = newText;
                    string actualText = tmp.text ?? "(空)";
                    
                    Debug.Log($"[YellowDuckHandler] ✓ 成功修改Paste对象的TMP文字");
                    Debug.Log($"[YellowDuckHandler]   旧文字: \"{oldText}\"");
                    Debug.Log($"[YellowDuckHandler]   新文字（验证）: \"{actualText}\"");
                    Debug.Log($"[YellowDuckHandler]   TMP组件位置: {UIFinder.GetFullPath(tmp.transform)}");
                }
                else
                {
                    Debug.LogWarning("[YellowDuckHandler] ✗ Paste对象及其子对象上未找到TextMeshProUGUI组件");
                }
                
                Debug.Log($"[YellowDuckHandler] ✓ Paste文字修改完成");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 修改Paste文字时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 更新YellowDuck对象中名为"Text (TMP)"的子对象的TextMeshProUGUI文字
        /// </summary>
        private static void UpdateYellowDuckText(GameObject yellowDuck, string presetName)
        {
            if (yellowDuck == null)
            {
                Debug.LogWarning("[MoreAppearancePreset] UpdateYellowDuckText: YellowDuck对象为空");
                return;
            }

            try
            {
                Debug.Log($"[YellowDuckHandler] 开始查找Text (TMP)子对象...");
                
                // 查找名为"Text (TMP)"的子对象（Unity创建TextMeshProUGUI时的默认子对象名）
                Transform? textTransform = null;
                Transform yellowDuckTransform = yellowDuck.transform;

                Debug.Log($"[YellowDuckHandler] YellowDuck对象 {yellowDuck.name} 的子对象数量: {yellowDuckTransform.childCount}");

                for (int i = 0; i < yellowDuckTransform.childCount; i++)
                {
                    Transform child = yellowDuckTransform.GetChild(i);
                    Debug.Log($"[YellowDuckHandler] 检查子对象 [{i}]: {child.name}");
                    if (child.name == "Text (TMP)")
                    {
                        textTransform = child;
                        Debug.Log($"[YellowDuckHandler] ✓ 找到Text (TMP)子对象: {child.name}");
                        break;
                    }
                }

                if (textTransform == null)
                {
                    Debug.LogWarning("[YellowDuckHandler] ✗ 未找到Text (TMP)子对象");
                    return;
                }

                Debug.Log($"[YellowDuckHandler] 开始移除TextLocalizer组件...");
                
                // 在修改文字前，先移除TextLocalizer组件
                TextLocalizerRemover.RemoveTextLocalizers(textTransform.gameObject);

                Debug.Log($"[YellowDuckHandler] 开始获取TextMeshProUGUI组件...");
                
                // 从Text对象获取TextMeshProUGUI组件
                TextMeshProUGUI? tmp = textTransform.GetComponent<TextMeshProUGUI>();

                if (tmp != null)
                {
                    string oldText = tmp.text ?? "(空)";
                    string newText = $"我是{presetName}";
                    tmp.text = newText;
                    string actualText = tmp.text ?? "(空)";
                    
                    Debug.Log($"[YellowDuckHandler] ✓ 成功修改Text子对象的文字");
                    Debug.Log($"[YellowDuckHandler]   旧文字: \"{oldText}\"");
                    Debug.Log($"[YellowDuckHandler]   新文字（验证）: \"{actualText}\"");
                    Debug.Log($"[YellowDuckHandler]   TMP组件位置: {UIFinder.GetFullPath(tmp.transform)}");
                }
                else
                {
                    Debug.LogWarning("[YellowDuckHandler] ✗ Text子对象上未找到TextMeshProUGUI组件");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 更新YellowDuck文字时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
            }
        }
    }
}

